using Fusee.Engine.Imp.Graphics.Desktop._3Dconnexion;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fusee.Engine.Imp.Graphics.Desktop._3Dconnexion
{
    class SiApp
    {
        const string SI_DLL = "siappdll";

        #region Enums

        public enum SpwRetVal
        {
            SPW_NO_ERROR,
            SPW_ERROR,
            SI_BAD_HANDLE,
            SI_BAD_ID,
            SI_BAD_VALUE,
            SI_IS_EVENT,
            SI_SKIP_EVENT,
            SI_NOT_EVENT,
            SI_NO_DRIVER,
            SI_NO_RESPONSE,
            SI_UNSUPPORTED,
            SI_UNINITIALIZED,
            SI_WRONG_DRIVER,
            SI_INTERNAL_ERROR,
            SI_BAD_PROTOCOL,
            SI_OUT_OF_MEMORY,
            SPW_DLL_LOAD_ERROR,
            SI_NOT_OPEN,
            SI_ITEM_NOT_FOUND,
            SI_UNSUPPORTED_DEVICE
        }

        public enum SiEventType
        {
            SI_BUTTON_EVENT = 1,
            SI_MOTION_EVENT,
            SI_COMBO_EVENT,
            SI_ZERO_EVENT,
            SI_EXCEPTION_EVENT,
            SI_OUT_OF_BAND,
            SI_ORIENTATION_EVENT,
            SI_KEYBOARD_EVENT,
            SI_LPFK_EVENT,
            SI_APP_EVENT,
            SI_SYNC_EVENT,
            SI_BUTTON_PRESS_EVENT,
            SI_BUTTON_RELEASE_EVENT,
            SI_DEVICE_CHANGE_EVENT,
            SI_MOUSE_EVENT,
            SI_JOYSTICK_EVENT
        }

        public enum SiDevType
        {
            SI_UNKNOWN_DEVICE = 0,
            SI_SPACEBALL_2003 = 1,
            SI_SPACEBALL_3003 = 2,
            SI_SPACE_CONTROLLER = 3,
            SI_SPACEEXPLORER = 4,
            SI_SPACENAVIGATOR_FOR_NOTEBOOKS = 5,
            SI_SPACENAVIGATOR = 6,
            SI_SPACEBALL_2003A = 7,
            SI_SPACEBALL_2003B = 8,
            SI_SPACEBALL_2003C = 9,
            SI_SPACEBALL_3003A = 10,
            SI_SPACEBALL_3003B = 11,
            SI_SPACEBALL_3003C = 12,
            SI_SPACEBALL_4000 = 13,
            SI_SPACEMOUSE_CLASSIC = 14,
            SI_SPACEMOUSE_PLUS = 15,
            SI_SPACEMOUSE_XT = 16,
            SI_CYBERMAN = 17,
            SI_CADMAN = 18,
            SI_SPACEMOUSE_CLASSIC_PROMO = 19,
            SI_SERIAL_CADMAN = 20,
            SI_SPACEBALL_5000 = 21,
            SI_TEST_NO_DEVICE = 22,
            SI_3DX_KEYBOARD_BLACK = 23,
            SI_3DX_KEYBOARD_WHITE = 24,
            SI_TRAVELER = 25,
            SI_TRAVELER1 = 26,
            SI_SPACEBALL_5000A = 27,
            SI_SPACEDRAGON = 28,
            SI_SPACEPILOT = 29,
            SI_MB = 30,
            SI_SPACEPILOT_PRO = 0xc629,
            SI_SPACEMOUSE_PRO = 0xc62b,
            SI_SPACEMOUSE_TOUCH = 0xc62c,
            SI_SPACEMOUSE_WIRELESS = 0xc62e
        }

        public enum SiDeviceChangeType
        {
            SI_DEVICE_CHANGE_CONNECT = 0,
            SI_DEVICE_CHANGE_DISCONNECT = 1
        }

        public enum SiOrientation
        {
            SI_LEFT = 0,
            SI_RIGHT
        }

        #endregion 

        #region Const Variables

        private const int SI_STRSIZE = 128;
        private const int MAX_PATH = 260;
        private const int SI_MAXPATH = 512;
        private const int SI_MAXPORTNAME = 260;
        private const int SI_MAXBUF = 128;
        private const int SI_KEY_MAXBUF = 5120;

        public const int SI_ANY_DEVICE = -1;
        public const int SI_NO_BUTTON = -1;
        public const int SI_EVENT = 0x0001;
        public const int SI_AVERAGE_EVENTS = 1;

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct SiDevInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_STRSIZE)]
            public string firmware;
            public int devType;
            public int numButtons;
            public int numDegrees;
            public bool canBeep;
            public int majorVersion;
            public int minorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiDevPort
        {
            public int devID;
            public int devType;
            public int devClass;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_STRSIZE)]
            public string devName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_MAXPORTNAME)]
            public string portName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiDeviceName
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_STRSIZE)]
            public String name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiButtonName
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_STRSIZE)]
            public String name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiPortName
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_MAXPATH)]
            public string name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiOpenData
        {
            public int hWnd;
            public IntPtr transCtl;
            public int processID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public String exeFile;
            public int libFlag;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiTypeMask
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public byte[] mask;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiGetEventData
        {
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiButtonData
        {
            public uint last;
            public uint current;
            public uint pressed;
            public uint released;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiSpwData
        {
            public SiButtonData bData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] mData;
            public int period;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SI_MAXBUF)]
            public byte[] exData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiSpwOOB
        {
            public byte code;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_MAXBUF - 1)]
            public byte[] message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiOrientation1
        {
            public int orientation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiKeyboardData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_KEY_MAXBUF)]
            public byte[] kstring;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiDeviceChangeEventData
        {
            public int type;
            public int devID;
            public SiPortName portName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiSpwEvent_JustType
        {
            public SiEventType type; // int
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiSpwEvent_SpwData
        {
            public SiEventType type; // int
            public SiSpwData spwData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiSpwEvent_DeviceChange
        {
            public SiEventType type; // int
            public SiDeviceChangeEventData deviceChangeEventData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SiDeviceIconPath
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SI_STRSIZE)]
            public String path;
        }

        #endregion

        #region DLL Imports

        [DllImport(SI_DLL, EntryPoint = "SiGetButtonName")]
        private static extern SpwRetVal pfnSiGetButtonName(IntPtr hdl, uint buttonNumber, ref SiButtonName name);
        [DllImport(SI_DLL, EntryPoint = "SiGetDeviceName")]
        private static extern SpwRetVal pfnSiGetDeviceName(IntPtr hdl, ref SiDeviceName name);

        [DllImport(SI_DLL, EntryPoint = "SiInitialize")]
        public static extern SpwRetVal SiInitialize();
        [DllImport(SI_DLL, EntryPoint = "SiTerminate")]
        public static extern int SiTerminate();
        [DllImport(SI_DLL, EntryPoint = "SiClose")]
        public static extern SpwRetVal SiClose(IntPtr hdl);
        [DllImport(SI_DLL, EntryPoint = "SiOpenWinInit")]
        public static extern int SiOpenWinInit(ref SiOpenData o, IntPtr hwnd);
        [DllImport(SI_DLL, EntryPoint = "SiOpen", CharSet = CharSet.Ansi)]
        public static extern IntPtr SiOpen(string pAppName, int devID, IntPtr pTMask, int mode, ref SiOpenData pData);
        [DllImport(SI_DLL, EntryPoint = "SiOpenPort", CharSet = CharSet.Ansi)]
        public static extern IntPtr SiOpenPort(string pAppName, ref SiDevPort pPort, int mode, ref SiOpenData pData);
        [DllImport(SI_DLL, EntryPoint = "SiGetDeviceInfo")]
        public static extern SpwRetVal SiGetDeviceInfo(IntPtr hdl, ref SiDevInfo pInfo);
        [DllImport(SI_DLL, EntryPoint = "SiGetDevicePort")]
        public static extern SpwRetVal SiGetDevicePort(IntPtr hdl, ref SiDevPort pPort);
        [DllImport(SI_DLL, EntryPoint = "SiGetEventWinInit")]
        public static extern void SiGetEventWinInit(ref SiGetEventData pData, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(SI_DLL, EntryPoint = "SiGetEvent")]
        public static extern SpwRetVal SiGetEvent(IntPtr hdl, int flags, ref SiGetEventData pData, ref SiSpwEvent_SpwData pEvent);

        [DllImport(SI_DLL, EntryPoint = "SiButtonPressed")]
        public static extern int SiButtonPressed(ref SiSpwEvent_SpwData pEvent);
        [DllImport(SI_DLL, EntryPoint = "SiRezero")]
        public static extern SpwRetVal SiRezero(IntPtr hdl);
        [DllImport(SI_DLL, EntryPoint = "SiSetLEDs")]
        public static extern SpwRetVal SiSetLEDs(IntPtr hdl, int mask);
        [DllImport(SI_DLL, EntryPoint = "SiGetNumDevices")]
        public static extern int SiGetNumDevices();
        [DllImport(SI_DLL, EntryPoint = "SiGetDeviceID")]
        public static extern int SiGetDeviceID(IntPtr hdl);
        [DllImport(SI_DLL, EntryPoint = "SiGetDeviceImageFileName")]
        private static extern SpwRetVal pfnSiGetDeviceImageFileName(IntPtr hdl, ref SiDeviceIconPath path, ref int len);

        #endregion

        #region Functions

        public static SpwRetVal SiGetDeviceImageFileName(IntPtr hdl, out string path)
        {
            SpwRetVal tmpRetVal;
            SiDeviceIconPath devicePathStruct = new();
            int len = SI_STRSIZE;
            tmpRetVal = pfnSiGetDeviceImageFileName(hdl, ref devicePathStruct, ref len);
            path = devicePathStruct.path;
            return tmpRetVal;
        }

        public static SpwRetVal SiGetDeviceName(IntPtr hdl, out string name)
        {
            SpwRetVal tmpRetVal;
            SiDeviceName deviceNameStruct = new();
            tmpRetVal = pfnSiGetDeviceName(hdl, ref deviceNameStruct);
            name = deviceNameStruct.name;
            return tmpRetVal;
        }

        public static SpwRetVal SiGetButtonName(IntPtr hdl, uint buttonNumber, out string name)
        {
            SpwRetVal tmpRetVal;
            SiButtonName buttonNameStruct = new();
            tmpRetVal = pfnSiGetButtonName(hdl, buttonNumber, ref buttonNameStruct);
            name = buttonNameStruct.name;
            return tmpRetVal;
        }

        #endregion
    }
}

