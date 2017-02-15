using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.External.Commands
{
    [CommandDefinition]
    public class ViewPropertyGridCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.PropertyGrid";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Properties"; }
        }

        public override string ToolTip
        {
            get { return "Open property grid"; }
        }
    }
}
