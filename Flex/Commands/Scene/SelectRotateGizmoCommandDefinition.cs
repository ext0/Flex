using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Commands.Scene
{
    [CommandDefinition]
    public class SelectRotateGizmoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Gizmo.SelectRotate";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_SelectRotate"; }
        }

        public override string ToolTip
        {
            get { return "Select Rotate"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/shape_rotate_clockwise.png"); }
        }
    }
}
