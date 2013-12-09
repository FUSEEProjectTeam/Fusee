namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class InputDeviceImplementor
    {
        /// <summary>
        /// Creates the audio implementation.
        /// </summary>
        /// <returns>An instance of SFMLAudioImp is returned.</returns>
        public static IInputDeviceImp CreateInputDeviceImp()
        {
            return new InputDeviceImp();
        }
    }
}