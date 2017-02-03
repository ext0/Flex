using Flex.Development.Instances;
using Flex.Development.Rendering;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Physics
{
    public class PhysicsInstance
    {
        private Shape _shape;
        private RigidBody _rigidBody;
        private PositionedInstance _instance;

        public PhysicsInstance(PositionedInstance instance)
        {
            _instance = instance;
            if (_instance is Part)
            {
                Part part = _instance as Part;
                _shape = new BoxShape((float)part.Size.X, (float)part.Size.Y, (float)part.Size.Z);
                _rigidBody = new RigidBody(_shape);
                part.OnChanged += (sender, e) =>
                {
                    _shape = new BoxShape((float)part.Size.X, (float)part.Size.Y, (float)part.Size.Z);
                    _rigidBody.Position = new Jitter.LinearMath.JVector((float)part.Position.X, (float)part.Position.Y, (float)part.Position.Z);
                };
            }
            else
            {
                throw new ArgumentException("Cannot create PhysicsInstance from " + instance.GetType().Name + ", does not support physics!");
            }
        }

        public Vector3 Position
        {
            get
            {
                return new Vector3(_rigidBody.Position.X, _rigidBody.Position.Y, _rigidBody.Position.Z);
            }
        }

        public PositionedInstance Instance
        {
            get
            {
                return _instance;
            }
        }

        public RigidBody RigidBody
        {
            get
            {
                return _rigidBody;
            }
        }
    }
}
