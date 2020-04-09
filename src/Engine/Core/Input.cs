using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Parameters sent when an InputDevice is connected or disconnected.
    /// </summary>
    public class DeviceConnectionArgs : EventArgs
    {
        /// <summary>
        /// The input device (such as a game pad) that was just connected or disconnected.
        /// </summary>
        public InputDevice InputDevice;
    }

    // Two-phased creation. First check if a match is given, then create.
    // This allows for preparing steps if a match is detected before the creation occurs.
    /// <summary>
    /// Checks if there is a matching device available.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public delegate bool MatchFunc(IInputDeviceImp device);
    /// <summary>
    /// Creates the input device.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public delegate InputDevice CreatorFunc(IInputDeviceImp device);

    class SpecialDeviceCreator
    {
        public MatchFunc Match;
        public CreatorFunc Creator;
    }

    /// <summary>
    /// Handles and manages all input devices. Input is a staticton (a singleton with an additional
    /// static interface). 
    /// </summary>
    /// <remarks>
    /// Use the input instance in cases where you actually need an 
    /// object to pass around (although there is no such use case in FUSEE code at all).
    /// Use the static access in all other cases to reduce typing Input.Instance
    /// over and over again. Use <code>using static Fusee.Engine.Core.Input</code> to
    /// directly access <see cref="Keyboard"/>, <see cref="Mouse"/>, <see cref="Touch"/>, 
    /// without even typing a namespace or class name.
    /// </remarks>
    public class Input
    {
        private readonly Dictionary<string, IInputDriverImp> _inputDrivers;
        /// <summary>
        /// Retrieves the input driver implementations currently registered.
        /// </summary>
        /// <value>
        /// The input driver implementations.
        /// </value>
        /// <remarks>
        /// This is an instance method. Use <see cref="Drivers"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public IEnumerable<IInputDriverImp> InputDrivers => _inputDrivers.Values;

        /// <summary>
        /// Retrieves the input driver implementations currently registered.
        /// </summary>
        /// <value>
        /// The input driver implementations.
        /// </value>
        /// <remarks>
        /// This is a static method. Use <see cref="InputDrivers"/> for an instance method 
        /// to the same functionality.
        /// </remarks>
        public static IEnumerable<IInputDriverImp> Drivers => Instance._inputDrivers.Values;

        /// <summary>
        /// Returns the values of an input device.
        /// </summary>
        public IEnumerable<InputDevice> InputDevices => _inputDevices.Values;

        /// <summary>
        /// Returns the input device values.
        /// </summary>
        public static IEnumerable<InputDevice> Devices => Instance._inputDevices.Values;

        /// <summary>
        /// The input of the space mouse.
        /// </summary>
        public readonly SixDOFDevice SpaceMouseInput;

        private readonly Dictionary<string, InputDevice> _inputDevices; 
        private readonly List<SpecialDeviceCreator> _specialDeviceCreators;       

        /// <summary>
        /// Gets the input devices of a certain type. Shortcut for
        /// <code>InputDevices.OfType&lt;TDevice&gt;()</code>
        /// </summary>
        /// <typeparam name="TDevice">The type of the devices to find.</typeparam>
        /// <returns>The input devices of the specified type</returns>
        /// <remarks>
        /// This is an instance method. Use <see cref="GetDevices{TDevice}"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public IEnumerable<TDevice> GetInputDevices<TDevice>() where TDevice : InputDevice => _inputDevices.Values.OfType<TDevice>();
        /// <summary>
        /// Gets the input devices of a certain type. Shortcut for
        /// <code>InputDevices.OfType&lt;TDevice&gt;()</code>
        /// </summary>
        /// <typeparam name="TDevice">The type of the devices to find.</typeparam>
        /// <returns>The input devices of the specified type</returns>
        /// <remarks>
        /// This is a static method. Use <see cref="GetInputDevices{TDevice}"/> for an instance method 
        /// to the same functionality.
        /// </remarks>
        public static IEnumerable<TDevice> GetDevices<TDevice>() where TDevice : InputDevice => Instance.GetInputDevices<TDevice>();

        /// <summary>
        /// Gets the first input device of a certain type. Shortcut for
        /// <code>InputDevices.OfType&lt;TDevice&gt;().FirstOrDefault()</code>
        /// </summary>
        /// <typeparam name="TDevice">The type of the device to find.</typeparam>
        /// <returns>The first device matching the given type, or null if no such device is currently present.</returns>
        /// <remarks>
        /// This is an instance method. Use <see cref="GetDevice{TDevice}"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public TDevice GetInputDevice<TDevice>(int id = 0) where TDevice : InputDevice => _inputDevices.Values.OfType<TDevice>().ElementAtOrDefault(id) ;
        /// <summary>
        /// Gets the first input device of a certain type. Shortcut for
        /// <code>InputDevices.OfType&lt;TDevice&gt;().FirstOrDefault()</code>
        /// </summary>
        /// <typeparam name="TDevice">The type of the device to find.</typeparam>
        /// <returns>The first device matching the given type, or null if no such device is currently present.</returns>
        /// <remarks>
        /// This is a static method. Use <see cref="GetInputDevice{TDevice}"/> for an instance method 
        /// to the same functionality.
        /// </remarks>
        public static TDevice GetDevice<TDevice>(int deviceid = 0) where TDevice : InputDevice => Instance.GetInputDevice<TDevice>(deviceid);

        /// <summary>
        /// Retrieves the first mouse device (if present).
        /// </summary>
        /// <value>
        /// The mouse (or null).
        /// </value>
        /// <remarks>
        /// This is an instance property. Use <see cref="Mouse"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public MouseDevice MouseInput => GetInputDevice<MouseDevice>(0);
        /// <summary>
        /// Retrieves the first mouse device (if present).
        /// </summary>
        /// <value>
        /// The mouse (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="MouseInput"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static MouseDevice Mouse => Instance.MouseInput;

        /// <summary>
        /// Retrieves the first keyboard device (if present).
        /// </summary>
        /// <value>
        /// The keyboard (or null).
        /// </value>
        /// <remarks>
        /// This is an instance property. Use <see cref="Keyboard"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public KeyboardDevice KeyboardInput => GetInputDevice<KeyboardDevice>(0);
        /// <summary>
        /// Retrieves the first keyboard device (if present).
        /// </summary>
        /// <value>
        /// The keyboard (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="KeyboardInput"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static KeyboardDevice Keyboard => Instance.KeyboardInput;

        /// <summary>
        /// Retrieves the first touch device (if present).
        /// </summary>
        /// <value>
        /// The touch device (or null).
        /// </value>
        /// <remarks>
        /// This is an instance property. Use <see cref="Touch"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public TouchDevice TouchInput => GetInputDevice<TouchDevice>(0);

        /// <summary>
        /// Retrieves the first touch device (if present).
        /// </summary>
        /// <value>
        /// The touch device (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="TouchInput"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static TouchDevice Touch => Instance.TouchInput;


        /// <summary>
        /// Occurs when a device such as a gamepad is connected.
        /// </summary>
        /// <remarks>
        /// This is an instance event. Use <see cref="DeviceConnected"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public event EventHandler<DeviceConnectionArgs> InputDeviceConnected;
        /// <summary>
        /// Occurs when a device such as a gamepad is connected.
        /// </summary>
        /// <remarks>
        /// This is a static event. Use <see cref="DeviceConnected"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static event EventHandler<DeviceConnectionArgs> DeviceConnected
        {
            add
            {
                lock (Instance.InputDeviceConnected)
                {
                    Instance.InputDeviceConnected += value;
                }
            }
            remove
            {
                lock (Instance.InputDeviceConnected)
                {
                    Instance.InputDeviceConnected -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a device such as a gamepad is disconnected.
        /// </summary>
        /// <remarks>
        /// This is an instance event. Use <see cref="DeviceConnected"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public event EventHandler<DeviceConnectionArgs> InputDeviceDisconnected;
        /// <summary>
        /// Occurs when a device such as a gamepad is disconnected.
        /// </summary>
        /// <remarks>
        /// This is a static event. Use <see cref="DeviceConnected"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static event EventHandler<DeviceConnectionArgs> DeviceDisconnected
        {
            add
            {
                lock (Instance.InputDeviceDisconnected)
                {
                    Instance.InputDeviceDisconnected += value;
                }
            }
            remove
            {
                lock (Instance.InputDeviceDisconnected)
                {
                    Instance.InputDeviceDisconnected -= value;
                }
            }
        }


        private Input()
        {
            _inputDrivers = new Dictionary<string, IInputDriverImp>();
            _inputDevices = new Dictionary<string, InputDevice>();
            _specialDeviceCreators = new List<SpecialDeviceCreator>();

            // Register devices usually present.
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.Mouse, imp => new MouseDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.Keyboard, imp => new KeyboardDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.Touch, imp => new TouchDevice(imp));
            // Users can register additional devices.
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.SixDOF, imp => new SixDOFDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.GameController, imp => new GamePadDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.GameController, imp => new GamePadDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.GameController, imp => new GamePadDevice(imp));
            RegisterInputDeviceType(imp => imp.Category == DeviceCategory.GameController, imp => new GamePadDevice(imp));

        }

        private static Input _instance;

        /// <summary>
        ///     Provides the singleton Instance of the Input Class.
        /// </summary>
        public static Input Instance => _instance ?? (_instance = new Input());
        /// <summary>
        /// Registers the type of input device available.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="creator"></param>
        public void RegisterInputDeviceType(MatchFunc match, CreatorFunc creator) 
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (creator == null) throw new ArgumentNullException(nameof(creator));

            _specialDeviceCreators.Add(new SpecialDeviceCreator {Match = match, Creator = creator});

            // Reconnect any existing devices matching the predicate
            // List<string> matchingDevices = (from device in _inputDevices.Values where device.DeviceImp != null && match(device.DeviceImp) select device.Id).ToList();
            List<string> matchingDevices = new List<string>();
            foreach (var device in _inputDevices.Values)
            {
                if (device.DeviceImp != null && match(device.DeviceImp)) matchingDevices.Add(device.Id);
            }

            foreach (var devId in matchingDevices)
            {
                InputDevice dev = _inputDevices[devId];

                // Set device to disconnected state
                dev.Disconnect();

                // Inform interested users about disconnection
                InputDeviceDisconnected?.Invoke(this, new DeviceConnectionArgs { InputDevice = dev });

                IInputDeviceImp inputDeviceImp = dev.DeviceImp;

                // Remove device from the list
                _inputDevices.Remove(devId);

                dev = creator(inputDeviceImp);
                _inputDevices[devId] = dev;

                // no need to call reconnect since device is constructed from scratch

                // Inform interested users about the newly connected device.
                InputDeviceConnected?.Invoke(this, new DeviceConnectionArgs { InputDevice = dev });
            }
        }

        private InputDevice CreateInputDevice(IInputDeviceImp imp)
        {
            // First see if a special device can be found to handle this
            foreach (var sdc in _specialDeviceCreators)
            {
                if (sdc.Match(imp))
                {
                    InputDevice ret = sdc.Creator(imp);
                    if (ret != null)
                        return ret;
                }
            }

            // Fallback - we don't know any special device, create a generic one around the device implementation.
            return new InputDevice(imp);
        }

        /// <summary>
        /// Adds an input driver implementation to the internal list. The input driver is queried about connected
        /// devices. All new devices will then show up in the <see cref="Devices"/> (or <see cref="InputDevices"/>).
        /// list (in addition to the already listed devices.
        /// </summary>
        /// <param name="inputDriver">The new input driver to add.</param>
        /// <remarks>
        /// This is an instance method. Use <see cref="AddDriverImp"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public void AddInputDriverImp(IInputDriverImp inputDriver)
        {
            if (inputDriver == null)
                throw new ArgumentNullException(nameof(inputDriver));

            foreach (var deviceImp in inputDriver.Devices)
            {
                _inputDevices[inputDriver.DriverId + "_" + deviceImp.Id] = CreateInputDevice(deviceImp);
            }

            inputDriver.DeviceDisconnected += OnDeviceImpDisconnected;
            inputDriver.NewDeviceConnected += OnNewDeviceImpConnected;

            _inputDrivers[inputDriver.DriverId] = inputDriver;
        }
        /// <summary>
        /// Adds an input driver implementation to the internal list. The input driver is queried about connected
        /// devices. All new devices will then show up in the <see cref="Devices"/> (or <see cref="InputDevices"/>).
        /// list (in addition to the already listed devices.
        /// </summary>
        /// <param name="inputDriver">The new input driver to add.</param>
        /// <remarks>
        /// This is a static method. Use <see cref="AddInputDriverImp"/> for an instance property 
        /// to the same functionality.
        /// </remarks>
        public static void AddDriverImp(IInputDriverImp inputDriver) => Instance.AddInputDriverImp(inputDriver);



        private void OnNewDeviceImpConnected(object sender, NewDeviceImpConnectedArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            IInputDriverImp driver = sender as IInputDriverImp;
            if (driver == null) throw new InvalidOperationException("Device connecting from unknown driver " + sender.ToString());

            if (args == null || args.InputDeviceImp == null)
                throw new ArgumentNullException(nameof(args), "Device or InputDeviceImp must not be null");

            string deviceKey = driver.DriverId + "_" + args.InputDeviceImp.Id;
            InputDevice existingDevice;
            if (_inputDevices.TryGetValue(deviceKey, out existingDevice))
            {
                existingDevice.Reconnect(args.InputDeviceImp);
            }
            else
            {
                existingDevice = CreateInputDevice(args.InputDeviceImp);
                _inputDevices[deviceKey] = existingDevice;
            }

            // Bubble up event to user code
            InputDeviceConnected?.Invoke(this, new DeviceConnectionArgs { InputDevice = existingDevice });
        }

        private void OnDeviceImpDisconnected(object sender, DeviceImpDisconnectedArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            IInputDriverImp driver = sender as IInputDriverImp;
            if (driver == null) throw new InvalidOperationException("Device disconnecting from unknown driver " + sender.ToString());

            string deviceKey = driver.DriverId + "_" + args.Id;
            InputDevice existingDevice;
            if (_inputDevices.TryGetValue(deviceKey, out existingDevice))
            {
                existingDevice.Disconnect();
            }
            else
            {
                throw new InvalidOperationException("Driver " + driver.DriverId + " trying to disconnect unknown device " + args.Id);
            }

            // Bubble up event to user code
            InputDeviceDisconnected?.Invoke(this, new DeviceConnectionArgs { InputDevice = existingDevice });
        }

        /// <summary>
        /// Should be called from the main (rendering-) loop. Typically not to be called by user code unless
        /// users implement their own rendering/application loop.
        /// </summary>
        public void PreRender()
        {
            foreach (var inputDevice in InputDevices)
            {
                inputDevice.PreRender();
            }
        }
        /// <summary>
        /// Should be called from the main (rendering-) loop. Typically not to be called by user code unless
        /// users implement their own rendering/application loop.
        /// </summary>
        public void PostRender()
        {
            foreach (var inputDevice in InputDevices)
            {
                inputDevice.PostRender();
            }
        }

        /// <summary>
        /// Should be called from the application framework before the application stops. Typically not to be called by user code unless
        /// users implement their own application framework.
        /// </summary>
        public void Dispose()
        {
            foreach (var device in _inputDevices.Values)
            {
                InputDeviceDisconnected?.Invoke(this, new DeviceConnectionArgs {InputDevice = device});
                device.Disconnect();
            }
            _inputDevices.Clear();
            foreach (var driver in _inputDrivers.Values)
            {
                driver.Dispose();
            }
            _inputDrivers.Clear();
        }
    }
}

