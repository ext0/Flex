using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class SizedInstance : PositionedInstance
    {
        protected Vector3 _size;

        protected SizedInstance() : base()
        {

        }

        [Category("3D")]
        [DisplayName("Size")]
        [Description("The size of this instance")]
        [ExpandableObject]
        [ScriptMember(ScriptAccess.Full)]
        public abstract Vector3 size
        {
            get;
            set;
        }
    }
}
