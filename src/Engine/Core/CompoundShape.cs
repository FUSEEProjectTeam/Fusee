using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A collision shape made up of other shapes of various types.
    /// </summary>
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

        /// <summary>
        /// Adds a box as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, BoxShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._boxShapeImp);
        }
        /// <summary>
        /// Adds a sphere as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, SphereShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._sphereShapeImp);
        }
        /// <summary>
        /// Adds a capsule as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, CapsuleShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._capsuleShapeImp);
        }
        /// <summary>
        /// Adds a cone as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, ConeShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._coneShapeImp);
        }
        /// <summary>
        /// Adds a cylinder as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, CylinderShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._cylinderShapeImp);
        }
        /// <summary>
        /// Adds a multi-sphere as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, MultiSphereShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._multiSphereShapeImp);
        }
        /// <summary>
        /// Adds an empty shape as a child shape.
        /// </summary>
        /// <param name="localTransform">The local transformation of the child shape.</param>
        /// <param name="childShape">The child shape.</param>
        public void AddChildShape(float4x4 localTransform, EmptyShape childShape)
        {
            _compoundShapeImp.AddChildShape(localTransform, childShape._emtyShapeImp);
        }

        /// <summary>
        /// Calculates the principal transformation axis.
        /// </summary>
        /// <param name="masses">The masses.</param>
        /// <param name="principal">The principal axis.</param>
        /// <param name="inertia">The inertia.</param>
        public void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia)
        {
            _compoundShapeImp.CalculatePrincipalAxisTransform(masses, principal, inertia);
        }

        //Inherited
        /// <summary>
        /// Retrieves or sets the margin.
        /// </summary>
        /// <value>
        /// The size of the collision shape's margin.
        /// </value>
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