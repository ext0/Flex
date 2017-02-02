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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flex.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        public static ExcludeMenuItemGroupDefinition ExcludeToolsOptionsMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.ViewPropertiesMenuGroup);

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

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));

            Shell.OpenDocument(IoC.Get<SceneViewModel>());

            Shell.ShowTool<IOutput>();
            Shell.ShowTool<IPropertyGrid>();
            Shell.ShowTool<IExplorer>();
        }
    }
}
