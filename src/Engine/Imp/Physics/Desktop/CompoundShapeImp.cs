using System.Diagnostics;
using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="ICompoundShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class CompoundShapeImp : CollisonShapeImp, ICompoundShapeImp
    {
        internal CompoundShape BtCompoundShape;
        //Inherited
        /// <summary>
        /// Gets and sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = BtCompoundShape.Margin;
                return retval;
            }
            set
            {
                var o = (CompoundShapeImp)BtCompoundShape.UserObject;
                o.BtCompoundShape.Margin = value;
            }
        }


        /// <summary>
        /// Adds a child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, IBoxShapeImp shape)
        {
            Debug.WriteLine("AddBox");
            var btHalfExtents = Translator.Float3ToBtVector3(shape.HalfExtents);
            var btChildShape = new BoxShape(btHalfExtents);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, ISphereShapeImp shape)
        {
            Debug.WriteLine("AddSphere");
            var btChildShape = new SphereShape(shape.Radius);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, ICapsuleShapeImp shape)
        {
            var btChildShape = new CapsuleShape(shape.Radius, shape.HalfHeight);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, IConeShapeImp shape)
        {
            var btChildShape = new ConeShape(shape.Radius, shape.Height);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, ICylinderShapeImp shape)
        {
            var btHalfExtents = Translator.Float3ToBtVector3(shape.HalfExtents);
            var btChildShape = new CylinderShape(btHalfExtents);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, IMultiSphereShapeImp shape)
        {
            var btPositions = new Vector3[shape.SphereCount];
            var btRadi = new float[shape.SphereCount];
            for (int i = 0; i < shape.SphereCount; i++)
            {
                var pos = Translator.Float3ToBtVector3(shape.GetSpherePosition(i));
                btPositions[i] = pos;
                btRadi[i] = shape.GetSphereRadius(i);
            }
            var btChildShape = new MultiSphereShape(btPositions, btRadi);
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        /// <summary>
        /// Adds the child shape.
        /// </summary>
        /// <param name="localTransform">The local transform.</param>
        /// <param name="shape">The shape.</param>
        public void AddChildShape(float4x4 localTransform, IEmptyShapeImp shape)
        {
            var btChildShape = new EmptyShape();
            var btLocalTransform = Translator.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }

        /// <summary>
        /// Calculates the principal axis transform.
        /// </summary>
        /// <param name="masses">The masses.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="inertia">The inertia.</param>
        public void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia)
        {
            var btPrincipal = Translator.Float4X4ToBtMatrix(principal);
            var btInertia = Translator.Float3ToBtVector3(inertia);
            BtCompoundShape.CalculatePrincipalAxisTransform(masses, ref btPrincipal, out btInertia);
        }
    }
}
