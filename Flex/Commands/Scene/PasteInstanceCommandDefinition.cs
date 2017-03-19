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
    public class PasteInstanceCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.PasteInstance";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Paste"; }
        }

        public override string ToolTip
        {
            get { return "Paste Instance"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/brick-add.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<PasteInstanceCommandDefinition>(new KeyGesture(Key.V, ModifierKeys.Control));
    }
}
