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
    [TrackClass]
    [Serializable]
    public class Vector3Property : NotifyPropertyChangedObject
    {
        private float _x;
        private float _y;
        private float _z;

        internal Vector3Property(Size3D size)
        {
            _x = (float)size.X;
            _y = (float)size.Y;
            _z = (float)size.Z;
        }

        internal Vector3Property(Point3D point)
        {
            _x = (float)point.X;
            _y = (float)point.Y;
            _z = (float)point.Z;
        }

        internal Vector3Property(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        [DisplayName("X")]
        [Description("The X axis value")]
        [TrackMember]
        public float x
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
        public float y
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
        public float z
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
        internal Size3D Size3D
        {
            get
            {
                return new Size3D(_x, _y, _z);
            }
        }

        [Browsable(false)]
        internal Point3D Point3D
        {
            get
            {
                return new Point3D(_x, _y, _z);
            }
        }

        [Browsable(false)]
        internal Vector3D Vector3D
        {
            get
            {
                return new Vector3D(_x, _y, _z);
            }
        }

        public void setTo(float x, float y, float z)
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
