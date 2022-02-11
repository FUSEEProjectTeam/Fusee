using Avalonia.Input;
using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using KeyEventArgs = Avalonia.Input.KeyEventArgs;

namespace Fusee.Avalonia.Desktop
{
    internal class Keymapper : Dictionary<Key, ButtonDescription>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal Keymapper()
        {
            this.Add(Key.Escape, new ButtonDescription { Name = KeyCodes.Escape.ToString(), Id = (int)KeyCodes.Escape });

            // Function keys
            for (int i = 0; i < 24; i++)
            {
                this.Add(Key.F1 + i, new ButtonDescription { Name = $"F{i}", Id = (int)KeyCodes.F1 + i });
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.D0 + i, new ButtonDescription { Name = $"D{i}", Id = 0x30 + i });
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(Key.A + i, new ButtonDescription { Name = ((KeyCodes)(0x41 + i)).ToString(), Id = 0x41 + i });
            }

            this.Add(Key.Tab, new ButtonDescription { Name = KeyCodes.Tab.ToString(), Id = (int)KeyCodes.Tab });
            this.Add(Key.CapsLock, new ButtonDescription { Name = KeyCodes.Capital.ToString(), Id = (int)KeyCodes.Capital });
            this.Add(Key.LeftCtrl, new ButtonDescription { Name = KeyCodes.LControl.ToString(), Id = (int)KeyCodes.LControl });
            this.Add(Key.LeftShift, new ButtonDescription { Name = KeyCodes.LShift.ToString(), Id = (int)KeyCodes.LShift });
            //this.Add(Key.Left, new ButtonDescription { Name = KeyCodes.LWin.ToString(), Id = (int)KeyCodes.LWin });
            this.Add(Key.LeftAlt, new ButtonDescription { Name = KeyCodes.LMenu.ToString(), Id = (int)KeyCodes.LMenu });
            this.Add(Key.Space, new ButtonDescription { Name = KeyCodes.Space.ToString(), Id = (int)KeyCodes.Space });
            this.Add(Key.RightAlt, new ButtonDescription { Name = KeyCodes.RMenu.ToString(), Id = (int)KeyCodes.RMenu });
            //this.Add(Key.RightSuper, new ButtonDescription { Name = KeyCodes.RWin.ToString(), Id = (int)KeyCodes.RWin });
            //this.Add(Key.Menu, new ButtonDescription { Name = KeyCodes.Apps.ToString(), Id = (int)KeyCodes.Apps });
            //this.Add(Key.RightControl, new ButtonDescription { Name = KeyCodes.RControl.ToString(), Id = (int)KeyCodes.RControl });
            this.Add(Key.RightShift, new ButtonDescription { Name = KeyCodes.RShift.ToString(), Id = (int)KeyCodes.RShift });
            this.Add(Key.Enter, new ButtonDescription { Name = KeyCodes.Return.ToString(), Id = (int)KeyCodes.Return });
            //this.Add(Key.Backspace, new ButtonDescription { Name = KeyCodes.Back.ToString(), Id = (int)KeyCodes.Back });

