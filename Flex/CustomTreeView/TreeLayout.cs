using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.CustomTreeView
{
    /// <summary>
    /// Encapsulates the basic layout (selected / expanded nodes)
    /// of a tree control.
    /// </summary>
    public class TreeLayout
    {
        /// <summary>
        /// The ID of the selected item, if any. Defaults to null
        /// (no node is selected).
        /// </summary>
        private string selectedItemId = null;

        /// <summary>
        /// A list of expanded nodes.
        /// </summary>
        private readonly List<string> expandedNodeIds = new List<string>();

        /// <summary>
        /// The selected group item.
        /// </summary>
        public string SelectedItemId
        {
            get { return selectedItemId; }
            set { selectedItemId = value; }
        }


        /// <summary>
        /// A list of expanded nodes.
        /// </summary>
        public List<string> ExpandedNodeIds
        {
            get { return expandedNodeIds; }
        }


        /// <summary>
        /// Checks whether a given node is supposed to be
        /// expanded or not.
        /// </summary>
        /// <param name="nodeId">The ID of the processed node.</param>
        /// <returns>True if <paramref name="nodeId"/> is contained
        /// in the list of expanded nodes.</returns>
        public bool IsNodeExpanded(string nodeId)
        {
            return expandedNodeIds.Contains(nodeId);
        }
    }
}
