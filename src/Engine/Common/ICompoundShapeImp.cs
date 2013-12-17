using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public interface ICompoundShapeImp : ICollisionShapeImp
    {
        void AddChildShape<TShapeType>(float4x4 localTransform, TShapeType childShape) 
            where TShapeType : ICollisionShapeImp, IBoxShapeImp, ISphereShapeImp;
    }
}
