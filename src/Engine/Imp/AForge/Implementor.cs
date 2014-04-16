namespace Fusee.Engine
{
    public class VideoManagerImplementor
    {
        public static IVideoManagerImp CreateVideoManagerImp()
        {
            return new VideoManagerImp();
        }
    }
}
