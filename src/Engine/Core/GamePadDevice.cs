using Fusee.Engine.Common;
using Fusee.Math.Core;

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
            _xDPadId = RegisterTwoButtonAxis(12, 13, AxisDirection.X).Id;
            _yDPadId = RegisterTwoButtonAxis(10, 11, AxisDirection.Y).Id;
        }

        /// <summary>
        /// Gets the left stick´s x value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the left analog stick in x direction.
        /// </value>
        public float LSX => GetAxis(0);

        /// <summary>
        /// Gets the left stick´s y value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point oft the left analog stick in y direction.
        /// </value>
        public float LSY => GetAxis(1);

        /// <summary>
        /// Gets the right stick´s x value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point of the right analog stick in x direction.
        /// </value>
        public float RSX => GetAxis(2);

        /// <summary>
        /// Gets the right stick´s y value.
        /// </summary>
        /// <value>
        /// Movement from the zeroing point oft the right analog stick in y direction.
        /// </value>
        public float RSY => GetAxis(3);

        /// <summary>
        /// Gets the left trigger´s y value
        /// </summary>
        /// <value>
        /// Movement from the upmost point of the left trigger.
        /// </value>
        public float LT => GetAxis(4);

        /// <summary>
        /// Gets the right trigger´s y value
        /// </summary>
        /// <value>
        /// Movement from the upmost point of the right trigger.
        /// </value>
        public float RT => GetAxis(5);

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
        public AxisDescription LXDesc => GetAxisDescription(0);
        /// <summary>
        /// Retrieves information about the Ly axis.
        /// </summary>
        /// <value>
        /// The description for the Ly axis.
        /// </value>
        public AxisDescription LYDesc => GetAxisDescription(1);
        /// <summary>
        /// Retrieves information about the Rx axis.
        /// </summary>
        /// <value>
        /// The description for the Rx axis.
        /// </value>
        public AxisDescription RXDesc => GetAxisDescription(2);
        /// <summary>
        /// Retrieves information about the Ry axis.
        /// </summary>
        /// <value>
        /// The description for the Ry axis.
        /// </value>
        public AxisDescription RYDesc => GetAxisDescription(3);
        /// <summary>
        /// Retrieves information about the LT axis.
        /// </summary>
        /// <value>
        /// The description for the LT axis.
        /// </value>
        public AxisDescription LTDesc => GetAxisDescription(4);
        /// <summary>
        /// Retrieves information about the RT axis.
        /// </summary>
        /// <value>
        /// The description for the RT axis.
        /// </value>
        public AxisDescription RTDesc => GetAxisDescription(5);

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
        public bool AButton => GetButton(0);
        /// <summary>
        /// Retrieves the current state of the x button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the x button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool XButton => GetButton(1);
        /// <summary>
        /// Retrieves the current state of the y button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the y button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool YButton => GetButton(2);

        /// <summary>
        /// Retrieves the current state of the b button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the b button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool BButton => GetButton(3);
        /// <summary>
        /// Retrieves the current state of the start button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the start button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool StartButton => GetButton(4);
        /// <summary>
        /// Retrieves the current state of the back button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the back button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool BackButton => GetButton(5);

        /// <summary>
        /// Retrieves the current state of the left button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the left button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool LeftButton => GetButton(6);
        /// <summary>
        /// Retrieves the current state of the right button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the right button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool RightButton => GetButton(7);
        /// <summary>
        /// Retrieves the current state of the L3 button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the L3 button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool L3Button => GetButton(8);
        /// <summary>
        /// Retrieves the current state of the R3 button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the R3 button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool R3Button => GetButton(9);
        /// <summary>
        /// Retrieves the current state of the dpadup button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadup button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadUpButton => GetButton(10);
        /// <summary>
        /// Retrieves the current state of the dpaddown button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpaddown button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadDownButton => GetButton(11);
        /// <summary>
        /// Retrieves the current state of the dpadleft button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadleft button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadLeftButton => GetButton(12);
        /// <summary>
        /// Retrieves the current state of the dpadright button.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dpadright button is currently pressed; otherwise, <c>false</c>.
        /// </value>
        public bool DPadRightButton => GetButton(13);

        /// <summary>
        /// Retrieves information about the a button.
        /// </summary>
        /// <value>
        /// The description for the a button.
        /// </value>
        public ButtonDescription AButtonDesc => GetButtonDescription(0);
        /// <summary>
        /// Retrieves information about the x button.
        /// </summary>
        /// <value>
        /// The description for the x button.
        /// </value>
        public ButtonDescription XDesc => GetButtonDescription(1);
        /// <summary>
        /// Retrieves information about the y button.
        /// </summary>
        /// <value>
        /// The description for the y button.
        /// </value>
        public ButtonDescription YButtonDesc => GetButtonDescription(2);
        /// <summary>
        /// Retrieves information about the b button.
        /// </summary>
        /// <value>
        /// The description for the b button.
        /// </value>
        public ButtonDescription BButtonDesc => GetButtonDescription(3);
        /// <summary>
        /// Retrieves information about the start button.
        /// </summary>
        /// <value>
        /// The description for the start button.
        /// </value>
        public ButtonDescription StartDesc => GetButtonDescription(4);
        /// <summary>
        /// Retrieves information about the back button.
        /// </summary>
        /// <value>
        /// The description for the back button.
        /// </value>
        public ButtonDescription BackButtonDesc => GetButtonDescription(5);
        /// <summary>
        /// Retrieves information about the left button.
        /// </summary>
        /// <value>
        /// The description for the left button.
        /// </value>
        public ButtonDescription LeftButtonDesc => GetButtonDescription(6);
        /// <summary>
        /// Retrieves information about the right button.
        /// </summary>
        /// <value>
        /// The description for the right button.
        /// </value>
        public ButtonDescription RightDesc => GetButtonDescription(7);
        /// <summary>
        /// Retrieves information about the L3 button.
        /// </summary>
        /// <value>
        /// The description for the L3 button.
        /// </value>
        public ButtonDescription L3ButtonDesc => GetButtonDescription(8);
        /// <summary>
        /// Retrieves information about the R3 button.
        /// </summary>
        /// <value>
        /// The description for the R3 button.
        /// </value>
        public ButtonDescription R3ButtonDesc => GetButtonDescription(9);
        /// <summary>
        /// Retrieves information about the dpadup button.
        /// </summary>
        /// <value>
        /// The description for the dpadup button.
        /// </value>
        public ButtonDescription DPadUpDesc => GetButtonDescription(10);
        /// <summary>
        /// Retrieves information about the dpaddown button.
        /// </summary>
        /// <value>
        /// The description for the dpaddown button.
        /// </value>
        public ButtonDescription DPadDButtonDesc => GetButtonDescription(11);
        /// <summary>
        /// Retrieves information about the dpadleft button.
        /// </summary>
        /// <value>
        /// The description for the dpadleft button.
        /// </value>
        public ButtonDescription DPadLeftDesc => GetButtonDescription(12);
        /// <summary>
        /// Retrieves information about the dpadright button.
        /// </summary>
        /// <value>
        /// The description for the dpadright button.
        /// </value>
        public ButtonDescription DPadRightButtonDesc => GetButtonDescription(13);
    }
}
