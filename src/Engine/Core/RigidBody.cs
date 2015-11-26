using System;
using System.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// The RigidBody class ...
    /// </summary>
    public class RigidBody
    {

        public event EventHandler WhileCollidingEvent = delegate { };
        internal IRigidBodyImp _iRigidBodyImp;

       // public Mesh Mesh { get; set; }

        public float3 Gravity
        {
            get
            {
                var retval = _iRigidBodyImp.Gravity;
                return retval;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                _iRigidBodyImp.Gravity = value;
            }
        }

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

        /// <summary>
        /// The orientation of the rigidbody in world space stored as a Quaternion.
        /// </summary>
        public Quaternion Rotation()
        {
            return _iRigidBodyImp.Rotation;
        }

        /// <summary>
        /// Adds a force to the rigidbody. As a result the rigidbody will start moving.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="relPos"></param>
        public void ApplyForce(float3 force, float3 relPos)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyForce(force, relPos);
        }

        /// <summary>
        /// Adds a torque to the rigidbody.
        /// </summary>
        public void ApplyTorque(float3 torque)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyTorque(torque);
        }

        public void ApplyImpulse(float3 impulse, float3 relPos)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyImpulse(impulse, relPos);
        }

        public void ApplyTorqueImpulse(float3 torqueImpulse)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyTorqueImpulse(torqueImpulse);
        }

        public void ApplyCentralForce(float3 centralForce)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyCentralForce(centralForce);
        }

        /// <summary>
        /// Adds an impulse 
        /// </summary>
        public void ApplyCentralImpulse(float3  centralImpulse)
        {
            var o = (RigidBody)_iRigidBodyImp.UserObject;
            o._iRigidBodyImp.ApplyCentralImpulse(centralImpulse);
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

        public float Restitution
        {
            get
            {
                return _iRigidBodyImp.Restitution;
            }
            set
            {
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                o._iRigidBodyImp.Restitution = value;
            }
        }

        public float Friction
        {
            get { return _iRigidBodyImp.Friction; }
            set
            {
                var o = (RigidBody) _iRigidBodyImp.UserObject;
                o._iRigidBodyImp.Friction = value;
            }
        }

        public void SetDrag(float linearDrag, float anglularDrag)
        {
            var o = (RigidBody) _iRigidBodyImp.UserObject;
            o._iRigidBodyImp.SetDrag(linearDrag, anglularDrag);
        }
        public float LinearDrag
        {
            get { return _iRigidBodyImp.LinearDrag; }

        }

        public float AngularDrag
        {
            get { return _iRigidBodyImp.AngularDrag; }
        }



        public CollisionShape CollisionShape
        {
            get
            {
                var shape = _iRigidBodyImp.CollisionShape;
                var shapeType = shape.GetType().ToString();
               /* var colShape = new CollisionShape();
                colShape._collisionShapeImp = (_collisionShapeImp)shape;
                shape.UserObject = colShape;
                return colShape;*/
                //Debug.WriteLine("shapeType" + shapeType);
                switch (shapeType)
                {
                    //Primitives
                    case "Fusee.Engine._boxShapeImp":
                        var box = new BoxShape();
                        box._boxShapeImp = (IBoxShapeImp)shape;
                        shape.UserObject = box;
                        return box;
                    case "Fusee.Engine._sphereShapeImp":
                        var sphere = new SphereShape();
                        sphere._sphereShapeImp = (ISphereShapeImp)shape;
                        shape.UserObject = sphere;
                        return sphere;
                    case "Fusee.Engine._capsuleShapeImp":
                        var capsule = new CapsuleShape();
                        capsule._capsuleShapeImp = (ICapsuleShapeImp)shape;
                        shape.UserObject = capsule;
                        return capsule;
                    case "Fusee.Engine._cylinderShapeImp":
                        var cylinder = new CylinderShape();
                        cylinder._cylinderShapeImp = (ICylinderShapeImp)shape;
                        shape.UserObject = cylinder;
                        return cylinder;
                    case "Fusee.Engine._coneShapeImp":
                        var cone = new ConeShape();
                        cone._coneShapeImp = (IConeShapeImp)shape;
                        shape.UserObject = cone;
                        return cone;
                    case "Fusee.Engine._multiSphereShapeImp":
                        var multiSphere = new MultiSphereShape();
                        multiSphere._multiSphereShapeImp = (IMultiSphereShapeImp)shape;
                        shape.UserObject = multiSphere;
                        return multiSphere;
                    //Meshes
                    case "Fusee.Engine._convexHullShapeImp":
                        var convHull = new ConvexHullShape();
                        convHull._convexHullShapeImp = (IConvexHullShapeImp) shape;
                        shape.UserObject = convHull;
                        return convHull;
                    case "Fusee.Engine._gImpactMeshShapeImp":
                        var gimp = new GImpactMeshShape();
                        gimp._gImpactMeshShapeImp = (IGImpactMeshShapeImp)shape;
                        shape.UserObject = gimp;
                        return gimp;
                    case "Fusee.Engine._staticPlaneShapeImp":
                        var staticPlane = new StaticPlaneShape();
                        staticPlane._staticPlaneShapeImp = (IStaticPlaneShapeImp)shape;
                        shape.UserObject = staticPlane;
                        return staticPlane;
                    //Misc
                    case "Fusee.Engine._compoundShapeImp":
                        //Debug.WriteLine("Fusee.Engine._compoundShapeImp");
                        var comp = new CompoundShape();
                        comp._compoundShapeImp = (ICompoundShapeImp)shape;
                        shape.UserObject = comp;
                        return comp;
                    case "Fusee.Engine.EmptyShape":
                        var empty = new EmptyShape();
                        empty._emtyShapeImp = (IEmptyShapeImp)shape;
                        shape.UserObject = empty;
                        return empty;
                    default:
                        return new EmptyShape();
                }

            }
            set
            {
                var shapeType = value.GetType().ToString();
                var o = (RigidBody)_iRigidBodyImp.UserObject;
                switch (shapeType)
                {
                    //Primitives
                    case "Fusee.Engine.BoxShape":
                        var box = (BoxShape)value;
                        o._iRigidBodyImp.CollisionShape = box._boxShapeImp;
                        break;
                    case "Fusee.Engine.CapsuleShape":
                        var capsule = (CapsuleShape)value;
                        o._iRigidBodyImp.CollisionShape = capsule._capsuleShapeImp;
                        break;
                    case "Fusee.Engine.ConeShape":
                        var cone = (ConeShape)value;
                        o._iRigidBodyImp.CollisionShape = cone._coneShapeImp;
                        break;
                    case "Fusee.Engine.CylinderShape":
                        var cylinder = (CylinderShape)value;
                        o._iRigidBodyImp.CollisionShape = cylinder._cylinderShapeImp;
                        break;
                    case "Fusee.Engine.MultiSphereShape":
                        var multiSphere = (MultiSphereShape)value;
                        o._iRigidBodyImp.CollisionShape = multiSphere._multiSphereShapeImp;
                        break;
                    case "Fusee.Engine.SphereShape":
                        var sphere = (SphereShape)value;
                        o._iRigidBodyImp.CollisionShape = sphere._sphereShapeImp;
                        break;
                    //Meshes
                    case "Fusee.Engine.ConvexHullShape":
                        var convHull = (ConvexHullShape)value;
                        o._iRigidBodyImp.CollisionShape = convHull._convexHullShapeImp;
                        break;
                    case "Fusee.Engine.StaticPlaneShape":
                        var staticPlane = (StaticPlaneShape)value;
                        o._iRigidBodyImp.CollisionShape = staticPlane._staticPlaneShapeImp;
                        break;
                    //Misc
                    case "Fusee.Engine.CompoundShape":
                        var compShape = (CompoundShape)value;
                        o._iRigidBodyImp.CollisionShape = compShape._compoundShapeImp;
                        break;
                    case "Fusee.Engine.EmptyShape":
                        var empty = (EmptyShape)value;
                        o._iRigidBodyImp.CollisionShape = empty._emtyShapeImp;
                        break;
                       
                    //Default
                    default:
                        //TODO: Exeption
                        var defaultShape = new EmptyShape();
                        Debug.WriteLine("default");
                        //rbi = _dwi.AddRigidBody(mass, worldTransform, defaultShape._emtyShapeImp, inertia);
                        break;
                }
            }
        }
    }
}
