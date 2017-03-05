using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using Mogre;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Sky : Instance
    {
        private ColorProperty _ambient;

        [field: NonSerialized]
        private Light _sun;

        private Properties.Vector3 _sunDirection;

        internal Sky(bool flag) : base()
        {
            _displayName = "Sky";
            _icon = "Legacy/weather_cloudy.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _isRoot = true;
            _ambient = new ColorProperty(25, 25, 25, 255);
            _sunDirection = new Properties.Vector3(-1, -1, -0.5);
            _sunDirection.PropertyChanged += SunDirectionPropertyChanged;

            Initialize();
        }

        private void SunDirectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _sun.SetDirection(_sunDirection.x / 360f, _sunDirection.y / 360f, _sunDirection.z / 360f);
        }

        public override void Initialize()
        {
            Engine.QueueInitializationAction(() =>
            {
                SceneNode sunNode = Engine.Renderer.Scene.CreateSceneNode();

                _sun = Engine.Renderer.Scene.CreateLight("Sun");

                _sun.Type = Light.LightTypes.LT_DIRECTIONAL;
                _sun.SetDiffuseColour(1.0f, 1.0f, 1.0f);
                _sun.SetDirection(_sunDirection.x / 360f, _sunDirection.y / 360f, _sunDirection.z / 360f);
                _sun.CastShadows = true;

                sunNode.AttachObject(_sun);

                Engine.Renderer.Scene.RootSceneNode.AddChild(sunNode);

                _initialized = true;
            });
        }

        public override void Cleanup()
        {
            throw new InvalidOperationException("Cannot remove Sky instance!");
        }

        public override void Reload()
        {

        }

        public override Instance clone()
        {
            throw new InvalidOperationException("Cannot clone Sky!");
        }

        [Category("Lighting")]
        [DisplayName("Ambient")]
        [Description("The ambient color of the world")]
        [ScriptMember(ScriptAccess.Full)]
        public System.Windows.Media.Color ambient
        {
            get
            {
                return _ambient.color;
            }
            set
            {
                if (value == _ambient.color) return;
                _ambient = new ColorProperty(value);
                Engine.Renderer.Scene.AmbientLight = new ColourValue(_ambient.r / 255f, _ambient.g / 255f, _ambient.b / 255f, _ambient.transparency / 255f);
                NotifyPropertyChanged("Ambient");
            }
        }

        [Category("Lighting")]
        [DisplayName("Sun Direction")]
        [Description("The direction of the light from the Sun")]
        [ExpandableObject]
        [ScriptMember(ScriptAccess.Full)]
        public Properties.Vector3 sunDirection
        {
            get
            {
                return _sunDirection;
            }
            set
            {
                if (value == _sunDirection) return;
                _sunDirection = value;
                _sun.SetDirection(_sunDirection.x / 360f, _sunDirection.y / 360f, _sunDirection.z / 360f);

                NotifyPropertyChanged("SunDirection");
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
                throw new InvalidOperationException();
            }
        }

        internal override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
