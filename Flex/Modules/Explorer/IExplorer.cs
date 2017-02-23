using Flex.Development.Instances;
using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.Explorer
{
    public interface IExplorer : ITool
    {
        void Update();

        bool SelectInstance(Instance o);
    }
}
