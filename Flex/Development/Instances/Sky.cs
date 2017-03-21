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
        private ColorProperty _shadowColor;

        private ColorProperty _diffuseColor;
        private ColorProperty _specularColor;

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
            _ambient = new ColorProperty(220, 220, 220, 255);
            _shadowColor = new ColorProperty(140, 140, 140, 255);
            _diffuseColor = new ColorProperty(0, 0, 0);
            _specularColor = new ColorProperty(0, 0, 0);
            _sunDirection = new Properties.Vector3(0.02f, -.94f, -.03f);
            _sunDirection.PropertyChanged += SunDirectionPropertyChanged;

            Initialize();
        }

        private void SunDirectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _sun.SetDirection(_sunDirection.x, _sunDirection.y, _sunDirection.z);
        }

        public override void Initialize()
        {
            Engine.QueueInitializationAction(() =>
            {
                SceneNode sunNode = Engine.Renderer.Scene.CreateSceneNode();

                _sun = Engine.Renderer.Scene.CreateLight("Sun");

                _sun.Type = Light.LightTypes.LT_DIRECTIONAL;
                _sun.SetDiffuseColour(_diffuseColor.r / 255f, _diffuseColor.g / 255f, _diffuseColor.b / 255f);
                _sun.SetSpecularColour(_specularColor.r / 255f, _specularColor.g / 255f, _specularColor.b / 255f);
                _sun.SetDirection(_sunDirection.x, _sunDirection.y, _sunDirection.z);
                _sun.CastShadows = true;

                sunNode.AttachObject(_sun);

                Engine.Renderer.Scene.ShadowColour = new ColourValue(_shadowColor.r / 255f, _shadowColor.g / 255f, _shadowColor.b / 255f, _shadowColor.transparency / 255f);

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
                _ambient.changeColor(value);
                Engine.Renderer.Scene.AmbientLight = new ColourValue(_ambient.r / 255f, _ambient.g / 255f, _ambient.b / 255f, _ambient.transparency / 255f);
                NotifyPropertyChanged("Ambient");
            }
        }

        /*
        [Category("Lighting")]
        [DisplayName("Diffuse Color")]
        [Description("The diffuse color of the world")]
        [ScriptMember(ScriptAccess.Full)]
        public System.Windows.Media.Color diffuse
        {
            get
            {
                return _diffuseColor.color;
            }
            set
            {
                if (value == _diffuseColor.color) return;
                _diffuseColor.changeColor(value);
                _sun.SetDiffuseColour(_diffuseColor.r / 255f, _diffuseColor.g / 255f, _diffuseColor.b / 255f);
                NotifyPropertyChanged("Diffuse");
            }
        }

        [Category("Lighting")]
        [DisplayName("Specular Color")]
        [Description("The specular color of the world")]
        [ScriptMember(ScriptAccess.Full)]
        public System.Windows.Media.Color specular
        {
            get
            {
                return _specularColor.color;
            }
            set
            {
                if (value == _specularColor.color) return;
                _specularColor.changeColor(value);
                _sun.SetSpecularColour(_specularColor.r / 255f, _specularColor.g / 255f, _specularColor.b / 255f);
                NotifyPropertyChanged("Specular");
            }
        }
        */

        [Category("Lighting")]
        [DisplayName("Shadow color")]
        [Description("The Shadow color of the world")]
        [ScriptMember(ScriptAccess.Full)]
        public System.Windows.Media.Color shadowColor
        {
            get
            {
                return _shadowColor.color;
            }
            set
            {
                if (value == _shadowColor.color) return;
                _shadowColor.changeColor(value);
                Engine.Renderer.Scene.ShadowColour = new ColourValue(_shadowColor.r / 255f, _shadowColor.g / 255f, _shadowColor.b / 255f, _shadowColor.transparency / 255f);
                NotifyPropertyChanged("ShadowColor");
            }
        }

        [Category("Lighting")]
        [DisplayName("Sun Direction")]
        [Description("The normal vector direction of the light from the Sun")]
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
                _sun.SetDirection(_sunDirection.x, _sunDirection.y, _sunDirection.z);

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
