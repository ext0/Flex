using Flex.Development.Execution.Runtime.Attributes;
using Flex.Misc.Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Flex.Development.Instances.Properties
{
    [DynamicExposedClass]
    [TrackClass]
    [Serializable]
    public class Vector3Property : NotifyPropertyChangedObject
    {
        private double _x;
        private double _y;
        private double _z;

        public Vector3Property(Size3D size)
        {
            _x = size.X;
            _y = size.Y;
            _z = size.Z;
        }

        public Vector3Property(Point3D point)
        {
            _x = point.X;
            _y = point.Y;
            _z = point.Z;
        }

        public Vector3Property(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        [DisplayName("X")]
        [Description("The X axis value")]
        [TrackMember]
        [DynamicExposedProperty(true, "x")]
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                if (value == _x) return;
                _x = value;
                NotifyPropertyChanged("X");
            }
        }

        [DisplayName("Y")]
        [Description("The Y axis value")]
        [TrackMember]
        [DynamicExposedProperty(true, "y")]
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (value == _y) return;
                _y = value;
                NotifyPropertyChanged("Y");
            }
        }

        [DisplayName("Z")]
        [Description("The Z axis value")]
        [TrackMember]
        [DynamicExposedProperty(true, "z")]
        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                if (value == _z) return;
                _z = value;
                NotifyPropertyChanged("Z");
            }
        }

        [Browsable(false)]
        public Size3D Size3D
        {
            get
            {
                return new Size3D(_x, _y, _z);
            }
        }

        [Browsable(false)]
        public Point3D Point3D
        {
            get
            {
                return new Point3D(_x, _y, _z);
            }
        }

        [Browsable(false)]
        public Vector3D Vector3D
        {
            get
            {
                return new Vector3D(_x, _y, _z);
            }
        }

        private delegate void SetToDelegate(double x, double y, double z);

        [DynamicExposedMethod(typeof(SetToDelegate), "setTo")]
        public void SetTo(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            NotifyPropertyChanged("XYZ");
        }

        public override string ToString()
        {
            return _x + ", " + _y + ", " + _z;
        }
    }
}
