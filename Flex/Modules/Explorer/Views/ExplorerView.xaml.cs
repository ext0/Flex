using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Misc.Utility;
using Flex.Modules.ScriptEditor.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Flex.Modules.Explorer.Views
{
    /// <summary>
    /// Interaction logic for ExplorerViewModel.xaml
    /// </summary>
    public partial class ExplorerView : UserControl
    {
        private Point _startPoint;
        private IPropertyGrid _propertyGrid;

        public ExplorerView()
        {
            InitializeComponent();
            _propertyGrid = IoC.Get<IPropertyGrid>();
        }

        private void ActiveInstancesSelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _propertyGrid.SelectedObject = e.NewValue;
            /*
            foreach (PropertyItem prop in _propertyGrid)
            {
                if (prop.IsExpandable)
                {
                    prop.IsExpanded = true;
                    prop.IsExpandable = false;
                }
            }
            */
            /*
            (_propertyGrid.SelectedObject as Instance).OnChanged += Tracker_Changed;
            */
        }

        private void TreeInstancePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void TreeInstancePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !ActiveScene.Running)
            {
                var mousePos = e.GetPosition(null);
                var diff = _startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    TreeView treeView = sender as TreeView;
                    TreeViewItem treeViewItem = FlexUtility.FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                    if (treeView == null || treeViewItem == null)
                        return;

                    Instance dragging = treeView.SelectedItem as Instance;

                    if (dragging == null)
                        return;

                    DataObject dragData = new DataObject(dragging);

                    try
                    {
                        DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void TreeInstancesDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(e.Data.GetFormats()[0]) && !ActiveScene.Running)
            {
                Instance dragging = e.Data.GetData(e.Data.GetFormats()[0], true) as Instance;
                TreeViewItem treeViewItem =
                    FlexUtility.FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                Instance dropTarget = treeViewItem.Header as Instance;

                if (dropTarget == null || dragging == null)
                    return;

                dragging.parent = dropTarget;

                ActiveInstances.Items.Refresh();
            }
        }

        private void TreeInstancesDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(e.Data.GetFormats()[0]))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void DisplayNameTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox box = sender as TextBox;

            box.Focusable = true;
            box.Focus();

            box.CaretIndex = box.Text.Length;

            e.Handled = true;
        }

        private void DisplayNameTreeViewLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            box.Focusable = false;
        }

        private void DisplayNameTreeViewFocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            box.Cursor = box.Focusable ? Cursors.IBeam : Cursors.Arrow;
        }

        private void DisplayNameTreeViewPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DisplayNameTreeViewLostFocus(sender, e);
            }
        }

        private void DisplayNameTreeViewPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void ImageTreeViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Object context = (sender as Image).DataContext;
                if (context is Script)
                {
                    IShell shell = IoC.Get<IShell>();
                    foreach (Document document in shell.Documents)
                    {
                        if (document is ScriptViewModel)
                        {
                            if ((document as ScriptViewModel).Script.Equals(context as Script))
                            {
                                shell.OpenDocument(document);
                                return;
                            }
                        }
                    }
                    shell.OpenDocument(new ScriptViewModel(context as Script));
                }
            }
        }
    }
}
