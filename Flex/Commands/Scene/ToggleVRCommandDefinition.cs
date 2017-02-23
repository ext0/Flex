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
    public class ToggleVRCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Scene.ToggleVR";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_ToggleVR"; }
        }

        public override string ToolTip
        {
            get { return "Toggle Virtual Reality"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/webcam.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<AddScriptCommandDefinition>(new KeyGesture(Key.V, ModifierKeys.Alt));
    }
}
