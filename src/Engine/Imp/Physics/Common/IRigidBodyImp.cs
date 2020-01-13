using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Common
{
    /// <summary>
    /// Implementation agnostic representation of the base class of all rigid body implementations.
    /// Contains members common to all rigid bodies.
    /// </summary>
    public interface IRigidBodyImp
    {
        // IRigidBodyImp RigidBody(float mass, float3 worldTransform, /*shape, */ float3 inertia);

        /// <summary>
        /// Gets and sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        float3 Gravity { get; set; }
        /// <summary>
        /// Gets and sets the mass.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        float Mass { get; set; }
        /// <summary>
        /// Gets and sets the inertia.
        /// </summary>
        /// <value>
        /// The inertia.
        /// </value>
        float3 Inertia { get; set; }

        /// <summary>
        /// Gets and sets the world transform.
        /// </summary>
        /// <value>
        /// The world transform.
        /// </value>
        float4x4 WorldTransform { get; set; }
        /// <summary>
        /// Gets and sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        float3 Position { get; set; }
        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        Quaternion Rotation { get; }

        /// <summary>
        /// Applies the force.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="relPos">The relative position.</param>
        void ApplyForce(float3 force, float3 relPos);      //Rel_Pos definition????
        /// <summary>
        /// Applies the impulse.
        /// </summary>
        /// <param name="impulse">The impulse.</param>
        /// <param name="relPos">The relative position.</param>
        void ApplyImpulse(float3 impulse, float3 relPos);  //Rel_Pos definition????


        /// <summary>
        /// Applies the specified torque on this rigigd body instance.
        /// </summary>
        /// <param name="torque">The torque.</param>
        void ApplyTorque(float3 torque);


        /// <summary>
        /// Applies the specified torque impulse on this rigid body instance.
        /// </summary>
        /// <param name="torqueImpulse">The torque impulse.</param>
        void ApplyTorqueImpulse(float3 torqueImpulse);

        /// <summary>
        /// Applies the central force apply central force.
        /// </summary>
        /// <param name="centralForce">The central force.</param>
        void ApplyCentralForce(float3 centralForce);

        /// <summary>
        /// Applies the given impulse on the center of this rigid body instance.
        /// </summary>
        /// <param name="centralImpulse">The central impulse.</param>
        void ApplyCentralImpulse(float3 centralImpulse);

        //Translate RigidBody by a Vector
        /// <summary>
        /// Gets and sets the linear velocity.
        /// </summary>
        /// <value>
        /// The linear velocity.
        /// </value>
        float3 LinearVelocity { get; set; }
        /// <summary>
        /// Gets and sets the angular velocity.
        /// </summary>
        /// <value>
        /// The angular velocity.
        /// </value>
        float3 AngularVelocity { get; set; }

        /// <summary>
        /// Gets and sets the linear factor.
        /// </summary>
        /// <value>
        /// The linear factor.
        /// </value>
        float3 LinearFactor { get; set; }
        /// <summary>
        /// Gets and sets the angular factor.
        /// </summary>
        /// <value>
        /// The angular factor.
        /// </value>
        float3 AngularFactor { get; set; }

        //"physic Matrial"
        /// <summary>
        /// Gets and sets the restitution.
        /// </summary>
        /// <value>
        /// The restitution.
        /// </value>
        float Restitution { get; set; }
        /// <summary>
        /// Gets and sets the friction.
        /// </summary>
        /// <value>
        /// The friction.
        /// </value>
        float Friction { get; set; }

        /// <summary>
        /// Sets the drag.
        /// </summary>
        /// <param name="linearDrag">The linear drag.</param>
        /// <param name="anglularDrag">The anglular drag.</param>
        void SetDrag(float linearDrag, float anglularDrag);
        /// <summary>
        /// Gets the linear drag.
        /// </summary>
        /// <value>
        /// The linear drag.
        /// </value>
        float LinearDrag { get; }
        /// <summary>
        /// Gets the angular drag.
        /// </summary>
        /// <value>
        /// The angular drag.
        /// </value>
        float AngularDrag { get; }

        /// <summary>
        /// Gets and sets the collision shape.
        /// </summary>
        /// <value>
        /// The collision shape.
        /// </value>
        ICollisionShapeImp CollisionShape { get; set; }

        /// <summary>
        /// Gets and sets a user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        object UserObject { get; set; }

        /// <summary>
        /// Called when a collision arises.
        /// </summary>
        /// <param name="other">The other object we're colliding with.</param>
        void OnCollision(IRigidBodyImp other);
    }
}
