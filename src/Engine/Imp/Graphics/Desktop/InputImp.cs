using System;
using OpenTK;
using OpenTK.Input;
using Fusee.Engine.Common;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;

namespace Fusee.Engine.Imp.Graphics.Desktop
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
            _gamePad = new GamePadDeviceImp(_gameWindow);
            _spaceMouse = new SpaceMouseDeviceImp(_gameWindow);
        }

        private GameWindow _gameWindow;
        private KeyboardDeviceImp _keyboard;
        private MouseDeviceImp _mouse;
        private GamePadDeviceImp _gamePad;
        private SpaceMouseDeviceImp _spaceMouse;

        /// <summary>
        /// Devices supported by this driver: One mouse and one keyboard.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _mouse;
                yield return _keyboard;
                yield return _gamePad;
                yield return _spaceMouse;
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

#pragma warning disable 0067
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
#pragma warning restore 0067

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Part of the Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RenderCanvasInputDriverImp() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Part of the dispose pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// Implements the Controls with a SixDOF device like a Spacmeouse
    /// </summary>
    public class SpaceMouseDeviceImp : IInputDeviceImp
    {
        private GameWindow _gameWindow;

        internal SpaceMouseDeviceImp(GameWindow window)
        {
            _gameWindow = window;

        }
        /// <summary>
        /// Returns the name of the device
        /// </summary>
        public string Id
        {

            get
            {
                return GetType().FullName;
            }
        }

        /// <summary>
        /// Returns the description of this implementation
        /// </summary>
        public string Desc => "Standard spacemouse implementation";

        /// <summary>
        /// Returns the Category this device belongs in
        /// </summary>
        public DeviceCategory Category => DeviceCategory.SixDOF;

        /// <summary>
        /// Returns the number of Axes this device implements
        /// </summary>
        public int AxesCount => 6;

        /// <summary>
        /// Returns the descriptions for all axes of this device
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc
        {
            get
            {
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Horizontal X",
                        Id = 0,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Horizontal Y",
                        Id = 1,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Horizontal Z",
                        Id = 2,
                        Direction = AxisDirection.Z,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Rotation Y",
                        Id = 3,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Rotation X",
                        Id = 4,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Rotation Z",
                        Id = 5,
                        Direction = AxisDirection.Z,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
            }
        }

        /// <summary>
        /// This device does not reveal any Buttons
        /// </summary>
        public int ButtonCount => 0;

        /// <summary>
        /// This device does not implement any Buttons
        /// </summary>
        #pragma warning disable 0067
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                yield return new ButtonImpDescription
                {
                    ButtonDesc = new ButtonDescription
                    {
                        Name = "no buttons implemented",
                        Id = 0
                       
                    },
                    PollButton = false
                };
            }
        }

        /// <summary>
        /// All Axis are poll based
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// This device does not implement any Buttons
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;
        #pragma warning restore 0067
        /// <summary>
        /// Returns the current value of the axis {AxisID}
        /// </summary>
        /// <param name="iAxisId"></param>
        /// <returns></returns>
        public float GetAxis(int iAxisId)
        {
            switch(iAxisId)
                {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                    return 5;
                case 5:
                    return 6;
            }
        throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis.");
        }

        /// <summary>
        /// This device does not implement any Buttons
        /// </summary>
        /// <param name="iButtonId"></param>
        /// <returns></returns>
        #pragma warning disable 0067
        public bool GetButton(int iButtonId)
        {
            throw new NotImplementedException();
        }
