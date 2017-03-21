using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Data.Data
{
    public class InstancePair
    {
        private Instance _old;
        private Instance _current;

        public InstancePair(Instance old, Instance current)
        {
            _old = old;
            _current = current;
        }

        public Instance Old
        {
            get
            {
                return _old;
            }
        }

        public Instance Current
        {
            get
            {
                return _current;
            }
        }
    }
}
