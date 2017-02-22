using Caliburn.Micro;
using Flex.Development.Execution.Data.States;
using Flex.Development.Execution.Runtime;
using Flex.Development.Instances;
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
        internal class InstancePair
        {
            private Instance _old;
            private Instance _current;

            public InstancePair(Instance old, Instance current)
            {
                _old = old;
                _current = current;
            }

            public Instance Old
            {
                get
                {
                    return _old;
                }
            }

            public Instance Current
            {
                get
                {
                    return _current;
                }
            }
        }

        private static DataContext _context;
        private static List<CancellationTokenSource> _activeTasks;
        private static EngineJS _currentEngine;
        private static SceneViewModel _viewModel;

        public static event EventHandler RunningChanged;

        private static byte[] _savedState;

        static ActiveScene()
        {
            _currentEngine = null;
            _context = new DataContext();
            _activeTasks = new List<CancellationTokenSource>();
            _viewModel = null;
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

        public static Instance AddInstance<T>() where T : Instance
        {
            Instance ret = null;
            FlexUtility.RunWindowAction(() =>
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
            }, System.Windows.Threading.DispatcherPriority.Normal, false);
            return ret;
        }

        public static void Run()
        {
            _activeTasks.Clear();
            _context.IsRunning = true;
            if (RunningChanged != null)
            {
                RunningChanged(null, new EventArgs());
            }
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
            Thread physicsThread = new Thread(PhysicsLoop);
            physicsThread.Start();

            //Output.Out.AddLine("Reloaded cached copy of current state: " + _savedState.Length);
        }

        public static void PhysicsLoop()
        {
            while (_context.IsRunning)
            {
                GetSceneViewModel().PhysicsStep();
                Thread.Sleep(1000 / 60);
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
            if (RunningChanged != null)
            {
                RunningChanged(null, new EventArgs());
            }
        }

        public static void Save()
        {
            _savedState = SerializedContext();
        }

        public static byte[] SerializedContext()
        {
            return FlexUtility.SerializeToBinary(_context.ActiveWorld);
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
