using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic interface for a collision shape made up of one or more other shapes.
    /// </summary>
    public interface ICompoundShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, IBoxShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, ISphereShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, ICapsuleShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, IConeShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, ICylinderShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, IMultiSphereShapeImp shape);
        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The child shape.</param>
        void AddChildShape(float4x4 localTransform, IEmptyShapeImp shape);
        /// <summary>
        /// Calculates the principal axis transform.
        /// </summary>
        /// <param name="masses">The masses.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="inertia">The inertia.</param>
        void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia);
    }
}
