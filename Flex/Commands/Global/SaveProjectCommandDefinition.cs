using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Flex.Commands.Global
{
    [CommandDefinition]
    public class SaveProjectCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Project.Save";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Save"; }
        }

        public override string ToolTip
        {
            get { return "Save project"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Flex;component/Resources/Icons/16/disk.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<SaveProjectCommandDefinition>(new KeyGesture(Key.S, ModifierKeys.Control));

        [Export]
        public static ExcludeCommandKeyboardShortcut ExcludeDefaultFileSaveShortcut = new ExcludeCommandKeyboardShortcut(SaveFileCommandDefinition.KeyGesture);
    }
}