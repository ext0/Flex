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

        private static double _wA = 1;
        private static double _aA = 1;
        private static double _sA = 1;
        private static double _dA = 1;

        private static double _defaultSpeed = 0.08d * 16;

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
            _preInitializationActions.Enqueue(action);
        }

        public static void Initialize(SceneViewModel view)
        {
            _renderer = new MogreRenderer();
            _view = view.View;
            Renderer.Init();

            _window = Window.GetWindow(_view);
            WindowInteropHelper handleSource = new WindowInteropHelper(_window);
            Renderer.AttachRenderWindow(handleSource.EnsureHandle());

            _renderer.CreateDefaultScene();

            while (_preInitializationActions.Count != 0)
            {
                _preInitializationActions.Dequeue().Invoke();
            }
            _initialized = true;

            _view.Render.MouseMove += MouseMove;

            view.BindMogreImage(Renderer.CreateMogreImage(view.GetViewPortSize()));

            Thread thread = new Thread(() =>
            {
                Mogre.Timer timer = new Mogre.Timer();
                int attemptedFrameRate = 60;
                while (true)
                {
                    while (_renderDispatcherActionQueue.Count != 0)
                    {
                        _renderDispatcherActionQueue.Dequeue().Invoke();
                    }
                    uint elapsed = timer.Milliseconds;
                    timer.Reset();

                    int wait = (int)((1000 / attemptedFrameRate) - elapsed);

                    System.Diagnostics.Debug.WriteLine(wait);

                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }

                    KeyboardTick();
                    PhysicsEngine.Step();
                    _renderer.Loop();
                }
            });

            thread.Start();
        }

        private static void KeyboardTick()
        {
            foreach (int key in ActiveScene.GetRegisteredKeyPressKeys())
            {
                bool down = false;
                RunOnUIThread(() =>
                {
                    down = System.Windows.Input.Keyboard.IsKeyDown((Key)key);
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

            RunOnUIThread(() =>
            {
                wDown = System.Windows.Input.Keyboard.IsKeyDown(Key.W);
                aDown = System.Windows.Input.Keyboard.IsKeyDown(Key.A);
                sDown = System.Windows.Input.Keyboard.IsKeyDown(Key.S);
                dDown = System.Windows.Input.Keyboard.IsKeyDown(Key.D);
            });
            if (wDown)
            {
                Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Direction * (float)(_defaultSpeed * _wA));
                none = false;
            }

            if (aDown)
            {
                Engine.Renderer.Camera.Move(-Engine.Renderer.Camera.Right * (float)(_defaultSpeed * _aA));
                none = false;
            }

            if (sDown)
            {
                Engine.Renderer.Camera.Move(-Engine.Renderer.Camera.Direction * (float)(_defaultSpeed * _sA));
                none = false;
            }

            if (dDown)
            {
                Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Right * (float)(_defaultSpeed * _dA));
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

        private static void MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePosition == null)
            {
                _mousePosition = e.GetPosition(_view);
                return;
            }
            Point point = e.GetPosition(_view);
            QueueForRenderDispatcher(() =>
            {
                if (e.RightButton == MouseButtonState.Pressed)
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

        public static void Destroy(PositionedInstance instance)
        {
            instance.SceneNode.RemoveAndDestroyAllChildren();
            _renderer.Scene.DestroySceneNode(instance.SceneNode);
        }

        public static void QueueForRenderDispatcher(System.Action action)
        {
            _renderDispatcherActionQueue.Enqueue(action);
        }

        public static void RunOnUIThread(System.Action action)
        {
            if (_window == null) //pre initialization actions incorrectly calling
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                }
                return;
            }

            if (Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
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
