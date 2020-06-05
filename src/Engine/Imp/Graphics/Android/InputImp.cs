using Android.Views;
using Fusee.Engine.Common;
using OpenTK.Platform.Android;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Input driver implementation for touch input on Android.
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
                throw new ArgumentException($"renderCanvas must be of type {typeof(RenderCanvasImp).FullName}.", nameof(renderCanvas));

            _view = ((RenderCanvasImp)renderCanvas).View;
            if (_view == null)
                throw new ArgumentNullException(nameof(_view));

            _touch = new TouchDeviceImp(_view);
            _keyboard = new KeyboardDeviceImp(_view);
            _mouse = new MouseDeviceImp(_view);
        }

        private View _view;
        private TouchDeviceImp _touch;
        private KeyboardDeviceImp _keyboard;
        private MouseDeviceImp _mouse;

        /// <summary>
        /// Devices supported by this driver: A touch device.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _touch;
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
                const string pf = "Android";
                return "OpenTK View Touch, Mouse and Keyboard input driver for " + pf;
            }
        }

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string DriverId => GetType().FullName;

#pragma warning disable 0067

        /// <summary>
        /// Not supported on this driver. Devices supported here are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs> DeviceDisconnected;

        /// <summary>
        /// Not supported on this driver. Devices supported here are considered to be connected all the time.
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

        #endregion IDisposable Support
    }

    /// <summary>
    /// Touch input device implementation for the Android platform using a View's Touch events .
    /// </summary>
    public class TouchDeviceImp : IInputDeviceImp
    {
        private Dictionary<int, AxisImpDescription> _tpAxisDescs;
        private Dictionary<int, ButtonImpDescription> _tpButtonDescs;
        private Dictionary<int, int> _activeTouchpoints;
        private int _nTouchPointsSupported = 5;
        private readonly View _view;

        #region Android handling

        private void DisconnectViewEvents()
        {
        }

        // Android tries some sense-less smartness in interweaving
        // MotionEventActions.Down and MotionEventActions.Up with
        // MotionEventActions.PointerDown and .. PointerUp.
        // So we need to code against this smartness...
        // Code is taken from
        // http://www.vogella.com/tutorials/AndroidTouch/article.html
        // Xamarin Docs are all but helpful here.
        // Its fun to see how HTML5 just works and does the right at this place ...
        private void ConnectViewEvents()
        {
            _view.Touch += delegate (object sender, View.TouchEventArgs args)
            {
                var evt = args.Event;

                // Get pointer index which pointer is really affected by this event.
                int pointerIndex = evt.ActionIndex;

                // get pointer ID of the index (Android loves indirection).
                int pointerId = evt.GetPointerId(pointerIndex);

                // get masked (not specific to a pointer) action
                var maskedAction = evt.ActionMasked;

                switch (maskedAction)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.PointerDown:
                        OnViewTouchStart(pointerId, evt.GetX(pointerIndex), evt.GetY(pointerIndex));
                        args.Handled = true;
                        break;

                    case MotionEventActions.Move: // A move action can be specified for more than one pointer...
                        for (int i = 0; i < evt.PointerCount; i++)
                        {
                            // double check if the touchpoint is really tracked.
                            if (_activeTouchpoints.ContainsKey(evt.GetPointerId(i)))
                                OnViewTouchMove(evt.GetPointerId(i), evt.GetX(i), evt.GetY(i));
                        }
                        args.Handled = true;
                        break;

                    case MotionEventActions.Up:
                    case MotionEventActions.PointerUp:
                        OnViewTouchEnd(pointerId, evt.GetX(pointerIndex), evt.GetY(pointerIndex));
                        args.Handled = true;
                        break;

                    case MotionEventActions.Cancel: // Not sure if Android doesn't mean: "Cancel ALL touchpoints" here.
                        OnViewTouchCancel(pointerId, evt.GetX(pointerIndex), evt.GetY(pointerIndex));
                        args.Handled = true;
                        break;
                }
            };
        }

        private float GetWindowWidth()
        {
            return _view.Width;
        }

        private float GetWindowHeight()
        {
            return _view.Height;
        }

        #endregion Android handling

        private int NextFreeTouchIndex
        {
            get
            {
                for (int i = 0; i < _nTouchPointsSupported; i++)
                    if (!_activeTouchpoints.Values.Contains(i))
                        return i;

                return -1;
            }
        }

        #region Android Callbacks

        internal void OnViewTouchStart(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchStart {id}");
            if (_activeTouchpoints.ContainsKey(id))
                throw new InvalidOperationException(
                    $"Android Touch id {id} is already tracked. Cannot track another touchpoint using this id.");

            var inx = NextFreeTouchIndex;
            if (inx < 0)
                return;

            _activeTouchpoints[id] = inx;
            ButtonValueChanged?.Invoke(this,
                new ButtonValueChangedArgs
                {
                    Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc,
                    Pressed = true
                });
            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc,
                    Value = x
                });
            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc,
                    Value = y
                });
        }

        internal void OnViewTouchMove(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchMove {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc,
                    Value = x
                });
            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc,
                    Value = y
                });
        }

        internal void OnViewTouchEnd(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchEnd {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc,
                    Value = x
                });
            AxisValueChanged?.Invoke(this,
                new AxisValueChangedArgs
                {
                    Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc,
                    Value = y
                });
            ButtonValueChanged?.Invoke(this,
                new ButtonValueChangedArgs
                {
                    Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc,
                    Pressed = false
                });
            _activeTouchpoints.Remove(id);
        }

        internal void OnViewTouchCancel(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchCancel {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;
            ButtonValueChanged?.Invoke(this,
                new ButtonValueChangedArgs
                {
                    Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc,
                    Pressed = false
                });
            _activeTouchpoints.Remove(id);
        }

        #endregion Android Callbacks

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchDeviceImp" /> class.
        /// </summary>
        /// <param name="view">The game window to hook on to revive
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx">WM_POINTER</a> messages.</param>
        public TouchDeviceImp(View view)
        {
            _view = view;
            ConnectViewEvents();
            _tpAxisDescs = new Dictionary<int, AxisImpDescription>(_nTouchPointsSupported * 2 + 5);
            _activeTouchpoints = new Dictionary<int, int>(_nTouchPointsSupported);

            _tpAxisDescs[(int)TouchAxes.ActiveTouchpoints] = new AxisImpDescription
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
            };
            _tpAxisDescs[(int)TouchAxes.MinX] = new AxisImpDescription
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
            };
            _tpAxisDescs[(int)TouchAxes.MaxX] = new AxisImpDescription
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
            };
            _tpAxisDescs[(int)TouchAxes.MinY] = new AxisImpDescription
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
            };
            _tpAxisDescs[(int)TouchAxes.MaxY] = new AxisImpDescription
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
        /// Short description string for this device to be used in dialogs.
        /// </summary>
        public string Desc => "Android View standard Touch device.";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// Occurs on value changes of axes exhibited by this device.
        /// Only applies for axes where the <see cref="F:Fusee.Engine.Common.AxisImpDescription.PollAxis" /> is set to false.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>A touchpoint's contact state is communicated by a button.</summary>
        /// <see cref="F:Fusee.Engine.Common.ButtonImpDescription.PollButton" />
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// Returns <see cref="DeviceCategory.Touch"/>, just because it's a touch device :-).
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Touch;

        /// <summary>
        /// Returns the number of axes. Up to five touchpoints (with two axes (X and Y) per Touchpoint plus
        /// one axis carrying the number of currently touched touchpoints plus four axes describing the minimum and
        /// maximum X and Y values.
        /// </summary>
        /// <value>
        /// The axes count.
        /// </value>
        public int AxesCount => _nTouchPointsSupported * 2 + 5;

        /// <summary>
        /// Returns description information for all axes.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc => _tpAxisDescs.Values;

        /// <summary>
        /// Retrieves values for the number of currently active touchpoints and the touches min and max values. The touchpoint X and Y axes themselves are event-based axes.
        /// Do not query them here.
        /// </summary>
        /// <param name="iAxisId">The axis to retrieve information for.</param>
        /// <returns>The value at the given axis.</returns>
        public float GetAxis(int iAxisId)
        {
            switch (iAxisId)
            {
                case (int)TouchAxes.ActiveTouchpoints:
                    return _activeTouchpoints.Count;

                case (int)TouchAxes.MinX:
                    return 0;

                case (int)TouchAxes.MaxX:
                    return GetWindowWidth();

                case (int)TouchAxes.MinY:
                    return 0;

                case (int)TouchAxes.MaxY:
                    return GetWindowHeight();
            }
            throw new InvalidOperationException(
                $"Unknown axis {iAxisId}.  Probably an event based axis or unsupported by this device.");
        }

        /// <summary>
        /// Retrieves the button count. One button for each of the up to five supported touchpoints signaling that the touchpoint currently has contact.
        /// </summary>
        /// <value>
        /// The button count.
        /// </value>
        public int ButtonCount => _nTouchPointsSupported;

        /// <summary>
        /// Retrieve a description for each button.
        /// </summary>
        /// <value>
        /// The button imp description.
        /// </value>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc => _tpButtonDescs.Values;

        /// <summary>
        /// Gets the button state. This device's buttons signal that a finger (nose, elbow, knee...) currently has contact with the scree surface.
        /// </summary>
        /// <param name="iButtonId">The button identifier.</param>
        /// <returns>true if the button is hit, else false</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException(
                $"Unknown button id {iButtonId}. This device supports no pollable buttons at all.");
        }
    }

    /// <summary>
    /// TODO: Implement this!!! Keyboard device implementation for the Android platforms.
    /// </summary>
    public class KeyboardDeviceImp : IInputDeviceImp
    {
        private AndroidGameView _view;
        private Keymapper _keymapper;

        /// <summary>
        /// Should be called by the driver only.
        /// </summary>
        /// <param name="View"></param>
        internal KeyboardDeviceImp(View view)
        {
            _view = (AndroidGameView)view;
            _keymapper = new Keymapper();
            /*
            _View.Keyboard.KeyDown += OnGameWinKeyDown;
            _View.Keyboard.KeyUp += OnGameWinKeyUp;
            */
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
                foreach (var k1 in _keymapper.OrderBy(k => k.Value.Id))
                    yield return new ButtonImpDescription { ButtonDesc = k1.Value, PollButton = false };
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

        /* TODO - find something working on Android
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
        */

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
        /// event to revive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException($"Button {iButtonId} does not exist or is not pollable. Listen to the ButtonValueChanged event to receive keyboard notifications from this device.");
        }
    }

    /// <summary>
    /// Mouse input device implementation for Desktop an Android platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private AndroidGameView _view;
        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;

        /// <summary>
        /// Creates a new mouse input device instance using an existing <see cref="View"/>.
        /// </summary>
        /// <param name="View">The game window providing mouse input.</param>
        public MouseDeviceImp(View view)
        {
            _view = (AndroidGameView)view;
            /* TODO
            _View.Mouse.ButtonDown += OnGameWinMouseDown;
            _View.Mouse.ButtonUp += OnGameWinMouseUp;
            */

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
        public string Desc => "Standard Android Mouse device.";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// No event-based axes are exposed by this device. Use <see cref="GetAxis"/> to acquire mouse axis information.
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
                    return 0;

                case (int)MouseAxes.Y:
                    return 0;

                case (int)MouseAxes.Wheel:
                    return 0;

                case (int)MouseAxes.MinX:
                    return 0;

                case (int)MouseAxes.MaxX:
                    return _view.Width;

                case (int)MouseAxes.MinY:
                    return 0;

                case (int)MouseAxes.MaxY:
                    return _view.Height;
            }
            throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis.");
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

        /* TODO: find something appropriate on Android
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
        }*/
    }
}