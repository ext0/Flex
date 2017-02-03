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
    public class PhysicsEngine
    {
        private CollisionSystem _collisionSystem;
        private Jitter.World _physicsWorld;
        private DateTime _lastStep;
        private List<PhysicsInstance> _physicsInstances;

        public PhysicsEngine()
        {
            _lastStep = DateTime.MinValue;
            _collisionSystem = new CollisionSystemSAP();
            _physicsWorld = new Jitter.World(_collisionSystem);
            _physicsInstances = new List<PhysicsInstance>();
        }

        public void AddVisualInstance(VisualInstance visualInstance)
        {
            PhysicsInstance physicsInstance = new PhysicsInstance(visualInstance.Instance as PositionedInstance);
            _physicsInstances.Add(physicsInstance);
            _physicsWorld.AddBody(physicsInstance.RigidBody);
        }

        public void Step()
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
                (physicsInstance.Instance as PositionedInstance).Position.SetTo(physicsInstance.Position.X, physicsInstance.Position.Y, physicsInstance.Position.Z);
            }
        }
    }
}
