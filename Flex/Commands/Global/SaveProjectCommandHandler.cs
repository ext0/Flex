using Flex.Development.Execution.Data;
using Flex.Misc.Utility;
using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flex.Commands.Global
{
    [CommandHandler]
    public class SaveProjectCommandHandler : CommandHandlerBase<SaveProjectCommandDefinition>
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
                data = FlexUtility.Compress(data);

                SaveFileDialog saveDialog = new SaveFileDialog();

                saveDialog.AddExtension = true;
                saveDialog.CheckFileExists = false;
                saveDialog.CheckPathExists = true;
                saveDialog.ValidateNames = true;
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.FileName = "MyProject.flex";
                saveDialog.Filter = "flex project (*.flex)|*.flex|All files (*.*)|*.*";
                saveDialog.FilterIndex = 0;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(saveDialog.FileName);
                    file.Directory.Create();
                    File.WriteAllBytes(saveDialog.FileName, data);
                }
            }
        }
    }
}
