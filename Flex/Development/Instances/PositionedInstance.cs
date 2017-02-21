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
        protected RotationProperty _rotation;

        [NonSerialized()]
        protected Visual3D _visual3D;

        [NonSerialized()]
        protected Transform3DGroup _transformGroup;

        [NonSerialized()]
        protected Model3D _model;

        protected abstract void InitializeVisual();

        protected PositionedInstance() : base()
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
        }

        [Category("3D")]
        [DisplayName("Rotation")]
        [Description("The rotation of this instance")]
        [ExpandableObject]
        [TrackMember]
        public RotationProperty rotation
        {
            get
            {
                return _rotation;
            }
        }
    }
}
