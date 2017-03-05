﻿using Caliburn.Micro;
using Flex.Commands.Scene;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Development.Rendering.Modules;
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
    public class SceneViewModel : Document, ICommandHandler<AddPartCommandDefinition>, ICommandHandler<AddScriptCommandDefinition>, ICommandHandler<ToggleVRCommandDefinition>
    {
        private static readonly int KeyboardInputPollingFrequency = 16;

        private CancellationToken _keyboardPollCancelToken;

        private SceneView _sceneView;

        private MogreImage _mogreImage;

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
            _sceneView.MouseWheel += SceneViewMouseWheel;
            _sceneView.KeyDown += SceneViewKeyDown;
            _sceneView.KeyUp += SceneViewKeyUp;

            _sceneView.Render.SizeChanged += OuterRender_SizeChanged;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _keyboardPollCancelToken = cancellationTokenSource.Token;
            Task listener = Task.Factory.StartNew(KeyboardTick, _keyboardPollCancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Engine.Initialize(this);
        }

        private void OuterRender_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_mogreImage != null)
            {
                _mogreImage.SetSize((uint)_sceneView.Render.ActualWidth, (uint)_sceneView.Render.ActualHeight);
            }
        }

        public void BindMogreImage(MogreImage image)
        {
            _sceneView.Render.Source = image;
            _mogreImage = image;
        }

        public Tuple<uint, uint> GetViewPortSize()
        {
            return new Tuple<uint, uint>((uint)_sceneView.Render.ActualWidth, (uint)_sceneView.Render.ActualHeight);
        }

        private void SceneViewKeyUp(object sender, KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyUp, key);
            }
        }

        private void SceneViewKeyDown(object sender, KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyDown, key);
            }
        }

        private void SceneViewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Direction * (e.Delta / 10f));
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
                    Engine.RunOnUIThread(() =>
                    {
                        if (!_sceneView.IsMouseOver)
                        {
                            return;
                        }

                        foreach (int key in ActiveScene.GetRegisteredKeyPressKeys())
                        {
                            if (Keyboard.IsKeyDown((Key)key))
                            {
                                ActiveScene.RunKeyCallback(KeyAction.KeyPress, key);
                            }
                        }

                        bool none = true;
                        if (Keyboard.IsKeyDown(Key.W))
                        {
                            Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Direction * (float)(defaultSpeed * wA));
                            wA = CameraDeltaCalculation(wA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.A))
                        {
                            Engine.Renderer.Camera.Move(-Engine.Renderer.Camera.Right * (float)(defaultSpeed * aA));
                            aA = CameraDeltaCalculation(aA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.S))
                        {
                            Engine.Renderer.Camera.Move(-Engine.Renderer.Camera.Direction * (float)(defaultSpeed * sA));
                            sA = CameraDeltaCalculation(sA, minimum, maximum, acceleration);
                            none = false;
                        }
                        if (Keyboard.IsKeyDown(Key.D))
                        {
                            Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Right * (float)(defaultSpeed * dA));
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
                    });

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

        void ICommandHandler<ToggleVRCommandDefinition>.Update(Command command)
        {

        }

        Task ICommandHandler<ToggleVRCommandDefinition>.Run(Command command)
        {
            ActiveScene.IsVR = !ActiveScene.IsVR;
            //Engine.ToggleVR();
            if (ActiveScene.IsVR)
            {
                command.IconSource = new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/webcam_delete.png");
            }
            else
            {
                command.IconSource = new Uri("pack://application:,,,/Flex;component/Resources/Icons/Legacy/webcam.png");
            }
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
    }
}
