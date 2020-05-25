using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Injection helper. Instantiates the root implementation object.
    /// </summary>
    public class PhysicsImplementor
    {
        /// <summary>
        /// Creates the dynamic world.
        /// </summary>
        /// <returns></returns>
        public static IDynamicWorldImp CreateDynamicWorldImp()
        {
            return new DynamicWorldImp();
        }
    }
}