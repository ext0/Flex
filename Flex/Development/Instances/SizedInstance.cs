using Flex.Development.Instances.Properties;
using Flex.Misc.Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    public abstract class SizedInstance : PositionedInstance
    {
        protected Vector3Property _size;

        protected SizedInstance() : base()
        {

        }

        [Category("3D")]
        [DisplayName("Size")]
        [Description("The size of this instance")]
        [ExpandableObject]
        [TrackMember]
        public Vector3Property size
        {
            get
            {
                return _size;
            }
        }
    }
}