            this.Add(Key.OemSemicolon, new ButtonDescription { Name = KeyCodes.Oem1.ToString(), Id = (int)KeyCodes.Oem1 });
            // this.Add(Key.Oem, new ButtonDescription { Name = KeyCodes.Oem2.ToString(), Id = (int)KeyCodes.Oem2 });
            //this.Add(Key.Tilde, new ButtonDescription { Name = KeyCodes.Oem3.ToString(), Id = (int)KeyCodes.Oem3 });
            this.Add(Key.OemOpenBrackets, new ButtonDescription { Name = KeyCodes.Oem4.ToString(), Id = (int)KeyCodes.Oem4 });
            this.Add(Key.OemBackslash, new ButtonDescription { Name = KeyCodes.Oem5.ToString(), Id = (int)KeyCodes.Oem5 });
            this.Add(Key.OemCloseBrackets, new ButtonDescription { Name = KeyCodes.Oem6.ToString(), Id = (int)KeyCodes.Oem6 });
            //this.Add(Key.Quote, new ButtonDescription { Name = KeyCodes.Oem7.ToString(), Id = (int)KeyCodes.Oem7 });
            //this.Add(Key.Plus, new ButtonDescription { Name = KeyCodes.OemPlus.ToString(), Id = (int)KeyCodes.OemPlus });
            this.Add(Key.OemComma, new ButtonDescription { Name = KeyCodes.OemComma.ToString(), Id = (int)KeyCodes.OemComma });
            this.Add(Key.OemMinus, new ButtonDescription { Name = KeyCodes.OemMinus.ToString(), Id = (int)KeyCodes.OemMinus });
            this.Add(Key.OemPeriod, new ButtonDescription { Name = KeyCodes.OemPeriod.ToString(), Id = (int)KeyCodes.OemPeriod });

            this.Add(Key.Home, new ButtonDescription { Name = KeyCodes.Home.ToString(), Id = (int)KeyCodes.Home });
            this.Add(Key.End, new ButtonDescription { Name = KeyCodes.End.ToString(), Id = (int)KeyCodes.End });
            this.Add(Key.Delete, new ButtonDescription { Name = KeyCodes.Delete.ToString(), Id = (int)KeyCodes.Delete });
            this.Add(Key.PageUp, new ButtonDescription { Name = KeyCodes.Prior.ToString(), Id = (int)KeyCodes.Prior });
            this.Add(Key.PageDown, new ButtonDescription { Name = KeyCodes.Next.ToString(), Id = (int)KeyCodes.Next });
            this.Add(Key.PrintScreen, new ButtonDescription { Name = KeyCodes.Print.ToString(), Id = (int)KeyCodes.Print });
            this.Add(Key.Pause, new ButtonDescription { Name = KeyCodes.Pause.ToString(), Id = (int)KeyCodes.Pause });
            this.Add(Key.NumLock, new ButtonDescription { Name = KeyCodes.NumLock.ToString(), Id = (int)KeyCodes.NumLock });

            //this.Add(Key.CapsLock new ButtonDescription { Name = KeyCodes.Scroll.ToString(), Id = (int)KeyCodes.Scroll });
            // Do we need to do something here?? this.Add(Key.PrintScreen,  new ButtonDescription {Name = KeyCodes.Snapshot.ToString(), Id = (int)KeyCodes.Snapshot});
            //this.Add(Key.Clear, new ButtonDescription { Name = KeyCodes.Clear.ToString(), Id = (int)KeyCodes.Clear });
            this.Add(Key.Insert, new ButtonDescription { Name = KeyCodes.Insert.ToString(), Id = (int)KeyCodes.Insert });

            //this.Add(Key.Sleep, new ButtonDescription { Name = KeyCodes.Sleep.ToString(), Id = (int)KeyCodes.Sleep });

