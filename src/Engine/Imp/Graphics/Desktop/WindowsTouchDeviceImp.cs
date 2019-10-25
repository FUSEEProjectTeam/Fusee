using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusee.Engine.Common;
using OpenTK;

namespace Fusee.Engine.Imp.Graphics.Desktop
{

    /// <summary>
    /// Input driver implementation supporting Windows 8 touch input as described in
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx
    /// </summary>
    public class WindowsTouchInputDriverImp : IInputDriverImp
    {
        GameWindow _gameWindow;
        WindowsTouchInputDeviceImp _touch;
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsTouchInputDriverImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas. Internally this must be a Windows canvas with a valid window handle.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="System.ArgumentException">RenderCanvas must be of type <see cref="RenderCanvasImp"/></exception>
        public WindowsTouchInputDriverImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException($"renderCanvas must be of type {typeof(RenderCanvasImp).FullName}", nameof(renderCanvas));

            _gameWindow = ((RenderCanvasImp)renderCanvas)._gameWindow;
            if (_gameWindow == null)
                throw new ArgumentNullException(nameof(_gameWindow));


            _touch = new WindowsTouchInputDeviceImp(_gameWindow);
        }

        /// <summary>
        /// Retrieves a list of devices supported by this input driver.
        /// </summary>
        /// <value>
        /// The list of devices.
        /// </value>
        /// <remarks>
        /// The devices yielded represent the current status. At any time other devices can connect or disconnect.
        /// Listen to the <see cref="E:Fusee.Engine.Common.IInputDriverImp.NewDeviceConnected" /> and <see cref="E:Fusee.Engine.Common.IInputDriverImp.DeviceDisconnected" /> events to get
        /// informed about new or vanishing devices. Drivers may implement "static" access to devices such that
        /// devices are connected at driver instantiation and never disconnected (in this case <see cref="E:Fusee.Engine.Common.IInputDriverImp.NewDeviceConnected" />
        /// and <see cref="E:Fusee.Engine.Common.IInputDriverImp.DeviceDisconnected" /> are never fired).
        /// </remarks>
        public IEnumerable<IInputDeviceImp> Devices { get { yield return _touch; } }

        /// <summary>
        /// Gets the unique driver identifier.
        /// </summary>
        /// <value>
        /// The driver identifier.
        /// </value>
        public string DriverId => GetType().FullName;

        /// <summary>
        /// Gets the driver description string.
        /// </summary>
        /// <value>
        /// A human-readable string describing the driver.
        /// </value>
        public string DriverDesc => "Driver providing a touch device implementation for Windows 8 (and up) touch input.";

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
    /// Touch input device implementation for the Windows platform. This implementation directly
    /// sniffes at the render window's message pump (identified by the <see cref="GameWindow"/> parameter passed
    /// to the constructor) to receive 
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx">WM_POINTER</a> messages.
    /// </summary>
    public class WindowsTouchInputDeviceImp : IInputDeviceImp
    {
        private Dictionary<int, AxisImpDescription> _tpAxisDescs;
        private Dictionary<int, ButtonImpDescription> _tpButtonDescs;
        private Dictionary<int, int> _activeTouchpoints;
        private int _nTouchPointsSupported = 5;
        private HandleRef _handle;
        private readonly GameWindow _gameWindow;


        #region Windows handling
        // This helper static method is required because the 32-bit version of user32.dll does not contain this API
        // (on any versions of Windows), so linking the method will fail at run-time. The bridge dispatches the request
        // to the correct function (GetWindowLong in 32-bit mode and GetWindowLongPtr in 64-bit mode)
        private static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        private readonly int GWLP_WNDPROC = -4;

        private enum WinMessage : int
        {
            WM_CLOSE = 0x0010,
            WM_NCPOINTERUP = 0x0243,
            WM_POINTERUPDATE = 0x0245,
            WM_POINTERDOWN = 0x0246,
            WM_POINTERUP = 0x0247,
        }

