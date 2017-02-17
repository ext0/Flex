﻿using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Tracker
{
    [TrackClass]
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
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
