using Flex.Development.Execution.Runtime;
using Flex.Development.Execution.Runtime.Attributes;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    [DynamicExposedClass]
    public abstract class Instance : NotifyPropertyChangedObject
    {
        protected String _displayName;

        protected String _UUID;

        protected String _icon;

        protected UISafeObservableCollection<Instance> _instances;

        protected Instance _parent;

        protected List<Type> _allowedChildren;

        protected bool _isRoot = false;

        [NonSerialized()]
        protected Tracker _tracker;

        [field: NonSerialized]
        protected event EventHandler<PropertyChangedEventArgs> _changed;

        protected Instance() : base()
        {
            _UUID = Guid.NewGuid().ToString();
            _tracker = new Tracker(this);
            _tracker.OnChanged += TrackerChanged;
        }

        private void TrackerChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (_changed != null)
            {
                _changed.Invoke(this, e);
            }
        }

        [Browsable(false)]
        public EventHandler<PropertyChangedEventArgs> OnChanged
        {
            get
            {
                return _changed;
            }
            set
            {
                _changed = value;
            }
        }

        [Browsable(false)]
        public abstract IEnumerable<Instance> ActiveInstances { get; }

        [Browsable(false)]
        public abstract IEnumerable<Type> AllowedChildren { get; }

        [Category("Information")]
        [DisplayName("Parent")]
        [Description("The parent of this instance")]
        public abstract Instance Parent { get; set; }

        [Category("Information")]
        [DisplayName("Name")]
        [Description("The name of this instance")]
        public abstract String DisplayName { get; set; }

        [Category("Information")]
        [DisplayName("Type")]
        [Description("The type of this instance")]
        [DynamicExposedProperty(true, "type")]
        public String Type
        {
            get
            {
                return GetType().Name;
            }
        }

        [Browsable(false)]
        public UISafeObservableCollection<Instance> Children
        {
            get
            {
                return _instances;
            }
            set
            {
                if (_instances == value) return;
                _instances = value;
                NotifyPropertyChanged("Instances");
            }
        }

        [Browsable(false)]
        public abstract String Icon { get; }

        public bool RemoveFromParent()
        {
            if (_parent == null || _parent._instances == null)
            {
                return false;
            }
            return _parent._instances.Remove(this);
        }

        protected bool ChangeParent(Instance instance)
        {
            if (_isRoot || instance.Equals(this))
            {
                return false;
            }

            Instance parent = instance.Parent;
            while (parent != null && parent._isRoot)
            {
                if (parent.Equals(this))
                {
                    return false;
                }
                parent = parent.Parent;
            }

            if (instance.AllowedChildren.Where((x) =>
            {
                return GetType().IsSubclassOf(x) || GetType().Equals(x);
            }).Count() == 0)
            {
                return false;
            }

            if (_parent != null)
            {
                _parent._instances.Remove(this);
            }
            _parent = instance;
            _parent._instances.Add(this);

            return true;
        }

        public IEnumerable<Instance> GetChildren(bool recursive = false)
        {
            if (recursive)
            {
                return GetChildrenHelper(this);
            }
            return _instances;
        }

        private delegate DynamicJS[] GetChildrenDelegate(bool recursive);

        [DynamicExposedMethod(typeof(GetChildrenDelegate), "getChildren")]
        public DynamicJS[] GetChildrenExec(bool recursive = false)
        {
            IEnumerable<Instance> instances;
            if (recursive)
            {
                instances = GetChildrenHelper(this);
            }
            else
            {
                instances = _instances;
            }
            return _instances.Select((x) =>
            {
                return new DynamicJS(x);
            }).ToArray();
        }

        private IEnumerable<Instance> GetChildrenHelper(Instance root)
        {
            List<Instance> instances = new List<Instance>();
            foreach (Instance instance in root.GetChildren(false))
            {
                instances.Add(instance);
                instances.AddRange(GetChildrenHelper(instance));
            }
            return instances;
        }


        public override string ToString()
        {
            return _displayName;
        }

        public override bool Equals(object obj)
        {
            if (obj is Instance)
            {
                return (obj as Instance)._UUID.Equals(_UUID);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _UUID.GetHashCode();
        }
    }
}
