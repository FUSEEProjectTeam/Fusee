using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.GameController"/>.
    /// Defines convenience methods to access the typical gamepad axes and buttons. Registers
    /// the gamepad dpad axes.
    /// </summary>
    public class GamePadDevice : InputDevice
    {
        private readonly int _xDPadId;
        private readonly int _yDPadId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The platform dependent connector to the underlying physical device.</param>
        public GamePadDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
            _xDPadId = RegisterTwoButtonAxis((int)Gamepad.DPadLeft, (int)Gamepad.DPadRight, AxisDirection.X).Id;
            _yDPadId = RegisterTwoButtonAxis((int)Gamepad.DPadUp, (int)Gamepad.DPadDown, AxisDirection.Y).Id;
        }

        /// <summary>
        /// Gets the left stick´s x value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the left analog stick in x direction.
        /// </value>
        public float LSX => GetAxis((int)Gamepad.LeftStickX);

        /// <summary>
        /// Gets the left stick´s y value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the left analog stick in y direction.
        /// </value>
        public float LSY => GetAxis((int)Gamepad.LeftStickY);

        /// <summary>
        /// Gets the right stick´s x value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the right analog stick in x direction.
        /// </value>
        public float RSX => GetAxis((int)Gamepad.RightStickX);

        /// <summary>
        /// Gets the right stick´s y value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the right analog stick in y direction.
        /// </value>
        public float RSY => GetAxis((int)Gamepad.RightStickY);

        /// <summary>
        /// Gets the left trigger´s y value
        /// </summary>
        /// <value>
        /// Movement from the upmost point of the left trigger.
        /// </value>
        public float LT => GetAxis((int)Gamepad.LeftTrigger);

        /// <summary>
        /// Gets the right trigger´s y value
        /// </summary>
        /// <value>
        /// Movement from the upmost point of the right trigger.
        /// </value>
        public float RT => GetAxis((int)Gamepad.RightTrigger);

        /// <summary>
        /// Retrieves the current dpad movement in x direction. 
        /// </summary>
        /// <value>
        /// The x movement.
        /// </value>
        public float XDPad => GetAxis(_xDPadId);
        /// <summary>
        /// Retrieves the current dpad movement y direction. 
        /// </summary>
        /// <value>
        /// The y movement.
        /// </value>
        public float YDPad => GetAxis(_yDPadId);



        /// <summary>
        /// Retrieves information about the Lx axis.
        /// </summary>
        /// <value>
        /// The description for the Lx axis.
        /// </value>
        public AxisDescription LXDesc => GetAxisDescription((int)Gamepad.LeftStickX);
        /// <summary>
        /// Retrieves information about the Ly axis.
        /// </summary>
        /// <value>
        /// The description for the Ly axis.
        /// </value>
        public AxisDescription LYDesc => GetAxisDescription((int)Gamepad.LeftStickY);
        /// <summary>
        /// Retrieves information about the Rx axis.
        /// </summary>
        /// <value>
        /// The description for the Rx axis.
        /// </value>
        public AxisDescription RXDesc => GetAxisDescription((int)Gamepad.RightStickX);
        /// <summary>
        /// Retrieves information about the Ry axis.
        /// </summary>
        /// <value>
        /// The description for the Ry axis.
        /// </value>
        public AxisDescription RYDesc => GetAxisDescription((int)Gamepad.RightStickY);
        /// <summary>
        /// Retrieves information about the LT axis.
        /// </summary>
        /// <value>
        /// The description for the LT axis.
        /// </value>
        public AxisDescription LTDesc => GetAxisDescription((int)Gamepad.LeftTrigger);
        /// <summary>
        /// Retrieves information about the RT axis.
        /// </summary>
        /// <value>
        /// The description for the RT axis.
        /// </value>
        public AxisDescription RTDesc => GetAxisDescription((int)Gamepad.RightTrigger);

        /// <summary>
        /// Retrieves information about the x dpad axis.
        /// </summary>
        /// <value>
        /// The description for the x dpad axis.
        /// </value>
        public AxisDescription XDPadDesc => GetAxisDescription(_xDPadId);
        /// <summary>
        /// Retrieves information about the y dpad axis.
        /// </summary>
        /// <value>
        /// The description for the y dpad  axis.
        /// </value>
        public AxisDescription YDPadDesc => GetAxisDescription(_yDPadId);

        /// <summary>
        /// Retrieves the current state of the a button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the a button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool AButton => GetButton((int)Gamepad.A);
        /// <summary>
        /// Retrieves the current state of the x button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the x button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool XButton => GetButton((int)Gamepad.X);
        /// <summary>
        /// Retrieves the current state of the y button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the y button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool YButton => GetButton((int)Gamepad.Y);

        /// <summary>
        /// Retrieves the current state of the b button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the b button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool BButton => GetButton((int)Gamepad.B);
        /// <summary>
        /// Retrieves the current state of the start button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the start button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool StartButton => GetButton((int)Gamepad.Start);
        /// <summary>
        /// Retrieves the current state of the back button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the back button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool BackButton => GetButton((int)Gamepad.Back);

        /// <summary>
        /// Retrieves the current state of the left button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the left button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool LeftButton => GetButton((int)Gamepad.LeftShoulder);
        /// <summary>
        /// Retrieves the current state of the right button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the right button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool RightButton => GetButton((int)Gamepad.RightShoulder);
        /// <summary>
        /// Retrieves the current state of the L3 button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the L3 button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool L3Button => GetButton((int)Gamepad.L3);
        /// <summary>
        /// Retrieves the current state of the R3 button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the R3 button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool R3Button => GetButton((int)Gamepad.R3);
        /// <summary>
        /// Retrieves the current state of the dpadup button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadup button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadUpButton => GetButton((int)Gamepad.DPadUp);
        /// <summary>
        /// Retrieves the current state of the dpaddown button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpaddown button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadDownButton => GetButton((int)Gamepad.DPadDown);
        /// <summary>
        /// Retrieves the current state of the dpadleft button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadleft button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadLeftButton => GetButton((int)Gamepad.DPadLeft);
        /// <summary>
        /// Retrieves the current state of the dpadright button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadright button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadRightButton => GetButton((int)Gamepad.DPadRight);

        /// <summary>
        /// Retrieves information about the a button.
        /// </summary>
        /// <value>
        /// The description for the a button.
        /// </value>
        public ButtonDescription AButtonDesc => GetButtonDescription((int)Gamepad.A);
        /// <summary>
        /// Retrieves information about the x button.
        /// </summary>
        /// <value>
        /// The description for the x button.
        /// </value>
        public ButtonDescription XDesc => GetButtonDescription((int)Gamepad.X);
        /// <summary>
        /// Retrieves information about the y button.
        /// </summary>
        /// <value>
        /// The description for the y button.
        /// </value>
        public ButtonDescription YButtonDesc => GetButtonDescription((int)Gamepad.Y);
        /// <summary>
        /// Retrieves information about the b button.
        /// </summary>
        /// <value>
        /// The description for the b button.
        /// </value>
        public ButtonDescription BButtonDesc => GetButtonDescription((int)Gamepad.B);
        /// <summary>
        /// Retrieves information about the start button.
        /// </summary>
        /// <value>
        /// The description for the start button.
        /// </value>
        public ButtonDescription StartDesc => GetButtonDescription((int)Gamepad.Start);
        /// <summary>
        /// Retrieves information about the back button.
        /// </summary>
        /// <value>
        /// The description for the back button.
        /// </value>
        public ButtonDescription BackButtonDesc => GetButtonDescription((int)Gamepad.Back);
        /// <summary>
        /// Retrieves information about the left button.
        /// </summary>
        /// <value>
        /// The description for the left button.
        /// </value>
        public ButtonDescription LeftButtonDesc => GetButtonDescription((int)Gamepad.LeftShoulder);
        /// <summary>
        /// Retrieves information about the right button.
        /// </summary>
        /// <value>
        /// The description for the right button.
        /// </value>
        public ButtonDescription RightDesc => GetButtonDescription((int)Gamepad.RightShoulder);
        /// <summary>
        /// Retrieves information about the L3 button.
        /// </summary>
        /// <value>
        /// The description for the L3 button.
        /// </value>
        public ButtonDescription L3ButtonDesc => GetButtonDescription((int)Gamepad.L3);
        /// <summary>
        /// Retrieves information about the R3 button.
        /// </summary>
        /// <value>
        /// The description for the R3 button.
        /// </value>
        public ButtonDescription R3ButtonDesc => GetButtonDescription((int)Gamepad.R3);
        /// <summary>
        /// Retrieves information about the dpadup button.
        /// </summary>
        /// <value>
        /// The description for the dpadup button.
        /// </value>
        public ButtonDescription DPadUpDesc => GetButtonDescription((int)Gamepad.DPadUp);
        /// <summary>
        /// Retrieves information about the dpaddown button.
        /// </summary>
        /// <value>
        /// The description for the dpaddown button.
        /// </value>
        public ButtonDescription DPadDButtonDesc => GetButtonDescription((int)Gamepad.DPadDown);
        /// <summary>
        /// Retrieves information about the dpadleft button.
        /// </summary>
        /// <value>
        /// The description for the dpadleft button.
        /// </value>
        public ButtonDescription DPadLeftDesc => GetButtonDescription((int)Gamepad.DPadLeft);
        /// <summary>
        /// Retrieves information about the dpadright button.
        /// </summary>
        /// <value>
        /// The description for the dpadright button.
        /// </value>
        public ButtonDescription DPadRightButtonDesc => GetButtonDescription((int)Gamepad.DPadRight);
    }
}