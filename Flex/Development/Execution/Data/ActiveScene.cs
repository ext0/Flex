using Caliburn.Micro;
using Flex.Development.Execution.Data.Data;
using Flex.Development.Execution.Data.States;
using Flex.Development.Execution.Runtime;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Runtime;
using Flex.Misc.Utility;
using Flex.Modules.Scene.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Flex.Development.Execution.Data
{
    public static class ActiveScene
    {
        private static DataContext _context;
        private static List<CancellationTokenSource> _activeTasks;
        private static EngineJS _currentEngine;
        private static SceneViewModel _viewModel;
        private static bool _vrToggle;

        public static event EventHandler OnRenderStep;
        public static event EventHandler OnStep;

        public static event EventHandler RunningChanged;

        public static event EventHandler GizmoChanged;

        private static Dictionary<KeyAction, Dictionary<int, List<System.Action>>> _runtimeKeybinds;

        private static byte[] _savedState;

        private static Instance _selected;

        private static GizmoType _activeGizmo = GizmoType.POINTER;

        private static Instance _copied;

        static ActiveScene()
        {
            _currentEngine = null;
            _context = new DataContext();
            _activeTasks = new List<CancellationTokenSource>();
            _runtimeKeybinds = new Dictionary<KeyAction, Dictionary<int, List<System.Action>>>();
            _viewModel = null;
        }

        public static Instance CopiedInstance
        {
            get
            {
                return _copied;
            }
            set
            {
                _copied = value;
            }
        }

        public static GizmoType ActiveGizmoType
        {
            get
            {
                return _activeGizmo;
            }
            set
            {
                if (_activeGizmo == value) return;
                _activeGizmo = value;
                GizmoChanged?.Invoke(null, null);
            }
        }

        public static Instance SelectedInstance
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        public static void RegisterKeyCallback(KeyAction keyAction, int key, System.Action action)
        {
            if (!_runtimeKeybinds.ContainsKey(keyAction))
            {
                _runtimeKeybinds.Add(keyAction, new Dictionary<int, List<System.Action>>());
            }
            if (!_runtimeKeybinds[keyAction].ContainsKey(key))
            {
                _runtimeKeybinds[keyAction].Add(key, new List<System.Action>());
            }

            _runtimeKeybinds[keyAction][key].Add(action);
        }

        public static IEnumerable<int> GetRegisteredKeyPressKeys()
        {
            if (_runtimeKeybinds.ContainsKey(KeyAction.KeyPress))
            {
                return _runtimeKeybinds[KeyAction.KeyPress].Keys;
            }
            return Enumerable.Empty<int>();
        }

        public static void RunKeyCallback(KeyAction keyAction, int key)
        {
            if (_runtimeKeybinds.ContainsKey(keyAction))
            {
                if (_runtimeKeybinds[keyAction].ContainsKey(key))
                {
                    foreach (System.Action action in _runtimeKeybinds[keyAction][key])
                    {
                        action();
                    }
                }
            }
        }

        private static SceneViewModel GetSceneViewModel()
        {
            if (_viewModel != null)
            {
                return _viewModel;
            }
            foreach (IDocument document in IoC.Get<IShell>().Documents)
            {
                if (document is SceneViewModel)
                {
                    _viewModel = document as SceneViewModel;
                    return _viewModel;
                }
            }
            throw new Exception("Could not find SceneViewModel!");
        }

        public static bool Running
        {
            get
            {
                return _context.IsRunning;
            }
        }

        public static bool IsVR
        {
            get
            {
                return _vrToggle;
            }
            set
            {
                _vrToggle = value;
            }
        }

        public static Instance AddInstance<T>() where T : Instance
        {
            Instance ret = null;
            Engine.RunOnUIThread(() =>
            {
                if (typeof(T).Equals(typeof(Part)))
                {
                    Part part = new Part(true);
                    part.parent = _context.ActiveWorld.World;
                    ret = part;
                }
                else if (typeof(T).Equals(typeof(Script)))
                {
                    Script script = new Script(true);
                    script.parent = _context.ActiveWorld.World;
                    ret = script;
                }
            });
            return ret;
        }

        public static void Run()
        {
            _activeTasks.Clear();
            _context.IsRunning = true;
            RunningChanged?.Invoke(null, new EventArgs());
            Save();
            //Output.Out.AddLine("Saved cached copy of current state: " + _savedState.Length);
            _currentEngine = new EngineJS();
            List<Task> scriptExecution = new List<Task>();
            foreach (Script script in _context.ActiveWorld.World.getChildren(true).Where((x) =>
            {
                return x.GetType().Equals(typeof(Script));
            }))
            {
                CancellationTokenSource token = new CancellationTokenSource();
                scriptExecution.Add(new Task((x) =>
                {
                    Thread currentThread = Thread.CurrentThread;
                    using (token.Token.Register(currentThread.Abort))
                    {
                        _currentEngine.Execute(x as Script);
                    }
                }, script, token.Token));
                _activeTasks.Add(token);
            }

            foreach (Task execution in scriptExecution)
            {
                execution.Start();
            }

            Thread mainLoop = new Thread(MainLoop);
            mainLoop.Start();
        }

        public static void NotifyRenderStep()
        {
            OnRenderStep?.Invoke(null, null);
        }

        public static void MainLoop()
        {
            while (Running)
            {
                Thread.Sleep(1000 / 60);
                OnStep?.Invoke(null, null);
            }
        }

        public static void Stop()
        {
            foreach (CancellationTokenSource token in _activeTasks)
            {
                token.Cancel();
            }
            if (_currentEngine != null)
            {
                _currentEngine.KillChildrenThreads();
            }
            Reset();
            _context.IsRunning = false;
            RunningChanged?.Invoke(null, new EventArgs());

            OnStep = null;
            OnRenderStep = null;
            _runtimeKeybinds.Clear();
        }

        public static void Save()
        {
            _savedState = SerializedContext;
        }

        public static byte[] SerializedContext
        {
            get
            {
                return FlexUtility.SerializeToBinary(_context.ActiveWorld);
            }
        }

        private static void Reset()
        {
            ActiveWorld world = FlexUtility.DeserializeToObject(_savedState) as ActiveWorld;

            List<InstancePair> correspondings;
            List<Instance> removedInstances;
            List<Instance> newInstances;

            List<Instance> newChildren = new List<Instance>();
            List<Instance> oldChildren = new List<Instance>();

            LoadResetInstanceHelper(_context.ActiveWorld.World, world.World, newChildren, oldChildren);

            newInstances = newChildren.Where(x => !oldChildren.Any(y => x.Equals(y))).ToList();
            removedInstances = oldChildren.Where(x => !newChildren.Any(y => x.Equals(y))).ToList();
            correspondings = oldChildren.Join(newChildren, x => x, y => y, (x, y) =>
            {
                return new InstancePair(x, y);
            }).ToList();

            foreach (Instance newInstance in newInstances)
            {
                newInstance.Cleanup();
            }

            foreach (Instance removedInstance in removedInstances)
            {
                /*
                Implement
                */
            }

            foreach (InstancePair instancePair in correspondings)
            {
                ObjectSave save = new ObjectSave(instancePair.Old, instancePair.Current, instancePair.Current.GetType());
                save.Reset();
                try
                {
                    InstancePair oldParent = correspondings.Where(x => x.Old.equals(instancePair.Old.parent)).FirstOrDefault();
                    if (oldParent != null && oldParent.Current != null)
                    {
                        instancePair.Current.parent = oldParent.Current;
                    }
                }
                catch { }
                instancePair.Current.Reload();
            }

            ResetInstance(world.Sky, _context.ActiveWorld.Sky);
        }

        private static void LoadResetInstanceHelper(Instance newRoot, Instance oldRoot, List<Instance> newFound, List<Instance> oldFound)
        {
            if (newRoot != null)
            {
                foreach (Instance newChild in newRoot.getChildren())
                {
                    newFound.Add(newChild);
                    LoadResetInstanceHelper(newChild, null, newFound, oldFound);
                }
            }
            if (oldRoot != null)
            {
                foreach (Instance oldChild in oldRoot.getChildren())
                {
                    oldFound.Add(oldChild);
                    LoadResetInstanceHelper(null, oldChild, newFound, oldFound);
                }
            }
        }

        private static void ResetInstance(Instance old, Instance current)
        {
            ObjectSave save = new ObjectSave(old, current, old.GetType());
            save.Reset();
            current.Reload();
        }

        public static DataContext Context
        {
            get
            {
                return _context;
            }
        }
    }
}
