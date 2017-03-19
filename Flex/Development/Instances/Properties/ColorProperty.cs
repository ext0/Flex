using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Flex.Development.Instances.Properties
{
    [Serializable]
    public class ColorProperty : NotifyPropertyChangedObject
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;

        public ColorProperty(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = byte.MaxValue;
        }

        public ColorProperty()
        {
            _r = 0;
            _g = 0;
            _b = 0;
            _a = byte.MaxValue;
        }

        internal ColorProperty(byte r, byte g, byte b, byte alpha)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = alpha;
        }

        internal ColorProperty(System.Windows.Media.Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
        }

        [ScriptMember(ScriptAccess.Full)]
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

        [ScriptMember(ScriptAccess.Full)]
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

        [ScriptMember(ScriptAccess.Full)]
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

        internal byte transparency
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

        public void changeColor(System.Windows.Media.Color color)
        {
            _r = color.R;
            _g = color.G;
            _b = color.B;
            _a = color.A;
            NotifyPropertyChanged("Color");
        }

        public System.Windows.Media.Color color
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
            }
        }
    }

    public class ColorEditor : ITypeEditor
    {
        /*
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(ColorProperty))
            {
                return value;
            }

            using (ColorDialog form = new ColorDialog())
            {
                System.Windows.Media.Color color = ((ColorProperty)value).color;
                form.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return new ColorProperty(System.Windows.Media.Color.FromArgb(form.Color.A, form.Color.R, form.Color.G, form.Color.B));
                }
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            System.Windows.Media.Color color = ((ColorProperty)e.Value).color;
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
        }
        */

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            System.Windows.Controls.StackPanel stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            System.Windows.Controls.TextBlock text = new System.Windows.Controls.TextBlock();
            text.Text = "..";
            button.Content = text;
            System.Windows.Controls.TextBlock colorLabel = new System.Windows.Controls.TextBlock();

            ColorProperty property = propertyItem.Value as ColorProperty;

            colorLabel.Text = property.color.ToString();
            stackPanel.Children.Add(colorLabel);
            stackPanel.Children.Add(button);

            button.Click += (sender, e) =>
           {
               using (ColorDialog form = new ColorDialog())
               {
                   System.Windows.Media.Color color = property.color;
                   form.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
                   if (form.ShowDialog() == DialogResult.OK)
                   {
                       property.changeColor(System.Windows.Media.Color.FromArgb(form.Color.A, form.Color.R, form.Color.G, form.Color.B));
                   }
               }
           };

            return stackPanel;
        }
    }

    public class ColorPropertyConverter : System.Drawing.ColorConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            System.Diagnostics.Trace.WriteLine(sourceType.Name);
            return sourceType == typeof(System.Windows.Media.Color);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            System.Diagnostics.Trace.WriteLine(destinationType.Name);
            return destinationType == typeof(System.Windows.Media.Color);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            System.Diagnostics.Trace.WriteLine(value.GetType());
            if (value.GetType() == typeof(System.Windows.Media.Color))
            {
                System.Windows.Media.Color color = (System.Windows.Media.Color)value;
                return new ColorProperty(color);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return (value as ColorProperty).color;
        }
    }
}
