using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.ComponentModel;
using Flex.Misc.Utility;
using System.Threading;

namespace Flex.Misc.Utility
{
    [Serializable]
    public class UISafeObservableCollection<T> : ObservableCollection<T>
    {
        public new void Add(T obj)
        {
            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher != null)
            {
                base.Add(obj);
            }
            else
            {
                FlexUtility.RunWindowAction(() =>
                {
                    base.Add(obj);
                }, DispatcherPriority.Normal, false);
            }
        }

        public new bool Remove(T obj)
        {
            bool value = true;
            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher != null)
            {
                value = base.Remove(obj);
            }
            else
            {
                FlexUtility.RunWindowAction(() =>
                {
                    value = base.Remove(obj);
                }, DispatcherPriority.Normal, false);
            }
            return value;
        }
    }
}
