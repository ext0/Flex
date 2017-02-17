using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class PositionedInstance : Instance
    {
        protected Vector3Property _position;
        protected Vector3Property _rotation;

        protected bool _visuallyInitialized = false;

        protected Visual3D _visual3D;
        protected TranslateTransform3D _translateTransform;
        protected RotateTransform3D _rotateTransform;
        protected Transform3DGroup _transformGroup;

        protected Model3D _model;

        protected abstract void InitializeVisual();

        internal PositionedInstance() : base()
        {

        }

        [Browsable(false)]
        public Visual3D Visual3D
        {
            get
            {
                return _visual3D;
            }
        }

        [Browsable(false)]
        public Model3D Model
        {
            get
            {
                return _model;
            }
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
