namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class AudioImplementor
    {
        public static IAudioImp CreateAudioImp()
        {
            return new NAudioImp();
        }
    }
}