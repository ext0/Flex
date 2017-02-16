using Flex.Development.Execution.Runtime;
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

        internal Part(double posx, double posy, double posz, double sizex, double sizey, double sizez, Color color) : base()
        {
            _position = new Vector3Property(posx, posy, posz);
            _position.PropertyChanged += (sender, e) => NotifyPropertyChanged("Position");

            _rotation = new Vector3Property(0, 0, 0);
            _rotation.PropertyChanged += (sender, e) => NotifyPropertyChanged("Rotation");

            _size = new Vector3Property(sizex, sizey, sizez);
            _size.PropertyChanged += (sender, e) => NotifyPropertyChanged("Size");

            _color = new ColorProperty(color);
            _color.PropertyChanged += (sender, e) => NotifyPropertyChanged("Color");

            _displayName = "Part";
            _icon = "16/brick.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            _allowedChildren.Add(typeof(Instance));

            _anchored = true;
            _collisions = true;
        }

        public Part()
        {
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
        public Vector3Property size
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
    }
}
