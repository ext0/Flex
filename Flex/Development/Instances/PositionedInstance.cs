using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class PositionedInstance : Instance
    {
        protected Vector3Property _position;
        protected Vector3Property _rotation;

        internal PositionedInstance() : base()
        {

        }

        [Category("3D")]
        [DisplayName("Position")]
        [Description("The 3D coordinates of this instance")]
        [ExpandableObject]
        [TrackMember]
        public Vector3Property position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value == _position) return;
                _position = value;
                NotifyPropertyChanged("Position");
            }
        }

        [Category("3D")]
        [DisplayName("Rotation")]
        [Description("The rotation of this instance")]
        [ExpandableObject]
        [TrackMember]
        public Vector3Property rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (value == _rotation) return;
                _rotation = value;
                NotifyPropertyChanged("Rotation");
            }
        }
    }
}
