using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface ICompoundShapeImp : ICollisionShapeImp
    {
        void AddChildShape(float4x4 localTransform, IBoxShapeImp shape);
        void AddChildShape(float4x4 localTransform, ISphereShapeImp shape);
        void AddChildShape(float4x4 localTransform, ICapsuleShapeImp shape);
        void AddChildShape(float4x4 localTransform, IConeShapeImp shape);
        void AddChildShape(float4x4 localTransform, ICylinderShapeImp shape);
        void AddChildShape(float4x4 localTransform, IMultiSphereShapeImp shape);
        void AddChildShape(float4x4 localTransform, IEmptyShapeImp shape);

        void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia);


    }
}
