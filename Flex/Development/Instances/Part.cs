using Flex.Development.Execution.Data;
using Flex.Development.Execution.Data.States;
using Flex.Development.Execution.Runtime;
using Flex.Development.Instances.Properties;
using Flex.Development.Physics;
using Flex.Development.Rendering;
using Flex.Development.Rendering.Manual;
using Flex.Development.Rendering.Modules;
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
        private MaterialPtr _baseMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _frontMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _backMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _rightMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _leftMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _topMaterialPtr;

        [field: NonSerialized]
        private MaterialPtr _bottomMaterialPtr;

        [field: NonSerialized]
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
            _shape = new MogreNewt.CollisionPrimitives.Box(PhysicsEngine.World, new Mogre.Vector3(_size.x, _size.y, _size.z), 0);

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
                float mass = _size.x * _size.y * _size.z;
                _rigidBody.SetMassMatrix(mass, intertia);
            }

            _rigidBody.IsFreezed = anchored;

            //_rigidBody.IsGravityEnabled = true;

            _rigidBody.ForceCallback += _rigidBody_ForceCallback;
        }

        private void _rigidBody_ForceCallback(Body body, float timeStep, int threadIndex)
        {
            _rigidBody.AddForce(
                new Mogre.Vector3(
                    0,
                    -9.8f * (_size.x * _size.y * _size.z) * 10f,
                    0));

            position.setToPhysics(body.Position.x - (_size.x / 2), body.Position.y - (_size.y / 2), body.Position.z - (_size.z / 2));
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
                        Mogre.Quaternion quaternion = new Mogre.Quaternion(_rotation.Matrix);
                        _sceneNode.SetOrientation((float)quaternion.w, (float)quaternion.x, (float)quaternion.y, (float)quaternion.z);
                        _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);
                    }

                    //LoadPhysicsStructure();

                    if (_showingBoundingBox)
                    {
                        Engine.QueueForNextRenderDispatcher(() =>
                        {

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
                    _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
                    _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);

                    LoadPhysicsInstance();

                    LoadMaterials();
                    ReloadVisual();

                    if (_showingBoundingBox)
                    {
                        Engine.QueueForNextRenderDispatcher(() =>
                        {
                            UpdateBoundingBox();
                        });
                    }

                    ReloadGizmo();
                }
            });
            NotifyPropertyChanged("Size");
        }

        private void ColorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Engine.QueueForRenderDispatcher(() =>
            {
                LoadMaterials();
            });
            NotifyPropertyChanged("Color");
        }

        [ScriptMember(ScriptAccess.None)]
        public override void Reload()
        {
            _rigidBody.Velocity = Mogre.Vector3.ZERO;
            _rigidBody.Torque = Mogre.Vector3.ZERO;
            _rigidBody.Omega = Mogre.Vector3.ZERO;

            _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
            _rigidBody.SetPositionOrientation(new Mogre.Vector3(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2)), _sceneNode.Orientation);

            RotationPropertyChanged(null, new PropertyChangedEventArgs("XYZ"));
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

                Engine.QueueForRenderDispatcher(() =>
                {
                    LoadMaterials(true);
                    ReloadVisual();
                });

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
                PositionPropertyChanged(this, new PropertyChangedEventArgs("Position"));
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
                RotationPropertyChanged(this, new PropertyChangedEventArgs("Rotation"));
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
                SizePropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
        }

        protected override void ReloadGizmo()
        {
            if (_gizmoVisual != null)
            {
                AxisAlignedBox bounding = _movableObject.BoundingBox;

                _gizmoVisual.XA?.SetPosition((bounding.Size.x / 2) - 1, 0, 0);
                _gizmoVisual.YA?.SetPosition(0, (bounding.Size.y / 2) - 1, 0);
                _gizmoVisual.ZA?.SetPosition(0, 0, (bounding.Size.z / 2) - 1);

                if (_gizmoVisual.IsTwoSided)
                {
                    _gizmoVisual.XB?.SetPosition(-(bounding.Size.x / 2) + 1, 0, 0);
                    _gizmoVisual.YB?.SetPosition(0, -(bounding.Size.y / 2) + 1, 0);
                    _gizmoVisual.ZB?.SetPosition(0, 0, -(bounding.Size.z / 2) + 1);
                }
            }
        }

        private void ReloadVisual()
        {
            MeshGenerator.UpdateCube(
                (ManualObject)_movableObject,
                _size.x,
                _size.y,
                _size.z,
                _frontMaterialPtr,
                _backMaterialPtr,
                _leftMaterialPtr,
                _rightMaterialPtr,
                _topMaterialPtr,
                _bottomMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP);

            LoadMaterials();
        }

        private void LoadMaterials(bool forceUpdate = false)
        {
            if (_baseMaterialPtr == null || forceUpdate)
            {
                _baseMaterialPtr = _material.GetMaterial().Clone(_UUID + "/DynamicMesh/" + Guid.NewGuid());
            }
            if (_frontMaterialPtr == null || forceUpdate)
            {
                _frontMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Front");
            }
            if (_backMaterialPtr == null || forceUpdate)
            {
                _backMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Back");
            }
            if (_leftMaterialPtr == null || forceUpdate)
            {
                _leftMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Left");
            }
            if (_rightMaterialPtr == null || forceUpdate)
            {
                _rightMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Right");
            }
            if (_topMaterialPtr == null || forceUpdate)
            {
                _topMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Top");
            }
            if (_bottomMaterialPtr == null || forceUpdate)
            {
                _bottomMaterialPtr = _baseMaterialPtr.Clone(_baseMaterialPtr.Name + "/Bottom");
            }

            _textureScale = _material.TextureScaling();

            _frontMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_frontMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_frontMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _frontMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.x, _textureScale.y / size.y);

            _backMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_backMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_backMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _backMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.x, _textureScale.y / size.y);

            _leftMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_leftMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_leftMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _leftMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.y, _textureScale.y / size.z);

            _rightMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_rightMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_rightMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _rightMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.y, _textureScale.y / size.z);

            _topMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_topMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_topMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _topMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.x, _textureScale.y / size.z);

            _bottomMaterialPtr.SetAmbient(new ColourValue(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f));
            //_bottomMaterialPtr.SetDiffuse(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            //_bottomMaterialPtr.SetSpecular(_color.r / 255f, _color.g / 255f, _color.b / 255f, _color.transparency / 255f);
            _bottomMaterialPtr.GetTechnique(0).GetPass(0).GetTextureUnitState(0).SetTextureScale(_textureScale.x / size.x, _textureScale.y / size.z);
        }

        protected override void InitializeVisual()
        {
            LoadMaterials();

            _movableObject = MeshGenerator.GenerateCube(
                _size.x,
                _size.y,
                _size.z,
                _UUID.ToString(),
                _frontMaterialPtr,
                _backMaterialPtr,
                _leftMaterialPtr,
                _rightMaterialPtr,
                _topMaterialPtr,
                _bottomMaterialPtr);

            _movableObject.QueryFlags = (uint)QueryFlags.INSTANCE_ENTITY;
            _movableObject.CastShadows = true;

            _sceneNode = Engine.Renderer.CreateSceneNode(this);

            _sceneNode.SetPosition(position.x + (size.x / 2), position.y + (size.y / 2), position.z + (size.z / 2));
        }
    }
}
