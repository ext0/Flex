using Flex.Development.Execution.Data;
using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
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

        public override void Initialize()
        {
            _position = new Vector3(0, 0, 0);
            _position.PropertyChanged += PositionPropertyChanged;

            _rotation = new Rotation();
            _rotation.PropertyChanged += RotationPropertyChanged;

            _initialized = true;
        }

        private void RotationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    //_transformGroup.Children[0] = new MatrixTransform3D(rotation.Matrix);
                }
            });
            NotifyPropertyChanged("Rotation");
        }

        private void PositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                if (_initialized)
                {
                    //_transformGroup.Children[1] = new TranslateTransform3D(position.Vector3D);
                }
            });
            NotifyPropertyChanged("Position");
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

        protected override void InitializeVisual()
        {

        }
    }
}
