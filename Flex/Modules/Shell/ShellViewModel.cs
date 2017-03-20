using Caliburn.Micro;
using Flex.Development.Rendering;
using Flex.Properties;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flex.Modules.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Gemini.Modules.Shell.ViewModels.ShellViewModel
    {
        static ShellViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(ShellViewModel).Namespace, typeof(ShellView).Namespace);
        }

        public override void CanClose(Action<bool> callback)
        {
            Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => callback(!e.WasCancelled));
        }

        private IEnumerable<IResult> CanClose()
        {
            yield return new MessageBoxResult();
        }

        private class MessageBoxResult : IResult
        {
            public event EventHandler<ResultCompletionEventArgs> Completed;

            public void Execute(CoroutineExecutionContext context)
            {
                var result = System.Windows.MessageBoxResult.Yes;

                result = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Completed(this, new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
                    Application.Current.Shutdown(0);
                }
            }
        }
    }
}
