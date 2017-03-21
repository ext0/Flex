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

        private Dictionary<SceneNode, Object> _metadata = new Dictionary<SceneNode, Object>();

        public PositionedInstance GetInstance(SceneNode node)
        {
            return _instances.ContainsKey(node) ? _instances[node] : null;
        }

        public void AddSceneNode(SceneNode node, PositionedInstance instance)
        {
            _instances.Add(node, instance);
        }

        public void RemoveSceneNode(SceneNode node)
        {
            _instances.Remove(node);
        }

        public Object GetSceneNodeMetadata(SceneNode node)
        {
            return _metadata.ContainsKey(node) ? _metadata[node] : null;
        }

        public void AddSceneNodeMetadata(SceneNode node, Object metadata)
        {
            _metadata.Add(node, metadata);
        }

        public void RemoveSceneNodeMetadata(SceneNode node)
        {
            _metadata.Remove(node);
        }
    }
}
