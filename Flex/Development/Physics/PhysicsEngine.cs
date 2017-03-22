using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Physics
{
    public static class PhysicsEngine
    {
        private static MogreNewt.World _physicsWorld;

        public static readonly Vector3 GRAVITY_CONSTANT = new Vector3(0, -9.8f * 5f, 0);

        static PhysicsEngine()
        {
            _physicsWorld = new MogreNewt.World();
            _physicsWorld.SetThreadCount(1);
            _physicsWorld.SetWorldSize(new Vector3(float.MinValue), new Vector3(float.MaxValue));
        }

        public static MogreNewt.World World
        {
            get
            {
                return _physicsWorld;
            }
        }

        public static void Step()
        {
            if (ActiveScene.Running)
            {
                _physicsWorld.Update(1 / 60f);
            }
        }
    }
}
