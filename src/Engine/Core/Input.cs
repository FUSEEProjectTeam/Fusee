using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Parameters sent with a <see cref="InputDriver.DeviceConnected"/> or <see cref="InputDriver.DeviceDisconnected"/>  event.
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
    public delegate bool MatchFunc(IInputDeviceImp device);
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
    /// Use the input instanmce in cases where you actually need an 
    /// object to pass around (although there is no such use case in FUSEE code at all).
    /// Use the static access in all other cases to reduce typing Input.Instance
    /// over and over again. Use <code>using static Fusee.Engine.Core.Input</code> to
    /// directly access <see cref="Keyboard"/>, <see cref="Mouse"/>, <see cref="Touch"/>, 
    /// <see cref="GamePad"/> and <see cref="SDOF"/>
    /// without even typing a namespace or classname.
    /// </remarks>
    public class Input
    {
        private readonly Dictionary<string, IInputDriverImp> _inputDrivers;
        /// <summary>
        /// Retrieves the the input driver implementations currently registered.
        /// </summary>
        /// <value>
        /// The input driver implmementations.
        /// </value>
        /// <remarks>
        /// This is an instance method. Use <see cref="Drivers"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public IEnumerable<IInputDriverImp> InputDrivers => _inputDrivers.Values;
        /// <summary>
        /// Retrieves the the input driver implementations currently registered.
        /// </summary>
        /// <value>
        /// The input driver implmementations.
        /// </value>
        /// <remarks>
        /// This is a static method. Use <see cref="InputDrivers"/> for an insatnce method 
        /// to the same functionality.
        /// </remarks>
        public static IEnumerable<IInputDriverImp> Drivers => Instance._inputDrivers.Values;

        private readonly Dictionary<string, InputDevice> _inputDevices;
        public IEnumerable<InputDevice> InputDevices => _inputDevices.Values;
        public static IEnumerable<InputDevice> Devices => Instance._inputDevices.Values;

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
        /// This is a static method. Use <see cref="GetInputDevices{TDevice}"/> for an insatnce method 
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
       public TDevice GetInputDevice<TDevice>() where TDevice : InputDevice => _inputDevices.Values.OfType<TDevice>().FirstOrDefault();
        /// <summary>
        /// Gets the first input device of a certain type. Shortcut for
        /// <code>InputDevices.OfType&lt;TDevice&gt;().FirstOrDefault()</code>
        /// </summary>
        /// <typeparam name="TDevice">The type of the device to find.</typeparam>
        /// <returns>The first device matching the given type, or null if no such device is currently present.</returns>
        /// <remarks>
        /// This is a static method. Use <see cref="GetInputDevice{TDevice}"/> for an insatnce method 
        /// to the same functionality.
        /// </remarks>
        public static TDevice GetDevice<TDevice>() where TDevice : InputDevice => Instance.GetInputDevice<TDevice>();

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
        public MouseDevice MouseInput => GetInputDevice<MouseDevice>();
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
        public KeyboardDevice KeyboardInput => GetInputDevice<KeyboardDevice>();
        /// <summary>
        /// Retrieves the first keyboard device (if present).
        /// </summary>
        /// <value>
        /// The keyboard (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="KeyboardInput"/> for an insatnce property 
        /// to the same functionality.
        /// </remarks>
        public static KeyboardDevice Keyboard => Instance.KeyboardInput;

        /// <summary>
        /// Retrieves the first GamePad device (if present).
        /// </summary>
        /// <value>
        /// The GamePad (or null).
        /// </value>
        /// <remarks>
        /// This is an instance property. Use <see cref="GamePad"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public GamePadDevice GamePadInput => GetInputDevice<GamePadDevice>();
        /// <summary>
        /// Retrieves the first GamePad device (if present).
        /// </summary>
        /// <value>
        /// The GamePad (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="GamePadInput"/> for an insatnce property 
        /// to the same functionality.
        /// </remarks>
        public static GamePadDevice GamePad => Instance.GamePadInput;

        /// <summary>
        /// Retrieves the first SixDOF device (if present).
        /// </summary>
        /// <value>
        /// The Device (or null).
        /// </value>
        /// <remarks>
        /// This is an instance property. Use <see cref="SDOF"/> for a static-over-singleton access
        /// to the same functionality.
        /// </remarks>
        public SixDOFDevice SixDOFInput => GetInputDevice<SixDOFDevice>();
        /// <summary>
        /// Retrieves the first SixDOF device (if present).
        /// </summary>
        /// <value>
        /// The SixDOFDevice (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="SixDOFInput"/> for an insatnce property 
        /// to the same functionality.
        /// </remarks>
        public static SixDOFDevice SDOF => Instance.SixDOFInput;

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
        public TouchDevice TouchInput => GetInputDevice<TouchDevice>();
        /// <summary>
        /// Retrieves the first touch device (if present).
        /// </summary>
        /// <value>
        /// The touch device (or null).
        /// </value>
        /// <remarks>
        /// This is a static property. Use <see cref="TouchInput"/> for an insatnce property 
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
        /// This is a static event. Use <see cref="DeviceConnected}"/> for an insatnce property 
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
        /// This is a static event. Use <see cref="DeviceConnected}"/> for an insatnce property 
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
            // Users can register additional devices.
            RegisterInputDeviceType(new MatchFunc(delegate(IInputDeviceImp imp) { return imp.Category == DeviceCategory.Mouse; }),  new CreatorFunc(delegate(IInputDeviceImp imp) { return new MouseDevice(imp);}));
            RegisterInputDeviceType(new MatchFunc(delegate (IInputDeviceImp imp) { return imp.Category == DeviceCategory.Keyboard; }), new CreatorFunc(delegate (IInputDeviceImp imp) { return new KeyboardDevice(imp); }));
            RegisterInputDeviceType(new MatchFunc(delegate (IInputDeviceImp imp) { return imp.Category == DeviceCategory.Touch; }), new CreatorFunc(delegate (IInputDeviceImp imp) { return new TouchDevice(imp); }));
            RegisterInputDeviceType(new MatchFunc(delegate (IInputDeviceImp imp) { return imp.Category == DeviceCategory.GameController; }), new CreatorFunc(delegate (IInputDeviceImp imp) { return new GamePadDevice(imp); }));
            // RegisterInputDeviceType(imp => imp.Category == DeviceCategory.Keyboard, imp => new KeyboardDevice(imp));
            // RegisterInputDeviceType(imp => imp.Category == DeviceCategory.Touch,    imp => new TouchDevice(imp));
        }


        private static Input _instance;

        /// <summary>
        ///     Provides the singleton Instance of the Input Class.
        /// </summary>
        public static Input Instance => _instance ?? (_instance = new Input());

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
        /// This is a static method. Use <see cref="AddInputDriverImp}"/> for an insatnce property 
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


    /*
    /// <summary>
    ///     The Input class takes care of all inputs. It is accessible from everywhere inside a Fusee project.
    ///     E.g. : Input.Instance.IsButtonDown(MouseButtons.Left);
    /// </summary>
    public class InputOld
    {
        #region Fields

        private static InputOld _instance;

        private IInputImp _inputImp;

        internal IInputImp InputImp
        {
            set
            {
                _inputImp = value;

                _inputImp.KeyDown += KeyDown;
                _inputImp.KeyUp += KeyUp;

                _inputImp.MouseButtonDown += ButtonDown;
                _inputImp.MouseButtonUp += ButtonUp;
                _inputImp.MouseMove += MouseMove;

                _axes = new float[(int)MouseAxes.Wheel + 1];
                _axesPreviousAbsolute = new float[(int)MouseAxes.Wheel + 1];

                _keys = new HashSet<int>();
                _keysUp = new HashSet<int>();
                _keysDown = new HashSet<int>();

                _buttonsPressed = new HashSet<int>();
            }
        }

        public event EventHandler<MouseEventArgs> OnMouseButtonDown;
        public event EventHandler<MouseEventArgs> OnMouseButtonUp;
        public event EventHandler<MouseEventArgs> OnMouseMove;

        private float[] _axes;
        private float[] _axesPreviousAbsolute;

        private HashSet<int> _keys;
        private HashSet<int> _keysUp;
        private HashSet<int> _keysDown;

        private HashSet<int> _buttonsPressed;

        /// <summary>
        ///     Gets or sets a value indicating whether to fix mouse at center.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the mouse is fixed at center; otherwise, <c>false</c>.
        /// </value>
        public bool FixMouseAtCenter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the cursor is visible; otherwise, <c>false</c>.
        /// </value>
        public bool CursorVisible
        {
            get { return _inputImp.CursorVisible; }
            set { _inputImp.CursorVisible = value; }
        }

        #endregion

        #region Members

        private void KeyDown(object sender, KeyEventArgs kea)
        {
            if (!_keys.Contains((int)kea.KeyCode))
            {
                _keys.Add((int)kea.KeyCode);

                if (!_keysDown.Contains((int)kea.KeyCode))
                    _keysDown.Add((int)kea.KeyCode);
            }
        }

        private void KeyUp(object sender, KeyEventArgs kea)
        {
            if (_keys.Contains((int)kea.KeyCode))
                _keys.Remove((int)kea.KeyCode);

            if (!_keysUp.Contains((int)kea.KeyCode))
                _keysUp.Add((int)kea.KeyCode);
        }

        private void ButtonDown(object sender, MouseEventArgs mea)
        {
            if (OnMouseButtonDown != null)
                OnMouseButtonDown(this, mea);

            if (!_buttonsPressed.Contains((int)mea.Button))
                _buttonsPressed.Add((int)mea.Button);
        }

        private void ButtonUp(object sender, MouseEventArgs mea)
        {
            if (OnMouseButtonUp != null)
                OnMouseButtonUp(this, mea);

            if (_buttonsPressed.Contains((int)mea.Button))
                _buttonsPressed.Remove((int)mea.Button);
        }

        private void MouseMove(object sender, MouseEventArgs mea)
        {
            if (OnMouseMove != null)
                OnMouseMove(this, mea);
        }

        /// <summary>
        ///     Returns the scalar value for the given axis. Typically these values are used as velocities.
        /// </summary>
        /// <param name="axis">The axis for which the value is to be returned.</param>
        /// <returns>
        ///     The current deflection of the given axis.
        /// </returns>
        public float GetAxis(MouseAxes axis)
        {
            return _axes[(int)axis];
        }

        /// <summary>
        ///     Sets the mouse position.
        /// </summary>
        /// <param name="pos">A <see cref="Point" /> with x and y values.</param>
        public void SetMousePos(Point pos)
        {
            _inputImp.SetMousePos(pos);
        }

        /// <summary>
        ///     Gets the mouse position.
        /// </summary>
        /// <returns>A <see cref="Point" /> with x and y values.</returns>
        public Point GetMousePos()
        {
            return _inputImp.GetMousePos();
        }

        /// <summary>
        ///     Check if a given key is pressed during the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the key is pressed. Otherwise false.
        /// </returns>
        public bool IsKey(KeyCodes key)
        {
            return _keys.Contains((int)key);
        }

        /// <summary>
        ///     Check if the user started pressing a key in the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the user started pressing the key in the current frame. Otherwise false.
        /// </returns>
        public bool IsKeyDown(KeyCodes key)
        {
            return _keysDown.Contains((int)key);
        }

        /// <summary>
        ///     Check if the user stopped pressing a key in the current frames.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the user stopped pressing the key in the current frame. Otherwise false.
        /// </returns>
        public bool IsKeyUp(KeyCodes key)
        {
            return _keysUp.Contains((int)key);
        }

        /// <summary>
        ///     Check if a given mouse button is pressed during the current frame.
        /// </summary>
        /// <param name="button">the mousebutton to check.</param>
        /// <returns>
        ///     True if the mousebutton is pressed. Otherwise false.
        /// </returns>
        public bool IsButton(MouseButtons button)
        {
            return _buttonsPressed.Contains((int)button);
        }

        
        /// <summary>
        /// Called when [button down] event is triggered. 
        /// Occurs when a Mouse button was pressed down once.
        /// </summary>
        /// <param name="button">The mousebutton.</param>
        /// <returns>True, if mousebutton was pressed down once. Otherwise false.</returns>

            public bool OnButtonDown(MouseButtons button)
            {
                // not implemented
                return false;
            }

        /// <summary>
        ///     Called when [button up] event is triggered.
        ///     Occurs when a Mouse button was released.
        /// </summary>
        /// <param name="button">The mousebutton.</param>
        /// <returns>True, if mousebutton was released. Otherwise false.</returns>

        public bool OnButtonUp(MouseButtons button)
        {
            // not implemented
            return false;
        }

        internal void OnUpdateFrame()
        {
            var p = _inputImp.GetMousePos();

            float currX = p.x;
            float currY = p.y;
            float currR = _inputImp.GetMouseWheelPos();

            const float deltaFix = 0.005f;

            _axes[(int)MouseAxes.X] = (currX - _axesPreviousAbsolute[(int)MouseAxes.X]) * deltaFix;
            _axes[(int)MouseAxes.Y] = (currY - _axesPreviousAbsolute[(int)MouseAxes.Y]) * deltaFix;
            _axes[(int)MouseAxes.Wheel] = (currR - _axesPreviousAbsolute[(int)MouseAxes.Wheel]) * deltaFix;

            // Fix to Center
            if (FixMouseAtCenter)
            {
                p = _inputImp.SetMouseToCenter();

                currX = p.x;
                currY = p.y;
            }

            _axesPreviousAbsolute[(int)MouseAxes.X] = currX;
            _axesPreviousAbsolute[(int)MouseAxes.Y] = currY;
            _axesPreviousAbsolute[(int)MouseAxes.Wheel] = currR;
        }

        internal void OnLateUpdate()
        {
            _keysDown.Clear();
            _keysUp.Clear();
        }

        internal void Dispose()
        {
            _instance = null;
        }

        /// <summary>
        ///     Provides the Instance of the Input Class.
        /// </summary>
        public static InputOld Instance
        {
            get { return _instance ?? (_instance = new InputOld()); }
        }

        #endregion

        #region InputDevices

        /// <summary>
        ///     Input devices like gamepads are managed here.
        /// </summary>
        public Collection<InputDevice> Devices = new Collection<InputDevice>();

        private IInputDriverImp  _inputDriverImp;

        /// <summary>
        ///     All connected input devices are added to <see cref="Devices" /> - List and the names and indices of
        ///     the devices are printed to the debugging - console.
        /// </summary>
        public void InitializeDevices()
        {
            foreach (var inputDevice in _inputDriverImp.Devices)
            {
                Devices.Add(new InputDevice(inputDevice));
            }
        }

        /// <summary>
        ///     Checks if a device at the specified index exists and returns it if it exists.
        /// </summary>
        /// <param name="deviceIndex">The index at <see cref="Devices" /></param>
        /// <returns>The device at the specified index </returns>
        public InputDevice GetDevice(int deviceIndex)
        {
            try
            {
                return Devices[deviceIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("Can not find Input Device with Device Index " + deviceIndex + "!");
            }
        }

        /// <summary>
        ///     Counts the devices.
        /// </summary>
        /// <returns>The amount of devices</returns>
        public int CountDevices()
        {
            return Devices.Count;
        }

        internal IInputDriverImp InputDriverImp
        {
            set { _inputDriverImp = value; }
        }

        #endregion
    }
    */

}

