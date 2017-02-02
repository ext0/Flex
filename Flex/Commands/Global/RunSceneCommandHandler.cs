using Flex.Development.Execution.Data;
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
    public class RunSceneCommandHandler : CommandHandlerBase<RunSceneCommandDefinition>
    {
        public override void Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        public override async Task Run(Command command)
        {
            ActiveScene.Run();
        }
    }
}
