
namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class InputDriverImplementor
    {
        /// <summary>
        /// Creates the audio implementation.
        /// </summary>
        /// <returns>An instance of SFMLAudioImp is returned.</returns>
        public static IInputDriverImp CreateInputDriverImp()
        {
            return new InputDriverImp();
        }
    }
}