        // As defined in <winuser.h>
        [Flags]
        private enum PointerMsgFlags : uint
        {
            // ReSharper disable InconsistentNaming
            POINTER_MESSAGE_FLAG_NEW = 0x00000001, // New pointer
            POINTER_MESSAGE_FLAG_INRANGE = 0x00000002, // Pointer has not departed
            POINTER_MESSAGE_FLAG_INCONTACT = 0x00000004, // Pointer is in contact
            POINTER_MESSAGE_FLAG_FIRSTBUTTON = 0x00000010, // Primary action
            POINTER_MESSAGE_FLAG_SECONDBUTTON = 0x00000020, // Secondary action
            POINTER_MESSAGE_FLAG_THIRDBUTTON = 0x00000040, // Third button
            POINTER_MESSAGE_FLAG_FOURTHBUTTON = 0x00000080, // Fourth button
            POINTER_MESSAGE_FLAG_FIFTHBUTTON = 0x00000100, // Fifth button
            POINTER_MESSAGE_FLAG_PRIMARY = 0x00002000, // Pointer is primary
            POINTER_MESSAGE_FLAG_CONFIDENCE = 0x00004000, // Pointer is considered unlikely to be accidental
            POINTER_MESSAGE_FLAG_CANCELED = 0x00008000, // Pointer is departing in an abnormal manner
            // ReSharper enable InconsistentNaming
        }

        private UInt16 LOWORD(UInt32 wParam) => unchecked((UInt16)wParam);
        private UInt16 HIWORD(UInt32 wParam) => unchecked((UInt16)((wParam >> 16) & 0xFFFF));

        private UInt16 GET_X_LPARAM(UInt32 lp) => LOWORD(lp);
        private UInt16 GET_Y_LPARAM(UInt32 lp) => HIWORD(lp);

        private int GET_POINTERID_WPARAM(UInt32 wParam) => LOWORD(wParam);

