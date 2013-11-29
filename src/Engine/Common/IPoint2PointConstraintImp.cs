using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public enum PointToPointFlags
    {
        CONSTRAINT_PARAM_ERP = 1,
        CONSTRAINT_PARAM_STOP_ERP = 2,
        CONSTRAINT_PARAM_CFM = 3,
        CONSTRAINT_PARAM_STOP_CFM = 4
    };

    public interface IPoint2PointConstraintImp
    {


        float3 PivotInA { get; set; }
        float3 PivotInB { get; set; }
        void SetParam(PointToPointFlags constraintParameter, float value, int axis = -1);
        float GetParam(PointToPointFlags param, int axis = -1);

        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }

        object UserObject { get; set; }
    }
}
