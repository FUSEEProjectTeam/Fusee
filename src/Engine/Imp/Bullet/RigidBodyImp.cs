using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using Fusee.Math;
using BulletSharp;



namespace Fusee.Engine
{
    public class RigidBodyImp : IRigidBodyImp
    {

        internal RigidBody _rbi;
        internal CollisionShape btColShape;
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

        /*public void SetCollisionShape(CollisionShape colShape)
        {
            BulletSharp.CollisionShape btShape = null;
           
            string type = colShape.GetType().ToString();
            Debug.WriteLine(colShape);
            switch (type)
            {   
                case "Fusee.Engine.CollisionShapeBox":
                    CollisionShapeBox box = (CollisionShapeBox)colShape;
                    var btBox = new BoxShape(box.HalfExtents.x , box.HalfExtents.y , box.HalfExtents.z );
                    btShape = btBox;
                    break;
                case "Fusee.Engine.CollisionShapeCapsule":
                    CollisionShapeCapsule capsule = (CollisionShapeCapsule)colShape;
                    var btCapsule = new CapsuleShape(capsule.Radius, capsule.Height);
                    btShape = btCapsule;
                    break;
                case "Fusee.Engine.CollisionShapeCone":
                    CollisionShapeCone cone = (CollisionShapeCone)colShape;
                    var btCone = new ConeShape(cone.Radius, cone.Height);
                    btCone.ConeUpIndex = cone.UpAxis;
                    btShape = btCone;
                    break;
                case "Fusee.Engine.CollisionShapeCylinder":
                    CollisionShapeCylinder cylinder = (CollisionShapeCylinder)colShape;
                    var btCylinder = new CylinderShape(cylinder.HalfExtents.x, cylinder.HalfExtents.y, cylinder.HalfExtents.z);
                    btShape = btCylinder;
                    break;
                case "Fusee.Engine.CollisionShapeMultiSphere":
                    CollisionShapeMultiSphere multiSphere = (CollisionShapeMultiSphere)colShape;

                    var positions = new Vector3[multiSphere.SphereCount()];
                    var radi = new float[multiSphere.SphereCount()];
                    for (int i = 0; i < multiSphere.SphereCount(); i++)
                    {
                        var posI = new Vector3(multiSphere.GetSpherePosition(i).x, multiSphere.GetSpherePosition(i).y,
                            multiSphere.GetSpherePosition(i).z);
                        positions[i] = posI;
                        radi[i] = multiSphere.GetSphereRadius(i);
                    }
                    var btMultiSphere = new MultiSphereShape(positions, radi);
                    btShape = btMultiSphere;
                    break;
                case "Fusee.Engine.CollisionShapeSphere":
                    CollisionShapeSphere sphere = (CollisionShapeSphere)colShape;
                    btShape = new SphereShape(sphere.GetRadius());
                    break;
                default:
                    Debug.WriteLine("default");
                    btShape = new EmptyShape();
                    break;
            }
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.CollisionShape = btShape;
        }*/

        //CollisionShapes

        //BoxShape
        public IBoxShapeImp AddBoxShape(float boxHalfExtents)
        {
            var btBoxShape = new BoxShape(boxHalfExtents);
            _rbi.CollisionShape = btBoxShape;

            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }
        public IBoxShapeImp AddBoxShape(float3 boxHalfExtents)
        {
            var btBoxShape = new BoxShape(Translater.Float3ToBtVector3(boxHalfExtents));
            _rbi.CollisionShape = btBoxShape;
           
            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }
        public IBoxShapeImp AddBoxShape(float boxHalfExtentsX, float boxHalfExtentsY, float boxHalfExtentsZ)
        {
            var btBoxShape = new BoxShape(boxHalfExtentsX, boxHalfExtentsY, boxHalfExtentsZ);
            _rbi.CollisionShape = btBoxShape;

            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }
       
        //SphereShape
        public ISphereShapeImp AddSphereShape(float radius)
        {
            var btSphereShape = new SphereShape(radius);
            _rbi.CollisionShape = btSphereShape;

            var retval = new SphereShapeImp();
            retval.BtSphereShape = btSphereShape;
            btSphereShape.UserObject = retval;
            return retval;
        }

        //CapsuleShape
        public ICapsuleShapeImp AddCapsuleShape(float radius, float height)
        {
            var btCapsuleShape = new CapsuleShape(radius, height);
            _rbi.CollisionShape = btCapsuleShape;

            var retval = new CapsuleShapeImp();
            retval.BtCapsuleShape = btCapsuleShape;
            btCapsuleShape.UserObject = retval;
            return retval;
        }

        public IConeShapeImp AddConeShape(float radius, float height)
        {
            var btConeShape = new ConeShape(radius, height);
            _rbi.CollisionShape = btConeShape;

            var retval = new ConeShapeImp();
            retval.BtConeShape = btConeShape;
            btConeShape.UserObject = retval;
            return retval;
        }

        public IMultiSphereShapeImp AddMultiSphereShapeImp(float3[] positions, float[] radi)
        {
            var btPositions = new Vector3[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                btPositions[i] = new Vector3(positions[i].x, positions[i].y, positions[i].z);
            }
            var btMultiSphereShape = new MultiSphereShape(btPositions, radi);
            _rbi.CollisionShape = btMultiSphereShape;

            var retval = new MultiSphereShapeImp();
            retval.BtMultiSphereShape = btMultiSphereShape;
            btMultiSphereShape.UserObject = retval;
            return retval;
        }


        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
