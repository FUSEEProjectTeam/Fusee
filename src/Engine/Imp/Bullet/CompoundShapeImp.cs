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
                var o = (BoxShapeImp)BtCompoundShape.UserObject;
                o.BtBoxShape.Margin = value;
            }
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }

        public void AddChildShape<TShapeType>(float4x4 localTransform, TShapeType childShape) 
            where TShapeType : ICollisionShapeImp, IBoxShapeImp, ISphereShapeImp  
        {
            CollisionShape btCollisionShape;
            var type = childShape.GetType().ToString();
            switch (type)
            {
                case "Fusee.Engine.BoxShape":
                    Debug.WriteLine(type);
                    var btHalfExtents = Translater.Float3ToBtVector3(childShape.HalfExtents);
                    btCollisionShape = new BoxShape(btHalfExtents);
                    break;
                case "Fusee.Engine.SphereShape":
                    Debug.WriteLine(type);
                    //var btHalfExtents = Translater.Float3ToBtVector3(childShape.HalfExtents);
                    btCollisionShape = new SphereShape(childShape.Radius);
                    break;
                /*
                 * TODO: For all cases
                 * */
                default:
                    Debug.WriteLine("Default");
                    btCollisionShape = new EmptyShape();
                    break;
            }


            BtCompoundShape.AddChildShape(Translater.Float4X4ToBtMatrix(localTransform), btCollisionShape);
        }
    }
}
