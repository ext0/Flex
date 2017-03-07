using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Flex.Development.Rendering.Modules;
using Flex.Development.Instances;
using System.Windows;
using Caliburn.Micro;
using Gemini.Modules.Shell.Views;
using Flex.Modules.Scene.ViewModels;
using System.Windows.Interop;
using Flex.Development.Physics;
using Flex.Development.Execution.Data;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using Flex.Modules.Scene.Views;
using Gemini.Modules.Output;
using System.Windows.Forms;

namespace Flex.Development.Rendering
{
    static class Engine
    {
        private static MogreRenderer _renderer;
        private static Window _window;

        private static bool _initialized = false;

        private static Queue<System.Action> _preInitializationActions = new Queue<System.Action>();

        private static Point _mousePosition;

        private static SceneView _view;
        private static SceneNode _hoverSceneNode;
        private static System.Windows.Forms.Panel _panel;

        private static double _wA = 1;
        private static double _aA = 1;
        private static double _sA = 1;
        private static double _dA = 1;

        private static double _defaultSpeed = 0.08d * 4;

        private static Queue<System.Action> _renderDispatcherActionQueue = new Queue<System.Action>();

        public static MogreRenderer Renderer
        {
            get
            {
                return _renderer;
            }
        }

        public static bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public static void QueueInitializationAction(System.Action action)
        {
            if (_initialized)
            {
                RunOnUIThread(action);
            }
            lock (_preInitializationActions)
            {
                _preInitializationActions.Enqueue(action);
            }
        }

        public static void Initialize(SceneViewModel view)
        {
            _renderer = new MogreRenderer();
            _view = view.View;

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();

            _panel = new Panel();
            _panel.Name = "MogrePanel";
            _panel.Location = new System.Drawing.Point(0, 0);
            _panel.Size = new System.Drawing.Size((int)_view.RenderWindow.Width, (int)_view.RenderWindow.Height);
            //_panel.Resize += _panel_Resize;

            host.Child = _panel;

            _view.RenderWindow.Children.Add(host);

            new Thread(() =>
            {
                RunOnUIThread(() =>
                {
                    _renderer.Init(_panel.Handle.ToString(), (uint)_panel.Width, (uint)_panel.Height);

                    _renderer.CreateScene();

                    while (_preInitializationActions.Count != 0)
                    {
                        _preInitializationActions.Dequeue().Invoke();
                    }
                    _initialized = true;

                    _panel.MouseMove += _panel_MouseMove;
                    _panel.MouseWheel += _panel_MouseWheel;

                    host.KeyDown += Host_KeyDown;
                    host.KeyUp += Host_KeyUp;

                    Thread thread = new Thread(() =>
                    {
                        Mogre.Timer timer = new Mogre.Timer();
                        int attemptedFrameRate = 90;
                        while (true)
                        {
                            lock (_renderDispatcherActionQueue)
                            {
                                while (_renderDispatcherActionQueue.Count != 0)
                                {
                                    _renderDispatcherActionQueue.Dequeue()?.Invoke();
                                }
                            }
                            uint elapsed = timer.Milliseconds;
                            timer.Reset();

                            int wait = (int)((1000 / attemptedFrameRate) - elapsed);

                            if (wait > 0)
                            {
                                Thread.Sleep(wait);
                            }

                            KeyboardTick();
                            PhysicsEngine.Step();
                            Renderer.Loop();
                        }
                    });

                    thread.Start();
                });
            }).Start();
        }

