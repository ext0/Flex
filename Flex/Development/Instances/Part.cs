using Flex.Development.Execution.Data;
using Flex.Development.Execution.Data.States;
using Flex.Development.Execution.Runtime;
using Flex.Development.Instances.Properties;
using Flex.Development.Physics;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Jitter.Collision.Shapes;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Part : PhysicsInstance
    {
        private ColorProperty _color;

        [field: NonSerialized]
        private Mogre.MaterialPtr _materialPtr;

        public Part() : base()
        {
            _displayName = "Part";
            _icon = "16/brick.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));

            _anchored = true;
            _collisions = true;
            parent = ActiveScene.Context.ActiveWorld.World;

            Initialize();
        }

        internal Part(bool flag) : base()
        {

            _displayName = "Part";
            _icon = "16/brick.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));

            _anchored = true;
            _collisions = true;

            Initialize();
        }

        public override void Initialize()
        {
            _position = new Vector3(0, 0, 0);
            _position.PropertyChanged += PositionPropertyChanged;

            _rotation = new Rotation();
            _rotation.PropertyChanged += RotationPropertyChanged;

            _size = new Vector3(8, 4, 4);
            _size.PropertyChanged += SizePropertyChanged;

            _color = new ColorProperty(Colors.Green);
            _color.PropertyChanged += ColorPropertyChanged;

            Engine.QueueForRenderDispatcher(() =>
            {
                LoadPhysicsInstance();
                InitializeVisual();
                _initialized = true;
            });
        }

        private void PositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    _sceneNode.SetPosition(position.x, position.y, position.z);
                    if (e.PropertyName.Equals("NOPHYSICS"))
                    {
                        return;
                    }
                    _rigidBody.Position = new Jitter.LinearMath.JVector(position.x, position.y, position.z);
                }
            });
            NotifyPropertyChanged("Position");
        }

        private void RotationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    Quaternion quaternion = rotation.Quaternion;
                    _sceneNode.SetOrientation((float)quaternion.W, (float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z);

                    if (e.PropertyName.Equals("NOPHYSICS"))
                    {
                        _rigidBody.Orientation = rotation.JMatrix;
                    }
                }
            });
            NotifyPropertyChanged("Rotation");
        }

        private void SizePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    _sceneNode.SetScale(_size.x, _size.y, _size.z);
                    _shape = new BoxShape(_size.x, _size.y, _size.z);
                    _rigidBody.Shape = _shape;
                }
            });
            NotifyPropertyChanged("Size");
        }

        private void ColorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    _materialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
                }
            });
            NotifyPropertyChanged("Color");
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Reload()
        {
            _rigidBody.IsActive = false;
            _rigidBody.IsActive = true;
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Cleanup()
        {
            if (_initialized)
            {
                Engine.QueueForRenderDispatcher(() =>
                {
                    UnloadPhysicsInstance();
                    Engine.Destroy(this);
                });
            }
            _position.PropertyChanged -= PositionPropertyChanged;
            _rotation.PropertyChanged -= RotationPropertyChanged;
            _size.PropertyChanged -= SizePropertyChanged;
            _color.PropertyChanged -= ColorPropertyChanged;

            RemoveFromParent();
        }

        public override Instance clone()
        {
            Part part = new Part();
            ObjectSave save = new ObjectSave(this, part, GetType());
            save.Reset();
            return part;
        }

        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("The color of this instance")]
        [ScriptMember(ScriptAccess.None)]
        public Color color
        {
            get
            {
                return _color.color;
            }
            set
            {
                if (_color.color == value) return;
                _color.changeColor(value);
                NotifyPropertyChanged("Color");
            }
        }

        [Browsable(false)]
        internal Material Material
        {
            get
            {
                return new DiffuseMaterial(new SolidColorBrush(_color.color));
            }
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
                if (value == _parent) return;
                ChangeParent(value);
                NotifyPropertyChanged("Parent");
            }
        }

        internal override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }

        public override Vector3 position
        {
            get
            {
                return _position;
            }

            set
            {
                if (_position == value) return;
                _position.PropertyChanged -= PositionPropertyChanged;
                _position = value;
                _position.PropertyChanged += PositionPropertyChanged;
                PositionPropertyChanged(this, null);
            }
        }

        public override Rotation rotation
        {
            get
            {
                return _rotation;
            }

            set
            {
                if (_rotation == value) return;
                _rotation.PropertyChanged -= RotationPropertyChanged;
                _rotation = value;
                _rotation.PropertyChanged += RotationPropertyChanged;
                RotationPropertyChanged(this, null);
            }
        }

        public override Vector3 size
        {
            get
            {
                return _size;
            }

            set
            {
                if (_size == value) return;
                _size.PropertyChanged -= SizePropertyChanged;
                _size = value;
                _size.PropertyChanged += SizePropertyChanged;
                SizePropertyChanged(this, null);
            }
        }

        protected override void InitializeVisual()
        {
            _sceneNode = Engine.Renderer.CreateEntity(out _entity, "box.mesh");
            _sceneNode.SetPosition(_position.x, _position.y, _position.z);
            _sceneNode.SetScale(_size.x, _size.y, _size.z);

            _materialPtr = _entity.GetSubEntity(0).GetMaterial().Clone(_UUID + "/material");

            _entity.GetSubEntity(0).SetMaterial(_materialPtr);

            _materialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
        }
    }
}
