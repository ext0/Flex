using Flex.Development.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Flex.Development.Rendering
{
    public class PhysicalInstance : VisualInstance
    {
        private Model3D _model;

        public PhysicalInstance(Visual3D visual, Model3D model, Instance instance) : base(visual, instance)
        {
            _model = model;
        }

        public Model3D Model3D
        {
            get
            {
                return _model;
            }
        }
    }
}
