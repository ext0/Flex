using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Gemini.Modules.Output;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public class EngineJS
    {
        private V8ScriptEngine _engine;

        private IOutput _output;

        private List<CancellationTokenSource> _childrenThreads;

        public EngineJS()
        {
            _output = IoC.Get<IOutput>();
            _engine = new V8ScriptEngine(V8ScriptEngineFlags.None);
            _childrenThreads = new List<CancellationTokenSource>();
        }

        private void print(dynamic obj)
        {
            if (obj != null)
            {
                _output.AppendLine(obj.ToString());
            }
            else
            {
                _output.AppendLine("null");
            }
        }

        private void delay(dynamic obj, double time)
        {
            CancellationTokenSource token = new CancellationTokenSource();
            _childrenThreads.Add(token);
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread currentThread = Thread.CurrentThread;
                using (token.Token.Register(currentThread.Abort))
                {
                    try
                    {
                        Thread.Sleep((int)(time * 1000d));
                        obj();
                    }
                    catch (ThreadAbortException)
                    {
                        //Ignore
                    }
                    catch (ScriptEngineException e)
                    {
                        _output.AppendLine(e.ErrorDetails);
                    }
                    catch (RuntimeBinderException e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                    catch (Exception e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                }
            }));
            thread.Start();
        }

        private void spawn(dynamic obj)
        {
            CancellationTokenSource token = new CancellationTokenSource();
            _childrenThreads.Add(token);
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread currentThread = Thread.CurrentThread;
                using (token.Token.Register(currentThread.Abort))
                {
                    try
                    {
                        obj();
                    }
                    catch (ThreadAbortException)
                    {
                        //Ignore
                    }
                    catch (ScriptEngineException e)
                    {
                        _output.AppendLine(e.ErrorDetails);
                    }
                    catch (RuntimeBinderException e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                    catch (Exception e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                }
            }));
            thread.Start();
        }

        private void loop(dynamic obj, double interval)
        {
            CancellationTokenSource token = new CancellationTokenSource();
            _childrenThreads.Add(token);
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread currentThread = Thread.CurrentThread;
                using (token.Token.Register(currentThread.Abort))
                {
                    try
                    {
                        while (true)
                        {
                            obj();
                            Thread.Sleep((int)(interval * 1000d));
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        //Ignore
                    }
                    catch (ScriptEngineException e)
                    {
                        _output.AppendLine(e.ErrorDetails);
                    }
                    catch (Exception e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                }
            }));
            thread.Start();
        }

        private void forLoop(dynamic obj, int max, double interval)
        {
            CancellationTokenSource token = new CancellationTokenSource();
            _childrenThreads.Add(token);
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread currentThread = Thread.CurrentThread;
                using (token.Token.Register(currentThread.Abort))
                {
                    try
                    {
                        for (int i = 0; i < max; i++)
                        {
                            obj(i);
                            Thread.Sleep((int)(interval * 1000d));
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        //Ignore
                    }
                    catch (ScriptEngineException e)
                    {
                        _output.AppendLine(e.ErrorDetails);
                    }
                    catch (Exception e)
                    {
                        _output.AppendLine("[external error] " + e.Message);
                    }
                }
            }));
            thread.Start();
        }

        private void onRenderStep(dynamic obj)
        {
            ActiveScene.OnRenderStep += (sender, e) =>
            {
                obj();
            };
        }

        private void onPhysicsStep(dynamic obj)
        {
            ActiveScene.OnPhysicsStep += (sender, e) =>
            {
                obj();
            };
        }

        private void onStep(dynamic obj)
        {
            ActiveScene.OnStep += (sender, e) =>
            {
                obj();
            };
        }

        private void onKeyPress(int key, dynamic obj)
        {
            ActiveScene.RegisterKeyCallback(KeyAction.KeyPress, key, () => obj());
        }

        private void onKeyDown(int key, dynamic obj)
        {
            ActiveScene.RegisterKeyCallback(KeyAction.KeyDown, key, () => obj());
        }

        private void onKeyUp(int key, dynamic obj)
        {
            ActiveScene.RegisterKeyCallback(KeyAction.KeyUp, key, () => obj());
        }

        public void Execute(Script script)
        {
            _engine.AddHostObject("script", script);
            _engine.AddHostObject("world", ActiveWorld.Active.World);
            _engine.AddHostObject("sky", ActiveWorld.Active.Sky);
            _engine.AddHostObject("camera", ActiveWorld.Active.Camera);

            _engine.Script.print = new Action<Object>(print);
            _engine.Script.spawn = new Action<Object>(spawn);
            _engine.Script.delay = new Action<Object, double>(delay);
            _engine.Script.loop = new Action<Object, double>(loop);
            _engine.Script.forLoop = new Action<Object, int, double>(forLoop);
            _engine.Script.onRenderStep = new Action<Object>(onRenderStep);
            _engine.Script.onPhysicsStep = new Action<Object>(onPhysicsStep);
            _engine.Script.onStep = new Action<Object>(onStep);
            _engine.Script.onKeyPress = new Action<int, Object>(onKeyPress);
            _engine.Script.onKeyDown = new Action<int, Object>(onKeyDown);
            _engine.Script.onKeyUp = new Action<int, Object>(onKeyUp);

            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Math));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Noise));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Random));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Key));

            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Camera));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Part));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Script));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Sky));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(World));

            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Vector3));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Rotation));

            //Allow for reflection on Instance types to access higher concrete implementation properties
            _engine.DisableTypeRestriction = true;

            _engine.AllowReflection = true;
            _engine.SuppressExtensionMethodEnumeration = false;
            _engine.UseReflectionBindFallback = true;

            _engine.DefaultAccess = ScriptAccess.None;

            try
            {
                V8Script v8Script = _engine.Compile((script.source != null) ? script.source : String.Empty);
                _engine.Execute(v8Script);
            }
            catch (ScriptEngineException e)
            {
                String errorDetails = e.ErrorDetails;
                Match errDetailsMatch = Regex.Match(errorDetails, @"'(?<type>Flex\..*)'");
                if (errDetailsMatch.Success)
                {
                    Match match = Regex.Match(errorDetails, @"'Flex\..*.\.(?<class>.+)'");
                    if (match.Success)
                    {
                        errorDetails = errorDetails.Replace(errDetailsMatch.Groups["type"].Value, match.Groups["class"].Value);
                    }
                }
                _output.AppendLine(errorDetails);
            }
            catch (Exception e)
            {
                _output.AppendLine("[external error] " + e.Message);
            }
        }

        public void KillChildrenThreads()
        {
            foreach (CancellationTokenSource source in _childrenThreads)
            {
                source.Cancel();
            }
        }
    }
}