namespace Fusee.Engine.Imp.Graphics.Desktop._3DconnexionDriver
{
    /// <summary>
    /// 3Dconnexion driver.
    /// </summary>
    public class _3DconnexionDevice : IDisposable
    {
        IntPtr _deviceHandle = IntPtr.Zero;
        int _leds = -1;

        /// <summary>
        /// Event when Device is at 0 Point
        /// </summary>
        public event EventHandler ZeroPoint;

        /// <summary>
        /// Event when Device is moved
        /// </summary>
        public event EventHandler<MotionEventArgs> Motion;

        ///// <summary>
        ///// Event when Device changes. Doesn't work yet
        ///// </summary>
        //public event EventHandler<DeviceChangeEventArgs> DeviceChange;

        /// <summary>
        /// Dispatching Thread
        /// </summary>
        private readonly System.Threading.Thread eventThread;

        /// <summary>
        /// Buffer for Events
        /// </summary>
        private readonly Dictionary<SiApp.SiEventType, EventArgs> eventBuffer = new();
        /// <summary>
        /// The 3Dconnexion device.
        /// </summary>
        /// <param name="appName"></param>
        public _3DconnexionDevice(string appName)
        {
            this.AppName = appName;
            eventThread = new System.Threading.Thread(EventThreadLoop)
            {
                IsBackground = true,
                Name = "3Dconnexion-Event-Dispatcher"
            };
            eventThread.Start();
        }
        /// <summary>
        /// Disposes of the connected device.
        /// </summary>
        ~_3DconnexionDevice()
        {
            Dispose();
        }

