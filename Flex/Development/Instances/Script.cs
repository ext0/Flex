﻿using Flex.Development.Execution.Data;
using Flex.Misc.Tracker;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    public class Script : Instance
    {
        private String _source;

        public Script() : base()
        {
            _displayName = "Script";
            _icon = "16/script.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();
            parent = ActiveScene.Context.ActiveWorld.World;

            Initialize();
        }

        internal Script(bool flag) : base()
        {
            _displayName = "Script";
            _icon = "16/script.png";
            _instances = new UISafeObservableCollection<Instance>();
            _allowedChildren = new List<Type>();

            Initialize();
        }

        internal void AddInstance(Instance instance)
        {
            instance.parent = this;
            _instances.Add(instance);
            NotifyPropertyChanged("Instances");
        }

        public override void Initialize()
        {
            _initialized = true;
        }

        public override void Cleanup()
        {

        }

        public override void Reload()
        {

        }

        [TrackMember]
        public override string name
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        [Browsable(false)]
        public String source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                NotifyPropertyChanged("Source");
            }
        }

        public override string Icon
        {
            get
            {
                return "/Resources/Icons/" + _icon;
            }
        }

        [TrackMember]
        public override Instance parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value == _parent) return;
                ChangeParent(value);
                NotifyPropertyChanged("Parent");
            }
        }

        internal override IEnumerable<Type> AllowedChildren
        {
            get
            {
                return _allowedChildren;
            }
        }
    }
}
