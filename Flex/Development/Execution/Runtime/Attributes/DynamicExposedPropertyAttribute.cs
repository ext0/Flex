using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DynamicExposedPropertyAttribute : Attribute
    {
        private String _name;
        private bool _readOnly;
        public DynamicExposedPropertyAttribute(bool readOnly = false, String name = null)
        {
            _name = name;
            _readOnly = readOnly;
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _readOnly;
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
