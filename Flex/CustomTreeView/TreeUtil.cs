using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Flex.CustomTreeView
{
    /// <summary>
    /// Provides static helper methods.
    /// </summary>
    public static class TreeUtil
    {
        /// <summary>
        /// Checks whether a given tree node contains a dummy node to
        /// ensure it's rendered with an expander, and removes the node.
        /// </summary>
        /// <param name="treeNode">The node to be checked for dummy
        /// child nodes.</param>
        public static void ClearDummyChildNode(TreeViewItem treeNode)
        {
            //if the item has never been expanded yet, it contains a dummy
            //node - replace that one and insert real data
            if (ContainsDummyNode(treeNode))
            {
                treeNode.Items.Clear();
            }
        }


        /// <summary>
        /// Validates whether a given node contains a single dummy item,
        /// which was added to ensure the submitted tree node renders
        /// an expander.
        /// </summary>
        /// <param name="treeNode">The tree node to be validated.</param>
        /// <returns>True if the node contains a dummy item.</returns>
        public static bool ContainsDummyNode(TreeViewItem treeNode)
        {
            return treeNode.Items.Count == 1 && ((TreeViewItem)treeNode.Items[0]).Header == null;
        }


        /// <summary>
        /// Determines whether a given tree node's <see cref="ItemsControl.Items"/>
        /// collection needs to be explicitely refreshed if it was changed. This
        /// is the case if the collection is either sorted or filtered. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool NeedsRefreshOnChildsUpdate(TreeViewItem node)
        {
            //currently, node.Items.NeedsRefresh is used which works fine
            return node.Items.SortDescriptions.Count > 0 ||
                   node.Items.Filter != null;
        }


        /// <summary>
        /// Recursively browses all descendants of a given item, starting at
        /// the item's child collection
        /// </summary>
        /// <param name="nodes">A collection of <see cref="TreeViewItem"/>
        /// instances to be processed recursively.</param>
        /// <returns>An enumerator for the tree's items, starting with the
        /// submitted <paramref name="nodes"/> collection.</returns>
        public static IEnumerable<TreeViewItem> BrowseNodes(ItemCollection nodes)
        {
            //process child groups
            foreach (TreeViewItem node in nodes)
            {
                if (node.Header == null)
                {
                    yield break;
                }
                else
                {
                    yield return node;

                    foreach (TreeViewItem item in BrowseNodes(node.Items))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
