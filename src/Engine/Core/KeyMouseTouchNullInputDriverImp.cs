using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Null implementation for the <see cref="DeviceCategory.Keyboard"/> device category. This implementation is one
    /// of three dummy implementations provided by the <see cref="KeyMouseTouchNullInputDriverImp"/>. Typically used
    /// in situations where the calls to <see cref="Input.Keyboard"/> should not result in a <see cref="NullReferenceException"/>
    /// when there's actually no physical keyboard present, such as headless (windowless) renderers, image generators etc.
    /// This device implements a keyboard never generating any input.
    /// </summary>
    internal class KeyNullDevImp : IInputDeviceImp
    {
        /// <summary>
        /// A unique identifier for this device implementor.
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// A descriptive human readable text to be used in input configurators.
        /// </summary>
        public string Desc => "This device is intended for headless operation. It implements a keyboard never generating any input.";

        /// <summary>
        /// The category for this device. Always returns <see cref="DeviceCategory.Keyboard"/>.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Keyboard;

        /// <summary>
        /// The number of Axes provided by this device.
        /// </summary>
        public int AxesCount => 0;

        /// <summary>
        /// A list of descriptions for each axis provided by this device.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc { get { yield break; } }

        /// <summary>
        /// The number of buttons provided by this device.
        /// </summary>
        public int ButtonCount => Enum.GetNames(typeof(KeyCodes)).Length;

        /// <summary>
        /// A list of descriptions for each button provided by this device.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                foreach (var keyId in (KeyCodes[])Enum.GetValues(typeof(KeyCodes)))
                {
                    yield return new ButtonImpDescription
                    {
                        ButtonDesc = new ButtonDescription
                        {
                            Id = (int)keyId,
                            Name = Enum.GetName(typeof(KeyCodes), keyId)
                        },
                        PollButton = false
                    };
                }
            }
        }

        /// <summary>
        /// Called whenever an axis value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Called whenever a button value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// Called by the input system to poll for axis changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iAxisId">The input axis ID in question.</param>
        /// <returns>0 (zero) for all possible input values.</returns>
        public float GetAxis(int iAxisId) => 0;

        /// <summary>
        /// Called by the input system to poll for button changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iButtonId">The input button ID in question.</param>
        /// <returns>false for all possible input values.</returns>
        public bool GetButton(int iButtonId) => false;
    }


    /// <summary>
    /// Null implementation for the <see cref="DeviceCategory.Mouse"/> device category. This implementation is one
    /// of three dummy implementations provided by the <see cref="KeyMouseTouchNullInputDriverImp"/>. Typically used
    /// in situations where the calls to <see cref="Input.Mouse"/> should not result in a <see cref="NullReferenceException"/>
    /// when there's actually no physical keyboard present, such as headless (windowless) renderers, image generators etc.
    /// This device implements a keyboard never generating any input.
    /// </summary>
    internal class MouseNullDevImp : IInputDeviceImp
    {

        /// <summary>
        /// A unique identifier for this device implementor.
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// A descriptive human readable text to be used in input configurators.
        /// </summary>
        public string Desc => "This device is intended for headless operation. It implements a mouse never generating any input.";

        /// <summary>
        /// The category for this device. Always returns <see cref="DeviceCategory.Mouse"/>.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Mouse;


        /// <summary>
        /// The number of Axes provided by this device.
        /// </summary>
        public int AxesCount => 7; // X, Y, Scroll Wheel, MiX, MaxY, MinY, MaxY


        /// <summary>
        /// A list of descriptions for each axis provided by this device.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc
        {
            get
            {
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "X",
                        Id = (int)MouseAxes.X,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.OtherAxis,
                        MinValueOrAxis = (int)MouseAxes.MinX,
                        MaxValueOrAxis = (int)MouseAxes.MaxX
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Y",
                        Id = (int)MouseAxes.Y,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.OtherAxis,
                        MinValueOrAxis = (int)MouseAxes.MinY,
                        MaxValueOrAxis = (int)MouseAxes.MaxY
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Wheel",
                        Id = (int)MouseAxes.Wheel,
                        Direction = AxisDirection.Z,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MinX",
                        Id = (int)MouseAxes.MinX,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MaxX",
                        Id = (int)MouseAxes.MaxX,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MinY",
                        Id = (int)MouseAxes.MinY,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MaxY",
                        Id = (int)MouseAxes.MaxY,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                };
            }
        }

        /// <summary>
        /// The number of buttons provided by this device.
        /// </summary>
        public int ButtonCount => 3; // Left, Middle, Right button

        /// <summary>
        /// A list of descriptions for each button provided by this device.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                yield return new ButtonImpDescription
                {
                    ButtonDesc = new ButtonDescription
                    {
                        Name = "Left",
                        Id = (int)MouseButtons.Left
                    },
                    PollButton = false
                };
                yield return new ButtonImpDescription
                {
                    ButtonDesc = new ButtonDescription
                    {
                        Name = "Middle",
                        Id = (int)MouseButtons.Middle
                    },
                    PollButton = false
                };
                yield return new ButtonImpDescription
                {
                    ButtonDesc = new ButtonDescription
                    {
                        Name = "Right",
                        Id = (int)MouseButtons.Right
                    },
                    PollButton = false
                };
            }
        }

        /// <summary>
        /// Called whenever an axis value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Called whenever a button value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// Called by the input system to poll for axis changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iAxisId">The input axis ID in question.</param>
        /// <returns>0 (zero) for all possible input values.</returns>
        public float GetAxis(int iAxisId) => 0;

        /// <summary>
        /// Called by the input system to poll for button changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iButtonId">The input button ID in question.</param>
        /// <returns>false for all possible input values.</returns>
        public bool GetButton(int iButtonId) => false;
    }

    /// <summary>
    /// Null implementation for the <see cref="DeviceCategory.Touch"/> device category. This implementation is one
    /// of three dummy implementations provided by the <see cref="KeyMouseTouchNullInputDriverImp"/>. Typically used
    /// in situations where the calls to <see cref="Input.Touch"/> should not result in a <see cref="NullReferenceException"/>
    /// when there's actually no physical keyboard present, such as headless (windowless) renderers, image generators etc.
    /// This device implements a keyboard never generating any input.
    /// </summary>
    internal class TouchNullDevImp : IInputDeviceImp
    {
        private readonly Dictionary<int, AxisImpDescription> _tpAxisDescs;
        private readonly Dictionary<int, ButtonImpDescription> _tpButtonDescs;
        private readonly int _nTouchPointsSupported = 5;


        /// <summary>
        /// Constructor. Initializes internal structures.
        /// </summary>        
        public TouchNullDevImp()
        {
            _tpAxisDescs = new Dictionary<int, AxisImpDescription>(_nTouchPointsSupported * 2 + 5)
            {
                [(int)TouchAxes.ActiveTouchpoints] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = $"Active Touchpoints",
                        Id = (int)TouchAxes.ActiveTouchpoints,
                        Direction = AxisDirection.Unknown,
                        Nature = AxisNature.Unknown,
                        Bounded = AxisBoundedType.Unbound
                    },
                    PollAxis = true
                },
                [(int)TouchAxes.MinX] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MinX",
                        Id = (int)TouchAxes.MinX,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                },
                [(int)TouchAxes.MaxX] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MaxX",
                        Id = (int)TouchAxes.MaxX,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                },
                [(int)TouchAxes.MinY] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MinY",
                        Id = (int)TouchAxes.MinY,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                },
                [(int)TouchAxes.MaxY] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "MaxY",
                        Id = (int)TouchAxes.MaxY,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Unbound,
                        MinValueOrAxis = float.NaN,
                        MaxValueOrAxis = float.NaN
                    },
                    PollAxis = true
                }
            };

            for (var i = 0; i < _nTouchPointsSupported; i++)
            {
                int id = 2 * i + (int)TouchAxes.Touchpoint_0_X;
                _tpAxisDescs[id] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = $"Touchpoint {id} X",
                        Id = id,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.OtherAxis,
                        MinValueOrAxis = (int)TouchAxes.MinX,
                        MaxValueOrAxis = (int)TouchAxes.MaxX
                    },
                    PollAxis = false
                };
                id++;
                _tpAxisDescs[id] = new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = $"Touchpoint {id} Y",
                        Id = id,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.OtherAxis,
                        MinValueOrAxis = (int)TouchAxes.MinY,
                        MaxValueOrAxis = (int)TouchAxes.MaxY
                    },
                    PollAxis = false
                };
            }

            _tpButtonDescs = new Dictionary<int, ButtonImpDescription>(_nTouchPointsSupported);
            for (var i = 0; i < _nTouchPointsSupported; i++)
            {
                int id = i + (int)TouchPoints.Touchpoint_0;
                _tpButtonDescs[id] = new ButtonImpDescription
                {
                    ButtonDesc = new ButtonDescription()
                    {
                        Name = $"Touchpoint {i} Active",
                        Id = id,
                    },
                    PollButton = false
                };
            }
        }

        /// <summary>
        /// Called whenever an axis value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Called whenever a button value changes. Since this is a dummy device this event will never fire.
        /// Only provided to meet the <see cref="IInputDriverImp"/> interface definition.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// The number of Axes provided by this device.
        /// </summary>
        public int AxesCount => _nTouchPointsSupported * 2 + 5;

        /// <summary>
        /// A list of descriptions for each axis provided by this device.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc => _tpAxisDescs.Values;

        /// <summary>
        /// The number of buttons provided by this device.
        /// </summary>
        public int ButtonCount => _nTouchPointsSupported;

        /// <summary>
        /// A list of descriptions for each button provided by this device.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc => _tpButtonDescs.Values;

        /// <summary>
        /// A unique identifier for this device implementor.
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// A descriptive human readable text to be used in input configurators.
        /// </summary>
        public string Desc => "This device is intended for headless operation. It implements a touch device never generating any input.";

        /// <summary>
        /// The category for this device. Always returns <see cref="DeviceCategory.Touch"/>.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Touch;

        /// <summary>
        /// Called by the input system to poll for axis changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iAxisId">The input axis ID in question.</param>
        /// <returns>0 (zero) for all possible input values.</returns>
        public float GetAxis(int iAxisId) => 0;

        /// <summary>
        /// Called by the input system to poll for button changes. 
        /// Since this is a dummy device this method will return 0 for all possible input axis IDs.
        /// </summary>
        /// <param name="iButtonId">The input button ID in question.</param>
        /// <returns>false for all possible input values.</returns>
        public bool GetButton(int iButtonId) => false;
    }


    /// <summary>
    /// Null implementation driver providing dummy devices for Keyboard, Mouse and Touch <see cref="DeviceCategory"/> types.
    /// Inject an instance of this class into your <see cref="Input"/> using <see cref="Input.AddDriverImp(IInputDriverImp)"/>
    /// in situations where no real input should be provided. This guarantees that your app can still access 
    /// <see cref="Input.Keyboard"/>, <see cref="Input.Mouse"/> and <see cref="Input.Touch"/> without causing a <see cref="NullReferenceException"/>. 
    /// These devices will never generate input values other than 0 (on all axes) or false (on all buttons) and will never fire
    /// axis or button events.
    /// </summary>
    internal class KeyMouseTouchNullInputDriverImp : IInputDriverImp
    {
        readonly KeyNullDevImp _keyNullDevImp = new KeyNullDevImp();
        readonly MouseNullDevImp _mouseNullDevImp = new MouseNullDevImp();
        readonly TouchNullDevImp _touchNullDevImp = new TouchNullDevImp();

        /// <summary>
        /// List the devices/device types supported by this driver.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _keyNullDevImp;
                yield return _mouseNullDevImp;
                yield return _touchNullDevImp;
            }
        }

        // Get this driver's unique ID.
        public string DriverId => GetType().FullName;

        /// <summary>
        /// A descriptive human readable text to be used in input configurators.
        /// </summary>
        public string DriverDesc => "Driver providing Null implementations for Keyboard, Mouse and Touch devices. Intended to be used in headless applications (command line tool without render window).";


        /// <summary>
        /// Never fired by this driver.
        /// </summary>
        public event EventHandler<NewDeviceImpConnectedArgs> NewDeviceConnected;

        /// <summary>
        /// Never fired by this driver.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs> DeviceDisconnected;

        /// <summary>
        /// Dispose pattern implementation.
        /// </summary>
        public void Dispose()
        {
        }
    }
}