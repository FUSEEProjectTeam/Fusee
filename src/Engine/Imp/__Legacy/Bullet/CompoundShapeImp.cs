using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CompoundShapeImp : CollisonShapeImp, ICompoundShapeImp
    {
        internal CompoundShape BtCompoundShape;
        internal Translater Translater = new Translater();
        //Inherited
        public float Margin
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

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }


        public void AddChildShape(float4x4 localTransform, IBoxShapeImp shape)
        {
            Debug.WriteLine("AddBox");
            var btHalfExtents = Translater.Float3ToBtVector3(shape.HalfExtents);
            var btChildShape = new BoxShape(btHalfExtents);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, ISphereShapeImp shape)
        {
            Debug.WriteLine("AddSphere");
            var btChildShape = new SphereShape(shape.Radius);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, ICapsuleShapeImp shape)
        {
            var btChildShape = new CapsuleShape(shape.Radius, shape.HalfHeight);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, IConeShapeImp shape)
        {
            var btChildShape = new ConeShape(shape.Radius, shape.Height);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, ICylinderShapeImp shape)
        {
            var btHalfExtents = Translater.Float3ToBtVector3(shape.HalfExtents);
            var btChildShape = new CylinderShape(btHalfExtents);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, IMultiSphereShapeImp shape)
        {
            var btPositions = new Vector3[shape.SphereCount];
            var btRadi = new float[shape.SphereCount];
            for (int i = 0; i < shape.SphereCount; i++)
            {
                var pos = Translater.Float3ToBtVector3(shape.GetSpherePosition(i));
                btPositions[i] = pos;
                btRadi[i] = shape.GetSphereRadius(i);
            }
            var btChildShape = new MultiSphereShape(btPositions, btRadi);
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }
        public void AddChildShape(float4x4 localTransform, IEmptyShapeImp shape)
        {
            var btChildShape = new EmptyShape();
            var btLocalTransform = Translater.Float4X4ToBtMatrix(localTransform);
            BtCompoundShape.AddChildShape(btLocalTransform, btChildShape);
        }

        public void CalculatePrincipalAxisTransform(float[] masses, float4x4 principal, float3 inertia)
        {
            var btPrincipal = Translater.Float4X4ToBtMatrix(principal);
            var btInertia = Translater.Float3ToBtVector3(inertia);
            BtCompoundShape.CalculatePrincipalAxisTransform(masses, ref btPrincipal, out btInertia);
        }
    }
}
