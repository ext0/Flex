using Flex.Commands.Global;
using Flex.Modules.Explorer.Commands;
using Flex.Modules.External.Commands;
using Flex.Properties;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Modules.Shell.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.External
{
    public static class MenuDefinitions
    {
        [Export]
        public static ExcludeMenuItemGroupDefinition ExcludeToolsOptionsMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.ViewPropertiesMenuGroup);

        [Export]
        public static ExcludeMenuItemGroupDefinition ExcludeFileNewMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup);

        [Export]
        public static ExcludeMenuItemGroupDefinition ExcludeFileSaveMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.FileSaveMenuGroup);

        [Export]
        public static ExcludeMenuItemGroupDefinition ExcludeFileCloseMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.FileCloseMenuGroup);

        [Export]
        public static MenuItemGroupDefinition FileNewMenuItemGroup = new MenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.FileMenu, 0);

        [Export]
        public static MenuItemDefinition OpenProjectMenuItem = new CommandMenuItemDefinition<OpenProjectCommandDefinition>(FileNewMenuItemGroup, 0);

        [Export]
        public static MenuItemDefinition SaveProjectMenuItem = new CommandMenuItemDefinition<SaveProjectCommandDefinition>(FileNewMenuItemGroup, 1);

        [Export]
        public static MenuItemDefinition ViewPropertyGridMenuItem = new CommandMenuItemDefinition<ViewPropertyGridCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);

        [Export]
        public static MenuDefinition ProjectMenu = new MenuDefinition(Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar, 3, "Project");

        [Export]
        public static MenuItemGroupDefinition ScriptMenuGroup = new MenuItemGroupDefinition(ProjectMenu, 0);

        [Export]
        public static MenuItemDefinition ViewExplorerMenuItem = new CommandMenuItemDefinition<ViewExplorerCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);

        [Export]
        public static MenuItemDefinition ImportScriptMenuItem = new CommandMenuItemDefinition<ImportScriptCommandDefinition>(
            ScriptMenuGroup, 0);
    }
}
