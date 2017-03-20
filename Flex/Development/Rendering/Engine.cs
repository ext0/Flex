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
using Flex.Modules.Explorer;
using Flex.Misc.Utility;

namespace Flex.Development.Rendering
{
    static class Engine
    {
        private static MogreRenderer _renderer;
        private static System.Windows.Forms.Integration.WindowsFormsHost _host;

        private static bool _initialized = false;

        private static Queue<System.Action> _preInitializationActions = new Queue<System.Action>();
        private static Queue<System.Action> _renderDispatcherActionQueue = new Queue<System.Action>();
        private static Queue<System.Action> _renderNextDispatcherActionQueue = new Queue<System.Action>();

        private static Panel _panel;

        private static SceneNodeStore _sceneNodeStore;
        private static KeyboardHandler _keyboardHandler;
        private static MouseHandler _mouseHandler;

        private static Thread _renderThread;

        public static MogreRenderer Renderer
        {
            get
            {
                return _renderer;
            }
        }

        public static SceneNodeStore SceneNodeStore
        {
            get
            {
                return _sceneNodeStore;
            }
        }

        public static MouseHandler MouseHandler
        {
            get
            {
                return _mouseHandler;
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
            _sceneNodeStore = new SceneNodeStore();
            _host = new System.Windows.Forms.Integration.WindowsFormsHost();


            _panel = new Panel();
            _panel.Name = "MogrePanel";
            _panel.Location = new System.Drawing.Point(0, 0);
            _panel.Size = new System.Drawing.Size((int)view.View.RenderWindow.Width, (int)view.View.RenderWindow.Height);
            //_panel.Resize += _panel_Resize;

            _keyboardHandler = new KeyboardHandler(_host, _panel);
            _mouseHandler = new MouseHandler(_host, _panel);

            _host.Child = _panel;

            view.View.RenderWindow.Children.Add(_host);

            FlexUtility.SpawnThread(() =>
            {
                RunOnUIThread(() =>
                {
                    _renderer = new MogreRenderer(_panel.Handle.ToString(), (uint)_panel.Width, (uint)_panel.Height);

                    _renderer.CreateScene();

                    while (_preInitializationActions.Count != 0)
                    {
                        _preInitializationActions.Dequeue().Invoke();
                    }

                    _mouseHandler.Initialize();
                    _keyboardHandler.Initialize();

                    _initialized = true;

                    FlexUtility.SpawnThread(() =>
                    {
                        _renderThread = Thread.CurrentThread;
                        Mogre.Timer timer = new Mogre.Timer();
                        int attemptedFrameRate = 90;
                        bool physicsRender = true;
                        while (true)
                        {
                            while (_renderDispatcherActionQueue.Count != 0)
                            {
                                System.Action action;
                                lock (_renderDispatcherActionQueue)
                                {
                                    action = _renderDispatcherActionQueue.Dequeue();
                                }
                                if (action != null)
                                {
                                    action.Invoke();
                                }
                            }
                            lock (_renderNextDispatcherActionQueue)
                            {
                                lock (_renderDispatcherActionQueue)
                                {
                                    while (_renderNextDispatcherActionQueue.Count != 0)
                                    {
                                        _renderDispatcherActionQueue.Enqueue(_renderNextDispatcherActionQueue.Dequeue());
                                    }
                                }
                            }
                            uint elapsed = timer.Milliseconds;
                            timer.Reset();

                            int wait = (int)((1000 / attemptedFrameRate) - elapsed);

                            if (wait > 0)
                            {
                                Thread.Sleep(wait);
                            }

                            _keyboardHandler.KeyboardTick();
                            if (physicsRender)
                            {
                                PhysicsEngine.Step();
                            }
                            physicsRender = !physicsRender;
                            Renderer.Loop();
                        }
                    });
                });
            });
        }

        public static Vector3 GetBestLocationFromYDown(Vector3 vector, float fallbackY, float sizeY)
        {
            Ray ray = new Ray(vector + new Vector3(0, (float.MaxValue), 0), Vector3.NEGATIVE_UNIT_Y);

            RaySceneQuery mRaySceneQuery = Engine.Renderer.Scene.CreateRayQuery(ray);
            mRaySceneQuery.SetSortByDistance(true, 64);
            mRaySceneQuery.QueryTypeMask = SceneManager.ENTITY_TYPE_MASK;
            mRaySceneQuery.QueryMask = (uint)QueryFlags.INSTANCE_ENTITY;

            RaySceneQueryResult result = mRaySceneQuery.Execute();
            RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

            Vector3 max = new Vector3(vector.x, fallbackY, vector.z);
            if (itr != null)
            {
                while (itr.MoveNext())
                {
                    RaySceneQueryResultEntry entry = itr.Current;
                    SceneNode parentNode = entry.movable.ParentSceneNode;
                    Vector3 current = new Vector3(vector.x, parentNode.Position.y + sizeY, vector.z);
                    if (current.y > max.y)
                    {
                        max = current;
                    }
                }
            }
            return max;
        }

        private static void _panel_Resize(object sender, EventArgs e)
        {
            if (_renderer != null)
            {
                _renderer.ReinitRenderWindow(_panel.Handle.ToString(), (uint)_panel.Width, (uint)_panel.Height);
                _renderer.OnResize();
            }
        }

        public static void Destroy(PositionedInstance instance)
        {
            if (MouseHandler.IsSelectedNode(instance.SceneNode))
            {
                MouseHandler.ClearSelectedNode();
            }
            if (MouseHandler.IsAlreadyHovered(instance.SceneNode))
            {
                MouseHandler.ClearHovered();
            }
            SceneNodeStore.RemoveSceneNode(instance.SceneNode);
            instance.SceneNode.RemoveAllChildren();
            _renderer.Scene.DestroySceneNode(instance.SceneNode);
        }

        public static void QueueForRenderDispatcher(System.Action action)
        {
            lock (_renderDispatcherActionQueue)
            {
                if (_renderThread != null && _renderThread.Equals(Thread.CurrentThread))
                {
                    action.Invoke();
                }
                _renderDispatcherActionQueue.Enqueue(action);
            }
        }

        public static void QueueForNextRenderDispatcher(System.Action action)
        {
            lock (_renderNextDispatcherActionQueue)
            {
                _renderNextDispatcherActionQueue.Enqueue(action);
            }
        }

        public static void RunOnUIThread(System.Action action)
        {
            if (System.Windows.Application.Current == null)
            {
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
