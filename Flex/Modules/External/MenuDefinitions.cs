using Flex.Modules.Explorer.Commands;
using Flex.Modules.External.Commands;
using Flex.Properties;
using Gemini.Framework.Menus;
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
