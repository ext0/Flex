using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using Mogre;
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
        protected Properties.Vector3 _position;
        protected Rotation _rotation;

        [NonSerialized()]
        protected SceneNode _sceneNode;

        [NonSerialized()]
        protected Entity _entity;

        protected abstract void InitializeVisual();

        protected PositionedInstance() : base()
        {

        }

        [Browsable(false)]
        public SceneNode SceneNode
        {
            get
            {
                return _sceneNode;
            }
        }

        [Browsable(false)]
        public Entity Entity
        {
            get
            {
                return _entity;
            }
        }

        [Category("3D")]
        [DisplayName("Position")]
        [Description("The 3D coordinates of this instance")]
        [ExpandableObject]
        [ScriptMember(ScriptAccess.Full)]
        public abstract Properties.Vector3 position
        {
            get;
            set;
        }

        [Category("3D")]
        [DisplayName("Rotation")]
        [Description("The rotation of this instance")]
        [ExpandableObject]
        [ScriptMember(ScriptAccess.Full)]
        public abstract Rotation rotation
        {
            get;
            set;
        }
    }
}
