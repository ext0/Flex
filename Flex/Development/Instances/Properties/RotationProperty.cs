using Flex.Misc.Tracker;
using Jitter.LinearMath;
using Microsoft.ClearScript;
using SharpDX;
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
        [TrackMember]
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
        [TrackMember]
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
        [TrackMember]
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

        private Matrix3D MatrixFromEulerAngles(float x, float y, float z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0) * matrix, y));
            matrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }

        [ScriptMember(ScriptAccess.None)]
        [Browsable(false)]
        public JMatrix JMatrix
        {
            get
            {
                return new JMatrix(
                    (float)_matrix.M11, (float)_matrix.M12, (float)_matrix.M13,
                    (float)_matrix.M21, (float)_matrix.M22, (float)_matrix.M23,
                    (float)_matrix.M31, (float)_matrix.M32, (float)_matrix.M33
                );
            }
        }

        [ScriptMember(ScriptAccess.None)]
        public void setTo(JMatrix matrix)
        {
            _matrix.M11 = matrix.M11;
            _matrix.M12 = matrix.M12;
            _matrix.M13 = matrix.M13;
            _matrix.M21 = matrix.M21;
            _matrix.M22 = matrix.M22;
            _matrix.M23 = matrix.M23;
            _matrix.M31 = matrix.M31;
            _matrix.M32 = matrix.M32;
            _matrix.M33 = matrix.M33;

            double sy = Math.Sqrt((matrix.M11 * matrix.M11) + (matrix.M21 * matrix.M21));

            bool singular = sy < 1e-6;

            if (!singular)
            {
                _x = MathUtil.RadiansToDegrees((float)Math.Atan2(matrix.M32, matrix.M33));
                _y = MathUtil.RadiansToDegrees((float)Math.Atan2(-matrix.M31, sy));
                _z = MathUtil.RadiansToDegrees((float)Math.Atan2(matrix.M21, matrix.M11));
            }
            else
            {
                _x = MathUtil.RadiansToDegrees((float)Math.Atan2(-matrix.M23, matrix.M22));
                _y = MathUtil.RadiansToDegrees((float)Math.Atan2(-matrix.M31, sy));
                _z = 0;
            }

            NotifyPropertyChanged("XYZ");
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

        public override string ToString()
        {
            return _x + ", " + _y + ", " + _z;
        }
    }
}
