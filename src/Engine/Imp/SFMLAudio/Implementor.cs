namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class AudioImplementor
    {
        /// <summary>
        /// Creates the audio implementation.
        /// </summary>
        /// <returns>An instance of SFMLAudioImp is returned.</returns>
        public static IAudioImp CreateAudioImp()
        {
            return new SFMLAudioImp();
        }
    }
}