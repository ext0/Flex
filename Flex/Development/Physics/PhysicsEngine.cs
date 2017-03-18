using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Rendering;
using Flex.Misc.Utility;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
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
        private static readonly float POSITION_EPSILION = 0.01f;

        private static CollisionSystem _collisionSystem;
        private static Jitter.World _physicsWorld;
        //private static DateTime _lastStep;
        private static List<PhysicsInstance> _physicsInstances;

        private static Object _collisionVectorResolvementLock;
        private static PhysicsInstance _collisionVectorResolvementInstance;
        private static JVector _collisionVectorResolvementResult;

        static PhysicsEngine()
        {
            _collisionVectorResolvementLock = new object();

            _collisionSystem = new CollisionSystemSAP();
            _physicsWorld = new Jitter.World(_collisionSystem);
            _physicsWorld.SetInactivityThreshold(0f, 0f, float.MaxValue);
            _physicsInstances = new List<PhysicsInstance>();
            _collisionSystem.PassedBroadphase += _collisionSystem_PassedBroadphase;
        }

        public static bool GetCollisionVectorResolvement(PhysicsInstance instance)
        {
            lock (_collisionVectorResolvementLock)
            {
                _collisionVectorResolvementResult = JVector.MinValue;
                _collisionVectorResolvementInstance = instance;
                _collisionSystem.PassedBroadphase += _collisionSystem_PassedBroadphase;
                _collisionSystem.Detect(false);
                _collisionSystem.PassedBroadphase -= _collisionSystem_PassedBroadphase;
                if (_collisionVectorResolvementResult != JVector.MinValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        private static void _collisionSystem_CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            System.Diagnostics.Trace.WriteLine("Collision!");
        }

        private static bool _collisionSystem_PassedBroadphase(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
        {
            RigidBody rigid1 = entity1 as RigidBody;
            RigidBody rigid2 = entity2 as RigidBody;

            PhysicsInstance physics1 = GetPhysicsInstanceFromRigidBody(rigid1);
            PhysicsInstance physics2 = GetPhysicsInstanceFromRigidBody(rigid2);

            System.Diagnostics.Trace.WriteLine("Broadphase!");
            int direction = 0;

            if (_collisionVectorResolvementInstance != null)
            {
                if (_collisionVectorResolvementInstance.equals(physics1))
                {
                    direction = 1;
                }
                else if (_collisionVectorResolvementInstance.equals(physics2))
                {
                    direction = -1;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            JMatrix rigid1Orientation = rigid1.Orientation;
            JMatrix rigid2Orientation = rigid2.Orientation;
            JVector rigid1Position = rigid1.Position;
            JVector rigid2Position = rigid2.Position;

            JVector p1;
            JVector normal;

            float penetration;

            bool collision = true;
            while (collision)
            {
                collision = XenoCollide.Detect((ISupportMappable)entity1, (ISupportMappable)entity2, ref rigid1Orientation, ref rigid2Orientation, ref rigid1Position, ref rigid2Position, out p1, out normal, out penetration);
                if (collision)
                {
                    if (direction == 1) //entity1 is the movable one
                    {
                        rigid1.Position += normal;
                    }
                    else //entity2 is the movable
                    {
                        rigid1.Position -= normal;
                    }
                }
            }

            if (direction == 1)
            {
                _collisionVectorResolvementResult = rigid1.Position;
            }
            else
            {
                _collisionVectorResolvementResult = rigid2.Position;
            }

            physics1.position.setToPhysics(rigid1Position.X, rigid1Position.Y, rigid1Position.Z);
            physics2.position.setToPhysics(rigid2Position.X, rigid2Position.Y, rigid2Position.Z);
            return false;
        }

        private static PhysicsInstance GetPhysicsInstanceFromRigidBody(RigidBody body)
        {
            foreach (PhysicsInstance instance in _physicsInstances)
            {
                if (instance.RigidBody.Equals(body))
                {
                    return instance;
                }
            }
            return null;
        }

        public static CollisionSystem CollisionSystem
        {
            get
            {
                return _collisionSystem;
            }
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
                _physicsWorld.Step((1 / 45f), true);
                if (ActiveScene.Running)
                {
                    for (int i = 0; i < _physicsInstances.Count; i++)
                    {
                        PhysicsInstance instance = _physicsInstances[i];
                        if (!instance.anchored)
                        {
                            if ((
                                System.Math.Abs(instance.RigidBody.Position.X - instance.position.x) > POSITION_EPSILION) ||
                                System.Math.Abs(instance.RigidBody.Position.Y - instance.position.y) > POSITION_EPSILION ||
                                System.Math.Abs(instance.RigidBody.Position.Z - instance.position.z) > POSITION_EPSILION)
                            {
                                instance.position.setToPhysics(instance.RigidBody.Position.X, instance.RigidBody.Position.Y, instance.RigidBody.Position.Z);
                            }

                            instance.rotation.setToPhysics(instance.RigidBody.Orientation);

                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
