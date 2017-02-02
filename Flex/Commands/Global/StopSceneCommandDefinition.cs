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
    public class StopSceneCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Scene.Stop";

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
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/stop.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<StopSceneCommandDefinition>(new KeyGesture(Key.T, ModifierKeys.Control));
    }
}
