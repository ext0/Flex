using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flex.Commands.Scene
{
    [CommandDefinition]
    public class SelectPointerGizmoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Gizmo.SelectPointer";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_SelectPointer"; }
        }

        public override string ToolTip
        {
            get { return "Select Pointer"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/cursor.png"); }
        }
    }
}
