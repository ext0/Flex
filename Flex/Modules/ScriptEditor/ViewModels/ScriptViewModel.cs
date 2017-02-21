using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Modules.ScriptEditor.Views;
using Gemini.Framework;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Flex.Modules.ScriptEditor.ViewModels
{
    [DisplayName("Script View Model")]
    [Export]
    public class ScriptViewModel : Document
    {
        private ScriptView _scriptView;
        private Script _script;

        public ScriptViewModel(Script script)
        {
            DisplayName = script.name;
            _script = script;
        }

        public Script Script
        {
            get
            {
                return _script;
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(true);
        }

        protected override void OnViewLoaded(object view)
        {
            _scriptView = view as ScriptView;
            ActiveScene.RunningChanged += (sender, e) =>
            {
                _scriptView.CodeEditor.IsReadOnly = ActiveScene.Running;
            };
            _scriptView.CodeEditor.Text = _script.source;
            _scriptView.CodeEditor.TextChanged += CodeEditorTextChanged;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Flex.Resources.FlexJS.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    _scriptView.CodeEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private void CodeEditorTextChanged(object sender, EventArgs e)
        {
            _script.source = _scriptView.CodeEditor.Text;
        }
    }
}
