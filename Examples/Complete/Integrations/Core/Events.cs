namespace Fusee.Examples.Integrations.Core
{
    public abstract class FusEvent { }

    public class FpsEvent : FusEvent
    {
        public float Fps;

        public FpsEvent(float fps)
        {
            Fps = fps;
        }
    }

    public class StartupInfoEvent : FusEvent
    {
        public bool VSync;

        public StartupInfoEvent(bool vsync)
        {
            VSync = vsync;
        }
    }
}