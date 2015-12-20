using System;
using OpenTK;
using OpenTK.Input;
using Fusee.Engine.Common;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Input driver implementation for keyboard and mouse input on Desktop and Android.
    /// </summary>
    public class RenderCanvasInputDriverImp : IInputDriverImp
    {
        /// <summary>
        /// Constructor. Use this in platform specific application projects. 
        /// </summary>
        /// <param name="renderCanvas">The render canvas to provide mouse and keyboard input for.</param>
        public RenderCanvasInputDriverImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");

            _gameWindow = ((RenderCanvasImp)renderCanvas)._gameWindow;
            if (_gameWindow == null)
                throw new ArgumentNullException(nameof(_gameWindow));

            _keyboard = new KeyboardDeviceImp(_gameWindow);
            _mouse = new MouseDeviceImp(_gameWindow);
        }

        private GameWindow _gameWindow;
        private KeyboardDeviceImp _keyboard;
        private MouseDeviceImp _mouse;

        /// <summary>
        /// Devices supported by this driver: One mouse and one keyboard.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _mouse;
                yield return _keyboard;
            }
        }

        /// <summary>
        /// Returns a human readable description of this driver.
        /// </summary>
        public string DriverDesc
        {
            get
            {
#if PLATFORM_DESKTOP
                const string pf = "Desktop";
#elif PLATFORM_ANDROID
                const string pf = "Android";
#endif
                return "OpenTK GameWindow Mouse and Keyboard input driver for " + pf;
            }
        }

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the 
        /// full class name (including namespace).
        /// </summary>
        public string DriverId
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs> DeviceDisconnected;

        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<NewDeviceImpConnectedArgs> NewDeviceConnected;
    }


    /// <summary>
    /// Keyboard input device implementation for Desktop an Android platforms.
    /// </summary>
    public class KeyboardDeviceImp : IInputDeviceImp
    {
        private GameWindow _gameWindow;
        private Keymapper _keymapper;

        /// <summary>
        /// Should be called by the driver only.
        /// </summary>
        /// <param name="gameWindow"></param>
        internal KeyboardDeviceImp(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _keymapper = new Keymapper();
            _gameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
            _gameWindow.Keyboard.KeyUp += OnGameWinKeyUp;

        }

        /// <summary>
        /// Returns the number of Axes (==0, keyboard does not support any axes).
        /// </summary>
        public int AxesCount
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Empty enumeration for keyboard, since <see cref="AxesCount"/> is 0.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Returns the number of enum values of <see cref="KeyCodes"/>
        /// </summary>
        public int ButtonCount
        {
            get
            {
                return Enum.GetNames(typeof(KeyCodes)).Length;
            }
        }

        /// <summary>
        /// Returns a description for each keyboard button.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                return from k in _keymapper orderby k.Value.Id select new ButtonImpDescription {ButtonDesc = k.Value, PollButton = false};
            }
        }

        /// <summary>
        /// This is a keyboard device, so this property returns <see cref="DeviceCategory.Keyboard"/>.
        /// </summary>
        public DeviceCategory Category
        {
            get
            {
                return DeviceCategory.Keyboard;
            }
        }

        /// <summary>
        /// Human readable description of this device (to be used in dialogs).
        /// </summary>
        public string Desc
        {
            get
            {
                return "Standard Keyboard implementation.";
            }
        }

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the 
        /// full class name (including namespace).
        /// </summary>
        public string Id
        {
            get
            {
                return GetType().FullName;
            }
        }

        /// <summary>
        /// No axes exist on this device, so listeners registered to this event will never get called.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// All buttons exhibited by this device are event-driven buttons, so this is the point to hook to in order
        /// to get information from this device.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// Called when keyboard button is pressed down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
            ButtonDescription btnDesc;
            if (ButtonValueChanged != null && _keymapper.TryGetValue(key.Key, out btnDesc))
            {
                ButtonValueChanged(this, new ButtonValueChangedArgs
                {
                    Pressed = true,
                    Button = btnDesc
                });
            }
        }

        /// <summary>
        /// Called when keyboard button is released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
            ButtonDescription btnDesc;
            if (ButtonValueChanged != null && _keymapper.TryGetValue(key.Key, out btnDesc))
            {
                ButtonValueChanged(this, new ButtonValueChangedArgs
                {
                    Pressed = false,
                    Button = btnDesc
                });
            }
        }

        /// <summary>
        /// This device does not support any axes at all. Always throws.
        /// </summary>
        /// <param name="iAxisId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public float GetAxis(int iAxisId)
        {
            throw new InvalidOperationException($"Unsopported axis {iAxisId}. This device does not support any axis at all.");
        }

        /// <summary>
        /// This device does not support to-be-polled-buttons. All keyboard buttons are event-driven. Listen to the <see cref="ButtonValueChanged"/>
        /// event to reveive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException($"Button {iButtonId} does not exist or is no pollable. Listen to the ButtonValueChanged event to reveive keyboard notifications from this device.");
        }
    }

    /// <summary>
    /// Mouse input device implementation for Desktop an Android platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private GameWindow _gameWindow;
        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;

        public MouseDeviceImp(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _gameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
            _gameWindow.Mouse.ButtonUp += OnGameWinMouseUp;

            _btnLeftDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Left",
                    Id = (int) MouseButtons.Left
                },
                PollButton = false
            };
            _btnMiddleDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Middle",
                    Id = (int) MouseButtons.Middle
                },
                PollButton = false
            };
            _btnRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Right",
                    Id = (int) MouseButtons.Right
                },
                PollButton = false
            };
        }

        /// <summary>
        /// Number of axes. Here three: "X", "Y" and "Wheel"
        /// </summary>
        public int AxesCount
        {
            get { return 6; }
        }

        /// <summary>
        /// Returns description information for all axes.
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
                        Id = (int) MouseAxes.X,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.OtherAxis,
                        MinValueOrAxis = (int) MouseAxes.MinX,
                        MaxValueOrAxis = (int) MouseAxes.MaxX
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Y",
                        Id = (int) MouseAxes.Y,
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
                        Id = (int) MouseAxes.Wheel,
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
        /// Number of buttons exposed by this device. Here three: Left, Middle and Right mouse buttons.
        /// </summary>
        public int ButtonCount => 3;

        /// <summary>
        /// A mouse exposes three buttons: left, middle and right.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                yield return _btnLeftDesc;
                yield return _btnMiddleDesc;
                yield return _btnRightDesc;
            }
        }

        /// <summary>
        /// Returns <see cref="DeviceCategory.Mouse"/>, just because it's a mouse.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Mouse;

        /// <summary>
        /// Short description string for this device to be used in dialogs.
        /// </summary>
        public string Desc => "Standard Mouse implementation.";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the 
        /// full class name (including namespace).
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// No event-based axes are exposed by this device. Use <see cref="GetAxis"/> to akquire mouse axis information.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// All three mouse buttons are event-based. Listen to this event to get information about mouse button state changes.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// Retrieves values for the X, Y and Wheel axes. No other axes are supported by this device.
        /// </summary>
        /// <param name="iAxisId">The axis to retrieve information for.</param>
        /// <returns>The value at the given axis.</returns>
        public float GetAxis(int iAxisId)
        {
            switch (iAxisId)
            {
                case (int) MouseAxes.X:
                    return _gameWindow.Mouse.X;
                case (int) MouseAxes.Y:
                    return _gameWindow.Mouse.Y;
                case (int) MouseAxes.Wheel:
                    return _gameWindow.Mouse.WheelPrecise;
                case (int)MouseAxes.MinX:
                    return 0;
                case (int)MouseAxes.MaxX:
                    return _gameWindow.Width;
                case (int)MouseAxes.MinY:
                    return 0;
                case (int)MouseAxes.MaxY:
                    return _gameWindow.Height;
            }
            throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis.");
        }

        /// <summary>
        /// This device does not support to-be-polled-buttons. All mouse buttons are event-driven. Listen to the <see cref="ButtonValueChanged"/>
        /// event to reveive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException(
                $"Unsopported axis {iButtonId}. This device does not support any to-be polled axes at all.");
        }

        /// <summary>
        /// Called when the game window's mouse is pressed down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (ButtonValueChanged != null)
            {
                ButtonDescription btnDesc;
                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        btnDesc = _btnLeftDesc.ButtonDesc;
                        break;
                    case MouseButton.Middle:
                        btnDesc = _btnMiddleDesc.ButtonDesc;
                        break;
                    case MouseButton.Right:
                        btnDesc = _btnRightDesc.ButtonDesc;
                        break;
                    default:
                        return;
                }

                ButtonValueChanged(this, new ButtonValueChangedArgs
                {
                    Pressed = true,
                    Button = btnDesc
                });
            }
        }

        /// <summary>
        /// Called when the game window's mouse is released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (ButtonValueChanged != null)
            {
                ButtonDescription btnDesc;
                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        btnDesc = _btnLeftDesc.ButtonDesc;
                        break;
                    case MouseButton.Middle:
                        btnDesc = _btnMiddleDesc.ButtonDesc;
                        break;
                    case MouseButton.Right:
                        btnDesc = _btnRightDesc.ButtonDesc;
                        break;
                    default:
                        return;
                }

                ButtonValueChanged(this, new ButtonValueChangedArgs
                {
                    Pressed = false,
                    Button = btnDesc
                });
            }
        }
    }


    /// <summary>
    /// This class accesses the underlying OpenTK adapter and is the implementation of the input interface <see cref="IInputImp" />.
    /// </summary>
    public class InputImp : IInputImp
    {
        #region Fields
        /// <summary>
        /// The game window where the content will be rendered to.
        /// </summary>
        protected GameWindow _gameWindow;
        internal KeymapperOld _keymapper;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InputImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <exception cref="System.ArgumentNullException">renderCanvas</exception>
        /// <exception cref="System.ArgumentException">renderCanvas must be of type RenderCanvasImp;renderCanvas</exception>
        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");

            _gameWindow = ((RenderCanvasImp) renderCanvas)._gameWindow;
            if (_gameWindow != null)
            {
                _gameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
                _gameWindow.Keyboard.KeyUp += OnGameWinKeyUp;
                _gameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
                _gameWindow.Mouse.ButtonUp += OnGameWinMouseUp;
                _gameWindow.Mouse.Move += OnGameWinMouseMove;
            }
            else
            {
                // Todo

            }

            _keymapper = new KeymapperOld();
        }
        #endregion

        #region Members
        /// <summary>
        /// Implement this to receive callbacks once a frame if your implementation needs
        /// regular updates.
        /// </summary>
        /// <param name="time">The elapsed time since the last frame.</param>
        public void FrameTick(double time)
        {
            // do nothing
        }

        /// <summary>
        /// Sets the mouse cursor to the center of the GameWindow.
        /// </summary>
        /// <returns>A Point with x,y,z properties.</returns>
        public Point SetMouseToCenter()
        {
            var ctrPoint = GetMousePos();

            if (_gameWindow.Focused)
            {
                ctrPoint.x = _gameWindow.Bounds.Left + (_gameWindow.Width/2);
                ctrPoint.y = _gameWindow.Bounds.Top + (_gameWindow.Height/2);
                Mouse.SetPosition(ctrPoint.x, ctrPoint.y);
            }

            return ctrPoint;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the cursor is visible; otherwise, <c>false</c>.
        /// </value>
        public bool CursorVisible
        {
            get { return _gameWindow.CursorVisible; }
            set { _gameWindow.CursorVisible = value; }
        }

        /// <summary>
        /// Sets the mouse position by using X and Y values (in pixel units).
        /// </summary>
        /// <param name="pos">The point containing window X and Y values.</param>
        public void SetMousePos(Point pos)
        {
            Mouse.SetPosition(pos.x, pos.y);
        }

        /// <summary>
        /// Retrieve the position(x,y values in pixel units) of the Mouse.
        /// </summary>
        /// <returns>
        /// The point containing window X and Y values.
        /// If gamewindow is null 0,0 position is returned.
        /// </returns>
        public Point GetMousePos()
        {
            if (_gameWindow != null)
                return new Point{x = _gameWindow.Mouse.X, y = _gameWindow.Mouse.Y};
            return new Point{x=0, y=0};
        }

        /// <summary>
        /// Implement this to return the absolute mouse wheel position
        /// </summary>
        /// <returns>
        /// The mouse wheel position.
        /// </returns>
        public int GetMouseWheelPos()
        {
            if (_gameWindow != null)
                return _gameWindow.Mouse.Wheel;
            return 0;
        }

        /// <summary>
        /// Trigger this event on any mouse button pressed down (and held).
        /// </summary>
        public event EventHandler<Common.MouseEventArgs> MouseButtonDown;

        /// <summary>
        /// Called when [game win mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonDown != null)
            {
                var mb = MouseButtons.Unknown;

                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                }

                MouseButtonDown(this, new Common.MouseEventArgs
                {
                        Button = mb,
                        Position = new Point {x = mouseArgs.X, y = mouseArgs.Y}
                    });
            }
        }

        /// <summary>
        /// Trigger this event on any mouse button release.
        /// </summary>
        public event EventHandler<Common.MouseEventArgs> MouseButtonUp;

        /// <summary>
        /// Called when mouse button is released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonUp != null)
            {
                var mb = MouseButtons.Unknown;

                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                }

                MouseButtonUp(this, new Common.MouseEventArgs
                {
                        Button = mb,
                        Position = new Point {x = mouseArgs.X, y = mouseArgs.Y}
                });
            }
        }

        /// <summary>
        /// Trigger this event on any mouse movement.
        /// </summary>
        public event EventHandler<Common.MouseEventArgs> MouseMove;

        /// <summary>
        /// Called when mouse is moving.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseMoveEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseMove(object sender, MouseMoveEventArgs mouseArgs)
        {
            if (MouseMove != null)
            {
                MouseMove(this, new Common.MouseEventArgs
                {
                    Button = MouseButtons.Unknown,
                    Position = new Point { x = mouseArgs.X, y = mouseArgs.Y }
                });
            }
        }

        /// <summary>
        /// Trigger this event once a key on the keyboard is pressed down.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Called when mouse button is pressed down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyDown != null && _keymapper.ContainsKey(key.Key))
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyDown(this, new KeyEventArgs
                    {
                        Alt = false,
                        Control = false,
                        Shift = false,
                        KeyCode = _keymapper[key.Key],
                    });
            }
        }

        /// <summary>
        /// Trigger this event in your implementation once a key on the keyboard is released.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;

        /// <summary>
        /// Called when keyboard key has been released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyUp != null && _keymapper.ContainsKey(key.Key))
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyUp(this, new KeyEventArgs
                    {
                        Alt = false,
                        Control = false,
                        Shift = false,
                        KeyCode = _keymapper[key.Key],
                    });
            }
        }

        
        #endregion
    }
}
