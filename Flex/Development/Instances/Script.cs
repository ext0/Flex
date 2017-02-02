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
    public class Script : Instance
    {
        private String _source;

        public Script() : base()
        {
            _displayName = "Script";
            _icon = "16/script.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
        }

        public void AddInstance(Instance instance)
        {
            instance.Parent = this;
            _instances.Add(instance);
            NotifyPropertyChanged("Instances");
        }

        public override IEnumerable<Instance> ActiveInstances
        {
            get
            {
                return _instances;
            }
        }

        [TrackMember]
        [DynamicExposedProperty(false, "name")]
        public override string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        [Browsable(false)]
        public String Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                NotifyPropertyChanged("Source");
            }
        }

        public override string Icon
        {
            get
            {
                return "/Resources/Icons/" + _icon;
            }
        }

        [TrackMember]
        [DynamicExposedProperty(false, "parent")]
        public override Instance Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value == _parent) return;
                ChangeParent(value);
                NotifyPropertyChanged("Parent");
            }
        }

        public override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
