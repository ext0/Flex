using Flex.Development.Execution.Data.States;
using Flex.Development.Physics;
using Flex.Misc.Tracker;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.ClearScript;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class PhysicsInstance : SizedInstance
    {
        [NonSerialized()]
        protected Shape _shape;

        [NonSerialized()]
        protected RigidBody _rigidBody;

        protected bool _anchored;
        protected bool _collisions;

        protected PhysicsInstance() : base()
        {

        }

        protected void LoadPhysicsInstance()
        {
            _shape = new BoxShape((float)_size.x, (float)_size.y, (float)_size.z);
            _rigidBody = new RigidBody(_shape);
            _rigidBody.Position = new Jitter.LinearMath.JVector(_position.x, _position.y, _position.z);
            _rigidBody.IsActive = !_anchored;
            _rigidBody.IsStatic = anchored;
            PhysicsEngine.AddVisualInstance(this);
        }

        protected void ReloadPhysicsInstance()
        {
            _shape = new BoxShape((float)_size.x, (float)_size.y, (float)_size.z);
            _rigidBody = new RigidBody(_shape);
            _rigidBody.Position = new Jitter.LinearMath.JVector(_position.x, _position.y, _position.z);
            _rigidBody.IsActive = !_anchored;
            _rigidBody.IsStatic = anchored;
            PhysicsEngine.ReloadVisualInstance(this);
        }

        protected void UnloadPhysicsInstance()
        {
            PhysicsEngine.RemoveVisualInstance(this);
        }

        [Browsable(false)]
        [ScriptMember(ScriptAccess.None)]
        internal RigidBody RigidBody
        {
            get
            {
                return _rigidBody;
            }
        }

        [Category("3D")]
        [DisplayName("Collidable")]
        [Description("Whether or not this object can be collided with by other objects")]
        [TrackMember]
        [ScriptMember(ScriptAccess.Full)]
        public bool collidable
        {
            get
            {
                return _collisions;
            }
            set
            {
                if (value == _collisions) return;
                _collisions = value;
                NotifyPropertyChanged("Collidable");
            }
        }


        [Category("3D")]
        [DisplayName("Anchored")]
        [Description("Whether or not this object is affected by physics")]
        [TrackMember]
        [ScriptMember(ScriptAccess.Full)]
        public bool anchored
        {
            get
            {
                return _anchored;
            }
            set
            {
                if (value == _anchored) return;
                _anchored = value;
                if (_rigidBody != null)
                {
                    _rigidBody.IsActive = !_anchored;
                    _rigidBody.IsStatic = anchored;
                }
                NotifyPropertyChanged("Anchored");
            }
        }
    }
}
