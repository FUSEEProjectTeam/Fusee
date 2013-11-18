using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;
using Microsoft.CSharp.RuntimeBinder;

namespace Fusee.Engine
{
    public class RigidBody
    {
        internal IRigidBodyImp _iRigidBodyImp;

        public Mesh Mesh { get; set; }


        public float Mass
        {
            get
            {
                return _iRigidBodyImp.Mass;
            }
            set
            {
                _iRigidBodyImp.Mass = value;
            }
        }

        public float3 Inertia
        {
            get
            {
                return _iRigidBodyImp.Inertia;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.Inertia = value;
            }
        }   

        public float4x4 WorldTransform
        {
            get
            {
                return _iRigidBodyImp.WorldTransform;
            }
            set
            {
               /* var wt = new float4x4(value.M11, value.M12, value.M13, value.M14,
                                      value.M21, value.M22, value.M23, value.M24,
                                      value.M31, value.M32, value.M33, value.M34,
                                      value.M41, value.M42, value.M43, value.M44
                                     );*/
                var o = (RigidBody) _iRigidBodyImp.UserObject;
                o._iRigidBodyImp.WorldTransform = value;
                
            }
        }

        public float3 Position
        {
            get
            {
                return _iRigidBodyImp.Position;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.Position = value;

            }
        }

        
        public void ApplyForce(float3 force, float3 relPos)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyForce(force, relPos);
        }

        public float3 ApplyTorque
        {
            get
            {
                return _iRigidBodyImp.ApplyTorque;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.ApplyTorque = value;
            }
        }

        public void ApplyImpulse(float3 impulse, float3 relPos)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyImpulse(impulse, relPos);
        }

        public float3 ApplyTorqueImpulse
        {
            get
            {
                return _iRigidBodyImp.ApplyTorqueImpulse;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.ApplyTorqueImpulse = value;
            }
        }

        public float3 ApplyCentralForce
        {
            get
            {
                return _iRigidBodyImp.ApplyCentralForce;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.ApplyCentralForce = value;
            }
        }

        public float3 ApplyCentralImpulse
        {
            get
            {
                return _iRigidBodyImp.ApplyCentralImpulse;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.ApplyCentralImpulse = value;
            }
        }


        public float3 LinearVelocity
        {
            get
            {
                return _iRigidBodyImp.LinearVelocity;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.LinearVelocity = value;
            }
        }

        public float3 AngularVelocity
        {
            get
            {
                return _iRigidBodyImp.AngularVelocity;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.AngularVelocity = value;
            }
        }


        public float3 LinearFactor
        {
            get
            {
                return _iRigidBodyImp.LinearFactor;
            }
            set
            {
                var o = (RigidBody) _iRigidBodyImp.UserObject;
                o._iRigidBodyImp.LinearFactor = value;
            }
        }

        public float3 AngularFactor
        {
            get
            {
                return _iRigidBodyImp.LinearFactor;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.AngularFactor = value;
            }
        }

        public float Bounciness
        {
            get
            {
                return _iRigidBodyImp.Bounciness;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.Bounciness = value;
            }
        }


    }
}
