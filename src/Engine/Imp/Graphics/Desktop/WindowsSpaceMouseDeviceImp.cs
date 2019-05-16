using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusee.Engine.Common;
using OpenTK;
using _3DconnexionDriver;
using System.Windows.Forms;

namespace Fusee.Engine.Imp.Graphics.Desktop
{

    /// <summary>
    /// Input driver implementation supporting Windows 8 spacemouse input.
    /// </summary>
    public class WindowsSpaceMouseDriverImp : IInputDriverImp
    {
        GameWindow _gameWindow;
       
        WindowsSpaceMouseInputDeviceImp _SMI;
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsSpaceMouseDriverImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas. Internally this must be a Windows canvas with a valid window handle.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="System.ArgumentException">RenderCanvas must be of type <see cref="RenderCanvasImp"/></exception>
        /// <remarks>
        /// The current implementation does not fire the <see cref="NewDeviceConnected"/> and  <see cref="DeviceDisconnected"/>
        /// events. This driver will always report one connected SpaceMouse no matter how many physical devices are connected
        /// to the machine. If no physical SpaceMouse is present all of its axes and buttons will return 0.
        /// </remarks>
        public WindowsSpaceMouseDriverImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException($"renderCanvas must be of type {typeof(RenderCanvasImp).FullName}", nameof(renderCanvas));

            _gameWindow = ((RenderCanvasImp)renderCanvas)._gameWindow;
            if (_gameWindow == null)
                throw new ArgumentNullException(nameof(_gameWindow));


            _SMI = new WindowsSpaceMouseInputDeviceImp(_gameWindow);
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
        public IEnumerable<IInputDeviceImp> Devices { get { yield return _SMI; } }

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
        public string DriverDesc => "Driver providing a spacemouse device implementation for Windows 8 (and up) spacemouse input.";

#pragma warning disable 0067
        /// <summary>
        /// Not supported on this driver. The device is considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs> DeviceDisconnected;

        /// <summary>
        /// Not supported on this driver. The device is considered to be connected all the time.
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
    /// SMI input device implementation for the Windows platform. This implementation directly
    /// sniffes at the render window's message pump (identified by the <see cref="GameWindow"/> parameter passed
    /// to the constructor) to receive 
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx">WM_POINTER</a> messages.
    /// </summary>
    public class WindowsSpaceMouseInputDeviceImp : IInputDeviceImp
    {
        
        private HandleRef _handle;
        private readonly GameWindow _gameWindow;
        private readonly _3DconnexionDevice _current3DConnexionDevice;


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


        private UInt16 LOWORD(UInt32 wParam) => unchecked((UInt16)wParam);
        private UInt16 HIWORD(UInt32 wParam) => unchecked((UInt16)((wParam >> 16) & 0xFFFF));

        private UInt16 GET_X_LPARAM(UInt32 lp) => LOWORD(lp);
        private UInt16 GET_Y_LPARAM(UInt32 lp) => HIWORD(lp);

        private int GET_POINTERID_WPARAM(UInt32 wParam) => LOWORD(wParam);


        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        // private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

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


        private IntPtr SpaceMouseWindowsProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            // TODO
            if(!_current3DConnexionDevice.IsDisposed)
                _current3DConnexionDevice.ProcessWindowMessage((int)Msg, wParam, lParam);

            return CallWindowProc(_oldWndProc, hWnd, Msg, wParam, lParam);
        }

