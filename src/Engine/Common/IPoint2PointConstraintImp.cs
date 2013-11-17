using Fusee.Math;

namespace Fusee.Engine
{
    public interface IPoint2PointConstraintImp
    {

        float3 PivotInA { get; set; }
        float3 PivotInB { get; set; }
        void SetParam(int num, float3 value, int axis = -1);
        float3 GetParam(int num, int axis = -1);

        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }

        object UserObject { get; set; }
    }
}
