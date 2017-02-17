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
        internal World(bool flag) : base()
        {
            _displayName = "World";
            _instances = new UISafeObservableCollection<Instance>();
            _icon = "16/world.png";
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));
            _isRoot = true;

            Initialize();
        }

        public override void Initialize()
        {
            _initialized = true;
        }

        public override void Cleanup()
        {

        }

        [TrackMember]
        public override string name
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

        public override Instance parent
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

        internal override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
