using Flex.Development.Instances;
using Flex.Modules.ScriptEditor.Views;
using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            DisplayName = script.DisplayName;
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
            _scriptView.CodeEditor.Text = _script.Source;
            _scriptView.CodeEditor.TextChanged += CodeEditorTextChanged;
        }

        private void CodeEditorTextChanged(object sender, EventArgs e)
        {
            _script.Source = _scriptView.CodeEditor.Text;
        }
    }
}