        private bool IS_POINTER_FLAG_SET_WPARAM(UInt32 wParam, PointerMsgFlags flag) => ((UInt32)HIWORD(wParam) & ((UInt32)flag)) == ((UInt32)flag);
        private bool IS_POINTER_NEW_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_NEW);
        private bool IS_POINTER_INRANGE_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_INRANGE);
        private bool IS_POINTER_INCONTACT_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_INCONTACT);
        private bool IS_POINTER_FIRSTBUTTON_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_FIRSTBUTTON);
        private bool IS_POINTER_SECONDBUTTON_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_SECONDBUTTON);
        private bool IS_POINTER_THIRDBUTTON_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_THIRDBUTTON);
        private bool IS_POINTER_FOURTHBUTTON_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_FOURTHBUTTON);
        private bool IS_POINTER_FIFTHBUTTON_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_FIFTHBUTTON);
        private bool IS_POINTER_PRIMARY_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_PRIMARY);
        private bool HAS_POINTER_CONFIDENCE_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_CONFIDENCE);
        private bool IS_POINTER_CANCELED_WPARAM(UInt32 wParam) => IS_POINTER_FLAG_SET_WPARAM(wParam, PointerMsgFlags.POINTER_MESSAGE_FLAG_CANCELED);


        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        // private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr EnableMouseInPointer(bool fEnable);
        /// <summary>
        /// The touch point.
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        private delegate IntPtr WinProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private WinProc _newWinProc;

        private IntPtr _oldWndProc = IntPtr.Zero;


        private void DisconnectWindowsEvents()
        {
            if (_handle.Handle != IntPtr.Zero)
            {
                SetWindowLongPtr(_handle, GWLP_WNDPROC, _oldWndProc);
            }
        }


        private IntPtr TouchWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            unchecked
            {
                POINT winPoint;
                switch (Msg)
                {
                    case (int)WinMessage.WM_CLOSE: // Seems defunkt.
                        DisconnectWindowsEvents();
                        break;
                    case (int)WinMessage.WM_POINTERUPDATE:
                        winPoint = new POINT(GET_X_LPARAM((uint)lParam), GET_Y_LPARAM((uint)lParam));
                        ScreenToClient(hWnd, ref winPoint);
                        OnWindowsTouchMove(GET_POINTERID_WPARAM((uint)wParam), winPoint.X, winPoint.Y);
                        return IntPtr.Zero;
                    case (int)WinMessage.WM_POINTERUP:
                        winPoint = new POINT(GET_X_LPARAM((uint)lParam), GET_Y_LPARAM((uint)lParam));
                        ScreenToClient(hWnd, ref winPoint);
                        OnWindowsTouchEnd(GET_POINTERID_WPARAM((uint)wParam), winPoint.X, winPoint.Y);
                        return IntPtr.Zero;
                    case (int)WinMessage.WM_POINTERDOWN:
                        winPoint = new POINT(GET_X_LPARAM((uint)lParam), GET_Y_LPARAM((uint)lParam));
                        ScreenToClient(hWnd, ref winPoint);
                        OnWindowsTouchStart(GET_POINTERID_WPARAM((uint)wParam), winPoint.X, winPoint.Y);
                        return IntPtr.Zero;
                    case (int)WinMessage.WM_NCPOINTERUP:
                        winPoint = new POINT(GET_X_LPARAM((uint)lParam), GET_Y_LPARAM((uint)lParam));
                        ScreenToClient(hWnd, ref winPoint);
                        OnWindowsTouchCancel(GET_POINTERID_WPARAM((uint)wParam), winPoint.X, winPoint.Y);
                        return IntPtr.Zero;
                }
            }
            return CallWindowProc(_oldWndProc, hWnd, Msg, wParam, lParam);
        }

        private void ConnectWindowsEvents()
        {
            OperatingSystem os = Environment.OSVersion;

            // See https://msdn.microsoft.com/library/windows/desktop/ms724832.aspx : Apps NOT targetet for a specific windows version (like 8.1 or 10)
            // retrieve Version# 6.2 (resembling Windows 8), which is the version where "Pointer" touch handling is first supported.
            if (os.Platform == PlatformID.Win32NT
                && (os.Version.Major > 6
                     || os.Version.Major == 6 && os.Version.Minor >= 2)
                )
            {
                EnableMouseInPointer(false);
                if (_handle.Handle != IntPtr.Zero)
                {
                    _newWinProc = new WinProc(TouchWindowProc);
                    _oldWndProc = SetWindowLongPtr(_handle, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(_newWinProc));
                }
            }
        }

        private float GetWindowWidth()
        {
            return _gameWindow.Width;
        }

        private float GetWindowHeight()
        {
            return _gameWindow.Height;
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


        #region Windows Callbacks
        internal void OnWindowsTouchStart(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchStart {id}");
            if (_activeTouchpoints.ContainsKey(id))
                throw new InvalidOperationException($"Windows Touch id {id} is already tracked. Cannot track another touchpoint using this id.");

            var inx = NextFreeTouchIndex;
            if (inx < 0)
                return;

            _activeTouchpoints[id] = inx;
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = true });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
        }

        internal void OnWindowsTouchMove(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchMove {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
        }
        internal void OnWindowsTouchEnd(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchEnd {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;

            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_X + 2 * inx].AxisDesc, Value = x });
            AxisValueChanged?.Invoke(this, new AxisValueChangedArgs { Axis = _tpAxisDescs[(int)TouchAxes.Touchpoint_0_Y + 2 * inx].AxisDesc, Value = y });
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }
        internal void OnWindowsTouchCancel(int id, float x, float y)
        {
            // Diagnostics.Log($"TouchCancel {id}");
            int inx;
            if (!_activeTouchpoints.TryGetValue(id, out inx))
                return;
            ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs { Button = _tpButtonDescs[(int)TouchPoints.Touchpoint_0 + inx].ButtonDesc, Pressed = false });
            _activeTouchpoints.Remove(id);
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsTouchInputDeviceImp" /> class.
        /// </summary>
        /// <param name="gameWindow">The game window to hook on to reveive 
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx">WM_POINTER</a> messages.</param>
        public WindowsTouchInputDeviceImp(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _handle = new HandleRef(_gameWindow, _gameWindow.WindowInfo.Handle);
            ConnectWindowsEvents();
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
        public string Desc => "MS Windows standard Touch device.";

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