        /// <summary>
        /// Device is available (initialized)
        /// </summary>
        public bool IsAvailable
        {
            get { return _deviceHandle != IntPtr.Zero; }
        }

        /// <summary>
        /// Device is disposed
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the app using this device
        /// </summary>
        public string AppName
        {
            get;
            private set;
        }

        /// <summary>
        /// The Name of the Device (e.g. Space Pilot)
        /// </summary>
        public string DeviceName
        {
            get;
            private set;
        }

        /// <summary>
        /// The DeviceID of the device
        /// </summary>
        public int DeviceID
        {
            get;
            private set;
        }

        /// <summary>
        /// The Firmware version of the device
        /// </summary>
        public string FirmwareVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Bitwise Flags for the LEDs of the Device
        /// </summary>
        public int LEDs
        {
            get { return _leds; }
            set
            {
                _leds = value;
                if (IsAvailable)
                {
                    SiApp.SiSetLEDs(_deviceHandle, _leds);
                }
            }
        }

        /// <summary>
        /// Path to an icon for that device, provided by the 3Dx Ware Driver
        /// </summary>
        public string IconPath
        {
            get
            {
                if (IsAvailable)
                {
                    _ = SiApp.SiGetDeviceImageFileName(_deviceHandle, out var path);
                    return path;
                }
                return null;
            }
        }

