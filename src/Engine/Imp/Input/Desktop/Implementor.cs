
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Input.Desktop
{
    // This class is instantiated dynamically (by reflection)
    /// <summary>
    /// Helper class to be injected.
    /// </summary>
    public class InputDriverImplementor
    {
        /// <summary>
        /// Creates the audio implementation.
        /// </summary>
        /// <returns>An instance of InputDriverImp is returned.</returns>
        public static IInputDriverImp CreateInputDriverImp()
        {
            return new InputDriverImp();
        }
    }
}
