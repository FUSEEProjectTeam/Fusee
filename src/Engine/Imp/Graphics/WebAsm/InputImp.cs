﻿using System;
using Fusee.Engine.Common;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using WebAssembly;
using System.Collections;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Input driver implementation for keyboard and mouse, as well as touch input in the browser.
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

            _canvas = ((RenderCanvasImp)renderCanvas)._canvas;
            if (_canvas == null)
                throw new ArgumentNullException(nameof(_canvas));

            _keyboard = new KeyboardDeviceImp(_canvas);
            _mouse = new MouseDeviceImp(_canvas);
            _touch = new TouchDeviceImp(_canvas);
        }

        // The webgl canvas. Will be set in the c# constructor
        internal JSObject _canvas;
        private KeyboardDeviceImp _keyboard;
        private MouseDeviceImp _mouse;
        private TouchDeviceImp _touch;

        /// <summary>
        /// Devices supported by this driver: One mouse and one keyboard.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _mouse;
                yield return _keyboard;
                yield return _touch;
            }
        }

        /// <summary>
        /// Returns a human readable description of this driver.
        /// </summary>
        public string DriverDesc => "WebAsm Mouse, Keyboard and Touch input driver";

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
    /// Keyboard input device implementation for the WebAsm platform.
    /// </summary>
    public class KeyboardDeviceImp : IInputDeviceImp
    {
        private Dictionary<int, ButtonDescription> _keyDescriptions;
        private JSObject _canvas;

        #region JS Connectors

        // [JSExternal]
        private void ConnectCanvasEvents()
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            {
                document.SetObjectProperty("onkeydown", new Action<JSObject>(evt => OnCanvasKeyDown((int)evt.GetObjectProperty("keyCode"))));
                document.SetObjectProperty("onkeyup", new Action<JSObject>(evt => OnCanvasKeyUp((int)evt.GetObjectProperty("keyCode"))));
            }
        }
        #endregion

        // Javascript callback code
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
        /// <param name="canvas">The javascript canvas.</param>
        internal KeyboardDeviceImp(JSObject canvas)
        {
            _keyDescriptions = new Dictionary<int, ButtonDescription>();
            IEnumerator enumName = Enum.GetNames(typeof(KeyCodes)).GetEnumerator();
            IEnumerator enumValue = Enum.GetValues(typeof(KeyCodes)).GetEnumerator();
            while (enumValue.MoveNext() & enumName.MoveNext())
            {
                _keyDescriptions[(int)enumValue.Current] = new ButtonDescription { Id = (int)enumValue.Current, Name = (string)enumName.Current };
            }

            _canvas = canvas;
            ConnectCanvasEvents();
        }

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
                return "Standard WebAsm Keyboard implementation.";
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
        /// This device does not support any axes at all. Always throws.
        /// </summary>
        /// <param name="iAxisId">No matter what you specify here, you'll evoke an exception.</param>
        /// <returns>No return, always throws.</returns>
        public float GetAxis(int iAxisId)
        {
            throw new InvalidOperationException($"Unsopported axis {iAxisId}. This device does not support any axes at all.");
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
    /// Mouse input device implementation for the Web platforms.
    /// </summary>
    public class MouseDeviceImp : IInputDeviceImp
    {
        private float _currentMouseWheel;

        #region JSExternals
        // [JSExternal]
        private void ConnectCanvasEvents()
        {
            _canvas.SetObjectProperty("onmousedown", new Action<JSObject>(evt => OnCanvasMouseDown((int)evt.GetObjectProperty("button"))));
            _canvas.SetObjectProperty("onmouseup", new Action<JSObject>(evt => OnCanvasMouseUp((int)evt.GetObjectProperty("button"))));
            _canvas.SetObjectProperty("onmousemove", new Action<JSObject>(evt => OnCanvasMouseMove(new float2((float)(int)evt.GetObjectProperty("offsetX"), (float)(int)evt.GetObjectProperty("offsetY")))));
            _canvas.SetObjectProperty("onwheel", new Action<JSObject>(evt => { evt.Invoke("preventDefault"); OnCanvasMouseWheel((float)(int)evt.GetObjectProperty("deltaY")); }));

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

        // [JSExternal]
        private float GetWindowWidth()
        {
            return (float)(int)_canvas.GetObjectProperty("width");
        }
        // [JSExternal]
        private float GetWindowHeight()
        {
            return (float)(int)_canvas.GetObjectProperty("height");
        }
        #endregion

        #region Javascript callback code
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
            MouseButtons mb;
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

        private JSObject _canvas;
        private ButtonImpDescription _btnLeftDesc, _btnRightDesc, _btnMiddleDesc;
        private AxisImpDescription _mouseXDesc, _mouseYDesc;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseDeviceImp" /> class.
        /// </summary>
        /// <param name="canvas">The (javascript) canvas object.</param>
        public MouseDeviceImp(JSObject canvas)
        {
            _currentMouseWheel = 0;
            _canvas = canvas;
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
        /// full class name (including namespace).
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
                    return GetWindowHeight();
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
                $"Unsopported button {iButtonId}. This device does not support any to-be polled buttons at all.");
        }
    }

    /// <summary>
    /// Touch input device implementation for the Web platforms. This implementation uses the HTML5 touch api
    /// which doesn't seem to provide a value how many simultaneous touchpoints are supported. Thus we 
    /// assume 5.
    /// </summary>
    public class TouchDeviceImp : IInputDeviceImp
    {
        private Dictionary<int, AxisImpDescription> _tpAxisDescs;
        private Dictionary<int, ButtonImpDescription> _tpButtonDescs;
        private Dictionary<int, int> _activeTouchpoints;
        private int _nTouchPointsSupported = 5;
        private JSObject _canvas;

        /// <summary>
        /// Converts the value of a touch point to float.
        /// </summary>
        /// <param name="o">The value of the point to convert.</param>
        /// <returns></returns>
        public static float P2F(object o)
        {
            switch (o)
            {
                case double d:
                    return (float)d;
                case int i:
                    return (float)i;
                case string s:
                    return float.Parse(s);
            }
            return 0;
        }


        #region JSExternals
        // [JSExternal]
        private void ConnectCanvasEvents()
        {
            _canvas.Invoke("addEventListener", new object[] { "touchstart", new Action<JSObject>(
                delegate(JSObject evt) 
                {
                    evt.Invoke("preventDefault");
                    using(JSObject touches = (JSObject) evt.GetObjectProperty("changedTouches"))
                    {
                        int nTouches = touches.Length;
                        for (int i = 0; i < nTouches; i++)
                        {
                            using(var touch = (JSObject) touches.GetObjectProperty(i.ToString()))
                            {
                                OnCanvasTouchStart((int)touch.GetObjectProperty("identifier"), P2F(touch.GetObjectProperty("pageX")), P2F(touch.GetObjectProperty("pageY")));
                            }
                        }
                    }
                }
            )});

            _canvas.Invoke("addEventListener", new object[] { "touchmove", new Action<JSObject>(
                delegate(JSObject evt)
                {
                    evt.Invoke("preventDefault");
                    using(JSObject touches = (JSObject) evt.GetObjectProperty("changedTouches"))
                    {
                        int nTouches = touches.Length;
                        for (int i = 0; i < nTouches; i++)
                        {
                            using(var touch = (JSObject) touches.GetObjectProperty(i.ToString()))
                            {
                                OnCanvasTouchMove((int)touch.GetObjectProperty("identifier"), P2F(touch.GetObjectProperty("pageX")), P2F(touch.GetObjectProperty("pageY")));
                            }
                        }
                    }
                }
            )});

            _canvas.Invoke("addEventListener", new object[] { "touchend", new Action<JSObject>(
                delegate(JSObject evt)
                {
                    evt.Invoke("preventDefault");
                    using (JSObject touches = (JSObject) evt.GetObjectProperty("changedTouches"))
                    {
                        int nTouches = touches.Length;
                        for (int i = 0; i < nTouches; i++)
                        {
                            using(var touch = (JSObject) touches.GetObjectProperty(i.ToString()))
                            {
                                OnCanvasTouchEnd((int)touch.GetObjectProperty("identifier"), P2F(touch.GetObjectProperty("pageX")), P2F(touch.GetObjectProperty("pageY")));
                            }
                        }
                    }
                }
            )});

            _canvas.Invoke("addEventListener", new object[] { "touchcancel", new Action<JSObject>(
                delegate(JSObject evt)
                {
                    evt.Invoke("preventDefault");
                    using (JSObject touches = (JSObject) evt.GetObjectProperty("changedTouches"))
                    {
                        int nTouches = touches.Length;
                        for (int i = 0; i < nTouches; i++)
                        {
                            using(var touch = (JSObject) touches.GetObjectProperty(i.ToString()))
                            {
                                OnCanvasTouchCancel((int)touch.GetObjectProperty("identifier"), P2F(touch.GetObjectProperty("pageX")), P2F(touch.GetObjectProperty("pageY")));
                            }
                        }
                    }
                }
            )});
        }

        // [JSExternal]
        private float GetWindowWidth()
        {
            return (float)(int)_canvas.GetObjectProperty("width");
        }
        // [JSExternal]
        private float GetWindowHeight()
        {
            return (float)(int)_canvas.GetObjectProperty("height");
        }
        #endregion

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
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
        }

        internal void OnCanvasTouchMove(int id, float x, float y)
        {
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
        }
        internal void OnCanvasTouchEnd(int id, float x, float y)
        {
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }
        internal void OnCanvasTouchCancel(int id, float x, float y)
        {
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }
        #endregion

        /// <summary>
        /// Implements a touch device for windows.
        /// </summary>
        /// <param name="canvas"></param>
        public TouchDeviceImp(JSObject canvas)
        {
            _canvas = canvas;
            ConnectCanvasEvents();
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
        /// Returns <see cref="DeviceCategory.Touch"/>, just because it's a touch tevice :-).
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
            throw new InvalidOperationException($"Unknown axis {iAxisId}.  Probably an event based axis or unsupported by this device.");
        }

        /// <summary>
        /// Retrieves the button count. One button for each of the up to five supported touchpoints signalling that the touchpoint currently has contact.
        /// </summary>
        /// <value>
        /// The button count.
        /// </value>
        public int ButtonCount => _nTouchPointsSupported;

        /// <summary>
        /// Retrieve a description for each button.
        /// </summary>
        /// <value>
        /// The button imp desc.
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