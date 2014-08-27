using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;
using System.Collections.Generic;

namespace Fusee.Engine
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

        /// <summary>
        /// Adds an impulse 
        /// </summary>
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
                colShape.ICollisionShapeImp = (ICollisionShapeImp)shape;
                shape.UserObject = colShape;
                return colShape;*/
                //Debug.WriteLine("shapeType" + shapeType);
                switch (shapeType)
                {
                    //Primitives
                    case "Fusee.Engine.BoxShapeImp":
                        var box = new BoxShape();
                        box.BoxShapeImp = (IBoxShapeImp)shape;
                        shape.UserObject = box;
                        return box;
                    case "Fusee.Engine.SphereShapeImp":
                        var sphere = new SphereShape();
                        sphere.SphereShapeImp = (ISphereShapeImp)shape;
                        shape.UserObject = sphere;
                        return sphere;
                    case "Fusee.Engine.CapsuleShapeImp":
                        var capsule = new CapsuleShape();
                        capsule.CapsuleShapeImp = (ICapsuleShapeImp)shape;
                        shape.UserObject = capsule;
                        return capsule;
                    case "Fusee.Engine.CylinderShapeImp":
                        var cylinder = new CylinderShape();
                        cylinder.CylinderShapeImp = (ICylinderShapeImp)shape;
                        shape.UserObject = cylinder;
                        return cylinder;
                    case "Fusee.Engine.ConeShapeImp":
                        var cone = new ConeShape();
                        cone.ConeShapeImp = (IConeShapeImp)shape;
                        shape.UserObject = cone;
                        return cone;
                    case "Fusee.Engine.MultiSphereShapeImp":
                        var multiSphere = new MultiSphereShape();
                        multiSphere.MultiSphereShapeImp = (IMultiSphereShapeImp)shape;
                        shape.UserObject = multiSphere;
                        return multiSphere;
                    //Meshes
                    case "Fusee.Engine.ConvexHullShapeImp":
                        var convHull = new ConvexHullShape();
                        convHull.ConvexHullShapeImp = (IConvexHullShapeImp) shape;
                        shape.UserObject = convHull;
                        return convHull;
                    case "Fusee.Engine.GImpactMeshShapeImp":
                        var gimp = new GImpactMeshShape();
                        gimp.GImpactMeshShapeImp = (IGImpactMeshShapeImp)shape;
                        shape.UserObject = gimp;
                        return gimp;
                    case "Fusee.Engine.StaticPlaneShapeImp":
                        var staticPlane = new StaticPlaneShape();
                        staticPlane.StaticPlaneShapeImp = (IStaticPlaneShapeImp)shape;
                        shape.UserObject = staticPlane;
                        return staticPlane;
                    //Misc
                    case "Fusee.Engine.CompoundShapeImp":
                        //Debug.WriteLine("Fusee.Engine.CompoundShapeImp");
                        var comp = new CompoundShape();
                        comp.CompoundShapeImp = (ICompoundShapeImp)shape;
                        shape.UserObject = comp;
                        return comp;
                    case "Fusee.Engine.EmptyShape":
                        var empty = new EmptyShape();
                        empty.EmtyShapeImp = (IEmptyShapeImp)shape;
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
                        o._iRigidBodyImp.CollisionShape = box.BoxShapeImp;
                        break;
                    case "Fusee.Engine.CapsuleShape":
                        var capsule = (CapsuleShape)value;
                        o._iRigidBodyImp.CollisionShape = capsule.CapsuleShapeImp;
                        break;
                    case "Fusee.Engine.ConeShape":
                        var cone = (ConeShape)value;
                        o._iRigidBodyImp.CollisionShape = cone.ConeShapeImp;
                        break;
                    case "Fusee.Engine.CylinderShape":
                        var cylinder = (CylinderShape)value;
                        o._iRigidBodyImp.CollisionShape = cylinder.CylinderShapeImp;
                        break;
                    case "Fusee.Engine.MultiSphereShape":
                        var multiSphere = (MultiSphereShape)value;
                        o._iRigidBodyImp.CollisionShape = multiSphere.MultiSphereShapeImp;
                        break;
                    case "Fusee.Engine.SphereShape":
                        var sphere = (SphereShape)value;
                        o._iRigidBodyImp.CollisionShape = sphere.SphereShapeImp;
                        break;
                    //Meshes
                    case "Fusee.Engine.ConvexHullShape":
                        var convHull = (ConvexHullShape)value;
                        o._iRigidBodyImp.CollisionShape = convHull.ConvexHullShapeImp;
                        break;
                    case "Fusee.Engine.StaticPlaneShape":
                        var staticPlane = (StaticPlaneShape)value;
                        o._iRigidBodyImp.CollisionShape = staticPlane.StaticPlaneShapeImp;
                        break;
                    //Misc
                    case "Fusee.Engine.CompoundShape":
                        var compShape = (CompoundShape)value;
                        o._iRigidBodyImp.CollisionShape = compShape.CompoundShapeImp;
                        break;
                    case "Fusee.Engine.EmptyShape":
                        var empty = (EmptyShape)value;
                        o._iRigidBodyImp.CollisionShape = empty.EmtyShapeImp;
                        break;
                       
                    //Default
                    default:
                        //TODO: Exeption
                        var defaultShape = new EmptyShape();
                        Debug.WriteLine("default");
                        //rbi = _dwi.AddRigidBody(mass, worldTransform, defaultShape.EmtyShapeImp, inertia);
                        break;
                }
            }
        }
    }
}
