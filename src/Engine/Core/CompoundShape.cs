using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class CompoundShape : CollisionShape
    {
        internal ICompoundShapeImp _compoundShapeImp;

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
                    var shape = b._boxShapeImp;
                    _compoundShapeImp.AddChildShape(localTransform, shape);
                    break;
                default:
                    Debug.WriteLine("default");
                    var empty = new BoxShape();
                    _compoundShapeImp.AddChildShape(localTransform, empty._boxShapeImp);
                    break;
            }
        }*/

        public void AddChildShape(float4x4 localTransform, BoxShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._boxShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, SphereShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._sphereShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, CapsuleShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._capsuleShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, ConeShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._coneShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, CylinderShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._cylinderShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, MultiSphereShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._multiSphereShapeImp);
        }
        public void AddChildShape(float4x4 localTransform, EmptyShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._emtyShapeImp);
        }

        public void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia)
        {
            _compoundShapeImp.CalculatePrincipalAxisTransform(masses, principal, inertia);
        }

        //Inherited
        public override float Margin
        {

            get
            {
                var retval = _compoundShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape)_compoundShapeImp.UserObject;
                o._boxShapeImp.Margin = value;
            }
        }
    }
}
