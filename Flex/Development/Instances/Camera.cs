using Flex.Development.Execution.Data;
using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
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
            _position = new Vector3Property(0, 0, 0);
            _position.PropertyChanged += (sender, e) =>
            {
                MainDXScene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        //_transformGroup.Children[1] = new TranslateTransform3D(position.Vector3D);
                    }
                });
                NotifyPropertyChanged("Position");
            };

            _rotation = new RotationProperty();
            _rotation.PropertyChanged += (sender, e) =>
            {
                MainDXScene.RunOnUIThread(() =>
                {
                    if (_initialized)
                    {
                        //_transformGroup.Children[0] = new MatrixTransform3D(rotation.Matrix);
                    }
                });
                NotifyPropertyChanged("Rotation");
            };

            _initialized = true;
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

        public override string icon
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

        protected override void InitializeVisual()
        {

        }
    }
}
