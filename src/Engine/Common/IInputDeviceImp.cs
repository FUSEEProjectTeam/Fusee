
namespace Fusee.Engine
{
    public interface IInputDeviceImp
    {
        /// <summary>
        /// Implement this to get the X-Axis.
        /// </summary>
        /// <returns>The X-Axis value.</returns>
        float GetXAxis();

        /// <summary>
        /// Implement this to get the Y-Axis.
        /// </summary>
        /// <returns>The Y-Axis value.</returns>
        float GetYAxis();

        /// <summary>
        /// Implement this to get the Z-Axis.
        /// </summary>
        /// <returns>The Z-Axis value.</returns>
        float GetZAxis();

        /// <summary>
        /// Implement this to get the Device Name.
        /// </summary>
        /// <returns>The Device Name.</returns>
        string GetName();

        /// <summary>
        /// Implement this to get the pressed button.
        /// </summary>
        /// <returns>The Index of the pressed button.</returns>
        int GetPressedButton();

        /// <summary>
        /// Implement this to check if button is down.
        /// </summary>
        /// <returns>True, if button is down</returns>
        bool IsButtonDown(int button);

        /// <summary>
        /// Implement this to check if button has been pressed.
        /// </summary>
        /// <returns>True, if button has been pressed.</returns>
        bool IsButtonPressed(int button);

        /// <summary>
        /// Implement this to get the amount of buttons.
        /// </summary>
        /// <returns>The amount of buttons.</returns>
        int GetButtonCount();

        /// <summary>
        /// Implement this to get the device category name.
        /// </summary>
        /// <returns>The name of the device categroy.</returns>
        string GetCategory();

    }
}
