using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IConstraintImp
    {
        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }
        
        int GetUid();
        object UserObject { get; set; }
    }
}
