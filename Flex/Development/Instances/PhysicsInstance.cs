using Flex.Development.Execution.Data;
using Flex.Development.Execution.Data.States;
using Flex.Development.Physics;
using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using MogreNewt;
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
        protected ConvexCollision _shape;

        [NonSerialized()]
        protected Body _rigidBody;

        protected bool _anchored;
        protected bool _collisions;

        protected PhysicsInstance() : base()
        {

        }

        protected abstract void LoadPhysicsInstance();

        [Category("3D")]
        [DisplayName("Collidable")]
        [Description("Whether or not this object can be collided with by other objects")]
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
                    LoadPhysicsInstance();
                }
                NotifyPropertyChanged("Anchored");
            }
        }
    }
}
