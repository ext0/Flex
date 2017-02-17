using Flex.Development.Instances;
using Flex.Development.Rendering;
using Jitter.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Physics
{
    public static class PhysicsEngine
    {
        private static CollisionSystem _collisionSystem;
        private static Jitter.World _physicsWorld;
        private static DateTime _lastStep;
        private static List<PhysicsInstance> _physicsInstances;

        static PhysicsEngine()
        {
            _lastStep = DateTime.MinValue;
            _collisionSystem = new CollisionSystemSAP();
            _physicsWorld = new Jitter.World(_collisionSystem);
            _physicsInstances = new List<PhysicsInstance>();
        }

        internal static void AddVisualInstance(PhysicsInstance physicsInstance)
        {
            _physicsInstances.Add(physicsInstance);
            _physicsWorld.AddBody(physicsInstance.RigidBody);
        }

        internal static void RemoveVisualInstance(PhysicsInstance physicsInstance)
        {
            _physicsInstances.Remove(physicsInstance);
            //_physicsWorld.RemoveBody(physicsInstance.RigidBody);
        }

        public static void Step()
        {
            if (_lastStep == DateTime.MinValue)
            {
                _physicsWorld.Step(1 / 60f, true);
                _lastStep = DateTime.Now;
            }
            else
            {
                _physicsWorld.Step((1 / 60f) + (float)(DateTime.Now - _lastStep).TotalSeconds, true);
            }
            foreach (PhysicsInstance physicsInstance in _physicsInstances)
            {
                physicsInstance.position.setTo(physicsInstance.RigidBody.Position.X, physicsInstance.RigidBody.Position.Y, physicsInstance.RigidBody.Position.Z);
            }
        }
    }
}
