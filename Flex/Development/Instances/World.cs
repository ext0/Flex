using Flex.Development.Execution.Runtime.Attributes;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    public class World : Instance
    {
        public World() : base()
        {
            _displayName = "World";
            _icon = "16/world.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));
            _isRoot = true;
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

        public override string Icon
        {
            get
            {
                return "/Resources/Icons/" + _icon;
            }
        }

        [DynamicExposedProperty(true, "parent")]
        public override Instance Parent
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException();
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
