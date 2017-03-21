using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Development.Rendering.Modules;
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
        protected SceneNode _boundingBox;

        [NonSerialized()]
        protected Entity _entity;

        [NonSerialized()]
        protected GizmoVisual _gizmoVisual;

        [NonSerialized()]
        protected WireBoundingBox _wireBoundingBox;

        [NonSerialized()]
        protected bool _showingBoundingBox;

        [NonSerialized()]
        private bool _isSelected;

        [NonSerialized()]
        private bool _visualizedFlag;

        protected abstract void InitializeVisual();

        protected abstract void ReloadGizmo();

        protected PositionedInstance() : base()
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                _wireBoundingBox = new WireBoundingBox();
                _showingBoundingBox = false;
                _isSelected = false;
            });
        }

        public void SetGizmoVisual(GizmoVisual visual)
        {
            _sceneNode.AddChild(visual.Base);
            _gizmoVisual = visual;
            ReloadGizmo();
        }

        public void RemoveGizmoVisual()
        {
            if (_gizmoVisual != null)
            {
                _sceneNode.RemoveChild(_gizmoVisual.Base);
            }
            _gizmoVisual = null;
        }

        private void SetSelected()
        {
            if (!_showingBoundingBox)
            {
                return;
            }
            _wireBoundingBox.SetMaterial("BoundingBox/BlueDark");
        }

        private void SetHover()
        {
            if (!_showingBoundingBox)
            {
                return;
            }
            _wireBoundingBox.SetMaterial("BoundingBox/BlueLight");
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value == _isSelected) return;

                if (value)
                {
                    SetSelected();
                }
                else
                {
                    SetHover();
                }

                _isSelected = value;
            }
        }

        [Browsable(false)]
        public bool IsBoundingBoxEnabled
        {
            get
            {
                return _showingBoundingBox;
            }
            set
            {
                if (value == _showingBoundingBox) return;

                if (value)
                {
                    ShowBoundingBox();
                }
                else
                {
                    HideBoundingBox();
                }
                _showingBoundingBox = value;

            }
        }

        private void ShowBoundingBox()
        {
            if (_showingBoundingBox || _wireBoundingBox == null || _entity == null)
            {
                return;
            }
            _wireBoundingBox.SetupBoundingBox(_entity.GetWorldBoundingBox());
            _boundingBox = Engine.Renderer.Scene.CreateSceneNode();
            _boundingBox.AttachObject(_wireBoundingBox);
            _sceneNode.AddChild(_boundingBox);
            _showingBoundingBox = true;
            _visualizedFlag = true;
            SetHover();
        }

        private void HideBoundingBox()
        {
            if (!_showingBoundingBox || _wireBoundingBox == null || _entity == null || _boundingBox == null)
            {
                return;
            }
            _boundingBox.RemoveAndDestroyAllChildren();
            Engine.Renderer.Scene.DestroySceneNode(_boundingBox);
            _wireBoundingBox.DetachFromParent();
            _showingBoundingBox = false;
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