        /// <summary>
        /// Initialize the Device
        /// </summary>
        /// <param name="windowHandle">Handle to window using the device</param>
        public void InitDevice(IntPtr windowHandle)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("");
            if (IsAvailable)
                return; //Init already done.

            SiApp.SpwRetVal v = SiApp.SiInitialize();

            //try
            //{
            //    v = SiApp.SiInitialize();
            //}
            //catch
            //{
            //    // throw new _3DxException("Driver not installed.");
            //    Diagnostics.Log("3DX Driver is not installed");
            //    eventThread.Abort();
            //    Dispose();
            //    return;
            //}

            if (v == SiApp.SpwRetVal.SPW_DLL_LOAD_ERROR)
                throw new _3DxException("Unable to load SiApp DLL");


            var o = default(SiApp.SiOpenData);
            _ = SiApp.SiOpenWinInit(ref o, windowHandle);

            _deviceHandle = SiApp.SiOpen(this.AppName, SiApp.SI_ANY_DEVICE, IntPtr.Zero, SiApp.SI_EVENT, ref o);
            if (_deviceHandle == IntPtr.Zero)
            {
                _ = SiApp.SiTerminate();
                throw new _3DxException("Unable to open device");
            }

            SiApp.SiGetDeviceName(_deviceHandle, out string devName);

            this.DeviceName = devName;

            this.DeviceID = SiApp.SiGetDeviceID(_deviceHandle);

            SiApp.SiDevInfo info = default;
            SiApp.SiGetDeviceInfo(_deviceHandle, ref info);

            this.FirmwareVersion = info.firmware;


        }

        /// <summary>
        /// Close the Device
        /// </summary>
        public void CloseDevice()
        {


            if (_deviceHandle != IntPtr.Zero)
            {
                SiApp.SiClose(_deviceHandle);
                _deviceHandle = IntPtr.Zero;
            }

        }

