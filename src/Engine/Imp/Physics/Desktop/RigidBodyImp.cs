
using System.Diagnostics;
using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Quaternion = Fusee.Math.Core.Quaternion;


namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IRigidBodyImp" /> interface using the bullet physics engine.
    /// </summary>
    public class RigidBodyImp : IRigidBodyImp
    {
        internal RigidBody _rbi;
        internal CollisionShape btColShape;

        /// <summary>
        /// Gets and sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        public float3 Gravity
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.Gravity);
                return retval;
            }
            set
            {
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.Gravity = Translator.Float3ToBtVector3(value);
            }
        }
        private float _mass;
        /// <summary>
        /// Gets and sets the mass.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
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
        /// <summary>
        /// Gets and sets the inertia.
        /// </summary>
        /// <value>
        /// The inertia.
        /// </value>
        public float3 Inertia 
        {
            get
            {
                return _inertia;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.SetMassProps(_mass, Translator.Float3ToBtVector3(value));
                _inertia = value;
            } 
        }

        /// <summary>
        /// Gets and sets the world transform.
        /// </summary>
        /// <value>
        /// The world transform.
        /// </value>
        public float4x4 WorldTransform
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_rbi.WorldTransform);           
                return retval;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.WorldTransform = Translator.Float4X4ToBtMatrix(value);
            }
        }

        /// <summary>
        /// Gets and sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public float3 Position
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.CenterOfMassPosition);
                return retval;
            }
            set
            {
                var o = (RigidBodyImp)_rbi.UserObject;
                var transform = _rbi.CenterOfMassTransform;
                transform.Origin = new Vector3(value.x, value.y, value.z);
                o._rbi.CenterOfMassTransform = transform;
            }
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public Quaternion Rotation
        {
            get
            {
                return Translator.BtQuaternionToQuaternion(_rbi.Orientation);
            }
        }

        /// <summary>
        /// Applies the force.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="relPos">The relative position.</param>
        public void ApplyForce(float3 force, float3 relPos)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyForce(Translator.Float3ToBtVector3(force), Translator.Float3ToBtVector3(relPos));
        }

        /// <summary>
        /// Applies the torque.
        /// </summary>
        /// <param name="torque">The torque.</param>
        public void ApplyTorque(float3 torque)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyTorque(Translator.Float3ToBtVector3(torque));
        }

        /// <summary>
        /// Applies the impulse.
        /// </summary>
        /// <param name="impulse">The impulse.</param>
        /// <param name="relPos">The relative position.</param>
        public void ApplyImpulse(float3 impulse, float3 relPos)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
           // impulse *= 10;
            o._rbi.ApplyImpulse(Translator.Float3ToBtVector3(impulse)*10, Translator.Float3ToBtVector3(relPos));
        }

        /// <summary>
        /// Applies the torque impulse.
        /// </summary>
        /// <param name="torqueImpulse">The torque impulse.</param>
        public void ApplyTorqueImpulse(float3 torqueImpulse)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyTorqueImpulse(Translator.Float3ToBtVector3(torqueImpulse));
            // _torqueImpulse = value*10;
        }

        /// <summary>
        /// Applies the central force.
        /// </summary>
        /// <param name="centralForce">The central force.</param>
        public void ApplyCentralForce(float3 centralForce)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyCentralForce(Translator.Float3ToBtVector3(centralForce));
        }

        /// <summary>
        /// Applies the central impulse.
        /// </summary>
        /// <param name="centralImpulse">The central impulse.</param>
        public void ApplyCentralImpulse(float3 centralImpulse)
        {
            var o = (RigidBodyImp)_rbi.UserObject;
            o._rbi.ApplyCentralImpulse(Translator.Float3ToBtVector3(centralImpulse));
        }

        /// <summary>
        /// Gets and sets the linear velocity.
        /// </summary>
        /// <value>
        /// The linear velocity.
        /// </value>
        public float3 LinearVelocity 
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.LinearVelocity);
                return retval;
            } 
            set
            {
                var linVel = Translator.Float3ToBtVector3(value);
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.LinearVelocity = linVel;
            }
        }
        /// <summary>
        /// Gets and sets the angular velocity.
        /// </summary>
        /// <value>
        /// The angular velocity.
        /// </value>
        public float3 AngularVelocity
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.AngularVelocity);
                return retval;
            }
            set
            {
                var angVel = Translator.Float3ToBtVector3(value);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.AngularVelocity = angVel;
            }
        }

        /// <summary>
        /// Gets and sets the linear factor.
        /// </summary>
        /// <value>
        /// The linear factor.
        /// </value>
        public float3 LinearFactor
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.LinearFactor);
                return retval;
            }
            set
            {
                var linfac = Translator.Float3ToBtVector3(value);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.LinearFactor = linfac;
            }
        }
        /// <summary>
        /// Gets and sets the angular factor.
        /// </summary>
        /// <value>
        /// The angular factor.
        /// </value>
        public float3 AngularFactor
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_rbi.AngularFactor);
                return retval;
            }
            set
            {
                var angfac = Translator.Float3ToBtVector3(value);
                var o = (RigidBodyImp)_rbi.UserObject;
                o._rbi.AngularFactor = angfac;
            }
        }


        /// <summary>
        /// Gets and sets the restitution.
        /// </summary>
        /// <value>
        /// The restitution.
        /// </value>
        public float Restitution
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

        /// <summary>
        /// Gets and sets the friction.
        /// </summary>
        /// <value>
        /// The friction.
        /// </value>
        public float Friction
        {
            get { return _rbi.Friction; }
            set
            {
                var o = (RigidBodyImp) _rbi.UserObject;
                o._rbi.Friction = value;
            }
        }

        /// <summary>
        /// Sets the drag.
        /// </summary>
        /// <param name="linearDrag">The linear drag.</param>
        /// <param name="anglularDrag">The angular drag.</param>
        public void SetDrag(float linearDrag, float anglularDrag)
        {
            var o = (RigidBodyImp) _rbi.UserObject;
            o._rbi.SetDamping(linearDrag, anglularDrag);
        }

        /// <summary>
        /// Gets the linear drag.
        /// </summary>
        /// <value>
        /// The linear drag.
        /// </value>
        public float LinearDrag
        {
            get { return _rbi.LinearDamping; }
            
        }

        /// <summary>
        /// Gets the angular drag.
        /// </summary>
        /// <value>
        /// The angular drag.
        /// </value>
        public float AngularDrag
        {
            get { return _rbi.AngularDamping; }
        }


        /// <summary>
        /// Gets and sets the collision shape.
        /// </summary>
        /// <value>
        /// The collision shape.
        /// </value>
        public ICollisionShapeImp CollisionShape
        {
            get
            {
                var type = _rbi.CollisionShape.GetType().ToString();
                var btShape = _rbi.CollisionShape;
                /*var colShape = new CollisonShapeImp();
                colShape.BtCollisionShape = btShape;
                btShape.UserObject = colShape;
                return colShape;*/
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
                    case "BulletSharp.GImpactMeshShape":
                        var btGImpactMesh = (GImpactMeshShape)btShape;
                        var gImpactMesh = new GImpactMeshShapeImp();
                        gImpactMesh.BtGImpactMeshShape = btGImpactMesh;
                        btGImpactMesh.UserObject = gImpactMesh;
                        return gImpactMesh;
                    case "BulletSharp.StaticPlaneShape":
                        var btStaticPlane = (StaticPlaneShape)btShape;
                        var staticPlane = new StaticPlaneShapeImp();
                        staticPlane.BtStaticPlaneShape = btStaticPlane;
                        btStaticPlane.UserObject = staticPlane;
                        return staticPlane;
                    //Misc
                    case "BulletSharp.CompoundShape":
                        //Debug.WriteLine("BulletSharp.CompoundShape");
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
                        var btBoxHalfExtents = Translator.Float3ToBtVector3(box.HalfExtents);
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
                        var btCylinderHalfExtents = Translator.Float3ToBtVector3(cylinider.HalfExtents);
                        btColShape = new CylinderShape(btCylinderHalfExtents);
                        break;
                    case "Fusee.Engine.MultiSphereShapeImp":
                        var multiSphere = (MultiSphereShapeImp)value;
                        var btPositions = new Vector3[multiSphere.SphereCount];
                        var btRadi = new float[multiSphere.SphereCount];
                        for (int i = 0; i < multiSphere.SphereCount; i++)
                        {
                            var pos = Translator.Float3ToBtVector3(multiSphere.GetSpherePosition(i));
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
                    case "Fusee.Engine.ConvexHullShapeImp":
                        var convHull = (ConvexHullShapeImp)value;
                        var btPoints = new Vector3[convHull.GetNumPoints()];
                        for (int i = 0; i < btPoints.Length; i++)
                        {
                            btPoints[i] = Translator.Float3ToBtVector3(convHull.GetScaledPoint(i));
                        }
                        convHull.GetUnscaledPoints();
                        btColShape = new ConvexHullShape(btPoints);
                        break;
                    case "Fusee.Engine.StaticPlaneShapeImp":
                        var staticPlane = (StaticPlaneShapeImp)value;
                        var btNormal = Translator.Float3ToBtVector3(staticPlane.PlaneNormal);
                        btColShape = new StaticPlaneShape(btNormal, staticPlane.PlaneConstant);
                        break;
                    case "Fusee.Engine.GImpactMeshShapeImp":
                        var gImpShape = (GImpactMeshShapeImp)value;
                        //var btRadius = sphere.Radius;
                        btColShape = new GImpactMeshShape(gImpShape.BtGImpactMeshShape.MeshInterface);
                        break;

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
        /// <summary>
        /// Gets and sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }


        /// <summary>
        /// OnCollision is called once per frame for every rigidbody.
        /// Send from here Massage to the Rigidbody.OnCollision(RigidBodyImp other) by an Events
        /// </summary>
        /// <param name="other">The other rigidbody that is collided.</param>
        public virtual void OnCollision(IRigidBodyImp other)
        {
            
            //Debug.WriteLine("RigidBodyImp.OnCollision");
            //TODO: Event to the RigidBody.cs class 
            //var otherRb = (RigidBodyImp)other;
            //otherRb.ApplyTorqueImpulse = new float3(10,10,10);

        }


    }
}
