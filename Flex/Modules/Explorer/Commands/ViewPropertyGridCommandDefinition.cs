using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.Explorer.Commands
{
    [CommandDefinition]
    public class ViewExplorerCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Explorer";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Explorer"; }
        }

        public override string ToolTip
        {
            get { return "Open explorer"; }
        }
    }
}
