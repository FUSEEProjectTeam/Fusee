using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.Mouse"/>.
    /// Defines convenience methods to access the typical mouse axes and buttons. Registers
    /// the mouse velocity derived axes.
    /// </summary>
    public class MouseDevice : InputDevice
    {
        private readonly int _xVelId;
        private readonly int _yVelId;
        private readonly int _wheelVelId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The platform dependent connector to the underlying physical device.</param>
        public MouseDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
            _xVelId = RegisterVelocityAxis((int)MouseAxes.X).Id;
            _yVelId = RegisterVelocityAxis((int)MouseAxes.Y).Id;
            _wheelVelId = RegisterVelocityAxis((int)MouseAxes.Wheel).Id;
        }

        /// <summary>
        /// Gets the current position relative to the upper left corner of the rendering window.
        /// The returned values are integers.
        /// </summary>
        /// <value>
        /// The mouse position. z contains the wheel's value.
        /// </value>
        public Point PositionInt
                    =>
                        new Point
                        {
                            x = (int)GetAxis((int)MouseAxes.X),
                            y = (int)GetAxis((int)MouseAxes.Y),
                            z = (int)GetAxis((int)MouseAxes.Wheel)
                        };

        /// <summary>
        /// Gets the current position relative to the upper left corner of the rendering window.
        /// </summary>
        /// <value>
        /// The current position of the mouse.
        /// </value>
        public float2 Position => new float2(GetAxis((int)MouseAxes.X), GetAxis((int)MouseAxes.Y));

        /// <summary>
        /// Gets the mouse's x value.
        /// </summary>
        /// <value>
        /// Number of pixels from the left border of the rendering window to the current mouse position.
        /// </value>
        public float X => GetAxis((int)MouseAxes.X);

        /// <summary>
        /// Gets the mouse's y value.
        /// </summary>
        /// <value>
        /// Number of pixels from the upper border of the rendering window to the current mouse position.
        /// </value>
        public float Y => GetAxis((int)MouseAxes.Y);

        /// <summary>
        /// Gets the current wheel value.
        /// </summary>
        /// <value>
        /// The wheel value.
        /// </value>
        public float Wheel => GetAxis((int)MouseAxes.Wheel);

        /// <summary>
        /// Retrieves the current mouse velocity (speed) on screen in pixels/second.
        /// Return values are integers. 
        /// </summary>
        /// <value>
        /// The mouse velocity. 
        /// z contains the wheel velocity.
        /// </value>
        public Point VelocityInt
                    =>
                        new Point
                        {
                            x = (int)GetAxis(_xVelId),
                            y = (int)GetAxis(_yVelId),
                            z = (int)GetAxis(_wheelVelId)
                        };

        /// <summary>
        /// Retrieves the current mouse velocity (speed) on screen in pixels/second.
        /// </summary>
        /// <value>
        /// The mouse velocity. 
        /// </value>
        public float2 Velocity => new float2(GetAxis(_xVelId), GetAxis(_yVelId));

        /// <summary>
        /// Retrieves the current mouse velocity in x direction. 
        /// </summary>
        /// <value>
        /// The x velocity.
        /// </value>
        public float XVel => GetAxis(_xVelId);
        /// <summary>
        /// Retrieves the current mouse velocity in y direction. 
        /// </summary>
        /// <value>
        /// The y velocity.
        /// </value>
        public float YVel => GetAxis(_yVelId);
        /// <summary>
        /// Retrieves the current mouse wheel velocity. 
        /// </summary>
        /// <value>
        /// The velocity of the mouse wheel.
        /// </value>
        public float WheelVel => GetAxis(_wheelVelId);


        /// <summary>
        /// Retrieves information about the x axis.
        /// </summary>
        /// <value>
        /// The description for the x axis.
        /// </value>
        public AxisDescription XDesc => GetAxisDescription((int)MouseAxes.X);
        /// <summary>
        /// Retrieves information about the y axis.
        /// </summary>
        /// <value>
        /// The description for the y axis.
        /// </value>
        public AxisDescription YDesc => GetAxisDescription((int)MouseAxes.Y);
        /// <summary>
        /// Retrieves information about the mouse wheel axis.
        /// </summary>
        /// <value>
        /// The description for the mouse wheel axis.
        /// </value>
        public AxisDescription WheelDesc => GetAxisDescription((int)MouseAxes.Wheel);

        /// <summary>
        /// Retrieves information about the x velocity axis.
        /// </summary>
        /// <value>
        /// The description for the x velocity axis.
        /// </value>
        public AxisDescription XVelDesc => GetAxisDescription(_xVelId);
        /// <summary>
        /// Retrieves information about the y velocity axis.
        /// </summary>
        /// <value>
        /// The description for the y velocity  axis.
        /// </value>
        public AxisDescription YVelDesc => GetAxisDescription(_yVelId);
        /// <summary>
        /// Retrieves information about the mouse wheel velocity axis.
        /// </summary>
        /// <value>
        /// The description for the mouse wheel velocity axis.
        /// </value>
        public AxisDescription WheelVelDesc => GetAxisDescription(_wheelVelId);

        /// <summary>
        /// Retrieves the current state of the left mouse button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the left mouse button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool LeftButton => GetButton((int)MouseButtons.Left);
        /// <summary>
        /// Retrieves the current state of the middle mouse button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the left middle button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool MiddleButton => GetButton((int)MouseButtons.Middle);
        /// <summary>
        /// Retrieves the current state of the right mouse button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the left right button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool RightButton => GetButton((int)MouseButtons.Right);

        /// <summary>
        /// Retrieves information about the left mouse button.
        /// </summary>
        /// <value>
        /// The description for the left mouse button.
        /// </value>
        public ButtonDescription LeftButtonDesc => GetButtonDescription((int)MouseButtons.Left);
        /// <summary>
        /// Retrieves information about the middle mouse button.
        /// </summary>
        /// <value>
        /// The description for the middle mouse button.
        /// </value>
        public ButtonDescription MiddleButtonDesc => GetButtonDescription((int)MouseButtons.Middle);
        /// <summary>
        /// Retrieves information about the right mouse button.
        /// </summary>
        /// <value>
        /// The description for the right mouse button.
        /// </value>
        public ButtonDescription RightButtonDesc => GetButtonDescription((int)MouseButtons.Right);
    }
}