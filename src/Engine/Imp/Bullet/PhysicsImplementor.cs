using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.Engine
{
    public class PhysicsImplementor
    {
        public static IDynamicWorldImp CreateDynamicWorldImp()
        {
            return new DynamicWorldImp();
        }
    }
}
