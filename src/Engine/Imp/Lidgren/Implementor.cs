namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class NetworkImplementor
    {
        public static INetworkImp CreateNetworkImp()
        {
            return new NetworkImp();
        }
    }
}