#pragma warning restore 0067
    }

    /// <summary>
    /// Implements a Gamepad Control option only works for XBox Gamepads
    /// </summary>
    public class GamePadDeviceImp : IInputDeviceImp
    {
        private GameWindow _gameWindow;
        private int DeviceID;
        private ButtonImpDescription _btnADesc, _btnXDesc, _btnYDesc, _btnBDesc, _btnStartDesc, _btnSelectDesc, _dpadUpDesc, _dpadDownDesc, _dpadLeftDesc, _dpadRightDesc, _btnLeftDesc, _btnRightDesc, _btnL3Desc, _btnR3Desc;

        internal GamePadDeviceImp(GameWindow window, int deviceID = 0)
        {        
            _gameWindow = window;
            DeviceID = deviceID;

            _btnADesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP A",
                    Id = 0
                },
                PollButton = true
            };
            _btnXDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP X",
                    Id = 1
                },
                PollButton = true
            };
            _btnYDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Y",
                    Id = 2
                },
                PollButton = true
            };
            _btnBDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP B",
                    Id = 3
                },
                PollButton = true
            };
            _btnStartDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Start",
                    Id = 4
                },
                PollButton = true
            };
            _btnSelectDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Select or Back",
                    Id = 5
                },
                PollButton = true
            };
            _btnLeftDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP left button",
                    Id = 6
                },
                PollButton = true
            };
            _btnRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP right button",
                    Id = 7
                },
                PollButton = true
            };
            _btnL3Desc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP L3 button",
                    Id = 8
                },
                PollButton = true
            };
            _btnR3Desc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP R3 button",
                    Id = 9
                },
                PollButton = true
            };
            _dpadUpDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad up",
                    Id = 10
                },
                PollButton = true
            };
            _dpadDownDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Down",
                    Id = 11
                },
                PollButton = true
            };
            _dpadLeftDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Left",
                    Id = 12
                },
                PollButton = true
            };
            _dpadRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Right",
                    Id = 13
                },
                PollButton = true
            };
        }

        /// <summary>
        /// Returns Name of Device.
        /// </summary>
        public string Id
        {

            get
            {
                return GetType().FullName;
            }
        }
        /// <summary>
        /// Description.
        /// </summary>
        public string Desc
        {
            get
            {
                return "Standard XBox-Gamepad implementation.";
            }
        }

        /// <summary>
        /// Returns Type of ínput device.
        /// </summary>
        public DeviceCategory Category
        {
            get
            {
                return DeviceCategory.GameController;
            }
        }
        /// <summary>
        /// Returns Number of Axes.
        /// </summary>
        public int AxesCount => 6;

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
                        Name = "Left Stick X",
                        Id = 0,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Left Stick Y",
                        Id = 1,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Right Stick X",
                        Id = 2,
                        Direction = AxisDirection.X,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Right Stick Y",
                        Id = 3,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = -1,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Left Trigger",
                        Id = 4,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = 0,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
                yield return new AxisImpDescription
                {
                    AxisDesc = new AxisDescription
                    {
                        Name = "Right Trigger",
                        Id = 5,
                        Direction = AxisDirection.Y,
                        Nature = AxisNature.Position,
                        Bounded = AxisBoundedType.Constant,
                        MinValueOrAxis = 0,
                        MaxValueOrAxis = 1
                    },
                    PollAxis = true
                };
            }
        }
        /// <summary>
        /// Returns Number of Buttons.
        /// </summary>
        public int ButtonCount
        {
            get
            {
                return 14;
            }

        }

        /// <summary>
        /// A gamepad exposes 14 buttons.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                yield return _btnADesc;
                yield return _btnXDesc;
                yield return _btnYDesc;
                yield return _btnBDesc;
                yield return _btnStartDesc;
                yield return _btnSelectDesc;
                yield return _btnRightDesc;
                yield return _btnLeftDesc;
                yield return _btnR3Desc;
                yield return _btnL3Desc;
                yield return _dpadUpDesc;
                yield return _dpadDownDesc;
                yield return _dpadLeftDesc;
                yield return _dpadRightDesc;

            }
        }
        ///<summary>
        /// All axis are Poll based see GetAxis
        ///</summary>
