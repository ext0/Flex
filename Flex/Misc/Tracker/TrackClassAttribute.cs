using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Tracker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TrackClassAttribute : Attribute
    {
        public TrackClassAttribute()
        {

        }
    }
}
