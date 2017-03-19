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
    public class Rotation : NotifyPropertyChangedObject
    {
        private float _x;
        private float _y;
        private float _z;

        private Matrix3D _matrix;

        public Rotation()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            _matrix = new Matrix3D();
        }

        public Rotation(double n)
        {
            _x = (float)n;
            _y = (float)n;
            _z = (float)n;
            _matrix = MatrixFromEulerAngles(_x, _y, _z);
        }

        public Rotation(double x, double y, double z)
        {
            _x = (float)x;
            _y = (float)y;
            _z = (float)z;
            _matrix = MatrixFromEulerAngles(_x, _y, _z);
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
                _matrix = MatrixFromEulerAngles(_x, _y, _z);
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
                _matrix = MatrixFromEulerAngles(_x, _y, _z);
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
                _matrix = MatrixFromEulerAngles(_x, _y, _z);
                NotifyPropertyChanged("Z");
            }
        }

        public void LoadFromMatrix(Mogre.Matrix3 matrix)
        {
            _matrix.M11 = matrix.m00;
            _matrix.M12 = matrix.m01;
            _matrix.M13 = matrix.m02;
            _matrix.M21 = matrix.m10;
            _matrix.M22 = matrix.m11;
            _matrix.M23 = matrix.m12;
            _matrix.M31 = matrix.m20;
            _matrix.M32 = matrix.m21;
            _matrix.M33 = matrix.m22;

            double sy = Math.Sqrt((_matrix.M11 * _matrix.M11) + (_matrix.M21 * _matrix.M21));

            bool singular = sy < 1e-6;

            if (!singular)
            {
                _x = Mogre.Math.RadiansToDegrees((float)Math.Atan2(_matrix.M32, _matrix.M33));
                _y = Mogre.Math.RadiansToDegrees((float)Math.Atan2(-_matrix.M31, sy));
                _z = Mogre.Math.RadiansToDegrees((float)Math.Atan2(_matrix.M21, _matrix.M11));
            }
            else
            {
                _x = Mogre.Math.RadiansToDegrees((float)Math.Atan2(-_matrix.M23, _matrix.M22));
                _y = Mogre.Math.RadiansToDegrees((float)Math.Atan2(-_matrix.M31, sy));
                _z = 0;
            }

            NotifyPropertyChanged("NOPHYSICS");
        }

        private Matrix3D MatrixFromEulerAngles(float x, float y, float z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix, y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }

        [ScriptMember(ScriptAccess.Full)]
        public void setTo(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            _matrix = MatrixFromEulerAngles(_x, _y, _z);
            NotifyPropertyChanged("XYZ");
        }

        [Browsable(false)]
        public Matrix3D Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public Quaternion Quaternion
        {
            get
            {
                double w = Math.Sqrt(1.0 + _matrix.M11 + _matrix.M22 + _matrix.M33) / 2.0;
                double w4 = w * 4.0;
                double x = (_matrix.M32 - _matrix.M23) / w4;
                double y = (_matrix.M13 - _matrix.M31) / w4;
                double z = (_matrix.M21 - _matrix.M12) / w4;
                return new Quaternion(x, y, z, w);
            }
        }

        public override string ToString()
        {
            return _x + ", " + _y + ", " + _z;
        }
    }
}
