using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Rendering.Modules
{
    public enum QueryFlags
    {
        INSTANCE_ENTITY = 1 << 0,
        NON_INSTANCE_ENTITY = 1 << 1
    }
}
