using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Data
{
    public class DataContext
    {
        private ActiveWorld _activeWorld;

        private bool _isRunning;

        public DataContext()
        {
            _isRunning = false;
            _activeWorld = new ActiveWorld();
        }


        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
            }
        }

        public ActiveWorld ActiveWorld
        {
            get
            {
                return _activeWorld;
            }
            set
            {
                _activeWorld = value;
            }
        }

        public ObservableCollection<Instance> ActiveInstances
        {
            get
            {
                return _activeWorld.Instances;
            }
        }
    }
}