        private static void _panel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                _renderer.Camera.Move(_renderer.Camera.Direction * (e.Delta / 10f));
            }
        }

        private static void _panel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_mousePosition == null)
            {
                _mousePosition = new Point(e.X, e.Y);
                return;
            }
            Point point = new Point(e.X, e.Y);
            QueueForRenderDispatcher(() =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    double deltaDirectionX = point.X - _mousePosition.X;
                    double deltaDirectionY = point.Y - _mousePosition.Y;
                    _renderer.Camera.Pitch((float)-deltaDirectionY / 200f);
                    _renderer.Camera.Yaw((float)-deltaDirectionX / 200f);
                }

                Ray mouseRay = _renderer.Camera.GetCameraToViewportRay((float)(point.X / _view.ActualWidth), (float)(point.Y / _view.ActualHeight));

                RaySceneQuery mRaySceneQuery = _renderer.Scene.CreateRayQuery(mouseRay);

                RaySceneQueryResult result = mRaySceneQuery.Execute();
                RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

                bool found = false;
                if (itr != null)
                {
                    while (itr.MoveNext())
                    {
                        RaySceneQueryResultEntry entry = itr.Current;
                        entry.movable.ParentSceneNode.ShowBoundingBox = true;
                        if (!entry.movable.ParentSceneNode.Equals(_hoverSceneNode))
                        {
                            if (_hoverSceneNode != null)
                            {
                                _hoverSceneNode.ShowBoundingBox = false;
                            }
                            _hoverSceneNode = entry.movable.ParentSceneNode;
                        }
                        found = true;
                        break;
                    }
                }

                if (!found && _hoverSceneNode != null)
                {
                    _hoverSceneNode.ShowBoundingBox = false;
                    _hoverSceneNode = null;
                }

                _mousePosition = point;
            });
        }

        private static void Host_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyUp, key);
            }
        }

        private static void Host_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyDown, key);
            }
        }

        private static void _panel_Resize(object sender, EventArgs e)
        {
            if (_renderer != null)
            {
                _renderer.ReinitRenderWindow(_panel.Handle.ToString(), (uint)_panel.Width, (uint)_panel.Height);
                _renderer.OnResize();
            }
        }

        private static void KeyboardTick()
        {
            foreach (int key in ActiveScene.GetRegisteredKeyPressKeys())
            {
                bool down = false;
                RunOnUIThread(() =>
                {
                    down = Keyboard.IsKeyDown((Key)key);
                });
                if (down)
                {
                    ActiveScene.RunKeyCallback(KeyAction.KeyPress, key);
                }
            }

            bool none = true;

            bool aDown = false;
            bool wDown = false;
            bool sDown = false;
            bool dDown = false;
            double shiftMultiplier = 1.0d;

            RunOnUIThread(() =>
            {
                wDown = Keyboard.IsKeyDown(Key.W);
                aDown = Keyboard.IsKeyDown(Key.A);
                sDown = Keyboard.IsKeyDown(Key.S);
                dDown = Keyboard.IsKeyDown(Key.D);
                shiftMultiplier = Keyboard.IsKeyDown(Key.LeftShift) ? 2.0d : shiftMultiplier;
            });
            if (wDown)
            {
                _renderer.Camera.Move(_renderer.Camera.Direction * (float)(_defaultSpeed * _wA * shiftMultiplier));
                none = false;
            }

            if (aDown)
            {
                _renderer.Camera.Move(-_renderer.Camera.Right * (float)(_defaultSpeed * _aA * shiftMultiplier));
                none = false;
            }

            if (sDown)
            {
                _renderer.Camera.Move(-_renderer.Camera.Direction * (float)(_defaultSpeed * _sA * shiftMultiplier));
                none = false;
            }

            if (dDown)
            {
                _renderer.Camera.Move(_renderer.Camera.Right * (float)(_defaultSpeed * _dA * shiftMultiplier));
                none = false;
            }

            if (none)
            {
                _wA = 1;
                _aA = 1;
                _sA = 1;
                _dA = 1;
            }
        }

        public static void Destroy(PositionedInstance instance)
        {
            instance.SceneNode.RemoveAndDestroyAllChildren();
            _renderer.Scene.DestroySceneNode(instance.SceneNode);
        }

        public static void QueueForRenderDispatcher(System.Action action)
        {
            lock (_renderDispatcherActionQueue)
            {
                _renderDispatcherActionQueue.Enqueue(action);
            }
        }

        public static void RunOnUIThread(System.Action action)
        {
            if (_window == null) //pre initialization actions incorrectly calling
            {
                if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                {
                    action.Invoke();
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                }
                return;
            }

            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
        }

        public static void PhysicsStep()
        {
            if (ActiveScene.Running)
            {
                PhysicsEngine.Step();
            }
        }

        public static void Shutdown()
        {
            Renderer.Shutdown();
        }
    }
}
