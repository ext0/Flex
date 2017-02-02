using Flex.Development.Execution.Runtime.Attributes;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Flex.Development.Instances.Properties
{
    [Serializable]
    [DynamicExposedClass]
    public class ColorProperty : NotifyPropertyChangedObject
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;

        public ColorProperty(byte r, byte g, byte b, byte alpha)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = alpha;
        }

        public ColorProperty(Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
        }

        public void ChangeColor(byte r, byte g, byte b, byte alpha)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = alpha;
            NotifyPropertyChanged("Color");
        }

        public void ChangeColor(Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
            NotifyPropertyChanged("Color");
        }

        private delegate void SetToDelegate(byte r, byte g, byte b);

        [DynamicExposedMethod(typeof(SetToDelegate), "setTo")]
        public void SetTo(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            NotifyPropertyChanged("RGB");
        }

        private delegate void SetTransparencyDelegate(double alpha);

        [DynamicExposedMethod(typeof(SetTransparencyDelegate), "setTransparency")]
        public void SetTransparency(double alpha)
        {
            alpha = Math.Round(FlexUtility.ConstrainValue(alpha, 0, 1) * 255);
            _a = (byte)alpha;
            NotifyPropertyChanged("alpha");
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(_a, _r, _g, _b);
            }
        }
    }
}
