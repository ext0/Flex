using Flex.Commands.Global;
using Flex.Commands.Scene;
using Gemini.Framework.ToolBars;
using Gemini.Modules.UndoRedo.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules
{
    internal static class ToolBarDefinitions
    {
        [Export]
        public static ToolBarDefinition InstanceToolBar = new ToolBarDefinition(0, "Instances");

        [Export]
        public static ToolBarItemGroupDefinition InstanceToolBarGroup = new ToolBarItemGroupDefinition(
            InstanceToolBar, 8);

        [Export]
        public static ToolBarItemDefinition AddPartToolBarItem = new CommandToolBarItemDefinition<AddPartCommandDefinition>(
           InstanceToolBarGroup, 2);

        [Export]
        public static ToolBarItemDefinition AddScriptToolBarItem = new CommandToolBarItemDefinition<AddScriptCommandDefinition>(
            InstanceToolBarGroup, 2);

        [Export]
        public static ToolBarDefinition SceneToolBar = new ToolBarDefinition(0, "Scene");

        [Export]
        public static ToolBarItemGroupDefinition SceneToolBarGroup = new ToolBarItemGroupDefinition(
            SceneToolBar, 8);

        [Export]
        public static ToolBarItemDefinition RunSceneToolBarItem = new CommandToolBarItemDefinition<RunSceneCommandDefinition>(
           SceneToolBarGroup, 2);

        [Export]
        public static ToolBarItemDefinition StopSceneToolBarItem = new CommandToolBarItemDefinition<StopSceneCommandDefinition>(
            SceneToolBarGroup, 2);

        [Export]
        public static ToolBarDefinition VRToolBar = new ToolBarDefinition(0, "VR");

        [Export]
        public static ToolBarItemGroupDefinition VRToolBarGroup = new ToolBarItemGroupDefinition(
            VRToolBar, 8);

        [Export]
        public static ToolBarItemDefinition ToggleVRToolBarItem = new CommandToolBarItemDefinition<ToggleVRCommandDefinition>(
           VRToolBarGroup, 2);
    }
}
