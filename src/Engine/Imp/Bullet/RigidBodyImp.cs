using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Xml.Schema;
using Fusee.Math;
using BulletSharp;


namespace Fusee.Engine
{
    public class RigidBodyImp : IRigidBodyImp
    {

        internal RigidBody _rbi;

        private float _mass;
        public float Mass 
        {
            get
            {
                return _mass;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                var btInertia = o._rbi.CollisionShape.CalculateLocalInertia(value);
                o._rbi.SetMassProps(value, btInertia);
                _mass = value;
            } 
        }

        private float3 _inertia;
        public float3 Inertia 
        {
            get
            {
                return _inertia;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.SetMassProps(_mass, new Vector3(value.x, value.y, value.z));
                _inertia = value;
            } 
        }
        
        public float4x4 WorldTransform
        {
            get
            {
                //Todo: Reusable function to convert btMatrix to fuseeFloat4x4   
                var retval = new float4x4(_rbi.WorldTransform.M11, _rbi.WorldTransform.M12, _rbi.WorldTransform.M13, _rbi.WorldTransform.M14,
                                          _rbi.WorldTransform.M21, _rbi.WorldTransform.M22, _rbi.WorldTransform.M23, _rbi.WorldTransform.M24,
                                          _rbi.WorldTransform.M31, _rbi.WorldTransform.M32, _rbi.WorldTransform.M33, _rbi.WorldTransform.M34,
                                          _rbi.WorldTransform.M41, _rbi.WorldTransform.M42, _rbi.WorldTransform.M43, _rbi.WorldTransform.M44
                                          );
                
                return retval;
            }
            set
            {
                var wt = new Matrix();

                //Todo: Reusable function to convert fuseeFloat4x4 to btMatrix 
                wt.M11 = value.M11; wt.M12 = value.M12; wt.M13 = value.M13; wt.M14 = value.M14;
                wt.M21 = value.M21; wt.M22 = value.M22; wt.M23 = value.M23; wt.M24 = value.M24;
                wt.M31 = value.M31; wt.M32 = value.M32; wt.M33 = value.M33; wt.M34 = value.M34;
                wt.M41 = value.M41; wt.M42 = value.M42; wt.M43 = value.M43; wt.M44 = value.M44;
                
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.WorldTransform = wt;
            }
        }

        public float3 Position
        {
            get
            {
                var retval = new float3(_rbi.WorldTransform.M41,
                                        _rbi.WorldTransform.M42,
                                        _rbi.WorldTransform.M43);
                return retval;
            }
            set
            {
                var m = new Matrix();
                m.set_Rows(3, new Vector4(value.x, value.y, value.z, 1));

                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.WorldTransform = m;// * 10.0f;
            }
        }

        
        public void ApplyForce(float3 force, float3 relPos)
        {
            Debug.WriteLine("RigidBodyImp: ApplyForce");
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyForce(new Vector3(force.x, force.y, force.z), new Vector3(relPos.x, relPos.y, relPos.z));
        }

        private float3 _applyTorque;
        public float3 ApplyTorque
        {
            get
            {
                return _applyTorque;
            }
            set
            {
                Debug.WriteLine("ApplyTorque");
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.ApplyTorque(new Vector3(value.x /* *10*10 */, value.y/* *10*10 */, value.z/* *10*10 */));
            }
        }

        public void ApplyImpulse(float3 impulse, float3 relPos)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            //impulse *= 10;
            o._rbi.ApplyImpulse(new Vector3(impulse.x, impulse.y, impulse.z), new Vector3(relPos.x, relPos.y, relPos.z));
        }


        private float3 _torqueImpulse;
        public float3 ApplyTorqueImpulse
        {
            get
            {
                return _torqueImpulse;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.ApplyTorqueImpulse(new Vector3(value.x /* *10 */, value.y/* *10 */, value.z/* *10 */));
                _torqueImpulse = value;//*10;
            }
        }

        private float3 _centralForce;
        public float3 ApplyCentralForce
        {
            get
            {
                return _centralForce;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.ApplyCentralForce(new Vector3(value.x/* *10 */, value.y/* *10 */, value.z/* *10 */));
                _centralForce = value;//*10;
            }
        }

        private float3 _centralImpulse;
        public float3 ApplyCentralImpulse
        {
            get
            {
                return _centralImpulse;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.ApplyCentralImpulse(new Vector3(value.x/* *10 */, value.y/* *10 */, value.z/* *10 */));
                _centralImpulse = value;//*10;
            }
        }

        public float3 LinearVelocity 
        {
            get
            {
                var retval = new float3(_rbi.LinearVelocity.X, _rbi.LinearVelocity.Y, _rbi.LinearVelocity.Z);
                return retval;
            } 
            set
            {
                var linVel = new Vector3(value.x, value.y, value.z);
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.LinearVelocity = linVel;
            }
        }

        public float3 AngularVelocity
        {
            get
            {
                var retval = new float3(_rbi.AngularVelocity.X, _rbi.AngularVelocity.Y, _rbi.AngularVelocity.Z);
                return retval;
            }
            set
            {
                var angVel = new Vector3(value.x, value.y, value.z);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.AngularVelocity = angVel;
            }
        }

        

        public float Bounciness
        {
            get
            {
                return _rbi.Restitution;
            }
            set
            {
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.Restitution = value;
            }
        }

       


        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
