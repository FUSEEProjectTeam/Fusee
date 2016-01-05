using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.Touch"/>.
    /// Defines convenience methods to access the touch buttons and axes.
    /// </summary>
    /// <remarks>
    /// A touch device represents input typically performed by fingers or a stylus. Touch devices support
    /// simultaneous presence of multiple touchpoints (performed by multiple fingers). Each touchpoint 
    /// is captured by four axes and one button. The button state (pressed or not pressed) signals the presence
    /// of the respective axis. Two positional axes for each touchpoint reveal the position in X/Y screen space 
    /// and two velocity axes represent the current velocity along X and Y.
    /// </remarks>
    public class TouchDevice : InputDevice
    {
        private readonly int[] _velocityIDs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The "driver".</param>
        public TouchDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
            int nTouchpoints = ButtonCount;
            int nVelAxes = nTouchpoints*2;
            _velocityIDs = new int[nVelAxes];
            int axisId = (int) TouchAxes.Touchpoint_0_X;
            int buttonId = (int) TouchPoints.Touchpoint_0;
            for (int i = 0; i < nVelAxes; i++)
            {
                _velocityIDs[i] = RegisterVelocityAxis(axisId++, buttonId).Id;
                i++;
                _velocityIDs[i] = RegisterVelocityAxis(axisId++, buttonId).Id;
            }
        }

        /// <summary>
        /// Returns a value signalling if the given touchpoint is currently active (if something hits the screen).
        /// </summary>
        /// <param name="touch">The touchpoint to check.</param>
        /// <returns>true if this touch is acitve (a finger is on the screen), otherwise false.</returns>
        public bool GetTouchActive(TouchPoints touch)
        {
            return GetButton((int) touch);
        }

        /// <summary>
        /// Returns the current position of the given touchpoint. 
        /// The returned values are only valid if <see cref="GetTouchActive"/> holds true for the same touchpoint.
        /// </summary>
        /// <param name="touch">The touchpoint.</param>
        /// <returns>The X/Y postion of the given touchpoint.</returns>
        public float2 GetPosition(TouchPoints touch)
        {
            return new float2(
                GetAxis((int)(touch - TouchPoints.Touchpoint_0 + TouchAxes.Touchpoint_0_X)),
                GetAxis((int)(touch - TouchPoints.Touchpoint_0 + TouchAxes.Touchpoint_0_Y))
                );
        }

        /// <summary>
        /// Retrieves the current velocity (in pixels per second) of the giben touch point. 
        /// The returned values are only valid if <see cref="GetTouchActive"/> holds true for the same touchpoint.
        /// </summary>
        /// <param name="touch">The touchpoint.</param>
        /// <returns>The two-dimensional velocitiy of the touchpoint.</returns>
        public float2 GetVelocity(TouchPoints touch)
        {
            return new float2(
                GetAxis(_velocityIDs[touch - TouchPoints.Touchpoint_0]),
                GetAxis(_velocityIDs[touch - TouchPoints.Touchpoint_0 + 1])
                );
        }

        /// <summary>
        /// Retrieves the number of currently active touchpoints (e.g. the number of fingers currently touching the screen).
        /// </summary>
        /// <value>
        /// The number of active touchpoints.
        /// </value>
        public int ActiveTouchpoints => (int)GetAxis((int)TouchAxes.ActiveTouchpoints);
    }
}