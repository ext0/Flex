using Flex.Development.Instances;
using Flex.Misc.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Data
{
    [Serializable]
    public class ActiveWorld
    {
        private UISafeObservableCollection<Instance> _instances;

        private World _world;

        private Sky _sky;

        private static ActiveWorld _activeWorld;

        public ActiveWorld()
        {
            if (_activeWorld != null)
            {
                throw new Exception("Multiple active worlds initialized! Only one active world object should exist!");
            }
            _activeWorld = this;
            _instances = new UISafeObservableCollection<Instance>();
            _world = new World();
            _instances.Add(_world);
            _sky = new Sky();
            _instances.Add(_sky);
        }

        public static ActiveWorld Active
        {
            get
            {
                return _activeWorld;
            }
        }

        public UISafeObservableCollection<Instance> Children
        {
            get
            {
                return _instances;
            }
        }

        public World World
        {
            get
            {
                return _world;
            }
        }

        public Sky Sky
        {
            get
            {
                return _sky;
            }
        }
    }
}
