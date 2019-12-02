using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Network.Desktop
{

    /// <summary>
    /// This class is instantiated dynamically (by reflection)
    /// </summary>
    public class NetworkImplementor
    {
        /// <summary>
        /// Creates the <see cref="NetworkImp"/>.
        /// </summary>
        /// <returns>An instance of <see cref="NetworkImp"/>.</returns>
        public static INetworkImp CreateNetworkImp()
        {
            return new NetworkImp();
        }
    }
}