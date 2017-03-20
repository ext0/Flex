using Caliburn.Micro;
using Flex.Commands.Scene;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Development.Rendering.Modules;
using Flex.Misc.Runtime;
using Flex.Misc.Utility;
using Flex.Modules.Scene.Views;
using Flex.Modules.ScriptEditor.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Mogre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Flex.Modules.Scene.ViewModels
{
    [DisplayName("Scene View Model")]
    [Export]
    public class SceneViewModel : Document,
        ICommandHandler<AddPartCommandDefinition>,
        ICommandHandler<AddScriptCommandDefinition>,
        ICommandHandler<ToggleVRCommandDefinition>,
        ICommandHandler<CopyInstanceCommandDefinition>,
        ICommandHandler<PasteInstanceCommandDefinition>,
        ICommandHandler<DeleteInstanceCommandDefinition>,
        ICommandHandler<RotatePartYCommandDefinition>,
        ICommandHandler<SelectPointerGizmoCommandDefinition>,
        ICommandHandler<SelectTranslateGizmoCommandDefinition>,
        ICommandHandler<SelectScaleGizmoCommandDefinition>,
        ICommandHandler<SelectRotateGizmoCommandDefinition>
    {
        private SceneView _sceneView;

        public SceneView View
        {
            get
            {
                return _sceneView;
            }
        }

        public SceneViewModel()
        {
            DisplayName = "World";
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(false);
        }

        protected override void OnViewLoaded(object view)
        {
            _sceneView = view as SceneView;

            Engine.Initialize(this);
        }


        private void KeyboardTick()
        {
            try
            {

            }
            catch
            {

            }
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        void ICommandHandler<ToggleVRCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<ToggleVRCommandDefinition>.Run(Command command)
        {
            ActiveScene.IsVR = !ActiveScene.IsVR;
            //Engine.ToggleVR();
            command.Checked = ActiveScene.IsVR;

            return TaskUtility.Completed;
        }

        void ICommandHandler<AddPartCommandDefinition>.Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        Task ICommandHandler<AddPartCommandDefinition>.Run(Command command)
        {
            ActiveScene.AddInstance<Part>();

            return TaskUtility.Completed;
        }

        void ICommandHandler<AddScriptCommandDefinition>.Update(Command command)
        {
            command.Enabled = !ActiveScene.Running;
        }

        Task ICommandHandler<AddScriptCommandDefinition>.Run(Command command)
        {
            Script script = ActiveScene.AddInstance<Script>() as Script;

            IoC.Get<IShell>().OpenDocument(new ScriptViewModel(script));

            return TaskUtility.Completed;
        }

        void ICommandHandler<CopyInstanceCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<CopyInstanceCommandDefinition>.Run(Command command)
        {
            Instance selected = ActiveScene.SelectedInstance;
            if (selected != null)
            {
                ActiveScene.CopiedInstance = selected;
            }
            return TaskUtility.Completed;
        }

        void ICommandHandler<PasteInstanceCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<PasteInstanceCommandDefinition>.Run(Command command)
        {
            if (ActiveScene.CopiedInstance != null)
            {
                Instance instance = ActiveScene.CopiedInstance.clone();
                SizedInstance positioned = instance as SizedInstance;
                if (positioned != null)
                {
                    Vector3 vector = Engine.GetBestLocationFromYDown(
                        new Vector3(
                            positioned.position.x + (positioned.size.x / 2),
                            positioned.position.y + (positioned.size.y / 2),
                            positioned.position.z + (positioned.size.z / 2)
                        ),
                        positioned.position.y + (positioned.size.y / 2),
                        positioned.size.y / 2);
                    positioned.position.y = vector.y;
                }
            }
            return TaskUtility.Completed;
        }

        void ICommandHandler<DeleteInstanceCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<DeleteInstanceCommandDefinition>.Run(Command command)
        {
            Instance selected = ActiveScene.SelectedInstance;
            if (selected != null)
            {
                try
                {
                    selected.remove();
                }
                catch
                {

                }
                finally
                {
                    ActiveScene.SelectedInstance = null;
                }
            }
            return TaskUtility.Completed;
        }

        void ICommandHandler<RotatePartYCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<RotatePartYCommandDefinition>.Run(Command command)
        {
            Instance selected = ActiveScene.SelectedInstance;
            if (selected != null)
            {
                PositionedInstance positioned = selected as PositionedInstance;
                if (positioned != null)
                {
                    positioned.rotation.y = (positioned.rotation.y + 90) % 360;
                }
            }
            return TaskUtility.Completed;
        }

        void ICommandHandler<SelectPointerGizmoCommandDefinition>.Update(Command command)
        {
            command.Checked = (ActiveScene.ActiveGizmoType == GizmoType.POINTER);
        }

        Task ICommandHandler<SelectPointerGizmoCommandDefinition>.Run(Command command)
        {
            ActiveScene.ActiveGizmoType = GizmoType.POINTER;
            return TaskUtility.Completed;
        }

        void ICommandHandler<SelectTranslateGizmoCommandDefinition>.Update(Command command)
        {
            command.Checked = (ActiveScene.ActiveGizmoType == GizmoType.TRANSLATE);
        }

        Task ICommandHandler<SelectTranslateGizmoCommandDefinition>.Run(Command command)
        {
            ActiveScene.ActiveGizmoType = GizmoType.TRANSLATE;
            return TaskUtility.Completed;
        }

        void ICommandHandler<SelectScaleGizmoCommandDefinition>.Update(Command command)
        {
            command.Checked = (ActiveScene.ActiveGizmoType == GizmoType.SCALE);
        }

        Task ICommandHandler<SelectScaleGizmoCommandDefinition>.Run(Command command)
        {
            ActiveScene.ActiveGizmoType = GizmoType.SCALE;
            return TaskUtility.Completed;
        }

        void ICommandHandler<SelectRotateGizmoCommandDefinition>.Update(Command command)
        {
            command.Checked = (ActiveScene.ActiveGizmoType == GizmoType.ROTATE);
        }

        Task ICommandHandler<SelectRotateGizmoCommandDefinition>.Run(Command command)
        {
            ActiveScene.ActiveGizmoType = GizmoType.ROTATE;
            return TaskUtility.Completed;
        }
    }
}
