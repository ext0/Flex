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
    public class DeleteInstanceCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Instance.DeleteInstance";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Delete"; }
        }

        public override string ToolTip
        {
            get { return "Delete Instance"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/brick-add.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<DeleteInstanceCommandDefinition>(new KeyGesture(Key.Delete));
    }
}
