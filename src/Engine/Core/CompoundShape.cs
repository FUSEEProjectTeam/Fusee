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

        public void AddChildShape<T>(float4x4 localTransform, T childShape)
        {

            //CompoundShapeImp.AddChildShape(localTransform, childShape);
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
