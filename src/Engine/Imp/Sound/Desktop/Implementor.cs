using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Sound.Desktop
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
        /// <returns>An instance of AudioImp is returned.</returns>
        public static IAudioImp CreateAudioImp()
        {
            return new AudioImp();
        }
    }
}