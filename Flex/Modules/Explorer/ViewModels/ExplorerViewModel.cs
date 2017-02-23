using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Misc.Utility;
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
using System.Windows.Controls;

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

        public bool SelectInstance(Instance o)
        {
            TreeViewItem item = SelectItemHelper(_view.ActiveInstances, o);
            if (item == null)
            {
                return false;
            }
            item.IsSelected = true;
            return true;
        }

        private TreeViewItem SelectItemHelper(ItemsControl control, Instance o)
        {
            //Search for the object model in first level children (recursively)
            TreeViewItem tvi = control.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (tvi != null) return tvi;
            //Loop through user object models
            foreach (object i in control.Items)
            {
                //Get the TreeViewItem associated with the iterated object model
                TreeViewItem tvi2 = control.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                tvi = SelectItemHelper(tvi2, o);
                if (tvi != null) return tvi;
            }
            return null;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = view as ExplorerView;
            _view.DataContext = ActiveScene.Context;

            //(_view.ActiveInstances.ItemsSource as UISafeObservableCollection<Instance>).ListChanged += ExplorerView_ListChanged;
        }

        private void ExplorerView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {

        }

        public void Update()
        {

        }
    }
}
