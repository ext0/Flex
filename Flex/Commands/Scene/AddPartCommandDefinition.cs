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
    public class AddPartCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.AddPart";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Add"; }
        }

        public override string ToolTip
        {
            get { return "Add Part"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/brick-add.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<AddPartCommandDefinition>(new KeyGesture(Key.U, ModifierKeys.Control));
    }
}
