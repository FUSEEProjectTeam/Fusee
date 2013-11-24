using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IHingeConstraintImp
    {

        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }

        object UserObject { get; set; }
    }
}