            // KeyPad
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.NumPad0 + i, new ButtonDescription { Name = $"Numpad{i}", Id = (int)KeyCodes.NumPad0 + i });
            }

            //this.Add(Key.Numpad, new ButtonDescription { Name = KeyCodes.Decimal.ToString(), Id = (int)KeyCodes.Decimal });
            //this.Add(Key.KeyPadAdd, new ButtonDescription { Name = KeyCodes.Add.ToString(), Id = (int)KeyCodes.Add });
            //this.Add(Key.KeyPadSubtract, new ButtonDescription { Name = KeyCodes.Subtract.ToString(), Id = (int)KeyCodes.Subtract });
            //this.Add(Key.KeyPadDivide, new ButtonDescription { Name = KeyCodes.Divide.ToString(), Id = (int)KeyCodes.Divide });
            //this.Add(Key.KeyPadMultiply, new ButtonDescription { Name = KeyCodes.Multiply.ToString(), Id = (int)KeyCodes.Multiply });

            // Navigation
            this.Add(Key.Up, new ButtonDescription { Name = KeyCodes.Up.ToString(), Id = (int)KeyCodes.Up });
            this.Add(Key.Down, new ButtonDescription { Name = KeyCodes.Down.ToString(), Id = (int)KeyCodes.Down });
            this.Add(Key.Left, new ButtonDescription { Name = KeyCodes.Left.ToString(), Id = (int)KeyCodes.Left });
            this.Add(Key.Right, new ButtonDescription { Name = KeyCodes.Right.ToString(), Id = (int)KeyCodes.Right });
            /*
            catch (ArgumentException e)
            {
                //Debug.Print("Exception while creating keymap: '{0}'.", e.ToString());
                System.Windows.Forms.MessageBox.Show(
                    String.Format("Exception while creating keymap: '{0}'.", e.ToString()));
            }
           */
        }
        #endregion
    }

    /// <summary>
    /// Input driver implementation for keyboard and mouse input on Desktop and Android.
    /// </summary>
    public class AvaloniaRenderCanvasInputDriverImp : IInputDriverImp
    {
        /// <summary>
        /// Constructor. Use this in platform specific application projects.
        /// </summary>
        /// <param name="renderCanvas">The render canvas to provide mouse and keyboard input for.</param>
        public AvaloniaRenderCanvasInputDriverImp(IRenderCanvasImp renderCanvas, FuseeWindowControl control)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (!(renderCanvas is AvaloniaRenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", nameof(renderCanvas));

            _control = control;
            if (_control == null)
                throw new ArgumentNullException(nameof(_control));

            _keyboard = new KeyboardDeviceImp(_control);
            _mouse = new MouseDeviceImp(_control);
        }

        private readonly FuseeWindowControl _control;
        private readonly KeyboardDeviceImp _keyboard;
        private readonly MouseDeviceImp _mouse;


        /// <summary>
        /// Devices supported by this driver: One mouse, one keyboard and up to four gamepads.
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
                const string pf = "Desktop";
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
    /// Mouse input device implementation for Desktop an Android platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private readonly FuseeWindowControl _control;
        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;

        /// <summary>
        /// Creates a new mouse input device instance using an existing <see cref="GameWindow"/>.
        /// </summary>
        /// <param name="gameWindow">The game window providing mouse input.</param>
        public MouseDeviceImp(FuseeWindowControl control)
        {
            _control = control;

            _control.PointerMoved += OnMouseMove;

            _control.PointerPressed += OnGameWinMouseDown;
            _control.PointerReleased += OnGameWinMouseUp;

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
                    PollAxis = false
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
                    PollAxis = false
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
        /// Mouse movement is event-based. Listen to this event to get information about mouse movement.
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
            return iAxisId switch
            {
                (int)MouseAxes.Wheel => 0,
                (int)MouseAxes.MinX => 0,
                (int)MouseAxes.MaxX => _control.GetPixelSize().Width,
                (int)MouseAxes.MinY => 0,
                (int)MouseAxes.MaxY => _control.GetPixelSize().Height,
                _ => throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis."),
            };
        }

        /// <summary>
        /// Called when the game window's mouse is moved.
        /// </summary>
        /// <param name="mouseArgs">The <see cref="MouseMoveEventArgs"/> instance containing the event data.</param>
        protected void OnMouseMove(object sender, PointerEventArgs mouseArgs)
        {
            var pos = mouseArgs.GetPosition(_control);

            if (AxisValueChanged != null)
            {
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = AxisImpDesc.First(x => x.AxisDesc.Id == (int)MouseAxes.X).AxisDesc, Value = (float)pos.X });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = AxisImpDesc.First(y => y.AxisDesc.Id == (int)MouseAxes.Y).AxisDesc, Value = (float)pos.Y });
            }
        }

        /// <summary>
        /// This device does not support to-be-polled-buttons. All mouse buttons are event-driven. Listen to the <see cref="ButtonValueChanged"/>
        /// event to revive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException(
                $"Unsupported axis {iButtonId}. This device does not support any to-be polled axes at all.");
        }

        /// <summary>
        /// Called when the game window's mouse is pressed down.
        /// </summary>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseDown(object sender, PointerPressedEventArgs mouseArgs)
        {
            var mouseButton = mouseArgs.GetCurrentPoint(_control).Properties;
            ButtonDescription btnDesc = new();

            if (ButtonValueChanged != null)
            {
                if (mouseButton.IsLeftButtonPressed)
                {
                    btnDesc = _btnLeftDesc.ButtonDesc;
                }
                else if (mouseButton.IsMiddleButtonPressed)
                {
                    btnDesc = _btnMiddleDesc.ButtonDesc;
                }
                else if (mouseButton.IsRightButtonPressed)
                {
                    btnDesc = _btnRightDesc.ButtonDesc;
                }
                else
                {
                    return;
                }
            }

            ButtonValueChanged(this, new ButtonValueChangedArgs
            {
                Pressed = true,
                Button = btnDesc
            });
        }


        /// <summary>
        /// Called when the game window's mouse is released.
        /// </summary>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseUp(object sender, PointerReleasedEventArgs mouseArgs)
        {
            var mouseButton = mouseArgs.GetCurrentPoint(_control).Properties;
            ButtonDescription btnDesc = new();

            if (ButtonValueChanged != null)
            {
                if (!mouseButton.IsLeftButtonPressed)
                {
                    btnDesc = _btnLeftDesc.ButtonDesc;
                }
                else if (!mouseButton.IsMiddleButtonPressed)
                {
                    btnDesc = _btnMiddleDesc.ButtonDesc;
                }
                else if (!mouseButton.IsRightButtonPressed)
                {
                    btnDesc = _btnRightDesc.ButtonDesc;
                }
                else
                {
                    return;
                }
            }

            ButtonValueChanged(this, new ButtonValueChangedArgs
            {
                Pressed = false,
                Button = btnDesc
            });
        }
    }




    /// <summary>
    /// Keyboard input device implementation for Desktop an Android platforms.
    /// </summary>
    public class KeyboardDeviceImp : IInputDeviceImp
    {
        private readonly FuseeWindowControl _control;
        private readonly Keymapper _keymapper;

        /// <summary>
        /// Should be called by the driver only.
        /// </summary>
        /// <param name="gameWindow"></param>
        internal KeyboardDeviceImp(FuseeWindowControl control)
        {

            _control = control;
            _keymapper = new Keymapper();
            _control.KeyDown += OnGameWinKeyDown;
            _control.KeyUp += OnGameWinKeyUp;
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

        /// <summary>
        /// Called when keyboard button is pressed down.
        /// </summary>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyDown(object sender, KeyEventArgs key)
        {
            if (ButtonValueChanged != null && _keymapper.TryGetValue(key.Key, out ButtonDescription btnDesc))
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
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinKeyUp(object sender, KeyEventArgs key)
        {
            if (ButtonValueChanged != null && _keymapper.TryGetValue(key.Key, out ButtonDescription btnDesc))
            {
                ButtonValueChanged(this, new ButtonValueChangedArgs
                {
                    Pressed = false,
                    Button = btnDesc
                });
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
        /// This device does not support any axes at all. Always throws.
        /// </summary>
        /// <param name="iAxisId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public float GetAxis(int iAxisId)
        {
            throw new InvalidOperationException($"Unsupported axis {iAxisId}. This device does not support any axis at all.");
        }

        /// <summary>
        /// This device does not support to-be-polled-buttons. All keyboard buttons are event-driven. Listen to the <see cref="ButtonValueChanged"/>
        /// event to receive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException($"Button {iButtonId} does not exist or is no pollable. Listen to the ButtonValueChanged event to receive keyboard notifications from this device.");
        }
    }
}

