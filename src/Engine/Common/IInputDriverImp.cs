using System;
using System.Collections.Generic;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Parameters sent with a <see cref="IInputDriverImp.NewDeviceConnected"/> event.
    /// </summary>
    public class NewDeviceImpConnectedArgs : EventArgs
    {
        /// <summary>
        /// The input device implementation object representing the input device (such as a game pad) that was just connected.
        /// </summary>
        public IInputDeviceImp InputDeviceImp;
    }

    /// <summary>
    /// Parameters sent with a <see cref="IInputDriverImp.DeviceDisconnected"/> event.
    /// </summary>
    public class DeviceImpDisconnectedArgs : EventArgs
    {
        /// <summary>
        /// The device identifier of the device just disconnected.
        /// </summary>
        public string Id;

        /// <summary>
        /// The human-readable description string of the device that just disconnected.
        /// </summary>
        public string Desc;
    }


    /// <summary>
    /// Implementations of this interface represent a piece of software capable of connecting to 
    /// one or more input devices (<see cref="IInputDeviceImp"/>. Members of this interface allow access to the devices.
    /// </summary>
    /// <remarks>
    /// An input "device" is considered a physical device such as keyboard, mouse, gamepad, kinect, etc.
    /// </remarks>
    public interface IInputDriverImp : IDisposable
    {
        /// <summary>
        /// Retrieves a list of devices supported by this input driver.
        /// </summary>
        /// <value>
        /// The list of devices.
        /// </value>
        /// <remarks>
        /// The devices yielded represent the current status. At any time other devices can connect or disconnect.
        /// Listen to the <see cref="NewDeviceConnected"/> and <see cref="DeviceDisconnected"/> events to get
        /// informed about new or vanishing devices. Drivers may implement "static" access to devices such that
        /// devices are connected at driver instantiation and never disconnected (in this case <see cref="NewDeviceConnected"/>
        /// and <see cref="DeviceDisconnected"/> are never fired).
        /// </remarks>
        IEnumerable<IInputDeviceImp> Devices { get; }

        /// <summary>
        /// Gets the unique driver identifier.
        /// </summary>
        /// <value>
        /// The driver identifier.
        /// </value>
        string DriverId { get; }

        /// <summary>
        /// Gets the driver description string.
        /// </summary>
        /// <value>
        /// A human-readable string describing the driver.
        /// </value>
        string DriverDesc { get; }

        /// <summary>
        /// Occurs when a new device is connected such as a gamepad is plugged into the system.
        /// </summary>
        event EventHandler<NewDeviceImpConnectedArgs> NewDeviceConnected;

        /// <summary>
        /// Occurs when a device is disconnected such as a gamepad is plugged off from the system.
        /// </summary>
        event EventHandler<DeviceImpDisconnectedArgs> DeviceDisconnected;
    }
}
