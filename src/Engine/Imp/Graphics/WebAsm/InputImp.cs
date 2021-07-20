using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    public static class EventHandler
    {

        public static EventHandler<int> OnKeyDown;
        public static EventHandler<int> OnKeyUp;

        public static EventHandler<int> OnMouseDown;
        public static EventHandler<int> OnMouseUp;
        public static EventHandler<float2> OnMouseMove;
        public static EventHandler<float> OnMouseWheel;

        [JSInvokable("OnKeyDown")]
        public static void KeyDown(int btn)
        {
            OnKeyDown?.Invoke(null, btn);
        }

        [JSInvokable("OnKeyUp")]
        public static void KeyUp(int btn)
        {
            OnKeyUp?.Invoke(null, btn);
        }

        [JSInvokable("OnMouseDown")]
        public static void MouseDown(int btn)
        {
            OnMouseDown?.Invoke(null, btn);
        }

        [JSInvokable("OnMouseUp")]
        public static void MouseUp(int btn)
        {
            OnMouseUp?.Invoke(null, btn);
        }

        [JSInvokable("OnMouseMove")]
        public static void MouseMove(float x, float y)
        {
            OnMouseMove?.Invoke(null, new float2(x, y));
        }

        [JSInvokable("OnMouseWheel")]
        public static void Wheel(float delta)
        {
            OnMouseWheel?.Invoke(null, delta);
        }

        // Deserialzation of EventArgs seems to be broken

        [JSInvokable("OnTouchStart")]
        public static void TouchStart(TouchEventArgs args)
        {
            var dbgBreak = args;
        }


        [JSInvokable("OnTouchEnd")]
        public static void TouchEnd(TouchEventArgs delta)
        {

        }


        [JSInvokable("OnTouchCancel")]
        public static void TouchCancel(TouchEventArgs delta)
        {

        }


        [JSInvokable("OnTouchMove")]
        public static void TouchMove(TouchEventArgs delta)
        {
        }
    }

    /// <summary>
    /// Input driver implementation for keyboard and mouse, as well as touch input in the browser.
    /// </summary>
    public class RenderCanvasInputDriverImp : IInputDriverImp
    {
        /// <summary>
        /// Constructor. Use this in platform specific application projects.
        /// </summary>
        /// <param name="renderCanvas">The render canvas to provide mouse and keyboard input for.</param>
        /// <param name="runtime"></param>
        public RenderCanvasInputDriverImp(IRenderCanvasImp renderCanvas, IJSRuntime runtime)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", nameof(renderCanvas));

            _canvas = ((RenderCanvasImp)renderCanvas)._canvas;
            if (_canvas == null)
                throw new ArgumentNullException(nameof(_canvas));

            this.runtime = runtime;

            _keyboard = new KeyboardDeviceImp(_canvas, runtime);
            _mouse = new MouseDeviceImp(_canvas, runtime);
            //_touch = new TouchDeviceImp(_canvas, runtime);
            //_gamePad = new GamePadDeviceImp(_window, runtime);
        }

        // The WebGL canvas. Will be set in the c# constructor
        internal IJSObjectReference _canvas;
        internal IJSObjectReference _window;
        internal IJSRuntime runtime;
        private readonly KeyboardDeviceImp _keyboard;
        private readonly MouseDeviceImp _mouse;
        private readonly TouchDeviceImp _touch;
        private readonly GamePadDeviceImp _gamePad;

        /// <summary>
        /// Devices supported by this driver: One mouse and one keyboard.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _mouse;
                yield return _keyboard;
                //yield return _touch;
                //yield return _gamePad;
            }
        }

        /// <summary>
        /// Returns a human readable description of this driver.
        /// </summary>
        public string DriverDesc => "WebAsm Mouse, Keyboard, Gamepad and Touch input driver";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string DriverId => GetType().FullName;

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
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Part of the dispose pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    /// <summary>
    /// Gamepad input device implementation for the WebAsm platform.
    /// </summary>
    public class GamePadDeviceImp : IInputDeviceImp
    {
        private ButtonImpDescription _btnADesc, _btnXDesc, _btnYDesc, _btnBDesc, _btnStartDesc, _btnSelectDesc, _dpadUpDesc, _dpadDownDesc, _dpadLeftDesc, _dpadRightDesc, _btnLeftDesc, _btnRightDesc, _btnL3Desc, _btnR3Desc;
        private readonly int DeviceID;
        private IJSRuntime runtime;

        internal GamePadDeviceImp(IJSObjectReference window, IJSRuntime runtime, int deviceID = 1)
        {
            DeviceID = deviceID;
            this.runtime = runtime;
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
                    Id = 2
                },
                PollButton = true
            };
            _btnYDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Y",
                    Id = 3
                },
                PollButton = true
            };
            _btnBDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP B",
                    Id = 1
                },
                PollButton = true
            };
            _btnStartDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Start",
                    Id = 9
                },
                PollButton = true
            };
            _btnSelectDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Back",
                    Id = 8
                },
                PollButton = true
            };
            _btnLeftDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP left button",
                    Id = 4
                },
                PollButton = true
            };
            _btnRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP right button",
                    Id = 5
                },
                PollButton = true
            };
            _btnL3Desc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP L3 button",
                    Id = 10
                },
                PollButton = true
            };
            _btnR3Desc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP R3 button",
                    Id = 11
                },
                PollButton = true
            };
            _dpadUpDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad up",
                    Id = 12
                },
                PollButton = true
            };
            _dpadDownDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Down",
                    Id = 13
                },
                PollButton = true
            };
            _dpadLeftDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Left",
                    Id = 14
                },
                PollButton = true
            };
            _dpadRightDesc = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "GP Dpad Right",
                    Id = 15
                },
                PollButton = true
            };
        }

        /// <summary>
        /// Returns the ID of the current controller
        /// </summary>
        public string Id
        {
            get
            {
                var Index = "0";

                using (var navigator = ((IJSInProcessRuntime)runtime).GetGlobalObject<IJSInProcessObjectReference>("navigator"))
                {
                    var Gamepads = navigator.Invoke<IJSInProcessObjectReference[]>("getGamepads");
                    Diagnostics.Debug($"Trying to connect to {Gamepads.Length} gamepads:");

                    for (var i = 0; i < Gamepads.Length; i++)
                    {
                        using var Gamepad = Gamepads.GetObjectProperty<IJSInProcessObjectReference>(i.ToString());
                        //Checks if the connected gamepads are actual gamepads or just dummy connections.
                        if (Gamepad == null)
                            Diagnostics.Debug($"Gamepad {i} can not be accessed.");
                        if (Gamepad != null)
                        {
                            var id = Gamepad.GetObjectProperty<string>("id");
                            Diagnostics.Debug($"Gamepad {i}: ID={id}");
                        }
                    }
                    using (var Gamepad = Gamepads.GetObjectProperty<IJSInProcessObjectReference>(DeviceID.ToString()))
                    {
                        if (Gamepad != null)
                        {
                            var _index = Gamepad.GetObjectProperty<string>("id");
                            {
                                Index = _index;
                            }
                        }
                    }
                }
                return Index;
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Desc => "WebAsm XBox-Gamepad input implementation.";

        /// <summary>
        /// Returns Type of input device.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.GameController;

        /// <summary>
        /// How many axes do we have for this device?
        /// </summary>
        public int AxesCount => 4;

        /// <summary>
        /// The axis description implementation
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
            }
        }

        /// <summary>
        /// How many buttons do we have for this device
        /// </summary>
        public int ButtonCount => 14;

        /// <summary>
        /// The button description implementation
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

        /// <summary>
        /// Called when the axis value changes
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Called when a button value changes
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        /// <summary>
        /// All Axes on this Device are polled.
        /// </summary>
        /// <param name="iAxisId">The Axis to be polled.</param>
        /// <returns>A float value between -1 and 1 determining the offset of the axis.</returns>
        public float GetAxis(int iAxisId)
        {
            IJSInProcessObjectReference _GamePad;
            IJSInProcessObjectReference Axes;
            using (var navigator = runtime.GetGlobalObject<IJSInProcessObjectReference>("navigator"))
            {
                using var Gamepads = navigator.Invoke<IJSInProcessObjectReference>("getGamepads");
                using (_GamePad = Gamepads.GetObjectProperty<IJSInProcessObjectReference>(DeviceID.ToString()))
                {
                    if (_GamePad != null)
                    {
                        using (Axes = _GamePad.GetObjectProperty<IJSInProcessObjectReference>("axes"))
                        {
                            return iAxisId switch
                            {
                                0 => Axes.GetObjectProperty<float>("[0]"),
                                1 => Axes.GetObjectProperty<float>("[1]"),
                                2 => Axes.GetObjectProperty<float>("[2]"),
                                3 => Axes.GetObjectProperty<float>("[3]"),
                                _ => throw new InvalidOperationException($"Unsupported axis {iAxisId}."),
                            };
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// All buttons on this device are polled.
        /// </summary>
        /// <param name="iButtonId">The button to be polled.</param>
        /// <returns>A Boolean value determining whether the button is pressed or not.</returns>
        public bool GetButton(int iButtonId)
        {
            IJSInProcessObjectReference _GamePad;
            IJSInProcessObjectReference Buttons;
            using (var navigator = runtime.GetGlobalObject<IJSInProcessObjectReference>("navigator"))
            using (var Gamepads = navigator.Invoke<IJSInProcessObjectReference>("getGamepads"))
            using (_GamePad = Gamepads.GetObjectProperty<IJSInProcessObjectReference>(DeviceID.ToString()))
            {
                if (_GamePad != null)
                {
                    using (Buttons = _GamePad.GetObjectProperty<IJSInProcessObjectReference>("buttons"))
                    {
                        return iButtonId switch
                        {
                            0 => (Buttons.GetObjectProperty<int>("[0]")) != 0,
                            1 => (Buttons.GetObjectProperty<int>("[1]")) != 0,
                            2 => (Buttons.GetObjectProperty<int>("[2]")) != 0,
                            3 => (Buttons.GetObjectProperty<int>("[3]")) != 0,
                            4 => (Buttons.GetObjectProperty<int>("[4]")) != 0,
                            5 => (Buttons.GetObjectProperty<int>("[5]")) != 0,
                            6 => (Buttons.GetObjectProperty<int>("[6]")) != 0,
                            7 => (Buttons.GetObjectProperty<int>("[7]")) != 0,
                            8 => (Buttons.GetObjectProperty<int>("[8]")) != 0,
                            9 => (Buttons.GetObjectProperty<int>("[9]")) != 0,
                            10 => (Buttons.GetObjectProperty<int>("[10]")) != 0,
                            11 => (Buttons.GetObjectProperty<int>("[11]")) != 0,
                            12 => (Buttons.GetObjectProperty<int>("[12]")) != 0,
                            13 => (Buttons.GetObjectProperty<int>("[13]")) != 0,
                            _ => throw new InvalidOperationException($"Unsupported button {iButtonId}."),
                        };
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Keyboard input device implementation for the WebAsm platform.
    /// </summary>
    public class KeyboardDeviceImp : IInputDeviceImp
    {
        private readonly Dictionary<int, ButtonDescription> _keyDescriptions;

        #region JS Connectors

        private void ConnectCanvasEvents()
        {
            EventHandler.OnKeyDown += (sender, e) => OnCanvasKeyDown(e);
            EventHandler.OnKeyUp += (sender, e) => OnCanvasKeyUp(e);
        }
        #endregion

        // JavaScript callback code
        private void OnCanvasKeyDown(int key)
        {
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
            {
                Button = _keyDescriptions[key],
                Pressed = true
            });
        }

        private void OnCanvasKeyUp(int key)
        {
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
            {
                Button = _keyDescriptions[key],
                Pressed = false
            });
        }

        /// <summary>
        /// Should be called by the driver only.
        /// </summary>
        /// <param name="">The JavaScript canvas.</param>
        /// <param name="runtime"></param>
        internal KeyboardDeviceImp(IJSObjectReference _, IJSRuntime runtime)
        {
            this.runtime = runtime;

            _keyDescriptions = new Dictionary<int, ButtonDescription>();
            var enumName = Enum.GetNames(typeof(KeyCodes)).GetEnumerator();
            var enumValue = Enum.GetValues(typeof(KeyCodes)).GetEnumerator();

            while (enumValue.MoveNext() && enumName.MoveNext())
            {
                _keyDescriptions[(int)enumValue.Current] = new ButtonDescription { Id = (int)enumValue.Current, Name = (string)enumName.Current };
            }

            ConnectCanvasEvents();
        }

        private IJSRuntime runtime;

        /// <summary>
        /// Returns the number of Axes (==0, keyboard does not support any axes).
        /// </summary>
        public int AxesCount => 0;

        /// <summary>
        /// Empty enumeration for keyboard, since <see cref="AxesCount"/> is 0.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc
        {
            get { yield break; }
        }

        /// <summary>
        /// Returns the number of enum values of <see cref="KeyCodes"/>
        /// </summary>
        public int ButtonCount => Enum.GetNames(typeof(KeyCodes)).Length;

        /// <summary>
        /// Returns a description for each keyboard button.
        /// </summary>
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                foreach (var key in _keyDescriptions.Values)
                {
                    yield return new ButtonImpDescription { ButtonDesc = key, PollButton = false };
                }
            }
        }

        /// <summary>
        /// This is a keyboard device, so this property returns <see cref="DeviceCategory.Keyboard"/>.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Keyboard;

        /// <summary>
        /// Human readable description of this device (to be used in dialogs).
        /// </summary>
        public string Desc => "Standard WebAsm Keyboard implementation.";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including name space).
        /// </summary>
        public string Id => GetType().FullName;

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
        /// This device does not support any axes at all. Always throws.
        /// </summary>
        /// <param name="iAxisId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public float GetAxis(int iAxisId)
        {
            throw new InvalidOperationException($"Unsupported axis {iAxisId}. This device does not support any axes at all.");
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


    /// <summary>
    /// Mouse input device implementation for the Web platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private float _currentMouseWheel;

        #region JSExternals
        private void ConnectCanvasEvents()
        {
            EventHandler.OnMouseDown += (sender, evt) => OnCanvasMouseDown(evt);
            EventHandler.OnMouseUp += (sender, evt) => OnCanvasMouseUp(evt);
            EventHandler.OnMouseMove += (sender, evt) => OnCanvasMouseMove(evt);
            EventHandler.OnMouseWheel += (sender, evt) => OnCanvasMouseWheel(evt);

            // TODO: Unify WHEEL values among browsers
            // var FU_is_edge = navigator.appVersion.toLowerCase().indexOf('edge') > -1;
            // var FU_is_firefox = !FU_is_edge && navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
            //        // Firefox reports event.deltaY == 3 and deltaMode == 1 (DOM_DELTA_LINE)
            //        // Edge reports event.deltaY == 256 and deltaMode == 0 (DOM_DELTA_PIXEL)
            //        // Chrome reports event.deltaY == 100 and deltaMode == 0 (DOM_DELTA_PIXEL)
            //        var _pixelsPerLine = 33;
            //var _browserFactor = (FU_is_edge) ? (100.0 / 256.0) : 1;

            //addWheelListener(this._canvas, function (event)
            //{
            //    var delta = -3 * _browserFactor * event.deltaY;
            //    if (event.deltaMode == 1)
            //                delta *= _pixelsPerLine;
            //    callbackClosure.OnCanvasMouseWheel.call(callbackClosure, delta);
            //            event.preventDefault();
            //});
            //this._canvas.onmousewheel = function (event) {
            //    callbackClosure.OnCanvasMouseWheel.call(callbackClosure, event.wheelDelta);
            //};
        }

        private float GetWindowWidth()
        {
            var w = _runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            var res = w.GetObjectProperty<int>("innerWidth");
            return res;
        }

        private float GetWindowHeight()
        {
            var w = _runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            var res = w.GetObjectProperty<int>("innerHeight");
            return res;
        }
        #endregion

        #region JavaScript callback code
        internal void OnCanvasMouseDown(int button)
        {
            switch (button)
            {
                case 0:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnLeftDesc.ButtonDesc,
                        Pressed = true
                    });
                    break;
                case 1:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnMiddleDesc.ButtonDesc,
                        Pressed = true
                    });
                    break;
                case 2:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnRightDesc.ButtonDesc,
                        Pressed = true
                    });
                    break;
            }
        }

        internal void OnCanvasMouseUp(int button)
        {
            switch (button)
            {
                case 0:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnLeftDesc.ButtonDesc,
                        Pressed = false
                    });
                    break;
                case 1:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnMiddleDesc.ButtonDesc,
                        Pressed = false
                    });
                    break;
                case 2:
                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs
                    {
                        Button = _btnRightDesc.ButtonDesc,
                        Pressed = false
                    });
                    break;
            }
        }

        internal void OnCanvasMouseMove(float2 mousePos)
        {
            if (AxisValueChanged == null)
                return;
            AxisValueChanged(this, new AxisValueChangedArgs { Axis = _mouseXDesc.AxisDesc, Value = mousePos.x });
            AxisValueChanged(this, new AxisValueChangedArgs { Axis = _mouseYDesc.AxisDesc, Value = mousePos.y });
        }

        internal void OnCanvasMouseWheel(float wheelDelta)
        {
            _currentMouseWheel += wheelDelta * 0.005f;
        }
        #endregion

        private readonly IJSObjectReference _canvas;
        private readonly IJSRuntime _runtime;

        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;
        private AxisImpDescription _mouseXDesc, _mouseYDesc;


        /// <summary>
        /// Initializes a new instance of the <see cref="MouseDeviceImp" /> class.
        /// </summary>
        /// <param name="canvas">The (JavaScript) canvas object.</param>
        public MouseDeviceImp(IJSObjectReference canvas, IJSRuntime runtime)
        {


            _currentMouseWheel = 0;

            _canvas = canvas;
            _runtime = runtime;

            ConnectCanvasEvents();

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
            _mouseXDesc = new AxisImpDescription
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
            _mouseYDesc = new AxisImpDescription
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
                yield return _mouseXDesc;
                yield return _mouseYDesc;
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
        public string Desc => "WebAsm standard Mouse implementation.";

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including name space).
        /// </summary>
        public string Id => GetType().FullName;

        /// <summary>
        /// This exposes mouseX and mouseY as event-based axes, while the mouseWheel is to be polled.
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
                case (int)MouseAxes.X:
                case (int)MouseAxes.Y:
                    throw new InvalidOperationException("MouseAxes.X/Y are event-based axes. Do not call GetAxis!");
                case (int)MouseAxes.Wheel:
                    return _currentMouseWheel;
                case (int)MouseAxes.MinX:
                    return 0;
                case (int)MouseAxes.MaxX:
                    return GetWindowWidth();
                case (int)MouseAxes.MinY:
                    return 0;
                case (int)MouseAxes.MaxY:
                    //return 9999;
                    return GetWindowHeight();
            }
            throw new InvalidOperationException($"Unknown axis {iAxisId}. Cannot get value for unknown axis.");
        }

        /// <summary>
        /// This device does not support to-be-polled-buttons. All mouse buttons are event-driven. Listen to the <see cref="ButtonValueChanged"/>
        /// event to receive keyboard notifications from this device.
        /// </summary>
        /// <param name="iButtonId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public bool GetButton(int iButtonId)
        {
            throw new InvalidOperationException(
                $"Unsupported button {iButtonId}. This device does not support any to-be polled buttons at all.");
        }
    }

    /// <summary>
    /// Touch input device implementation for the Web platforms. This implementation uses the HTML5 touch api
    /// which doesn't seem to provide a value how many simultaneous touchpoints are supported. Thus we
    /// assume 5.
    /// </summary>
    public class TouchDeviceImp : IInputDeviceImp
    {
        private readonly Dictionary<int, AxisImpDescription> _tpAxisDescs;
        private readonly Dictionary<int, ButtonImpDescription> _tpButtonDescs;
        private readonly Dictionary<int, int> _activeTouchpoints;
        private readonly int _nTouchPointsSupported = 5;
        private readonly IJSObjectReference _canvas;

        /// <summary>
        /// Converts the value of a touch point to float.
        /// </summary>
        /// <param name="o">The value of the point to convert.</param>
        /// <returns></returns>
        public static float P2F(object o)
        {
            return o switch
            {
                double d => (float)d,
                int i => i,
                string s => float.Parse(s),
                _ => 0,
            };
        }


        #region JSExternals
        private void ConnectCanvasEvents()
        {
            ((IJSInProcessObjectReference)_canvas).InvokeVoid("addEventListener", new object[] { "touchstart", new Action<IJSInProcessObjectReference>(
                (IJSInProcessObjectReference evt) => {
                    evt.InvokeVoid("preventDefault");
                    var touches = evt.GetObjectProperty<IJSInProcessObjectReference[]>("changedTouches"); var nTouches = touches.Length;
                        for (var i = 0; i < nTouches; i++)
                        {
                            using var touch = touches.GetObjectProperty<IJSInProcessObjectReference>(i.ToString());
                            OnCanvasTouchStart(
                                touch.GetObjectProperty<int>("identifier"),
                                P2F(touch.GetObjectProperty<object>("pageX")),
                                P2F(touch.GetObjectProperty<object>("pageY")));
                        }
                }
            )});

            ((IJSInProcessObjectReference)_canvas).InvokeVoid("addEventListener", new object[] { "touchmove", new Action<IJSInProcessObjectReference>(
                (IJSInProcessObjectReference evt) => {
                    evt.InvokeVoid("preventDefault");
                    var touches = evt.GetObjectProperty<IJSInProcessObjectReference[]>("changedTouches"); var nTouches = touches.Length;
                        for (var i = 0; i < nTouches; i++)
                        {
                            using var touch = touches.GetObjectProperty<IJSInProcessObjectReference>(i.ToString());
                        OnCanvasTouchMove(
                            touch.GetObjectProperty < int >("identifier"),
                            P2F(touch.GetObjectProperty<object>("pageX")),
                            P2F(touch.GetObjectProperty<object>("pageY")));
                        }
                }
            )});

            ((IJSInProcessObjectReference)_canvas).InvokeVoid("addEventListener", new object[] { "touchend", new Action<IJSInProcessObjectReference>(
                (IJSInProcessObjectReference evt) => {
                    evt.InvokeVoid("preventDefault");
                    var touches = evt.GetObjectProperty<IJSInProcessObjectReference[]>("changedTouches"); var nTouches = touches.Length;
                        for (var i = 0; i < nTouches; i++)
                        {
                            using var touch = touches.GetObjectProperty<IJSInProcessObjectReference>(i.ToString());
                        OnCanvasTouchEnd(
                            touch.GetObjectProperty<int>("identifier"),
                            P2F(touch.GetObjectProperty<object>("pageX")),
                            P2F(touch.GetObjectProperty<object>("pageY")));
                        }
                }
            )});

            ((IJSInProcessObjectReference)_canvas).InvokeVoid("addEventListener", new object[] { "touchcancel", new Action<IJSInProcessObjectReference>(
                (IJSInProcessObjectReference evt) => {
                    evt.InvokeVoid("preventDefault");
                    var touches = evt.GetObjectProperty<IJSInProcessObjectReference[]>("changedTouches"); var nTouches = touches.Length;
                        for (var i = 0; i < nTouches; i++)
                        {
                            using var touch = touches.GetObjectProperty<IJSInProcessObjectReference>(i.ToString());
                        OnCanvasTouchCancel(
                            touch.GetObjectProperty<int>("identifier"),
                            P2F(touch.GetObjectProperty<object>("pageX")),
                            P2F(touch.GetObjectProperty<object>("pageY")));
                        }
                }
            )});
        }


        private float GetWindowWidth()
        {
            using var w = _runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            return w.GetObjectProperty<int>("innerWidth");
        }

        private float GetWindowHeight()
        {
            using var w = _runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            return w.GetObjectProperty<int>("innerHeight");
        }
        #endregion

        private int NextFreeTouchIndex
        {
            get
            {
                for (var i = 0; i < _nTouchPointsSupported; i++)
                {
                    if (!_activeTouchpoints.Values.Contains(i))
                        return i;
                }

                return -1;
            }
        }

        #region Called back from JavaScript
        internal void OnCanvasTouchStart(int id, float x, float y)
        {
            if (_activeTouchpoints.ContainsKey(id))
                throw new InvalidOperationException($"HTML Touch id {id} is already tracked. Cannot track another touchpoint using this id.");

            var inx = NextFreeTouchIndex;
            if (inx < 0)
                return;

            _activeTouchpoints[id] = inx;
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = true });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + (2 * inx)].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + (2 * inx)].AxisDesc, Value = y });
        }

        internal void OnCanvasTouchMove(int id, float x, float y)
        {
            if (!_activeTouchpoints.TryGetValue(id, out var inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + (2 * inx)].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + (2 * inx)].AxisDesc, Value = y });
        }
        internal void OnCanvasTouchEnd(int id, float x, float y)
        {
            if (!_activeTouchpoints.TryGetValue(id, out var inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + (2 * inx)].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + (2 * inx)].AxisDesc, Value = y });
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }

        internal void OnCanvasTouchCancel(int id, float x, float y)
        {
            if (!_activeTouchpoints.TryGetValue(id, out var inx))
                return;
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }
        #endregion

        private readonly IJSRuntime _runtime;

        /// <summary>
        /// Implements a touch device for windows.
        /// </summary>
        /// <param name="canvas"></param>
        public TouchDeviceImp(IJSObjectReference canvas, IJSRuntime runtime)
        {
            _canvas = canvas;
            _runtime = runtime;
            //ConnectCanvasEvents();
            _tpAxisDescs = new Dictionary<int, AxisImpDescription>((_nTouchPointsSupported * 2) + 5);
            _activeTouchpoints = new Dictionary<int, int>(_nTouchPointsSupported);

            _tpAxisDescs[(int)TouchAxes.ActiveTouchpoints] = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Active Touchpoints",
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
                var id = (2 * i) + (int)TouchAxes.Touchpoint_0_X;
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
                var id = i + (int)TouchPoints.Touchpoint_0;
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
        public string Desc => "Web browser standard Touch implementation.";

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

        /// <summary>A touchpoints's contact state is communicated by a button.</summary>
        /// <see cref="F:Fusee.Engine.Common.ButtonImpDescription.PollButton" />
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;


        /// <summary>
        /// Returns <see cref="DeviceCategory.Touch"/>, just because it's a touch device.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.Touch;
        /// <summary>
        /// Returns the number of axes. Up to five touchpoints (with two axes (X and Y) per touchpoint plus
        /// one axis carrying the number of currently touched touchpoints plus four axes describing the minimum and
        /// maximum X and Y values.
        /// </summary>
        /// <value>
        /// The axes count.
        /// </value>
        public int AxesCount => (_nTouchPointsSupported * 2) + 5;

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
            return iAxisId switch
            {
                (int)TouchAxes.ActiveTouchpoints => _activeTouchpoints.Count,
                (int)TouchAxes.MinX => 0,
                (int)TouchAxes.MaxX => GetWindowWidth(),
                (int)TouchAxes.MinY => 0,
                (int)TouchAxes.MaxY => GetWindowHeight(),
                _ => throw new InvalidOperationException($"Unknown axis {iAxisId}.  Probably an event based axis or unsupported by this device."),
            };
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
            throw new InvalidOperationException($"Unknown button id {iButtonId}. This device supports no pollable buttons at all.");
        }
    }
}