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
    public class AddScriptCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.AddScript";

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
            get { return "Add Script"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/script-add.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<AddScriptCommandDefinition>(new KeyGesture(Key.I, ModifierKeys.Control));
    }
}
