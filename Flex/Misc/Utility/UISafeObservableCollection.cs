using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Flex.Misc.Utility
{
    [Serializable]
    public class UISafeObservableCollection<T> : ObservableCollection<T>
    {
        public new void Add(T obj)
        {
            FlexUtility.RunWindowAction(() =>
            {
                base.Add(obj);
            }, DispatcherPriority.Normal, false);
        }

        public new bool Remove(T obj)
        {
            bool value = true;
            FlexUtility.RunWindowAction(() =>
            {
                value = base.Remove(obj);
            }, DispatcherPriority.Normal, false);
            return value;
        }
    }
}
