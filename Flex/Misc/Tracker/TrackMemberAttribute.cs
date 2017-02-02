using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Tracker
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrackMemberAttribute : Attribute
    {
        public TrackMemberAttribute()
        {

        }
    }
}
