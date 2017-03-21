using Flex.Development.Execution.Data;
using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using Mogre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Camera : PositionedInstance
    {
        [field: NonSerialized]
        private Mogre.Camera _viewCamera;

        internal Camera(bool flag) : base()
        {
            _displayName = "Camera";
            _icon = "Legacy/camera.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();

            //Strange setup, but we have to ChangeParent before we set to isRoot

            ChangeParent(ActiveWorld.Active.World);
            _isRoot = true;

            Initialize();
        }

        [Browsable(false)]
        public Mogre.Camera MogreCamera
        {
            get
            {
                return _viewCamera;
            }
        }

        public override void Initialize()
        {
            _position = new Properties.Vector3(0, 0, 0);
            _position.PropertyChanged += PositionPropertyChanged;

            Engine.QueueInitializationAction(() =>
            {
                _viewCamera = Engine.Renderer.Scene.CreateCamera("ViewPoint");
                _viewCamera.ProjectionType = ProjectionType.PT_PERSPECTIVE;

                _viewCamera.Position = Mogre.Vector3.ZERO;
                _viewCamera.LookAt(Mogre.Vector3.ZERO);
                _viewCamera.NearClipDistance = 0.01f;
                _viewCamera.FarClipDistance = 1000.0f;
                _viewCamera.FOVy = new Degree(100f);
                _viewCamera.AutoAspectRatio = true;

                Engine.Renderer.Camera = _viewCamera;
                Engine.Renderer.RenderWindow.AddViewport(_viewCamera);

                _initialized = true;
            });
        }

        private void PositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    _viewCamera.SetPosition(position.x, position.y, position.z);
                }
            });
            NotifyPropertyChanged("Position");
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void move(Mogre.Vector3 direction, float distance)
        {
            _viewCamera.Move(direction * distance);
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void move(Mogre.Vector3 vector)
        {
            move(vector, 1);
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void pitch(float n)
        {
            _viewCamera.Pitch(new Degree(n));
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void roll(float n)
        {
            _viewCamera.Roll(new Degree(n));
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public void yaw(float n)
        {
            _viewCamera.Yaw(new Degree(n));
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        [Browsable(false)]
        public Properties.Vector3 direction
        {
            get
            {
                Mogre.Vector3 direction = _viewCamera.Direction;
                return new Properties.Vector3(direction.x, direction.y, direction.z);
            }
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Reload()
        {

        }

        [ScriptMember(ScriptAccess.None)]
        public override void Cleanup()
        {
            throw new InvalidOperationException("Cannot remove Camera instance!");
        }

        public override Instance clone()
        {
            throw new InvalidOperationException("Cannot clone Camera!");
        }

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

        public override string icon
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
                return _parent;
            }

            set
            {
                throw new InvalidOperationException("Cannot change the parent of Camera!");
            }
        }

        internal override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }

        [ScriptMember(ScriptAccess.Full)]
        public override Properties.Vector3 position
        {
            get
            {
                if (_viewCamera != null)
                {
                    Mogre.Vector3 position = _viewCamera.Position;
                    return new Properties.Vector3(position.x, position.y, position.z);
                }
                else
                {
                    return new Properties.Vector3(0);
                }
            }

            set
            {
                if (value != null)
                {
                    _viewCamera.SetPosition(value.x, value.y, value.z);
                    PositionPropertyChanged(this, null);
                }
            }
        }

        [ScriptMember(ScriptAccess.None)]
        [Browsable(false)]
        public override Rotation rotation
        {
            get
            {
                return new Rotation(0);
                //throw new InvalidOperationException("Cannot access rotation on Camera instance, use 'camera.direction' instead.");
            }

            set
            {
                throw new InvalidOperationException("Cannot access rotation on Camera instance, use 'camera.direction' instead.");
            }
        }

        protected override void InitializeVisual()
        {

        }

        protected override void ReloadGizmo()
        {
            throw new InvalidOperationException("Cannot use a Gizmo on a Camera instance!");
        }
    }
}
