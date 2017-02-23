using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Gemini.Modules.Output;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
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

        private void wait(double time)
        {
            _engine.GetType().GetMethod("ScriptInvoke", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(System.Action) }, null)
                .Invoke(_engine, new object[] {
                    new System.Action(() => {
                        Thread.Sleep((int)(time * 1000d));
                    })
                });
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
                }
            }));
            thread.Start();
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

            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Math));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Noise));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Random));

            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Instance));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(PositionedInstance));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Part));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Script));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Sky));
            _engine.AddHostType(HostItemFlags.DirectAccess, typeof(World));

            MainDXScene.Scene.RunOnUIThread(() =>
            {
                if (script.source == null)
                {
                    script.source = String.Empty;
                }
            });
            try
            {
                V8Script v8Script = _engine.Compile(script.source);
                _engine.Execute(v8Script);
            }
            catch (ScriptEngineException e)
            {
                MainDXScene.Scene.RunOnUIThread(() =>
                {
                    try
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
                    catch (Exception ee)
                    {
                        System.Diagnostics.Debug.WriteLine(ee.Message);
                    }
                });
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
