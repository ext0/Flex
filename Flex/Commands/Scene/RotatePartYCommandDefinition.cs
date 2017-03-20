using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
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
    public class RotatePartYCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.RotatePartY";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_RotatePartY"; }
        }

        public override string ToolTip
        {
            get { return "Rotate Part (Y Axis)"; }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<RotatePartYCommandDefinition>(new KeyGesture(Key.R, ModifierKeys.Control));
    }
}
