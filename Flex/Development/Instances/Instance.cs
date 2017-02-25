using Flex.Development.Execution.Runtime;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class Instance : NotifyPropertyChangedObject
    {
        protected String _displayName;

        protected String _UUID;

        protected String _icon;

        protected UISafeObservableCollection<Instance> _instances;

        protected Instance _parent;

        protected List<Type> _allowedChildren;

        protected bool _isRoot = false;

        protected bool _initialized = false;

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
        internal EventHandler<PropertyChangedEventArgs> OnChanged
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
        internal abstract IEnumerable<Type> AllowedChildren { get; }

        [Category("Information")]
        [DisplayName("Parent")]
        [Description("The parent of this instance")]
        [ScriptMember(ScriptAccess.Full)]
        public abstract Instance parent { get; set; }

        [Category("Information")]
        [DisplayName("Name")]
        [Description("The name of this instance")]
        [ScriptMember(ScriptAccess.Full)]
        public abstract String name { get; set; }

        [Category("Information")]
        [DisplayName("Type")]
        [Description("The type of this instance")]
        [ScriptMember(ScriptAccess.ReadOnly)]
        public String type
        {
            get
            {
                return GetType().Name;
            }
        }

        [Browsable(false)]
        [ScriptMember(ScriptAccess.None)]
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

        [ScriptMember(ScriptAccess.None)]
        [Browsable(true)]
        public abstract String icon { get; }

        internal bool RemoveFromParent()
        {
            if (_parent == null || _parent._instances == null)
            {
                return false;
            }
            return _parent._instances.Remove(this);
        }

        internal bool ChangeParent(Instance instance)
        {
            if (_isRoot || instance.Equals(this))
            {
                return false;
            }

            Instance parent = instance.parent;
            while (parent != null && !parent._isRoot)
            {
                if (parent.Equals(this))
                {
                    return false;
                }
                parent = parent.parent;
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

        [ScriptMember(ScriptAccess.ReadOnly)]
        public IEnumerable<Instance> getChildren(bool recursive = false)
        {
            if (recursive)
            {
                return GetChildrenHelper(this).AsEnumerable<Instance>();
            }
            return _instances.AsEnumerable<Instance>();
        }

        [ScriptMember(ScriptAccess.None)]
        public abstract void Initialize();

        [ScriptMember(ScriptAccess.None)]
        public abstract void Cleanup();

        [ScriptMember(ScriptAccess.None)]
        public abstract void Reload();

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void remove()
        {
            Cleanup();
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public Instance getChild(String name, bool recursive = false)
        {
            IEnumerable<Instance> instances = getChildren(recursive);
            foreach (Instance instance in _instances)
            {
                if (instance.name.Equals(name))
                {
                    return instance;
                }
            }
            return null;
        }

        private IEnumerable<Instance> GetChildrenHelper(Instance root)
        {
            List<Instance> instances = new List<Instance>();
            foreach (Instance instance in root.getChildren(false))
            {
                instances.Add(instance);
                instances.AddRange(GetChildrenHelper(instance));
            }
            return instances;
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public String toString()
        {
            return ToString();
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public bool equals(object obj)
        {
            return Equals(obj);
        }

        [ScriptMember(ScriptAccess.None)]
        public override string ToString()
        {
            return _displayName;
        }

        [ScriptMember(ScriptAccess.None)]
        public override bool Equals(object obj)
        {
            if (obj is Instance)
            {
                return (obj as Instance)._UUID.Equals(_UUID);
            }
            return false;
        }

        [ScriptMember(ScriptAccess.None)]
        public override int GetHashCode()
        {
            return _UUID.GetHashCode();
        }
    }
}
