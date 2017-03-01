using Ab3d.Visuals;
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

            MainDXScene.RunOnUIThread(() =>
            {
                LoadPhysicsInstance();
                InitializeVisual();
                _initialized = true;
            });
        }

        private void PositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    _transformGroup.Children[1] = new TranslateTransform3D(position.Vector3D);
                    _rigidBody.Position = new Jitter.LinearMath.JVector(position.x, position.y, position.z);
                }
            });
            NotifyPropertyChanged("Position");
        }

        private void RotationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    _transformGroup.Children[0] = new MatrixTransform3D(rotation.Matrix);
                    _rigidBody.Orientation = rotation.JMatrix;
                }
            });
            NotifyPropertyChanged("Rotation");
        }

        private void SizePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    (_visual3D as BoxVisual3D).Size = _size.Size3D;
                    _shape = new BoxShape(_size.x, _size.y, _size.z);
                    _rigidBody.Shape = _shape;
                }
            });
            NotifyPropertyChanged("Size");
        }

        private void ColorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    (_visual3D as BoxVisual3D).Material = Material;
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
                MainDXScene.RunOnUIThread(() =>
                {
                    UnloadPhysicsInstance();
                    MainDXScene.VisualInstances.Remove(this);
                    MainDXScene.RemoveChildVisual(_visual3D);
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
            _visual3D = new BoxVisual3D();
            _transformGroup = new Transform3DGroup();
            _transformGroup.Children.Add(new MatrixTransform3D(rotation.Matrix));
            _transformGroup.Children.Add(new TranslateTransform3D(position.Vector3D));
            _visual3D.Transform = _transformGroup;
            _model = (_visual3D as BoxVisual3D).Content;
            (_visual3D as BoxVisual3D).Size = size.Size3D;
            (_visual3D as BoxVisual3D).Material = Material;

            MainDXScene.VisualInstances.Add(this);
            MainDXScene.AddChildVisual(_visual3D);
        }
    }
}
