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
        internal Translater Translater = new Translater();
        

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
                var retval = Translater.BtMatrixToFloat4X4(_rbi.WorldTransform);           
                return retval;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.WorldTransform = Translater.Float4X4ToBtMatrix(value);
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
                o._rbi.WorldTransform = m * 10.0f;
            }
        }

        public float4 Rotation
        {
            get;
           
            set;
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
                o._rbi.ApplyTorque(new Vector3(value.x  *10*10 , value.y *10*10 , value.z *10*10 ));
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
                o._rbi.ApplyTorqueImpulse(new Vector3(value.x  *10 , value.y *10 , value.z *10 ));
                _torqueImpulse = value*10;
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
                o._rbi.ApplyCentralForce(new Vector3(value.x *10 , value.y *10 , value.z *10 ));
                _centralForce = value*10;
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
                o._rbi.ApplyCentralImpulse(new Vector3(value.x *10 , value.y *10 , value.z *10 ));
                _centralImpulse = value*10;
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

        public float3 LinearFactor
        {
            get
            {
                var retval = new float3(_rbi.LinearFactor.X, _rbi.LinearFactor.Y, _rbi.LinearFactor.Z);
                return retval;
            }
            set
            {
                var linfac = new Vector3(value.x, value.y, value.z);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.LinearFactor = linfac;
            }
        }

        public float3 AngularFactor
        {
            get
            {
                var retval = new float3(_rbi.AngularFactor.X, _rbi.AngularFactor.Y, _rbi.AngularFactor.Z);
                return retval;
            }
            set
            {
                var angfac = new Vector3(value.x, value.y, value.z);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.AngularFactor = angfac;
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
