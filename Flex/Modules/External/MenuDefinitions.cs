using Flex.Modules.External.Commands;
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
        public static MenuItemDefinition ViewPropertyGridMenuItem = new CommandMenuItemDefinition<ViewPropertyGridCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);
    }
}
