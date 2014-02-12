using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface IRigidBodyImp
    {
        // IRigidBodyImp RigidBody(float mass, float3 worldTransform, /*shape, */ float3 inertia);

        float3 Gravity { get; set; }
        float Mass { get; set; }
        float3 Inertia { get; set; }

        float4x4 WorldTransform { get; set; }
        float3 Position { get; set; }
        Quaternion Rotation { get; }

        void ApplyForce(float3 force, float3 relPos);      //Rel_Pos definition????
        float3 ApplyTorque { get; set; } // als Field in bullet als void
        void ApplyImpulse(float3 impulse, float3 relPos);  //Rel_Pos definition????
        

        // diese drei als Field in bullet als void
        float3 ApplyTorqueImpulse { get; set; }             
        float3 ApplyCentralForce{ get; set; }             
        float3 ApplyCentralImpulse { get; set; }
         

        //Translate RigidBody by a Vector
        float3 LinearVelocity { get; set; }
        float3 AngularVelocity { get; set; }

        float3 LinearFactor { get; set; }
        float3 AngularFactor { get; set; }

        //"physic Matrial"
        float Restitution { get; set; }
        float Friction { get; set; }

        void SetDrag(float linearDrag, float anglularDrag);
        float LinearDrag { get; }
        float AngularDrag { get; }

        ICollisionShapeImp CollisionShape { get; set; }
        
        object UserObject { get; set; }

        void OnCollision(IRigidBodyImp other);
    }
}
