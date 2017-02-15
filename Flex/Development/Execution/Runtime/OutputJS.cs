using Caliburn.Micro;
using Gemini.Modules.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public class OutputJS
    {
        private IOutput _output;

        public OutputJS()
        {
            _output = IoC.Get<IOutput>();
        }

        public void print(String line)
        {
            _output.AppendLine(line);
        }

        public void print(Object obj)
        {
            _output.AppendLine(obj.ToString());
        }
    }
}
