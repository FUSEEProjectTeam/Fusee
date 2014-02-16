using System;
using System.Drawing;
using Fusee.Math;
using Fusee.Engine;


namespace Fusee.Engine
{
    public interface IDynamicWorldImp
    {
        float3 Gravity { get; set; }

        IRigidBodyImp AddRigidBody(float mass, float3 position, float3 orientation, ICollisionShapeImp colShape/*, float3 intertia*/);
        IRigidBodyImp GetRigidBody(int i);
        int StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps);
        int NumberRigidBodies();

        IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA);
        IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB);
       
        //IPoint2PointConstraintImp GetConstraint(int i);

        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA);
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA, float3 axisInA, bool useReferenceFrameA);
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB, float3 axisInA, float3 axisInB, bool useReferenceFrameA);
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 brAFrame, float4x4 brBFrame, bool useReferenceFrameA);

        ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useLinearReferenceFrameA);
        ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useLinearReferenceFrameA);

        IGearConstraintImp AddGearConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 axisInA, float3 axisInB, float ratio);

        IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, float4x4 rbAFrame);
        IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 rbAFrame, float4x4 rbBFrame);

        IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA);
        IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useReferenceFrameA = false);


        IBoxShapeImp AddBoxShape(float boxHalfExtents);
        IBoxShapeImp AddBoxShape(float3 boxHalfExtents);
        IBoxShapeImp AddBoxShape(float boxHalfExtentsX, float boxHalfExtentsY, float boxHalfExtentsZ);

        ISphereShapeImp AddSphereShape(float radius);

        ICapsuleShapeImp AddCapsuleShape(float radius, float height);

        ICylinderShapeImp AddCylinderShape(float halfExtents);
        ICylinderShapeImp AddCylinderShape(float3 halfExtents);
        ICylinderShapeImp AddCylinderShape(float halfExtentsX, float halfExtentsY, float halfExtentsZ);
        IConeShapeImp AddConeShape(float radius, float height);

        IMultiSphereShapeImp AddMultiSphereShape(float3[] positions, float[] radi);

        ICompoundShapeImp AddCompoundShape(bool enableDynamicAabbTree);

        IEmptyShapeImp AddEmptyShape();

        IConvexHullShapeImp AddConvexHullShape();
        IConvexHullShapeImp AddConvexHullShape(float3[] points, bool optimized);

        IStaticPlaneShapeImp AddStaticPlaneShape(float3 planeNormal, float planeConstant);

        IGImpactMeshShapeImp AddGImpactMeshShape(int[] meshTriangles, float3[]meshVertecies);

        int NumberConstraints();

        void Dispose();
    }
}