        /// <summary>
        /// Called in WndPrc to process the Window Messages
        /// </summary>
        /// <param name="msg">Message Number</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        public void ProcessWindowMessage(int msg, IntPtr wParam, IntPtr lParam)
        {
            if (this.IsDisposed) //We don't throw an Exception in the Message Loop, just return
                return;

            SiApp.SiGetEventData evd = default;
            SiApp.SiGetEventWinInit(ref evd, msg, wParam, lParam);
            SiApp.SiSpwEvent_SpwData edata = default;
            var t = SiApp.SiGetEvent(_deviceHandle, SiApp.SI_AVERAGE_EVENTS, ref evd, ref edata);
            if (t == SiApp.SpwRetVal.SI_IS_EVENT)
            {
                switch (edata.type)
                {
                    case SiApp.SiEventType.SI_ZERO_EVENT:
                        lock (eventBuffer)
                        {
                            eventBuffer[SiApp.SiEventType.SI_ZERO_EVENT] = EventArgs.Empty;
                            Monitor.Pulse(eventBuffer);
                        }
                        break;
                    case SiApp.SiEventType.SI_MOTION_EVENT:
                        lock (eventBuffer)
                        {
                            eventBuffer[SiApp.SiEventType.SI_MOTION_EVENT] = MotionEventArgs.FromEventArray(edata.spwData.mData);
                            Monitor.Pulse(eventBuffer);
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Is called if the mouse is not moved.
        /// </summary>
        protected virtual void OnZeroPoint()
        {
            ZeroPoint?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Is called if the mouse is moved. 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMotion(MotionEventArgs args)
        {
            Motion?.Invoke(this, args);
        }

        //protected virtual void OnDeviceChange(DeviceChangeEventArgs args)
        //{
        //    if (DeviceChange != null)
        //        DeviceChange(this, args);
        //}

        #region IDisposable Member

        /// <summary>
        /// Disposes of the device.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                CloseDevice();
                ZeroPoint = null;
                Motion = null;
            }
            this.IsDisposed = true;
        }

        #endregion

        private void EventThreadLoop()
        {
            while (!this.IsDisposed)
            {
                lock (eventBuffer)
                {
                    while (eventBuffer.Count == 0)
                        Monitor.Wait(eventBuffer);
                    foreach (var c in eventBuffer)
                    {
                        switch (c.Key)
                        {
                            case SiApp.SiEventType.SI_MOTION_EVENT:
                                OnMotion(c.Value as MotionEventArgs); break;
                            case SiApp.SiEventType.SI_ZERO_EVENT:
                                OnZeroPoint(); break;
                        }
                    }
                    eventBuffer.Clear();
                }
                Thread.Sleep(50);
            }
        }
    }

    /// <summary>
    /// A class for special 3D exceptions.
    /// </summary>
    public class _3DxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="_3DxException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public _3DxException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="_3DxException"/> class and passes an inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception to pass.</param>
        public _3DxException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Contains event data for a device change event.
    /// </summary>
    public class DeviceChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Device connection type.
        /// </summary>
        public readonly EDeviceChangeType Type;

        /// <summary>
        /// The device ID.
        /// </summary>
        public readonly int DeviceID;

        /// <summary>
        /// Creates a new instance of the <see cref="DeviceChangeEventArgs"/> class.
        /// </summary>
        /// <param name="deviceId">The id of the changed device.</param>
        /// <param name="type">The type of the change, can be 0 (for CONNECTED), 1 (for DISCONNECTED), or any other number (for UNKNOWN).</param>
        public DeviceChangeEventArgs(int deviceId, int type)
        {
            this.DeviceID = deviceId;
            Type = type switch
            {
                0 => EDeviceChangeType.CONNECTED,
                1 => EDeviceChangeType.DISCONNECTED,
                _ => EDeviceChangeType.UNKNOWN,
            };
        }
    }

    /// <summary>
    /// Device connection type.
    /// </summary>
    public enum EDeviceChangeType
    {
        /// <summary>
        /// Device is connected.
        /// </summary>
        CONNECTED,
        /// <summary>
        /// Device is disconnected
        /// </summary>
        DISCONNECTED,
        /// <summary>
        /// Connection type unknown.
        /// </summary>
        UNKNOWN
    }


    /// <summary>
    /// Contains event data for motion events.
    /// </summary>
    public class MotionEventArgs : EventArgs
    {
        /// <summary>
        /// Translation axes.
        /// </summary>
        public readonly int TX, TY, TZ;
        /// <summary>
        /// Rotation axes.
        /// </summary>
        public readonly int RX, RY, RZ;

        /// <summary>
        /// Creates a new instance of the <see cref="MotionEventArgs"/> class.
        /// </summary>
        /// <param name="tx">X coordinate of the translation.</param>
        /// <param name="ty">Y coordinate of the translation.</param>
        /// <param name="tz">Z coordinate of the translation.</param>
        /// <param name="rx">X coordinate of the rotation.</param>
        /// <param name="ry">Y coordinate of the rotation.</param>
        /// <param name="rz">Z coordinate of the rotation.</param>
        public MotionEventArgs(int tx, int ty, int tz, int rx, int ry, int rz)
        {
            this.TX = tx;
            this.TY = ty;
            this.TZ = tz;
            this.RX = rx;
            this.RY = ry;
            this.RZ = rz;
        }

        /// <summary>
        /// Returns a new instance of the <see cref="MotionEventArgs"/> class from a given array of event data.
        /// </summary>
        /// <param name="data">The array of event data.</param>
        /// <returns>The newly created <see cref="MotionEventArgs"/> instance.</returns>
        public static MotionEventArgs FromEventArray(int[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < 6)
                throw new ArgumentException("data array to small");

            return new MotionEventArgs(data[0], data[1], data[2], data[3], data[4], data[5]);
        }
    }
}