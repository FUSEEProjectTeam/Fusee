using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CompoundShape : CollisionShape
    {
        internal ICompoundShapeImp CompoundShapeImp;

        /*public void AddChildShape(float4x4 localTransform, CollisionShape childShape)
        {
            var type = childShape.GetType().ToString();
            Debug.WriteLine(type);
            switch (type)
            {
                case "Fusee.Engine.BoxShape":
                    Debug.WriteLine(type);
                    var b = new BoxShape();
                    b = (BoxShape) childShape;
                    var shape = b.BoxShapeImp;
                    CompoundShapeImp.AddChildShape(localTransform, shape);
                    break;
                default:
                    Debug.WriteLine("default");
                    var empty = new BoxShape();
                    CompoundShapeImp.AddChildShape(localTransform, empty.BoxShapeImp);
                    break;
            }
        }*/

        public void AddChildShape(float4x4 localTransform, BoxShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.BoxShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, SphereShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.SphereShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, CapsuleShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.CapsuleShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, ConeShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.ConeShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, CylinderShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.CylinderShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, MultiSphereShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.MultiSphereShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, EmptyShape childShape)
        {
            CompoundShapeImp.AddChildShape(localTransform, childShape.EmtyShapeImp);
        }

        public void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia)
        {
            CompoundShapeImp.CalculatePrincipalAxisTransform(masses, principal, inertia);
        }

        //Inherited
        public float Margin
        {

            get
            {
                var retval = CompoundShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape)CompoundShapeImp.UserObject;
                o.BoxShapeImp.Margin = value;
            }
        }
    }
}
