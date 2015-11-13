using System;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of the base class of all rigid body implementations.
    /// Contains members common to all rigid bodies.
    /// </summary>
    public interface IRigidBodyImp
    {
        // IRigidBodyImp RigidBody(float mass, float3 worldTransform, /*shape, */ float3 inertia);

        /// <summary>
        /// Gets or sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        float3 Gravity { get; set; }
        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        float Mass { get; set; }
        /// <summary>
        /// Gets or sets the inertia.
        /// </summary>
        /// <value>
        /// The inertia.
        /// </value>
        float3 Inertia { get; set; }

        /// <summary>
        /// Gets or sets the world transform.
        /// </summary>
        /// <value>
        /// The world transform.
        /// </value>
        float4x4 WorldTransform { get; set; }
        /// <summary>
        /// Gets or sets the position.
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
        /// Gets or sets the apply torque.
        /// </summary>
        /// <value>
        /// The apply torque.
        /// </value>
        float3 ApplyTorque { get; set; } // als Field in bullet als void
        /// <summary>
        /// Applies the impulse.
        /// </summary>
        /// <param name="impulse">The impulse.</param>
        /// <param name="relPos">The relative position.</param>
        void ApplyImpulse(float3 impulse, float3 relPos);  //Rel_Pos definition????


        // diese drei als Field in bullet als void
        /// <summary>
        /// Gets or sets the apply torque impulse.
        /// </summary>
        /// <value>
        /// The apply torque impulse.
        /// </value>
        float3 ApplyTorqueImpulse { get; set; }
        /// <summary>
        /// Gets or sets the apply central force.
        /// </summary>
        /// <value>
        /// The apply central force.
        /// </value>
        float3 ApplyCentralForce { get; set; }
        /// <summary>
        /// Gets or sets the apply central impulse.
        /// </summary>
        /// <value>
        /// The apply central impulse.
        /// </value>
        float3 ApplyCentralImpulse { get; set; }


        //Translate RigidBody by a Vector
        /// <summary>
        /// Gets or sets the linear velocity.
        /// </summary>
        /// <value>
        /// The linear velocity.
        /// </value>
        float3 LinearVelocity { get; set; }
        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        /// <value>
        /// The angular velocity.
        /// </value>
        float3 AngularVelocity { get; set; }

        /// <summary>
        /// Gets or sets the linear factor.
        /// </summary>
        /// <value>
        /// The linear factor.
        /// </value>
        float3 LinearFactor { get; set; }
        /// <summary>
        /// Gets or sets the angular factor.
        /// </summary>
        /// <value>
        /// The angular factor.
        /// </value>
        float3 AngularFactor { get; set; }

        //"physic Matrial"
        /// <summary>
        /// Gets or sets the restitution.
        /// </summary>
        /// <value>
        /// The restitution.
        /// </value>
        float Restitution { get; set; }
        /// <summary>
        /// Gets or sets the friction.
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
        /// Gets or sets the collision shape.
        /// </summary>
        /// <value>
        /// The collision shape.
        /// </value>
        ICollisionShapeImp CollisionShape { get; set; }

        /// <summary>
        /// Gets or sets a user object.
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
