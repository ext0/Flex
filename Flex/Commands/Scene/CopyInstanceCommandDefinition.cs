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
    public class CopyInstanceCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.CopyInstance";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Copy"; }
        }

        public override string ToolTip
        {
            get { return "Copy Instance"; }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<CopyInstanceCommandDefinition>(new KeyGesture(Key.C, ModifierKeys.Control));
    }
}
