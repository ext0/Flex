using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Gemini.Modules.Output;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
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
            /*
            _engine.Context.DefineVariable("print").Assign(JSValue.Marshal(new Action<Object>(text =>
            {
                JSValue val = (JSValue)text;
                if (val != null && val.Value is DynamicJS)
                {
                    _output.AppendLine((val.Value as DynamicJS).ToString());
                }
                else if (val != null)
                {
                    _output.AppendLine(val.ToString());
                }
                else
                {
                    _output.AppendLine("null");
                }
            })));
            _engine.Context.DefineVariable("wait").Assign(JSValue.Marshal(new Action<double>(time =>
            {
                Thread.Sleep((int)(time * 1000));
            })));
            _engine.Context.DefineVariable("spawn").Assign(JSValue.Marshal(new Action<Object>(obj =>
            {
                JSValue val = (JSValue)obj;
                if (val.ValueType == JSValueType.Function)
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
                                (val.Value as Function).Call(null);
                            }
                            catch (ThreadAbortException)
                            {
                                //Ignore
                            }
                            catch (Exception e)
                            {
                                _output.AppendLine(e.Message);
                            }
                        }
                    }));
                    thread.Start();
                }
            })));
            */
        }

        public void Execute(Script script)
        {
            try
            {
                /*
                _engine.Context.DefineVariable("script").Assign(new DynamicJS(script));
                _engine.Context.DefineVariable("world").Assign(new DynamicJS(ActiveWorld.Active.World));
                _engine.Context.DefineVariable("sky").Assign(new DynamicJS(ActiveWorld.Active.Sky));
                _engine.Context.DefineVariable("Instance").Assign(new DynamicJS(new InstanceJS()));
                */

                _engine.AddHostObject("output", new OutputJS());
                _engine.AddHostObject("script", script);
                _engine.AddHostObject("world", ActiveWorld.Active.World);
                _engine.AddHostObject("sky", ActiveWorld.Active.Sky);

                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Instance));
                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(PositionedInstance));
                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Part));
                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Script));
                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(Sky));
                _engine.AddHostType(HostItemFlags.DirectAccess, typeof(World));

                V8Script v8Script = _engine.Compile(script.source);

                _engine.Execute(v8Script);
            }
            catch (Exception e)
            {
                _output.AppendLine(e.Message);
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
