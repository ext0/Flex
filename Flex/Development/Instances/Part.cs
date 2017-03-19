using Flex.Development.Execution.Data;
using Flex.Development.Execution.Data.States;
using Flex.Development.Execution.Runtime;
using Flex.Development.Instances.Properties;
using Flex.Development.Physics;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using Mogre;
using MogreNewt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
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

        private Properties.Material _material;

        [field: NonSerialized]
        private MaterialPtr _materialPtr;

        private Vector2 _textureScale;

        public Part() : base()
        {
            _displayName = "Part";
            _icon = "16/brick.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));

            _anchored = true;
            _collisions = true;
            _material = Properties.Material.GRASS;
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
            _material = Properties.Material.GRASS;

            Initialize();
        }

        public override void Initialize()
        {
            _position = new Properties.Vector3(0, 0, 0);
            _position.PropertyChanged += PositionPropertyChanged;

            _rotation = new Rotation();
            _rotation.PropertyChanged += RotationPropertyChanged;

            _size = new Properties.Vector3(8, 4, 4);
            _size.PropertyChanged += SizePropertyChanged;

            _color = new ColorProperty(Colors.White);
            _color.PropertyChanged += ColorPropertyChanged;

            Engine.QueueForRenderDispatcher(() =>
            {
                InitializeVisual();
                LoadPhysicsInstance();

                _initialized = true;
            });
        }

        protected override void LoadPhysicsInstance()
        {
            _shape = new MogreNewt.CollisionPrimitives.Box(PhysicsEngine.World, _sceneNode.GetScale(), 0);
            Mogre.Vector3 scale = _sceneNode.GetScale();

            if (_rigidBody != null)
            {
                _rigidBody.AttachNode(null);
                _rigidBody.Dispose();
            }

            _rigidBody = new Body(PhysicsEngine.World, _shape);
            _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);
            _rigidBody.AttachNode(_sceneNode);

            if (!_anchored)
            {
                Mogre.Vector3 intertia;
                Mogre.Vector3 offset;
                _shape.CalculateInertialMatrix(out intertia, out offset);
                float mass = scale.x * scale.y * scale.z;
                _rigidBody.SetMassMatrix(mass, intertia);
            }

            _rigidBody.IsFreezed = anchored;

            _rigidBody.IsGravityEnabled = true;

            _rigidBody.ForceCallback += _rigidBody_ForceCallback;
        }

        private void _rigidBody_ForceCallback(Body body, float timeStep, int threadIndex)
        {
            position.setToPhysics(body.Position.x - (size.x / 2), body.Position.y - (size.y / 2), body.Position.z - (size.z / 2));
            rotation.LoadFromMatrix(body.Orientation.ToRotationMatrix());
        }

        private void PositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                if (_initialized)
                {
                    //LoadPhysicsStructure();

                    if (!e.PropertyName.Equals("NOPHYSICS"))
                    {
                        _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
                        _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);
                    }

                    if (_showingBoundingBox)
                    {
                        Engine.QueueForNextRenderDispatcher(() =>
                        {
                            AxisAlignedBox box = _entity.GetWorldBoundingBox();
                            _wireBoundingBox.SetupBoundingBox(box);
                        });
                    }
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
                    if (!e.PropertyName.Equals("NOPHYSICS"))
                    {
                        System.Windows.Media.Media3D.Quaternion quaternion = rotation.Quaternion;
                        _sceneNode.SetOrientation((float)quaternion.W, (float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z);
                        _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);
                    }

                    //LoadPhysicsStructure();

                    if (_showingBoundingBox)
                    {
                        Engine.QueueForNextRenderDispatcher(() =>
                        {
                            AxisAlignedBox box = _entity.GetWorldBoundingBox();
                            _wireBoundingBox.SetupBoundingBox(box);
                        });
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
                    _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
                    _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);

                    LoadPhysicsInstance();

                    if (_showingBoundingBox)
                    {
                        Engine.QueueForNextRenderDispatcher(() =>
                        {
                            AxisAlignedBox box = _entity.GetWorldBoundingBox();
                            _wireBoundingBox.SetupBoundingBox(box);
                        });
                    }

                    Vector2 vector = _size.GetLargestValues();
                    _materialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / vector.x, _textureScale.y / vector.y);
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
                    _materialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
                    _materialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
                    _materialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
                }
            });
            NotifyPropertyChanged("Color");
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Reload()
        {
            _rigidBody.Velocity = Mogre.Vector3.ZERO;
            _rigidBody.Torque = Mogre.Vector3.ZERO;
            _rigidBody.Omega = Mogre.Vector3.ZERO;

            Mogre.Quaternion orientation = new Mogre.Quaternion();
            System.Windows.Media.Media3D.Quaternion rotationQuaternion = rotation.Quaternion;
            orientation.w = (float)rotationQuaternion.W;
            orientation.x = (float)rotationQuaternion.X;
            orientation.y = (float)rotationQuaternion.Y;
            orientation.z = (float)rotationQuaternion.Z;

            _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
            _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Cleanup()
        {
            if (_initialized)
            {
                Engine.QueueForRenderDispatcher(() =>
                {
                    _rigidBody.Dispose();

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
        //[Editor(typeof(ColorEditor), typeof(ColorEditor))]
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

        [Category("Appearance")]
        [DisplayName("Material")]
        [Description("The material of this instance")]
        [ScriptMember(ScriptAccess.Full)]
        [TypeConverter(typeof(MaterialConverter))]
        public Properties.Material material
        {
            get
            {
                return _material;
            }
            set
            {
                if (_material == value) return;
                _material = value;
                _textureScale = _material.TextureScaling();

                _materialPtr = _material.GetMaterial();

                if (_initialized)
                {
                    _entity.SetMaterial(_materialPtr);

                    _entity.GetSubEntity(0).SetMaterial(_materialPtr);

                    Vector2 vector = _size.GetLargestValues();
                    _materialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / vector.x, _textureScale.y / vector.y);

                    _materialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
                    _materialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
                    _materialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
                }

                NotifyPropertyChanged("Material");
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

        public override Properties.Vector3 position
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

        public override Properties.Vector3 size
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
            _sceneNode = Engine.Renderer.CreateEntity(out _entity, "box.mesh", this);
            _sceneNode.SetPosition(_position.x + (_size.x / 2), _position.y + (_size.y / 2), _position.z + (_size.z / 2));
            _sceneNode.SetScale(_size.x, _size.y, _size.z);

            _textureScale = _material.TextureScaling();
            _materialPtr = _material.GetMaterial();

            _entity.SetMaterial(_materialPtr);

            _entity.GetSubEntity(0).SetMaterial(_materialPtr);

            Vector2 vector = _size.GetLargestValues();
            _materialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / vector.x, _textureScale.y / vector.y);

            _materialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            _materialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _materialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
        }
    }
}
