using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flex.Commands.Global
{
    [CommandDefinition]
    public class RunSceneCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Scene.Run";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Run"; }
        }

        public override string ToolTip
        {
            get { return "Run Scene"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/world-go.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<RunSceneCommandDefinition>(new KeyGesture(Key.R, ModifierKeys.Control));
    }
}
