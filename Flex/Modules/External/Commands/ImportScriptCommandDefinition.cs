using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.External.Commands
{
    [CommandDefinition]
    public class ImportScriptCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Project.ImportScript";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Import Script"; }
        }

        public override string ToolTip
        {
            get { return "Import existing external script"; }
        }
    }
}
