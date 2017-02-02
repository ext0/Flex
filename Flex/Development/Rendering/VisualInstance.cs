using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Flex.Development.Rendering
{
    public class VisualInstance
    {
        protected Visual3D _visual;
        protected Instance _instance;

        public VisualInstance(Visual3D visual, Instance instance)
        {
            _visual = visual;
            _instance = instance;
        }

        public Visual3D Visual3D
        {
            get
            {
                return _visual;
            }
        }

        public Instance Instance
        {
            get
            {
                return _instance;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is VisualInstance)
            {
                return (obj as VisualInstance).Instance.Equals(_instance);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _instance.GetHashCode();
        }
    }
}
