using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of a plane shaped collision shape.
    /// </summary>
    public interface IStaticPlaneShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets the plane constant.
        /// </summary>
        /// <value>
        /// The plane constant.
        /// </value>
        float PlaneConstant { get; }
        /// <summary>
        /// Gets the plane normal.
        /// </summary>
        /// <value>
        /// The plane normal.
        /// </value>
        float3 PlaneNormal { get; }
    }
}
