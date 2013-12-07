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


        public float3 Gravity
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_rbi.Gravity);
                return retval;
            }
            set
            {
                var o = (RigidBodyImp) _rbi.UserObject;
                _rbi.Gravity = Translater.Float3ToBtVector3(value);
            }
        }
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
                o._rbi.SetMassProps(_mass, Translater.Float3ToBtVector3(value));
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

       
        
        public void ApplyForce(float3 force, float3 relPos)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyForce(Translater.Float3ToBtVector3(force), Translater.Float3ToBtVector3(relPos));
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
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.ApplyTorque(Translater.Float3ToBtVector3(value*10));
            }
        }

        public void ApplyImpulse(float3 impulse, float3 relPos)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
           // impulse *= 10;
            o._rbi.ApplyImpulse(Translater.Float3ToBtVector3(impulse), Translater.Float3ToBtVector3(relPos));
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
                o._rbi.ApplyTorqueImpulse(Translater.Float3ToBtVector3(value * 10));
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
                o._rbi.ApplyCentralForce(Translater.Float3ToBtVector3(value * 10));
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
                o._rbi.ApplyCentralImpulse(Translater.Float3ToBtVector3(value * 10));
                _centralImpulse = value*10;
            }
        }

        public float3 LinearVelocity 
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_rbi.LinearVelocity);
                return retval;
            } 
            set
            {
                var linVel = Translater.Float3ToBtVector3(value);
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.LinearVelocity = linVel;
            }
        }

        public float3 AngularVelocity
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_rbi.AngularVelocity);
                return retval;
            }
            set
            {
                var angVel = Translater.Float3ToBtVector3(value);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.AngularVelocity = angVel;
            }
        }

        public float3 LinearFactor
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_rbi.LinearFactor);
                return retval;
            }
            set
            {
                var linfac = Translater.Float3ToBtVector3(value);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.LinearFactor = linfac;
            }
        }

        public float3 AngularFactor
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_rbi.AngularFactor);
                return retval;
            }
            set
            {
                var angfac = Translater.Float3ToBtVector3(value);
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
