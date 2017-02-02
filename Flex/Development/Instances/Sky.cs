using Flex.Development.Execution.Runtime.Attributes;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Sky : Instance
    {
        private int _sunHorizontalAngle = 20;
        private int _sunVerticalAngle = 30;
        private int _sunLightDistance = 500;
        private bool _sunShadows = false;

        public Sky() : base()
        {
            _displayName = "Sky";
            _icon = "Legacy/weather_cloudy.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _isRoot = true;
        }

        public override IEnumerable<Instance> ActiveInstances
        {
            get
            {
                return _instances;
            }
        }

        [Category("Lighting")]
        [DisplayName("Sun Horizontal Angle")]
        [Description("The angle of the directional light from the sun around the X axis")]
        [TrackMember]
        [DynamicExposedProperty(false, "sunHorizontalAngle")]
        public int SunHorizontalAngle
        {
            get
            {
                return _sunHorizontalAngle;
            }
            set
            {
                if (value == _sunHorizontalAngle) return;
                _sunHorizontalAngle = value;
                NotifyPropertyChanged("SunHorizontalAngle");
            }
        }

        [Category("Lighting")]
        [DisplayName("Sun Vertical Angle")]
        [Description("The angle of the directional light from the sun around the Y axis")]
        [TrackMember]
        [DynamicExposedProperty(false, "sunVerticalAngle")]
        public int SunVerticalAngle
        {
            get
            {
                return _sunVerticalAngle;
            }
            set
            {
                if (value == _sunVerticalAngle) return;
                _sunVerticalAngle = value;
                NotifyPropertyChanged("SunVerticalAngle");
            }
        }

        [Category("Lighting")]
        [DisplayName("Sun Distance")]
        [Description("The distance of the sun from the origin of the world")]
        [TrackMember]
        [DynamicExposedProperty(false, "sunDistance")]
        public int SunDistance
        {
            get
            {
                return _sunLightDistance;
            }
            set
            {
                if (value == _sunLightDistance) return;
                _sunLightDistance = value;
                NotifyPropertyChanged("SunDistance");
            }
        }

        [Category("Lighting")]
        [DisplayName("Cast Shadows")]
        [Description("Whether or not to cast shadows from the sun")]
        [TrackMember]
        [DynamicExposedProperty(false, "castShadows")]
        public bool CastShadows
        {
            get
            {
                return _sunShadows;
            }
            set
            {
                if (value == _sunShadows) return;
                _sunShadows = value;
                NotifyPropertyChanged("SunShadows");
            }
        }

        [TrackMember]
        [DynamicExposedProperty(false, "name")]
        public override string DisplayName
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

        [DynamicExposedProperty(true, "parent")]
        public override Instance Parent
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

        public override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
