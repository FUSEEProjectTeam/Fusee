using System;
using System.Diagnostics;
using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    public class DynamicWorld
    {
        internal /*readonly*/ IDynamicWorldImp _dwi;
        
        public DynamicWorld([InjectMe] IDynamicWorldImp dynamicWorld)
        {
            _dwi = dynamicWorld;
        }
        /// <summary>
        /// Returns the gravity value.
        /// </summary>
        public float3 Gravity
        {
            get { return _dwi.Gravity; }
            set { _dwi.Gravity = value; }
        }

        public RigidBody AddRigidBody(float mass, float3 position, float3 orientation, CollisionShape colShape/*, float3 inertia*/)
        {

           /* var meshTrianglesCount = mesh.Triangles.Length;
            int [] meshTrianglesArray = new int[meshTrianglesCount];
            for (int c = 0; c < meshTrianglesCount; c++)
            {
                meshTrianglesArray[c] = Convert.ToInt32(mesh.Triangles[c]);
            }

            int meshVerteciesCount = mesh.Vertices.Length;
            float3[] meshVerteciesArray = new float3[meshVerteciesCount];
            meshVerteciesArray = mesh.Vertices;
            */
            var shapeType = colShape.GetType().ToString();
            IRigidBodyImp rbi;
            switch (shapeType)
            {
                //Primitives
                case "Fusee.Engine.BoxShape":
                    var box =(BoxShape) colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, box._boxShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.CapsuleShape":
                    var capsule = (CapsuleShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, capsule._capsuleShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.ConeShape":
                    var cone = (ConeShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, cone._coneShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.CylinderShape":
                    var cylinder = (CylinderShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, cylinder._cylinderShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.MultiSphereShape":
                    var multiSphere = (MultiSphereShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, multiSphere._multiSphereShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.SphereShape":
                    var sphere = (SphereShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, sphere._sphereShapeImp/*, inertia*/);
                    break;

                //Misc
                case "Fusee.Engine.CompoundShape":
                    var compShape = (CompoundShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, compShape._compoundShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.EmptyShape":
                    var empty = (EmptyShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, empty._emtyShapeImp/*, inertia*/);
                    break;

                //Meshes
                case "Fusee.Engine.ConvexHullShape":
                    var convHullShape = (ConvexHullShape) colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, convHullShape._convexHullShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.GImpactMeshShape":
                    var gImpMeshShape = (GImpactMeshShape) colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, gImpMeshShape._gImpactMeshShapeImp/*, inertia*/);
                    break;
                case "Fusee.Engine.StaticPlaneShape": //static Shape
                    var staticPaneShape = (StaticPlaneShape)colShape;
                    rbi = _dwi.AddRigidBody(mass, position, orientation, staticPaneShape._staticPlaneShapeImp/*, inertia*/);
                    break;
                //Default
                default:
                    var defaultShape = new EmptyShape();
                    Debug.WriteLine("default");
                    rbi = _dwi.AddRigidBody(mass, position, orientation, defaultShape._emtyShapeImp/*, inertia*/);
                    break;
            }

           // IRigidBodyImp rbi = _dwi.AddRigidBody(mass, worldTransform, /* shape, */inertia);
       
            var retval = new RigidBody();
            //retval.Mesh = mesh;
            retval._iRigidBodyImp = rbi;
            rbi.UserObject = retval;

            return retval;
        }
        /// <summary>
        /// Returns the step simulation.
        /// </summary>
        /// <param name="timeSteps"></param>
        /// <param name="maxSubSteps"></param>
        /// <param name="fixedTimeSteps"></param>
        /// <returns></returns>
        public int StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps = 1/60)
        {
            return _dwi.StepSimulation(timeSteps, maxSubSteps, fixedTimeSteps);
        }
        /// <summary>
        /// Returns the rigidbody.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public RigidBody GetRigidBody(int i)
        {
            var rbi = _dwi.GetRigidBody(i);
            var retval = (RigidBody) rbi.UserObject;
            return retval;
        }
        /// <summary>
        /// Returns the number of rigid body´s.
        /// </summary>
        /// <returns></returns>
        public int NumberRigidBodies()
        {
            var number = _dwi.NumberRigidBodies();
            return number;
        }

        #region Constraints
        //P2pConstraint

        public Point2PointConstraint AddPoint2PointConstraint(RigidBody rigidBodyA, float3 pivotInA)
        {
            IPoint2PointConstraintImp ip2pci = _dwi.AddPoint2PointConstraint(rigidBodyA._iRigidBodyImp, pivotInA);
            var retval = new Point2PointConstraint();
            retval._iP2PConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }
        public Point2PointConstraint AddPoint2PointConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float3 pivotInA, float3 pivotInB)
        {
            IPoint2PointConstraintImp ip2pci = _dwi.AddPoint2PointConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, pivotInA, pivotInB);
            var retval = new Point2PointConstraint();
            retval._iP2PConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }
      /*  public Point2PointConstraint GetConstraint(int i)
        {
            //Point2PointConstraint tp2pci = _dwi.GetConstraint(i).UserObject;
            var retval = (Point2PointConstraint)_dwi.GetConstraint(i).UserObject;
            return retval;
        }*/

        //HingeConstraint
        public HingeConstraint AddHingeConstraint(RigidBody rigidBodyA, float4x4 frameInA, bool useReferenceFrameA = false)
        {
            IHingeConstraintImp ip2pci = _dwi.AddHingeConstraint(rigidBodyA._iRigidBodyImp, frameInA, useReferenceFrameA);
            var retval = new HingeConstraint();
            retval._iHConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }
        public HingeConstraint AddHingeConstraint(RigidBody rigidBodyA, float3 pivotInA, float3 axisInA, bool useReferenceFrameA = false)
        {
            IHingeConstraintImp ip2pci = _dwi.AddHingeConstraint(rigidBodyA._iRigidBodyImp, pivotInA, axisInA, useReferenceFrameA);
            var retval = new HingeConstraint();
            retval._iHConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }
        public HingeConstraint AddHingeConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float3 pivotInA, float3 pivotInB, float3 axisInA, float3 axisInB, bool useReferenceFrameA = false)
        {
            IHingeConstraintImp ip2pci = _dwi.AddHingeConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, pivotInA, pivotInB, axisInA, axisInB, useReferenceFrameA);
            var retval = new HingeConstraint();
            retval._iHConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }
        public HingeConstraint AddHingeConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float4x4 brAFrame, float4x4 brBFrame, bool useReferenceFrameA = false)
        {
            IHingeConstraintImp ip2pci = _dwi.AddHingeConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, brAFrame, brBFrame, useReferenceFrameA);
            var retval = new HingeConstraint();
            retval._iHConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }

        //SliderConstraint
        public SliderConstraint AddSliderConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useLinearReferenceFrameA = false)
        {
            ISliderConstraintImp isci = _dwi.AddSliderConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, frameInA, frameInB, useLinearReferenceFrameA);
            var retval = new SliderConstraint();
            retval._iSliderConstraintImp = isci;
            isci.UserObject = retval;
            return retval;
        }
        public SliderConstraint AddSliderConstraint(RigidBody rigidBodyA, float4x4 frameInA, bool useLinearReferenceFrameA = false)
        {
            ISliderConstraintImp isci = _dwi.AddSliderConstraint(rigidBodyA._iRigidBodyImp,frameInA, useLinearReferenceFrameA);
            var retval = new SliderConstraint();
            retval._iSliderConstraintImp = isci;
            isci.UserObject = retval;
            return retval;
        }

        //GearConstraint
        public GearConstraint AddGearConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float3 axisInA, float3 axisInB, float ratio = 1.0f)
        {
            IGearConstraintImp igci = _dwi.AddGearConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, axisInA, axisInB, ratio);
            var retval = new GearConstraint();
            retval._iGearConstraintImp = igci;
            igci.UserObject = retval;
            return retval;
        }

        //ConeTwistConstraint
        public ConeTwistConstraint AddConeTwistConstraint(RigidBody rigidBodyA, float4x4 rbAFrame)
        {
            IConeTwistConstraintImp icti = _dwi.AddConeTwistConstraint(rigidBodyA._iRigidBodyImp, rbAFrame);
            var retval = new ConeTwistConstraint();
            retval._iCTConstraintImp = icti;
            icti.UserObject = retval;
            return retval;
        }
        public ConeTwistConstraint AddConeTwistConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float4x4 rbAFrame, float4x4 rbBFrame)
        {
            IConeTwistConstraintImp icti = _dwi.AddConeTwistConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, rbAFrame, rbBFrame);
            var retval = new ConeTwistConstraint();
            retval._iCTConstraintImp = icti;
            icti.UserObject = retval;
            return retval;
        }


        //Generic6DofConstraint
        public Generic6DofConstraint AddGeneric6DofConstraint(RigidBody rigidBodyA, float4x4 frameInA, bool useReferenceFrameA)
        {
            IGeneric6DofConstraintImp ig6dofci = _dwi.AddGeneric6DofConstraint(rigidBodyA._iRigidBodyImp, frameInA, useReferenceFrameA);
            var retval = new Generic6DofConstraint();
            retval._IG6DofConstraintImp = ig6dofci;
            ig6dofci.UserObject = retval;
            return retval;
        }
        public Generic6DofConstraint AddGeneric6DofConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useReferenceFrameA = false)
        {
            IGeneric6DofConstraintImp ig6dofci = _dwi.AddGeneric6DofConstraint(rigidBodyA._iRigidBodyImp, rigidBodyB._iRigidBodyImp, frameInA, frameInB, useReferenceFrameA);
            var retval = new Generic6DofConstraint();
            retval._IG6DofConstraintImp = ig6dofci;
            ig6dofci.UserObject = retval;
            return retval;
        }
        #endregion Constraints
        
        #region CollisionShapes
        //CollisionShapes

        //BoxShape
        public BoxShape AddBoxShape(float boxHalfExtents)
        {
            IBoxShapeImp iBoxShapeImp = _dwi.AddBoxShape(boxHalfExtents);
            var retval = new BoxShape();
            retval._boxShapeImp = iBoxShapeImp;
            iBoxShapeImp.UserObject = retval;
            return retval;
        }
        public BoxShape AddBoxShape(float boxHalfExtentsX, float boxHalfExtentsY, float boxHalfExtentsZ)
        {
            IBoxShapeImp iBoxShapeImp = _dwi.AddBoxShape(boxHalfExtentsX, boxHalfExtentsY, boxHalfExtentsZ);
            var retval = new BoxShape();
            retval._boxShapeImp = iBoxShapeImp;
            iBoxShapeImp.UserObject = retval;
            return retval;
        }
        public BoxShape AddBoxShape(float3 boxHalfExtents)
        {
            IBoxShapeImp iBoxShapeImp = _dwi.AddBoxShape(boxHalfExtents);
            var retval = new BoxShape();
            retval._boxShapeImp = iBoxShapeImp;
            iBoxShapeImp.UserObject = retval;
            return retval;
        }

        //SphereShape
        public SphereShape AddSphereShape(float radius)
        {
            ISphereShapeImp iSphereShapeImp = _dwi.AddSphereShape(radius);
            var retval = new SphereShape();
            retval._sphereShapeImp = iSphereShapeImp;
            iSphereShapeImp.UserObject = retval;
            return retval;
        }

        //CapsuleShape
        public CapsuleShape AddCapsuleShape(float radius, float height)
        {
            ICapsuleShapeImp iCapsuleShapeImp = _dwi.AddCapsuleShape(radius, height);
            var retval = new CapsuleShape();
            retval._capsuleShapeImp = iCapsuleShapeImp;
            iCapsuleShapeImp.UserObject = retval;
            return retval;
        }

        //CylinderShape
        public CylinderShape AddCylinderShape(float halfExtents)
        {
            ICylinderShapeImp iCylinderShapeImp = _dwi.AddCylinderShape(halfExtents);
            var retval = new CylinderShape();
            retval._cylinderShapeImp = iCylinderShapeImp;
            iCylinderShapeImp.UserObject = retval;
            return retval;
        }
        public CylinderShape AddCylinderShape(float halfExtentsX, float halfExtentsY, float halfExtentsZ)
        {
            ICylinderShapeImp iCylinderShapeImp = _dwi.AddCylinderShape(halfExtentsX, halfExtentsY, halfExtentsZ);
            var retval = new CylinderShape();
            retval._cylinderShapeImp = iCylinderShapeImp;
            iCylinderShapeImp.UserObject = retval;
            return retval;
        }
        public CylinderShape AddCylinderShape(float3 halfExtents)
        {
            ICylinderShapeImp iCylinderShapeImp = _dwi.AddCylinderShape(halfExtents);
            var retval = new CylinderShape();
            retval._cylinderShapeImp = iCylinderShapeImp;
            iCylinderShapeImp.UserObject = retval;
            return retval;
        }

        //ConeShape
        public ConeShape AddConeShape(float radius, float height)
        {
            IConeShapeImp iConeShapeImp = _dwi.AddConeShape(radius, height);
            var retval = new ConeShape();
            retval._coneShapeImp = iConeShapeImp;
            iConeShapeImp.UserObject = retval;
            return retval;
        }

        //MultiSphere
        public MultiSphereShape AddMultiSphereShape(float3[] positions, float[] radi)
        {
            IMultiSphereShapeImp iMultiSphereShapeImp = _dwi.AddMultiSphereShape(positions, radi);
            var retval = new MultiSphereShape();
            retval._multiSphereShapeImp = iMultiSphereShapeImp;
            iMultiSphereShapeImp.UserObject = retval;
            return retval;
        }

        //CompountShape
        public CompoundShape AddCompoundShape(bool enableDynamicAabbTree)
        {
            ICompoundShapeImp iCompoundImp = _dwi.AddCompoundShape(enableDynamicAabbTree);
            var retval = new CompoundShape();
            retval._compoundShapeImp = iCompoundImp;
            iCompoundImp.UserObject = retval;
            return retval;
        }

        public EmptyShape AddEmptyShape()
        {
            IEmptyShapeImp iEmptyShapeImp = _dwi.AddEmptyShape();
            var retval = new EmptyShape();
            retval._emtyShapeImp = iEmptyShapeImp;
            iEmptyShapeImp.UserObject = retval;
            return retval;
        }

        public ConvexHullShape AddConvexHullShape()
        {
            IConvexHullShapeImp iConvexHullShapeImp = _dwi.AddConvexHullShape();
            var retval = new ConvexHullShape();
            retval._convexHullShapeImp = iConvexHullShapeImp;
            iConvexHullShapeImp.UserObject = retval;
            return retval;
        }

        public ConvexHullShape AddConvexHullShape(float3[] points, bool optimized = true)
        {
            IConvexHullShapeImp iConvexHullShapeImp = _dwi.AddConvexHullShape(points, optimized);
            var retval = new ConvexHullShape();
            retval._convexHullShapeImp = iConvexHullShapeImp;
            iConvexHullShapeImp.UserObject = retval;
            return retval;
        }

        public StaticPlaneShape AddStaticPlaneShape(float3 planeNormal, float planeConstant)
        {
            IStaticPlaneShapeImp iStaticPlaneShapeImp = _dwi.AddStaticPlaneShape(planeNormal, planeConstant);
            var retval = new StaticPlaneShape();
            retval._staticPlaneShapeImp = iStaticPlaneShapeImp;
            iStaticPlaneShapeImp.UserObject = retval;
            return retval;
        }

        public GImpactMeshShape AddGImpactMeshShape(Mesh mesh)
        {
            int[] meshTrianglesArray = new int[mesh.Triangles.Length];
            for (int c = 0; c < mesh.Triangles.Length; c++)
            {
                meshTrianglesArray[c] = Convert.ToInt32(mesh.Triangles[c]);
            }

            int meshVerteciesCount = mesh.Vertices.Length;
            float3[] meshVerteciesArray = new float3[meshVerteciesCount];
            meshVerteciesArray = mesh.Vertices;
            IGImpactMeshShapeImp iGImpactMeshShapeImp = _dwi.AddGImpactMeshShape(meshTrianglesArray, mesh.Vertices);
            var retval = new GImpactMeshShape();
            retval._gImpactMeshShapeImp = iGImpactMeshShapeImp;
            iGImpactMeshShapeImp.UserObject = retval;
            return retval;
        }

        #endregion CollisionShapes
        /// <summary>
        /// Returns the number of constraints.
        /// </summary>
        /// <returns></returns>
        public int NumberConstraints()
        {
            var number = _dwi.NumberConstraints();
            return number;
        }
        /// <summary>
        /// Disposes the rigid body.
        /// </summary>
        public void Dispose()
        {
            _dwi.Dispose();
        }
    }
}
