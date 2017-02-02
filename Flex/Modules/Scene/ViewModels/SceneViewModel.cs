using Caliburn.Micro;
using Flex.Commands.Scene;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Flex.Modules.Scene.Views;
using Flex.Modules.ScriptEditor.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
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
    public class SceneViewModel : Document, ICommandHandler<AddPartCommandDefinition>, ICommandHandler<AddScriptCommandDefinition>
    {
        private static readonly int KeyboardInputPollingFrequency = 16;

        private CancellationToken _keyboardPollCancelToken;

        private SceneView _sceneView;
        private MainDXScene _scene;

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
            _sceneView.MouseWheel += SceneViewMouseWheel;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _keyboardPollCancelToken = cancellationTokenSource.Token;
            Task listener = Task.Factory.StartNew(KeyboardTick, _keyboardPollCancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _scene = new MainDXScene(ActiveScene.Context, _sceneView);
        }

        private void SceneViewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _sceneView.Camera.MoveForward(e.Delta / 10d);
        }

        private double CameraDeltaCalculation(double value, double minimum, double maximum, double acceleration)
        {
            double lowerThreshold = 0.2;
            double upperThreshold = 0.8;

            double delta = FlexUtility.ConstrainValue((value - minimum) / (maximum - minimum), lowerThreshold, upperThreshold);
            delta *= delta * acceleration;
            return FlexUtility.ConstrainValue(value + delta, minimum, maximum);
        }

        private void KeyboardTick()
        {
            try
            {
                double wA = 1;
                double aA = 1;
                double sA = 1;
                double dA = 1;

                double defaultSpeed = 0.08d * 16;

                double acceleration = 0.01;

                double minimum = 1d;

                double maximum = 2d;

                while (true)
                {
                    Thread.Sleep(KeyboardInputPollingFrequency);
                    FlexUtility.RunWindowAction(() =>
                    {
                        if (!_sceneView.IsMouseOver)
                        {
                            return;
                        }
                        bool none = true;
                        if (Keyboard.IsKeyDown(Key.W))
                        {
                            _sceneView.Camera.MoveForward(defaultSpeed * wA);
                            wA = CameraDeltaCalculation(wA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.A))
                        {
                            _sceneView.Camera.MoveLeft(defaultSpeed * aA);
                            aA = CameraDeltaCalculation(aA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.S))
                        {
                            _sceneView.Camera.MoveBackward(defaultSpeed * sA);
                            sA = CameraDeltaCalculation(sA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.D))
                        {
                            _sceneView.Camera.MoveRight(defaultSpeed * dA);
                            dA = CameraDeltaCalculation(dA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (none)
                        {
                            wA = 1;
                            aA = 1;
                            sA = 1;
                            dA = 1;
                        }
                    }, DispatcherPriority.Normal);

                    if (_keyboardPollCancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        public void AddInstance(Instance instance)
        {
            _scene.AddInstance(instance);
        }

        public bool RemoveInstance(Instance instance)
        {
            return _scene.RemoveInstance(instance);
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
    }
}
