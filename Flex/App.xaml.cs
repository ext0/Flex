using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Modules.MainWindow.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Framework;
using Flex.Modules.ScriptEditor.ViewModels;
using Flex.Development.Instances;

namespace Flex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Folder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (image.DataContext is Script)
                {
                    Script script = image.DataContext as Script;
                    foreach (Document document in IoC.Get<IShell>().Documents)
                    {
                        if (document is ScriptViewModel)
                        {
                            if ((document as ScriptViewModel).Script.Equals(script))
                            {
                                IoC.Get<IShell>().OpenDocument(document);
                                return;
                            }
                        }
                    }
                    IoC.Get<IShell>().OpenDocument(new ScriptViewModel(script));
                }
            }
        }
    }
}
