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
using Flex.Development.Rendering;

namespace Flex.Misc.Utility
{
    [Serializable]
    public class UISafeObservableCollection<T> : ObservableCollection<T>
    {
        public new void Add(T obj)
        {
            MainDXScene.RunOnUIThread(() =>
            {
                base.Add(obj);
            });
        }

        public new bool Remove(T obj)
        {
            bool value = true;
            MainDXScene.RunOnUIThread(() =>
            {
                value = base.Remove(obj);
            });
            return value;
        }
    }
}
