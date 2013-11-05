namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    /// <summary>
    /// SFMLAudio Implementor class. This class is called by ImpFactory. Do not use this.
    /// </summary>
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