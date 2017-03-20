using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Commands.Scene
{
    [CommandDefinition]
    public class SelectTranslateGizmoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Gizmo.SelectTranslate";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_SelectTranslate"; }
        }

        public override string ToolTip
        {
            get { return "Select Translate"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/shape_move_forwards.png"); }
        }
    }
}
