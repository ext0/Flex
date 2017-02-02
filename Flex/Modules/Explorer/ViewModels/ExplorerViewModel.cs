using Flex.Development.Execution.Data;
using Flex.Modules.Explorer.ViewModels;
using Flex.Modules.Explorer.Views;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.ToolBars;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.Explorer.ViewModels
{
    [Export(typeof(IExplorer))]
    public class ExplorerViewModel : Tool, IExplorer
    {
        private ExplorerView _view;
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }

        public override string DisplayName
        {
            get
            {
                return "Explorer";
            }
        }

        protected override void OnViewLoaded(object view)
        {
            _view =  view as ExplorerView;
            _view.DataContext = ActiveScene.Context;
        }

        public void Update()
        {

        }
    }
}
