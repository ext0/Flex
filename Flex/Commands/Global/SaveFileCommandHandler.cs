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
    public class SaveFileCommandHandler : CommandHandlerBase<SaveFileCommandDefinition>
    {
        public override void Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        public override async Task Run(Command command)
        {
            if (!ActiveScene.Running)
            {
                byte[] data = ActiveScene.SerializedContext;
                System.Diagnostics.Debug.WriteLine("Save: " + data.Length + " bytes!");
            }
        }
    }
}
