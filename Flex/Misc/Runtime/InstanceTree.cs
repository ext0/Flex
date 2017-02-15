using Flex.CustomTreeView;
using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Runtime
{
    public class InstanceTree : TreeViewBase<Instance>
    {
        //the sample uses the category's name as the identifier
        public override string GetItemKey(Instance item)
        {
            return item.name + item.GetHashCode() + item.type;
        }

        //returns subcategories that should be available through the tree

        public override ICollection<Instance> GetChildItems(Instance parent)
        {
            return parent.Children;
        }

        //get the parent category, or null if it's a root category
        public override Instance GetParentItem(Instance item)
        {
            return item.parent;
        }
    }
}
