using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Flex.Development.Instances.Properties
{
    [Serializable]
    public class Vector3 : NotifyPropertyChangedObject
    {
        private float _x;
        private float _y;
        private float _z;

        public Vector3()
        {
            _x = 0;
            _y = 0;
            _z = 0;
        }

        public Vector3(double n)
        {
            _x = (float)n;
            _y = (float)n;
            _z = (float)n;
        }

        public Vector3(double x, double y, double z)
        {
            _x = (float)x;
            _y = (float)y;
            _z = (float)z;
        }

        internal Vector3(Size3D size)
        {
            _x = (float)size.X;
            _y = (float)size.Y;
            _z = (float)size.Z;
        }

        internal Vector3(Point3D point)
        {
            _x = (float)point.X;
            _y = (float)point.Y;
            _z = (float)point.Z;
        }

        internal Vector3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        [DisplayName("X")]
        [Description("The X axis value")]
        [ScriptMember(ScriptAccess.Full)]
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
        [ScriptMember(ScriptAccess.Full)]
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
        [ScriptMember(ScriptAccess.Full)]
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
        [ScriptMember(ScriptAccess.ReadOnly)]
        public double magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        [Browsable(false)]
        [ScriptMember(ScriptAccess.ReadOnly)]
        public Vector3 normalized
        {
            get
            {
                double m = magnitude;
                return new Vector3(x / m, y / m, z / m);
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

        [ScriptMember(ScriptAccess.Full)]
        public void setTo(float n)
        {
            _x = n;
            _y = n;
            _z = n;
            NotifyPropertyChanged("XYZ");
        }

        [ScriptMember(ScriptAccess.Full)]
        public void setTo(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            NotifyPropertyChanged("XYZ");
        }

        public void setToPhysics(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            NotifyPropertyChanged("NOPHYSICS");
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 add(Vector3 other)
        {
            return (this + other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 add(float other)
        {
            return (this + other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 subtract(Vector3 other)
        {
            return (this - other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 subtract(float other)
        {
            return (this - other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 multiply(Vector3 other)
        {
            return (this * other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 multiply(float other)
        {
            return (this * other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 divide(Vector3 other)
        {
            return (this / other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public Vector3 divide(float other)
        {
            return (this / other);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator +(Vector3 x, Vector3 y)
        {
            return new Vector3(x.x + y.x, x.y + y.y, x.z + y.z);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator +(Vector3 x, float y)
        {
            return new Vector3(x.x + y, x.y + y, x.z + y);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator -(Vector3 x, Vector3 y)
        {
            return new Vector3(x.x - y.x, x.y - y.y, x.z - y.z);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator -(Vector3 x, float y)
        {
            return new Vector3(x.x - y, x.y - y, x.z - y);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator *(Vector3 x, Vector3 y)
        {
            return new Vector3(x.x * y.x, x.y * y.y, x.z * y.z);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator *(Vector3 x, float y)
        {
            return new Vector3(x.x * y, x.y * y, x.z * y);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator /(Vector3 x, Vector3 y)
        {
            return new Vector3(x.x / y.x, x.y / y.y, x.z / y.z);
        }

        [ScriptMember(ScriptAccess.Full)]
        public static Vector3 operator /(Vector3 x, float y)
        {
            return new Vector3(x.x / y, x.y / y, x.z / y);
        }

        public override string ToString()
        {
            return _x + ", " + _y + ", " + _z;
        }
    }
}