#pragma warning disable 0067
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// no Buttons implemented
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;
#pragma warning restore 0067

        /// <summary>
        /// Retrieves values for the X, Y and Trigger axes. No other axes are supported by this device.
        /// </summary>
        /// <param name="iAxisId">The axis to retrieve information for.</param>
        /// <returns>The value at the given axis.</returns>
        public float GetAxis(int iAxisId)
        {
            var currentThumbSticks = GamePad.GetState(DeviceID).ThumbSticks;
            var currentTrigger = GamePad.GetState(DeviceID).Triggers;

            switch (iAxisId)
            {
                case 0:
                    return currentThumbSticks.Left.X;
                case 1:
                    return currentThumbSticks.Left.Y;
                case 2:
                    return currentThumbSticks.Right.X;
                case 3:
                    return currentThumbSticks.Right.Y;
                case 4:
                    return currentTrigger.Left;
                case 5:
                    return currentTrigger.Right;

            }
            throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis.");
        }
        /// <summary>
        /// Returns a Boolean Value for Controller Input.
        /// </summary>
        public bool GetButton(int iButtonId)
        {
            var GPB = GamePad.GetState(DeviceID).Buttons;
            var Dpad = GamePad.GetState(DeviceID).DPad;

            switch (iButtonId)
            {
                case 0:
                    return GPB.A == ButtonState.Pressed;
                case 1:
                    return GPB.X == ButtonState.Pressed;
                case 2:
                    return GPB.Y == ButtonState.Pressed;
                case 3:
                    return GPB.B == ButtonState.Pressed;
                case 4:
                    return GPB.Start == ButtonState.Pressed;
                case 5:
                    return GPB.Back == ButtonState.Pressed;
                case 6:
                    return GPB.LeftShoulder == ButtonState.Pressed;
                case 7:
                    return GPB.RightShoulder == ButtonState.Pressed;
                case 8:
                    return GPB.LeftStick == ButtonState.Pressed;
                case 9:
                    return GPB.RightStick == ButtonState.Pressed;
                case 10:
                    return Dpad.Up == ButtonState.Pressed;
                case 11:
                    return Dpad.Down == ButtonState.Pressed;
                case 12:
                    return Dpad.Left == ButtonState.Pressed;
                case 13:
                    return Dpad.Right == ButtonState.Pressed;
            }
            throw new InvalidOperationException($"Unknown button {iButtonId}. Cannot get value for unknown button.");
        }
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
                return from k in _keymapper orderby k.Value.Id select new ButtonImpDescription { ButtonDesc = k.Value, PollButton = false };
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


#pragma warning disable 0067
        /// <summary>
        /// No axes exist on this device, so listeners registered to this event will never get called.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// All buttons exhibited by this device are event-driven buttons, so this is the point to hook to in order
        /// to get information from this device.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;
#pragma warning restore 0067

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
            throw new InvalidOperationException($"Button {iButtonId} does not exist or is no pollable. Listen to the ButtonValueChanged event to receive keyboard notifications from this device.");
        }
    }

    /// <summary>
    /// Mouse input device implementation for Desktop an Android platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private GameWindow _gameWindow;
        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;

        /// <summary>
        /// Creates a new mouse input device instance using an existing <see cref="OpenTK.GameWindow"/>.
        /// </summary>
        /// <param name="gameWindow">The game window providing mouse input.</param>
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
                    Id = (int)MouseButtons.Left
                },
                PollButton = false
            };
            _btnMiddleDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Middle",
                    Id = (int)MouseButtons.Middle
                },
                PollButton = false
            };
            _btnRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Right",
                    Id = (int)MouseButtons.Right
                },
                PollButton = false
            };
        }

        /// <summary>
        /// Number of axes. Here seven: "X", "Y" and "Wheel" as well as MinX, MaxX, MinY and MaxY
        /// </summary>
        public int AxesCount => 7;

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
#pragma warning disable 0067
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// All three mouse buttons are event-based. Listen to this event to get information about mouse button state changes.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;
#pragma warning restore 0067

        /// <summary>
        /// Retrieves values for the X, Y and Wheel axes. No other axes are supported by this device.
        /// </summary>
        /// <param name="iAxisId">The axis to retrieve information for.</param>
        /// <returns>The value at the given axis.</returns>
        public float GetAxis(int iAxisId)
        {
            switch (iAxisId)
            {
                case (int)MouseAxes.X:
                    return _gameWindow.Mouse.X;
                case (int)MouseAxes.Y:
                    return _gameWindow.Mouse.Y;
                case (int)MouseAxes.Wheel:
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
}
