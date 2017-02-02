using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Commands.Global
{
    [CommandHandler]
    public class OpenFileCommandHandler : CommandHandlerBase<OpenFileCommandDefinition>
    {
        public override void Update(Command command)
        {

        }

        public override async Task Run(Command command)
        {
            System.Diagnostics.Debug.WriteLine("Run!");
        }
    }
}
