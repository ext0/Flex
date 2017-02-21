﻿using Flex.Misc.Tracker;
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
    public class ColorProperty : NotifyPropertyChangedObject
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;

        internal ColorProperty(byte r, byte g, byte b, byte alpha)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = alpha;
        }

        internal ColorProperty(Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
        }

        public byte r
        {
            get
            {
                return _r;
            }
            set
            {
                _r = value;
                NotifyPropertyChanged("R");
            }
        }

        public byte g
        {
            get
            {
                return _g;
            }
            set
            {
                _r = value;
                NotifyPropertyChanged("G");
            }
        }

        public byte b
        {
            get
            {
                return _b;
            }
            set
            {
                _b = value;
                NotifyPropertyChanged("B");
            }
        }

        public byte transparency
        {
            get
            {
                return _a;
            }
            set
            {
                _a = value;
                NotifyPropertyChanged("A");
            }
        }

        public void changeColor(byte r, byte g, byte b, byte alpha)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = alpha;
            NotifyPropertyChanged("Color");
        }

        public void changeColor(Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
            NotifyPropertyChanged("Color");
        }

        public Color color
        {
            get
            {
                return Color.FromArgb(_a, _r, _g, _b);
            }
        }
    }
}
