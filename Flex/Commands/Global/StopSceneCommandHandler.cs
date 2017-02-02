using Flex.Development.Execution.Data;
using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Commands.Global
{
    [CommandHandler]
    public class StopSceneCommandHandler : CommandHandlerBase<StopSceneCommandDefinition>
    {
        public override void Update(Command command)
        {
            command.Enabled = ActiveScene.Running;
        }

        public override async Task Run(Command command)
        {
            ActiveScene.Stop();
        }
    }
}
