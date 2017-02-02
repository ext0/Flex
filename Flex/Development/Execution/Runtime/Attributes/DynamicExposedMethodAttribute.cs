using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DynamicExposedMethodAttribute : Attribute
    {
        private String _name;
        private Type _delegateType;

        public DynamicExposedMethodAttribute(Type delegateType, String name = null)
        {
            _name = name;
            _delegateType = delegateType;
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }

        public Type DelegateType
        {
            get
            {
                return _delegateType;
            }
        }

        public bool HasCustomName
        {
            get
            {
                return _name != null;
            }
        }
    }
}
