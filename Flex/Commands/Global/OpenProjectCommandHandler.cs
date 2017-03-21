using Flex.Development.Execution.Data;
using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flex.Commands.Global
{
    [CommandHandler]
    public class OpenProjectCommandHandler : CommandHandlerBase<OpenProjectCommandDefinition>
    {
        public override void Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        public override async Task Run(Command command)
        {
            if (!ActiveScene.Running)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Filter = "flex project (*.flex)|*.flex|All files (*.*)|*.*";
                dialog.FilterIndex = 0;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }
    }
}
