using Flex.Development.Instances;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Rendering.Modules
{
    public class SceneNodeStore
    {
        private Dictionary<SceneNode, PositionedInstance> _instances = new Dictionary<SceneNode, PositionedInstance>();

        public PositionedInstance GetInstance(SceneNode node)
        {
            return _instances[node];
        }

        public void AddSceneNode(SceneNode node, PositionedInstance instance)
        {
            _instances.Add(node, instance);
        }

        public void RemoveSceneNode(SceneNode node)
        {
            _instances.Remove(node);
        }
    }
}
