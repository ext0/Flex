using Ab3d.Visuals;
using Flex.Development.Execution.Data;
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
            _position = new Vector3Property(0, 0, 0);
            _position.PropertyChanged += (sender, e) =>
            {
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        _transformGroup.Children[1] = new TranslateTransform3D(position.Vector3D);
                        _rigidBody.Position = new Jitter.LinearMath.JVector(position.x, position.y, position.z);
                    }
                });
                NotifyPropertyChanged("Position");
            };

            _rotation = new RotationProperty();
            _rotation.PropertyChanged += (sender, e) =>
            {
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        _transformGroup.Children[0] = new MatrixTransform3D(rotation.Matrix);
                        _rigidBody.Orientation = rotation.JMatrix;
                    }
                });
                NotifyPropertyChanged("Rotation");
            };

            _size = new Vector3Property(8, 4, 4);
            _size.PropertyChanged += (sender, e) =>
            {
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        (_visual3D as BoxVisual3D).Size = _size.Size3D;
                        _shape = new BoxShape(_size.x, _size.y, _size.z);
                        _rigidBody.Shape = _shape;
                    }
                });
                NotifyPropertyChanged("Size");
            };

            _color = new ColorProperty(Colors.Green);
            _color.PropertyChanged += (sender, e) =>
            {
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        (_visual3D as BoxVisual3D).Material = Material;
                    }
                });
                NotifyPropertyChanged("Color");
            };

            MainDXScene.Scene.RunOnUIThread(() =>
            {
                LoadPhysicsInstance();
                InitializeVisual();
            });

            _initialized = true;
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
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    UnloadPhysicsInstance();
                    MainDXScene.Scene.VisualInstances.Remove(this);
                    MainDXScene.Scene.RemoveChildVisual(_visual3D);
                });
            }
        }

        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("The color of this instance")]
        [TrackMember]
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

        [TrackMember]
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

        public override string Icon
        {
            get
            {
                return "/Resources/Icons/" + _icon;
            }
        }

        [TrackMember]
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

            MainDXScene.Scene.VisualInstances.Add(this);
            MainDXScene.Scene.AddChildVisual(_visual3D);
        }
    }
}
