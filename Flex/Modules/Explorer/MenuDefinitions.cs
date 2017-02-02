using Flex.Modules.Explorer.Commands;
using Flex.Modules.External.Commands;
using Gemini.Framework.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.Explorer
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewExplorerMenuItem = new CommandMenuItemDefinition<ViewExplorerCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);
    }
}
