using Caliburn.Micro;
using Gemini.Modules.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ClearScript.V8;
using System.Threading.Tasks;
using System.Runtime.InteropServices.Expando;

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
            try
            {
                dynamic dynamicResult = obj;
                _output.AppendLine(dynamicResult.toString());
            }
            catch
            {
                _output.AppendLine(obj.ToString());
            }
        }
    }
}
