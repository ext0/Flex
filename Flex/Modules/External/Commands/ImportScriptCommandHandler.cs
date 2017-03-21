using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Modules.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flex.Modules.External.Commands
{
    [CommandHandler]
    public class ImportScriptCommandHandler : CommandHandlerBase<ImportScriptCommandDefinition>
    {
        private readonly IShell _shell;

        public override void Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        [ImportingConstructor]
        public ImportScriptCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "flex scripts (*.flexscript)|*.flexscript|All files (*.*)|*.*";
            dialog.FilterIndex = 0;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String data = File.ReadAllText(dialog.FileName);
                    Script script = new Script();
                    script.name = Path.GetFileNameWithoutExtension(dialog.FileName);
                    script.source = data;
                }
                catch
                {

                }
            }
            return TaskUtility.Completed;
        }
    }
}
