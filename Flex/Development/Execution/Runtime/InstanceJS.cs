using Flex.Development.Execution.Data;
using Flex.Development.Execution.Runtime.Attributes;
using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    [DynamicExposedClass]
    public class InstanceJS
    {
        public InstanceJS()
        {

        }

        private delegate DynamicJS CreateDelegate(String type);

        [DynamicExposedMethod(typeof(CreateDelegate), "create")]
        public DynamicJS Create(String type)
        {
            if (type.ToLower().Equals("script"))
            {
                return new DynamicJS(ActiveScene.AddInstance<Script>());
            }
            else if (type.ToLower().Equals("part"))
            {
                return new DynamicJS(ActiveScene.AddInstance<Part>());
            }
            else {
                throw new TypeAccessException("Invalid type creation (\"" + type + "\") failed!");
            }
        }
    }
}
