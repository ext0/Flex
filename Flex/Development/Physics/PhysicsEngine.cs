using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Jitter.Collision;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Physics
{
    public static class PhysicsEngine
    {
        private static readonly float POSITION_EPSILION = 0.01f;

        private static CollisionSystem _collisionSystem;
        private static Jitter.World _physicsWorld;
        //private static DateTime _lastStep;
        private static List<PhysicsInstance> _physicsInstances;

        static PhysicsEngine()
        {
            _collisionSystem = new CollisionSystemSAP();
            _physicsWorld = new Jitter.World(_collisionSystem);
            _physicsWorld.SetInactivityThreshold(0f, 0f, float.MaxValue);
            _physicsInstances = new List<PhysicsInstance>();
        }

        internal static void ReloadVisualInstance(PhysicsInstance physicsInstance)
        {
            RemoveVisualInstance(physicsInstance);
            AddVisualInstance(physicsInstance);
        }

        internal static void AddVisualInstance(PhysicsInstance physicsInstance)
        {
            _physicsInstances.Add(physicsInstance);
            _physicsWorld.AddBody(physicsInstance.RigidBody);
        }

        internal static void RemoveVisualInstance(PhysicsInstance physicsInstance)
        {
            _physicsInstances.Remove(physicsInstance);
            _physicsWorld.RemoveBody(physicsInstance.RigidBody);
        }

        public static void Step()
        {
            try
            {
                _physicsWorld.Step((1 / 12f), true);
                if (ActiveScene.Running)
                {
                    for (int i = 0; i < _physicsInstances.Count; i++)
                    {
                        PhysicsInstance instance = _physicsInstances[i];
                        if ((Math.Abs(instance.RigidBody.Position.X - instance.position.x) > POSITION_EPSILION) ||
                                Math.Abs(instance.RigidBody.Position.Y - instance.position.y) > POSITION_EPSILION ||
                                Math.Abs(instance.RigidBody.Position.Z - instance.position.z) > POSITION_EPSILION)
                        {
                            instance.position.setTo(instance.RigidBody.Position.X, instance.RigidBody.Position.Y, instance.RigidBody.Position.Z);
                        }

                        instance.rotation.setTo(instance.RigidBody.Orientation);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
