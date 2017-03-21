using Flex.Development.Rendering.Modules.Enum;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Rendering.Modules
{
    public class GizmoVisual
    {
        private SceneNode _base;

        public GizmoVisual(SceneNode center)
        {
            _base = center;
        }

        public bool IsTwoSided
        {
            get
            {
                return (XB != null || YB != null || ZB != null);
            }
        }

        private SceneNode GetSceneNodeSafe(String name)
        {
            Node.ChildNodeIterator iterator = _base.GetChildIterator();
            while (iterator.MoveNext())
            {
                SceneNode node = iterator.Current as SceneNode;
                if (node != null)
                {
                    Object metadata = Engine.SceneNodeStore.GetSceneNodeMetadata(node);
                    if (metadata.Equals(name))
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public SceneNode Base
        {
            get
            {
                return _base;
            }
        }

        public SceneNode XA
        {
            get
            {
                return GetSceneNodeSafe("XA");
            }
        }

        public SceneNode YA
        {
            get
            {
                return GetSceneNodeSafe("YA");
            }
        }

        public SceneNode ZA
        {
            get
            {
                return GetSceneNodeSafe("ZA");
            }
        }

        public SceneNode XB
        {
            get
            {
                return GetSceneNodeSafe("XB");
            }
        }

        public SceneNode YB
        {
            get
            {
                return GetSceneNodeSafe("YB");
            }
        }

        public SceneNode ZB
        {
            get
            {
                return GetSceneNodeSafe("ZB");
            }
        }

        public DirectionalTransformDragging SupportedDirections
        {
            get
            {
                DirectionalTransformDragging ret = 0;

                ret |= (XA != null) ? DirectionalTransformDragging.XA : 0;
                ret |= (YA != null) ? DirectionalTransformDragging.YA : 0;
                ret |= (ZA != null) ? DirectionalTransformDragging.ZA : 0;
                ret |= (XB != null) ? DirectionalTransformDragging.XB : 0;
                ret |= (YB != null) ? DirectionalTransformDragging.YB : 0;
                ret |= (ZB != null) ? DirectionalTransformDragging.ZA : 0;

                return ret;
            }
        }
    }
}
