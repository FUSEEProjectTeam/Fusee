using Fusee.Math;

namespace Fusee.Engine
{
    public interface IPoint2PointConstraintImp
    {

        float3 PivotInA { get; set; }
        float3 PivotInB { get; set; }
        void SetParam(float value, int num, int axis = -1);
        float GetParam(int num, int axis = -1);

        IRigidBodyImp RigidBodyA { get; }
        IRigidBodyImp RigidBodyB { get; }

        object UserObject { get; set; }
    }
}
