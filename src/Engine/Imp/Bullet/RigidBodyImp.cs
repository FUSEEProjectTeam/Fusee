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

        public ICollisionShapeImp CollisionShape
        {
            get
            {
                var type = _rbi.CollisionShape.GetType().ToString();
                var btShape = _rbi.CollisionShape;
                
                switch (type)
                {
                    //Primitives
                    case "BulletSharp.BoxShape":
                        var btBox = (BoxShape) btShape;
                        var box = new BoxShapeImp();
                        box.BtBoxShape = btBox;
                        btBox.UserObject = box;
                        return box;
                    case "BulletSharp.SphereShape":
                        var btSphere = (SphereShape) btShape;
                        var sphere = new SphereShapeImp();
                        sphere.BtSphereShape = btSphere;
                        btSphere.UserObject = sphere;
                        return sphere;
                    case "BulletSharp.CapsuleShape":
                        var btCapsule = (CapsuleShape) btShape;
                        var capsule = new CapsuleShapeImp();
                        capsule.BtCapsuleShape = btCapsule;
                        btCapsule.UserObject = capsule;
                        return capsule;
                    case "BulletSharp.CylinderShape":
                        var btCylinder = (CylinderShape) btShape;
                        var cylinder = new CylinderShapeImp();
                        cylinder.BtCylinderShape = btCylinder;
                        btCylinder.UserObject = cylinder;
                        return cylinder;
                    case "BulletSharp.ConeShape":
                        var btCone = (ConeShape) btShape;
                        var cone = new ConeShapeImp();
                        cone.BtConeShape = btCone;
                        btCone.UserObject = cone;
                        return cone;
                    case "BulletSharp.MultiSphereShape":
                        var btMulti = (MultiSphereShape) btShape;
                        var multi = new MultiSphereShapeImp();
                        multi.BtMultiSphereShape = btMulti;
                        btMulti.UserObject = multi;
                        return multi;
                    //Meshes
                    case "BulletSharp.ConvexHullShape":
                        var btConvHull = (ConvexHullShape) btShape;
                        var convHull = new ConvexHullShapeImp();
                        convHull.BtConvexHullShape = btConvHull;
                        btConvHull.UserObject = convHull;
                        return convHull;
                    //Misc
                    case "BulletSharp.CompoundShape":
                        var btComp = (CompoundShape) btShape;
                        var comp = new CompoundShapeImp();
                        comp.BtCompoundShape = btComp;
                        btComp.UserObject = comp;
                        return comp;
                    default:
                        return new EmptyShapeImp();
                }

            }
            set
            {
                var shape = value;
                var shapeType = value.GetType().ToString();

                CollisionShape btColShape;

                switch (shapeType)
                {
                    //Primitives
                    case "Fusee.Engine.BoxShapeImp":
                        var box = (BoxShapeImp)value;
                        var btBoxHalfExtents = Translater.Float3ToBtVector3(box.HalfExtents);
                        btColShape = new BoxShape(btBoxHalfExtents);
                        break;
                    case "Fusee.Engine.CapsuleShapeImp":
                        var capsule = (CapsuleShapeImp)value;
                        btColShape = new CapsuleShape(capsule.Radius, capsule.HalfHeight);
                        break;
                    case "Fusee.Engine.ConeShapeImp":
                        var cone = (ConeShapeImp)value;
                        btColShape = new ConeShape(cone.Radius, cone.Height);
                        break;
                    case "Fusee.Engine.CylinderShapeImp":
                        var cylinider = (CylinderShapeImp)value;
                        var btCylinderHalfExtents = Translater.Float3ToBtVector3(cylinider.HalfExtents);
                        btColShape = new CylinderShape(btCylinderHalfExtents);
                        break;
                    case "Fusee.Engine.MultiSphereShapeImp":
                        var multiSphere = (MultiSphereShapeImp)value;
                        var btPositions = new Vector3[multiSphere.SphereCount];
                        var btRadi = new float[multiSphere.SphereCount];
                        for (int i = 0; i < multiSphere.SphereCount; i++)
                        {
                            var pos = Translater.Float3ToBtVector3(multiSphere.GetSpherePosition(i));
                            btPositions[i] = pos;
                            btRadi[i] = multiSphere.GetSphereRadius(i);
                        }
                        btColShape = new MultiSphereShape(btPositions, btRadi);
                        break;
                    case "Fusee.Engine.SphereShapeImp":
                        var sphere = (SphereShapeImp)value;
                        var btRadius = sphere.Radius;
                        btColShape = new SphereShape(btRadius);
                        break;

                    //Misc
                    case "Fusee.Engine.CompoundShapeImp":
                        var compShape = (CompoundShapeImp)value;
                        btColShape = new CompoundShape();
                        break;
                    case "Fusee.Engine.EmptyShapeImp":
                        btColShape = new EmptyShape();
                        break;
                    //Meshes

                    //Default
                    default:
                        Debug.WriteLine("defaultImp");
                        btColShape = new EmptyShape();
                        break;
                }

                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.CollisionShape = btColShape;
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
