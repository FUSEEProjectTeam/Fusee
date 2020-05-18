using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a dynamic world. The dynamic world is the root object of 
    /// any physics simulation. It keeps track of all objects moving and rotating within the world.
    /// </summary>
    public interface IDynamicWorldImp
    {
        /// <summary>
        /// Gets and sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        float3 Gravity { get; set; }

        /// <summary>
        /// Adds the rigid body.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="position">The position.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="colShape">The col shape.</param>
        /// <returns></returns>
        IRigidBodyImp AddRigidBody(float mass, float3 position, float3 orientation, ICollisionShapeImp colShape/*, float3 intertia*/);
        /// <summary>
        /// Gets the rigid body.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        IRigidBodyImp GetRigidBody(int i);
        /// <summary>
        /// Steps the simulation.
        /// </summary>
        /// <param name="timeSteps">The time steps.</param>
        /// <param name="maxSubSteps">The maximum sub steps.</param>
        /// <param name="fixedTimeSteps">The fixed time steps.</param>
        /// <returns></returns>
        int StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps);
        /// <summary>
        /// Numbers the rigid bodies.
        /// </summary>
        /// <returns></returns>
        int NumberRigidBodies();

        /// <summary>
        /// Adds the point2 point constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <returns></returns>
        IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA);
        /// <summary>
        /// Adds the point2 point constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="pivotInB">The pivot in b.</param>
        /// <returns></returns>
        IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB);

        //IPoint2PointConstraintImp GetConstraint(int i);

        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA);
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA, float3 axisInA, bool useReferenceFrameA);
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="pivotInB">The pivot in b.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="axisInB">The axis in b.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB, float3 axisInA, float3 axisInB, bool useReferenceFrameA);
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="brAFrame">The br a frame.</param>
        /// <param name="brBFrame">The br b frame.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 brAFrame, float4x4 brBFrame, bool useReferenceFrameA);

        /// <summary>
        /// Adds the slider constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="frameInB">The frame in b.</param>
        /// <param name="useLinearReferenceFrameA">if set to <c>true</c> [use linear reference frame a].</param>
        /// <returns></returns>
        ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useLinearReferenceFrameA);
        /// <summary>
        /// Adds the slider constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useLinearReferenceFrameA">if set to <c>true</c> [use linear reference frame a].</param>
        /// <returns></returns>
        ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useLinearReferenceFrameA);

        /// <summary>
        /// Adds the gear constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="axisInB">The axis in b.</param>
        /// <param name="ratio">The ratio.</param>
        /// <returns></returns>
        IGearConstraintImp AddGearConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 axisInA, float3 axisInB, float ratio);

        /// <summary>
        /// Adds the cone twist constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rbAFrame">The rb a frame.</param>
        /// <returns></returns>
        IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, float4x4 rbAFrame);
        /// <summary>
        /// Adds the cone twist constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="rbAFrame">The rb a frame.</param>
        /// <param name="rbBFrame">The rb b frame.</param>
        /// <returns></returns>
        IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 rbAFrame, float4x4 rbBFrame);

        /// <summary>
        /// Adds the generic6 dof constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA);
        /// <summary>
        /// Adds the generic6 dof constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="frameInB">The frame in b.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useReferenceFrameA = false);


        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtents">The box half extents.</param>
        /// <returns></returns>
        IBoxShapeImp AddBoxShape(float boxHalfExtents);
        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtents">The box half extents.</param>
        /// <returns></returns>
        IBoxShapeImp AddBoxShape(float3 boxHalfExtents);
        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtentsX">The box half extents x.</param>
        /// <param name="boxHalfExtentsY">The box half extents y.</param>
        /// <param name="boxHalfExtentsZ">The box half extents z.</param>
        /// <returns></returns>
        IBoxShapeImp AddBoxShape(float boxHalfExtentsX, float boxHalfExtentsY, float boxHalfExtentsZ);

        /// <summary>
        /// Adds the sphere shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        ISphereShapeImp AddSphereShape(float radius);

        /// <summary>
        /// Adds the capsule shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        ICapsuleShapeImp AddCapsuleShape(float radius, float height);

        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtents">The half extents.</param>
        /// <returns></returns>
        ICylinderShapeImp AddCylinderShape(float halfExtents);
        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtents">The half extents.</param>
        /// <returns></returns>
        ICylinderShapeImp AddCylinderShape(float3 halfExtents);
        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtentsX">The half extents x.</param>
        /// <param name="halfExtentsY">The half extents y.</param>
        /// <param name="halfExtentsZ">The half extents z.</param>
        /// <returns></returns>
        ICylinderShapeImp AddCylinderShape(float halfExtentsX, float halfExtentsY, float halfExtentsZ);
        /// <summary>
        /// Adds the cone shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        IConeShapeImp AddConeShape(float radius, float height);

        /// <summary>
        /// Adds the multi sphere shape.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <param name="radi">The radi.</param>
        /// <returns></returns>
        IMultiSphereShapeImp AddMultiSphereShape(float3[] positions, float[] radi);

        /// <summary>
        /// Adds the compound shape.
        /// </summary>
        /// <param name="enableDynamicAabbTree">if set to <c>true</c> [enable dynamic aabb tree].</param>
        /// <returns></returns>
        ICompoundShapeImp AddCompoundShape(bool enableDynamicAabbTree);

        /// <summary>
        /// Adds the empty shape.
        /// </summary>
        /// <returns></returns>
        IEmptyShapeImp AddEmptyShape();

        /// <summary>
        /// Adds the convex hull shape.
        /// </summary>
        /// <returns></returns>
        IConvexHullShapeImp AddConvexHullShape();
        /// <summary>
        /// Adds the convex hull shape.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="optimized">if set to <c>true</c> [optimized].</param>
        /// <returns></returns>
        IConvexHullShapeImp AddConvexHullShape(float3[] points, bool optimized);

        /// <summary>
        /// Adds the static plane shape.
        /// </summary>
        /// <param name="planeNormal">The plane normal.</param>
        /// <param name="planeConstant">The plane constant.</param>
        /// <returns></returns>
        IStaticPlaneShapeImp AddStaticPlaneShape(float3 planeNormal, float planeConstant);

        /// <summary>
        /// Adds the g impact mesh shape.
        /// </summary>
        /// <param name="meshTriangles">The mesh triangles.</param>
        /// <param name="meshVertecies">The mesh vertices.</param>
        /// <returns></returns>
        IGImpactMeshShapeImp AddGImpactMeshShape(int[] meshTriangles, float3[]meshVertecies);

        /// <summary>
        /// Numbers the constraints.
        /// </summary>
        /// <returns></returns>
        int NumberConstraints();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();
    }
}
