namespace Fusee.Engine
{

    /// <summary>
    /// This class is instantiated dynamically (by reflection)
    /// </summary>
    public class NetworkImplementor
    {
        /// <summary>
        /// Creates the <see cref="NetworkImp"/>.
        /// </summary>
        /// <returns></returns>
        public static INetworkImp CreateNetworkImp()
        {
            return new NetworkImp();
        }
    }
}