using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public enum PointToPointFlags
    {
        PointToPointFlagsErp = 1,
        PointToPointFlagsStopErp = 2,
        PointToPointFlagsCfm = 3,
        PointToPointFlagsStopCfm = 4
    };

    public interface IPoint2PointConstraintImp : IConstraintImp
    {


        float3 PivotInA { get; set; }
        float3 PivotInB { get; set; }

        void UpdateRhs(float timeStep);

        void SetParam(PointToPointFlags constraintParameter, float value, int axis = -1);
        float GetParam(PointToPointFlags param, int axis = -1);
        

    }
}
