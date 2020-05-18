using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Flags specifying point-to-point constraint details.
    /// </summary>
    public enum PointToPointFlags
    {
#pragma warning disable 1591
        PointToPointFlagsErp = 1,
        PointToPointFlagsStopErp = 2,
        PointToPointFlagsCfm = 3,
        PointToPointFlagsStopCfm = 4
#pragma warning restore 1591
    };

    /// <summary>
    /// Implementation agnostic representation of a point-to-point constraint.
    /// </summary>
    public interface IPoint2PointConstraintImp : IConstraintImp
    {
        /// <summary>
        /// Gets and sets the pivot in a.
        /// </summary>
        /// <value>
        /// The pivot in a.
        /// </value>
        float3 PivotInA { get; set; }
        /// <summary>
        /// Gets and sets the pivot in b.
        /// </summary>
        /// <value>
        /// The pivot in b.
        /// </value>
        float3 PivotInB { get; set; }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        void UpdateRhs(float timeStep);

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="constraintParameter">The constraint parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="axis">The axis.</param>
        void SetParam(PointToPointFlags constraintParameter, float value, int axis = -1);
        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="axis">The axis.</param>
        /// <returns></returns>
        float GetParam(PointToPointFlags param, int axis = -1);
    }
}
