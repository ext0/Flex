using Flex.Development.Execution.Runtime;
using Flex.Development.Execution.Runtime.Attributes;
using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Part : PositionedInstance
    {
        private Vector3Property _size;

        private ColorProperty _color;

        public Part(double posx, double posy, double posz, double sizex, double sizey, double sizez, Color color) : base()
        {
            _position = new Vector3Property(posx, posy, posz);
            _rotation = new Vector3Property(0, 0, 0);
            _size = new Vector3Property(sizex, sizey, sizez);
            _color = new ColorProperty(color);

            _displayName = "Part";
            _icon = "16/brick.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));
        }

        [Category("3D")]
        [DisplayName("Size")]
        [Description("The size of this instance")]
        [ExpandableObject]
        [TrackMember]
        [DynamicExposedProperty(true, "size")]
        public Vector3Property Size
        {
            get
            {
                return _size;
            }
        }

        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("The color of this instance")]
        [TrackMember]
        [DynamicExposedProperty(true, "color")]
        public Color Color
        {
            get
            {
                return _color.Color;
            }
            set
            {
                if (_color.Color == value) return;
                _color.ChangeColor(value);
                NotifyPropertyChanged("Color");
            }
        }

        [Browsable(false)]
        public Material Material
        {
            get
            {
                return new DiffuseMaterial(new SolidColorBrush(_color.Color));
            }
        }

        public override IEnumerable<Instance> ActiveInstances
        {
            get
            {
                return _instances;
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

        [TrackMember]
        [DynamicExposedProperty(false, "parent")]
        public override Instance Parent
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

        public override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
