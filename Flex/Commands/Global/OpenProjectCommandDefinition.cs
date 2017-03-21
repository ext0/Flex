using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Flex.Commands.Global
{
    [CommandDefinition]
    public class OpenProjectCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Project.Open";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Open"; }
        }

        public override string ToolTip
        {
            get { return "Open existing project"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/folder.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<OpenProjectCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control));

        [Export]
        public static ExcludeCommandKeyboardShortcut ExcludeDefaultFileOpenShortcut = new ExcludeCommandKeyboardShortcut(OpenFileCommandDefinition.KeyGesture);
    }
}