        private void ConnectWindowsEvents()
        {
            OperatingSystem os = Environment.OSVersion;

            // See https://msdn.microsoft.com/library/windows/desktop/ms724832.aspx : Apps NOT targetet for a specific windows version (like 8.1 or 10)
            // retrieve Version# 6.2 (resembling Windows 8), which is the version where "Pointer" SMI handling is first supported.
            if (os.Platform == PlatformID.Win32NT
                && (os.Version.Major > 6
                     || os.Version.Major == 6 && os.Version.Minor >= 2)
                )
            {
                if (_handle.Handle != IntPtr.Zero)
                {
                    _newWinProc = new WinProc(SpaceMouseWindowsProc);
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



        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsSpaceMouseInputDeviceImp" /> class.
        /// </summary>
        /// <param name="gameWindow">The game window to hook on to reveive 
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh454904(v=vs.85).aspx">WM_POINTER</a> messages.</param>
        public WindowsSpaceMouseInputDeviceImp(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            _handle = new HandleRef(_gameWindow, _gameWindow.WindowInfo.Handle);

            _current3DConnexionDevice = new _3DconnexionDevice(_handle.ToString());
            _current3DConnexionDevice.InitDevice((IntPtr)_handle);

            _current3DConnexionDevice.Motion += HandleMotion;
            
            ConnectWindowsEvents();


            _TX = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Translation X",
                    Id = (int)SixDOFAxis.TX,
                    Direction = AxisDirection.X,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            _TY = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Translation Y",
                    Id = (int)SixDOFAxis.TY,
                    Direction = AxisDirection.Y,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            _TZ = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Translation Z",
                    Id = (int)SixDOFAxis.TZ,
                    Direction = AxisDirection.Z,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            _RX = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Rotation Y",
                    Id = (int)SixDOFAxis.RX,
                    Direction = AxisDirection.Y,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            _RY = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Rotation X",
                    Id = (int)SixDOFAxis.RY,
                    Direction = AxisDirection.X,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            _RZ = new AxisImpDescription
            {
                AxisDesc = new AxisDescription
                {
                    Name = "Rotation Z",
                    Id = (int)SixDOFAxis.RZ,
                    Direction = AxisDirection.Z,
                    Nature = AxisNature.Position,
                    Bounded = AxisBoundedType.Constant,
                    MinValueOrAxis = -1,
                    MaxValueOrAxis = 1
                },
                PollAxis = false
            };
            Button1 = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Button1",
                    Id = 0

                },
                PollButton = false
            };
            Button2 = new ButtonImpDescription
            {
                ButtonDesc = new ButtonDescription
                {
                    Name = "Button2",
                    Id = 1

                },
                PollButton = false
            };


        }

        
        /// <summary>
        /// Descriptions of the available axes.
        /// </summary>
        public AxisImpDescription _TX, _TY, _TZ, _RX, _RY, _RZ;

        /// <summary>
        /// Descriptions of the available buttons.
        /// </summary>
        public ButtonImpDescription Button1, Button2;

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        public string Id => _current3DConnexionDevice.DeviceName;

        /// <summary>
        /// Returns the description of this implementation.
        /// </summary>
        public string Desc => "Standard spacemouse implementation";

        /// <summary>
        /// Returns the Category this device belongs in.
        /// </summary>
        public DeviceCategory Category => DeviceCategory.SixDOF;

        /// <summary>
        /// Returns the number of Axes this device implements.
        /// </summary>
        public int AxesCount => 6;

        /// <summary>
        /// Returns the descriptions for all axes of this device.
        /// </summary>
        public IEnumerable<AxisImpDescription> AxisImpDesc
        {
            get
            {
                yield return _TX;
                yield return _TY;
                yield return _TZ;
                yield return _RX;
                yield return _RY;
                yield return _RZ;
            }
        }

        /// <summary>
        /// This device does not reveal any Buttons.
        /// </summary>
        public int ButtonCount => 0;

        /// <summary>
        /// This device does not implement any Buttons.
        /// </summary>
#pragma warning disable 0067
        public IEnumerable<ButtonImpDescription> ButtonImpDesc
        {
            get
            {
                yield return Button1;
                yield return Button2;

            }
        }

        /// <summary>
        /// All axes are event based.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Event to listen to to get the SDOF motion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleMotion(object sender, MotionEventArgs args)
        {
            if (AxisValueChanged != null)
            {
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _TX.AxisDesc, Value = args.TX });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _TY.AxisDesc, Value = args.TY });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _TZ.AxisDesc, Value = args.TZ });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _RX.AxisDesc, Value = args.RX });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _RY.AxisDesc, Value = args.RY });
                AxisValueChanged(this, new AxisValueChangedArgs { Axis = _RZ.AxisDesc, Value = args.RZ });
            }
            if (ButtonValueChanged != null)
            {
                //ButtonValueChanged(this, new ButtonValueChangedArgs { Button = Button1.ButtonDesc, Pressed });
                //ButtonValueChanged(this, new ButtonValueChangedArgs { Button = Button2.ButtonDesc, Pressed });
            }
        }
        

        /// <summary>
        /// This device does not implement any Buttons.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;
#pragma warning restore 0067
        /// <summary>
        /// Returns the current value of the axis {AxisID}.
        /// </summary>
        /// <param name="iAxisId"></param>
        /// <returns></returns>
        public float GetAxis(int iAxisId)
        {
            throw new InvalidOperationException("Device has no axis to be polled.");
        }

        /// <summary>
        /// This device does not implement any Buttons.
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
    
    
}