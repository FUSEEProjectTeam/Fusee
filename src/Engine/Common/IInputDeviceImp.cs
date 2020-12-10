
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Symbolic value describing the general nature of an input device.
    /// </summary>
    public enum DeviceCategory
    {
        /// <summary>
        /// The input device is a mouse. A mouse exhibits two axes 'X' and 'Y', one axis 'Wheel', 
        /// three buttons 'Left', 'Middle', and 'Right'.
        /// </summary>
        Mouse,
        /// <summary>
        /// A keyboard input device exhibits the buttons defined in <see cref="KeyCodes"/>.
        /// </summary>
        Keyboard,
        /// <summary>
        /// A Game controller exhibits the axes defined in <see cref="ControllerButton"/>.
        /// </summary>
        GameController,
        /// <summary>
        /// A touch input device.
        /// </summary>
        Touch,
        /// <summary>
        /// A Kinect Input device.
        /// </summary>
        Kinect,
        /// <summary>
        /// A skeleton input device.
        /// </summary>
        Skeleton,
        /// <summary>
        /// A six-degrees-of-freedom input device such as a SpaceMouse.
        /// </summary>
        SixDOF,
        /// <summary>
        /// Other kind of input.
        /// </summary>
        Other,
    }

    /// <summary>
    /// Symbolic value describing the nature of the axis (the typical usage of this axis in applications).
    /// Use this value to query unknown devices/axes.
    /// </summary>
    public enum AxisNature
    {
        /// <summary>
        /// The axis' values rather describe absolute situations like a position or a rotation
        /// </summary>
        Position,
        /// <summary>
        /// The axis' values rather describe velocities to be applied to objects like positional or angular speed. 
        /// </summary>
        Speed,
        /// <summary>
        /// This input device's axis' typical usage is not known.
        /// </summary>
        Unknown,
    }


    /// <summary>
    /// Symbolic value describing the logical direction of an input axis.
    /// </summary>
    public enum AxisDirection
    {
        /// <summary>
        /// Axis directs towards X (first component of a compound axis).
        /// </summary>
        X,
        /// <summary>
        /// Axis directs towards Y (second component of a compound axis).
        /// </summary>
        Y,
        /// <summary>
        /// Axis directs towards Z (third component of a compound axis).
        /// </summary>
        Z,
        /// <summary>
        /// The axis' direction is unknown.
        /// </summary>
        Unknown,
    }


    /// <summary>
    /// Symbolic value describing if and how the values produced by this axis are bound.
    /// </summary>
    public enum AxisBoundedType
    {
        /// <summary>
        /// The axis values are not bound.
        /// </summary>
        Unbound,
        /// <summary>
        /// The axis values are bound by constant values. The values can be read from <see cref="AxisDescription.MaxValueOrAxis"/> 
        /// and <see cref="AxisDescription.MinValueOrAxis"/>.
        /// </summary>
        Constant,
        /// <summary>
        /// This axis' values are bound by the values from other axes (and may possibly change over time). The bounding axes'
        /// Ids can be read from <see cref="AxisDescription.MaxValueOrAxis"/> and <see cref="AxisDescription.MinValueOrAxis"/> (cast to integer).
        /// </summary>
        OtherAxis,
    }

    /// <summary>
    /// A structure describing an input device's axis.
    /// </summary>
    public struct AxisDescription
    {
        /// <summary>
        /// The human readable name of the axis.
        /// </summary>
        public string Name;
        /// <summary>
        /// The identifier of the axis. Use this identifier to query values.
        /// An axis' Id might be different than the order of occurrence (index) of the axis
        /// when enumerated from the device.
        /// </summary>
        public int Id;
        /// <summary>
        /// The direction of the axis (used with devices exhibiting two- or more-dimensional input such as joysticks on gamepads, mouse, etc).
        /// </summary>
        public AxisDirection Direction;
        /// <summary>
        /// The nature of the axis.
        /// </summary>
        public AxisNature Nature;
        /// <summary>
        /// Indicating if the axis' values are within a defined range. Depending on this value 
        /// <see cref="MinValueOrAxis"/> and <see cref="MaxValueOrAxis"/> contain either constant range bounds 
        /// or other axes' Ids delivering the (possibly changing) range values.
        /// </summary>
        public AxisBoundedType Bounded;
        /// <summary>
        /// The minimum value returned by the axis. If <see cref="Bounded"/> is <see cref="AxisBoundedType.Constant"/> this field contains constant value.
        /// If <see cref="Bounded"/> is <see cref="AxisBoundedType.OtherAxis"/> this field contains the Id of the axis keeping the (possibly changing) minimum value.
        /// In this case cast this value to integer before passing it to InputDevice methods.
        /// </summary>
        public float MinValueOrAxis;
        /// <summary>
        /// The maximum value returned by the axis. If <see cref="Bounded"/> is <see cref="AxisBoundedType.Constant"/> this field contains constant value.
        /// If <see cref="Bounded"/> is <see cref="AxisBoundedType.OtherAxis"/> this field contains the Id of the axis keeping the (possibly changing) maximum value.
        /// In this case cast this value to integer before passing it to InputDevice methods.
        /// </summary>
        public float MaxValueOrAxis;
        /// <summary>
        /// The value at which the axis is assumed to be zero.
        /// </summary>
        public float Deadzone;
    }

    /// <summary>
    /// Implementation specific information about an axis exhibited by a device.
    /// </summary>
    public struct AxisImpDescription
    {
        /// <summary>
        /// The axis description that can be passed to user code.
        /// </summary>
        public AxisDescription AxisDesc;
        /// <summary>
        /// true, if the axis needs to be polled. false if value changes on the axis are event-triggered.
        /// </summary>
        public bool PollAxis;
    }

    /// <summary>
    /// Implementation specific information about a button exhibited by a device.
    /// </summary>
    public struct ButtonImpDescription
    {
        /// <summary>
        /// The button description that can be passed to user code.
        /// </summary>
        public ButtonDescription ButtonDesc;
        /// <summary>
        /// true, if the button needs to be polled. false if state changes on the button are event-triggered.
        /// </summary>
        public bool PollButton;
    }

    /// <summary>
    /// Information about a button exhibited by a device. Contains the Name of the button and the Id that
    /// can be used to access the button state. 
    /// </summary>
    public struct ButtonDescription
    {
        /// <summary>
        /// The human readable name of the button.
        /// </summary>
        public string Name;
        /// <summary>
        /// The identifier of the button. Use this identifier to query button state.
        /// </summary>
        /// <remards>
        /// The id of a button might be different than the order of occurrence (index) of the button
        /// when enumerated from the device.
        /// </remards>
        public int Id;
    }


    /// <summary>
    /// Event arguments sent with value changing events for input axes.
    /// </summary>
    public class AxisValueChangedArgs : EventArgs
    {
        /// <summary>
        /// The new value exhibited by the axis.
        /// </summary>
        public float Value;
        /// <summary>
        /// The axis description which axis' value has changed.
        /// </summary>
        public AxisDescription Axis;
    }

    /// <summary>
    /// Event arguments sent with state changing events for input buttons.
    /// </summary>
    public class ButtonValueChangedArgs : EventArgs
    {
        /// <summary>
        /// The new button state 
        /// </summary>
        public bool Pressed;
        /// <summary>
        /// The button description which button's state has changed.
        /// </summary>
        public ButtonDescription Button;
    }


    /// <summary>
    /// Implementors represent a physical input device. 
    /// </summary>
    public interface IInputDeviceImp
    {
        /// <summary>
        /// Gets an identifier. Implementors take care that this
        /// id is unique across all devices managed by a driver.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the human readable name of this device. This
        /// parameter can be used in user dialogs to identify devices.
        /// </summary>
        /// <value>
        /// The device description.
        /// </value>
        string Desc { get; }

        /// <summary>
        /// Gets the category of this device. Device categories define a minimal common
        /// set of buttons and axes which are identical across all devices sharing the same
        /// category.
        /// </summary>
        /// <value>
        /// The device category.
        /// </value>
        DeviceCategory Category { get; }

        /// <summary>
        /// Gets number of axes supported by this device.
        /// </summary>
        /// <value>
        /// The axes count.
        /// </value>
        int AxesCount { get; }
        /// <summary>
        /// Gets a description of the axis. This value can be used in user setup-dialogs or 
        /// to match axes of devices of different categories.
        /// </summary>
        /// <value>
        /// The description of the axis.
        /// </value>
        IEnumerable<AxisImpDescription> AxisImpDesc { get; }
        /// <summary>
        ///     Gets the value currently present at the given axis if its <see cref="AxisImpDesc"/> identifies it as a to-be-polled axis.
        /// </summary>
        /// <param name="iAxisId">The axis' Id.</param>
        /// <returns>The value currently set on the axis.</returns>
        /// <returns>The value currently set on the axis.</returns>
        /// <remarks>
        ///     See <see cref="AxisDescription"/> to get information about how to interpret the
        ///     values returned by a given axis.
        ///  </remarks>
        float GetAxis(int iAxisId);

        /// <summary>
        /// Occurs on value changes of axes exhibited by this device. 
        /// Only applies for axes where the <see cref="AxisImpDescription.PollAxis"/> is set to false.
        /// </summary>
        event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        ///     Gets the number of buttons supported by this device.
        /// </summary>
        /// <value>
        ///     The button count.
        /// </value>
        int ButtonCount { get; }
        /// <summary>
        ///     Gets information about of the specified button. This value can be used in user setup-dialogs or 
        ///     to match buttons of devices of different categories.
        /// </summary>
        /// <value>
        ///     Information about the button.
        /// </value>
        IEnumerable<ButtonImpDescription> ButtonImpDesc { get; }
        /// <summary>
        /// Gets the state of the given button if its <see cref="ButtonImpDesc"/> identifies it as a to-be-polled button
        /// </summary>
        /// <param name="iButtonId">The Id of the button.</param>
        /// <returns>true if the button is currently pressed. false, if the button is currently released.</returns>
        bool GetButton(int iButtonId);

        /// Occurs on state changes of buttons exhibited by this device. 
        /// Only applies for buttons where the <see cref="ButtonImpDescription.PollButton"/> is set to false.
        event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /*
        /// <summary>
        /// Implementors of polling devices can implement polling functionality within this method.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        void PollTick(float deltaTime);
        */
    }


    /// <summary>
    /// Abstracts the platform/device dependent implementation of an input device.
    /// </summary>
    public interface IInputDeviceImpOld
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
        /// <returns>The name of the device category.</returns>
        string GetCategory();

    }
}