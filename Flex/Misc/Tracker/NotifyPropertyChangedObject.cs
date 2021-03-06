﻿using Flex.Development.Rendering;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Tracker
{
    [Serializable]
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        [ScriptMember(ScriptAccess.None)]
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                Engine.RunOnUIThread(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
                });
            }
        }
    }
}
