using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using Mogre;
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

        private Matrix3 _matrix;

        public Rotation()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            _matrix = new Matrix3();
        }

        public Rotation(double n)
        {
            _x = (float)n;
            _y = (float)n;
            _z = (float)n;
            _matrix = new Matrix3();
            _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
        }

        public Rotation(double x, double y, double z)
        {
            _x = (float)x;
            _y = (float)y;
            _z = (float)z;
            _matrix = new Matrix3();
            _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
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
                _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
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
                _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
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
                _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
                NotifyPropertyChanged("Z");
            }
        }

        public void LoadFromMatrix(Matrix3 matrix)
        {
            _matrix = matrix;
            NotifyPropertyChanged("NOPHYSICS");
        }

        [ScriptMember(ScriptAccess.Full)]
        public void setTo(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            _matrix.FromEulerAnglesXYZ(new Degree(_x).ValueRadians, new Degree(_y).ValueRadians, new Degree(_z).ValueRadians);
            NotifyPropertyChanged("XYZ");
        }

        [Browsable(false)]
        public Matrix3 Matrix
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
