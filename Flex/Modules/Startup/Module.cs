using Caliburn.Micro;
using Flex.Commands.Scene;
using Flex.Modules.Explorer;
using Flex.Modules.Explorer.ViewModels;
using Flex.Modules.Explorer.Views;
using Flex.Modules.Scene.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.Output;
using Gemini.Modules.PropertyGrid;
using Gemini.Modules.PropertyGrid.ViewModels;
using Gemini.Modules.PropertyGrid.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flex.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                yield return IoC.Get<SceneViewModel>();
            }
        }

        [ImportingConstructor]
        public Module()
        {

        }

        public override void Initialize()
        {
            Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            //MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "Flex Development Tool";

            Bitmap bmp = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("Flex.Resources.FlexIcon.png"));
            IntPtr hBmp = bmp.GetHbitmap();
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
            hBmp, IntPtr.Zero, Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());
            MainWindow.Icon = wpfBitmap;

            Shell.StatusBar.AddItem("Flex - By Patrick Bell", new GridLength(1, GridUnitType.Star));

            Shell.OpenDocument(IoC.Get<SceneViewModel>());

            Shell.ShowTool<IOutput>();
            Shell.ShowTool<IPropertyGrid>();
            Shell.ShowTool<IExplorer>();
        }
    }
}
