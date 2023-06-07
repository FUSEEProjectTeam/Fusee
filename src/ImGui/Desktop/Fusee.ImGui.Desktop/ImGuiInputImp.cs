﻿using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.Desktop;
using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Fusee.ImGuiImp.Desktop
{
    public unsafe class ImGuiInputImp : IInputDriverImp, IDisposable
    {
        private readonly GameWindow _gameWindow;
        private readonly KeyboardDeviceImp _keyboard;
        private readonly MouseDeviceImp _mouse;
        //private readonly GamePadDeviceImp _gamePad0;
        //private readonly GamePadDeviceImp _gamePad1;
        //private readonly GamePadDeviceImp _gamePad2;
        //private readonly GamePadDeviceImp _gamePad3;

        public ImGuiInputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (renderCanvas is not ImGuiRenderCanvasImp)
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", nameof(renderCanvas));

            _gameWindow = ((ImGuiRenderCanvasImp)renderCanvas)._gameWindow;
            if (_gameWindow == null)
                throw new ArgumentNullException(nameof(_gameWindow));

            _keyboard = new KeyboardDeviceImp(_gameWindow);
            _mouse = new MouseDeviceImp(_gameWindow);
            //_gamePad0 = new GamePadDeviceImp(_gameWindow, 0);
            //_gamePad1 = new GamePadDeviceImp(_gameWindow, 1);
            //_gamePad2 = new GamePadDeviceImp(_gameWindow, 2);
            //_gamePad3 = new GamePadDeviceImp(_gameWindow, 3);
        }

        /// <summary>
        /// Devices supported by this driver: One mouse, one keyboard and up to four gamepads.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _keyboard;
                yield return _mouse;
                // TODO(mr): Implement gamepad support
                // reference impl: https://github.com/ocornut/imgui/blob/master/backends/imgui_impl_win32.cpp#L278
                //yield return _gamePad0;
                //yield return _gamePad1;
                //yield return _gamePad2;
                //yield return _gamePad3;
            }
        }

        /// <summary>
        /// Returns a human readable description of this driver.
        /// </summary>
        public string DriverDesc
        {
            get
            {
                string pf = "ImGui";
                return "ImGui and OpenTk GameWindow Mouse and Keyboard input driver for " + pf;
            }
        }

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string DriverId
        {
            get { return GetType()?.FullName ?? "Fusee.DImGui.Desktop"; }
        }


        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs>? DeviceDisconnected;

        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<NewDeviceImpConnectedArgs>? NewDeviceConnected;


        private static readonly Dictionary<KeyCodes, ImGuiKey> _translateKeyCodeToImGuiKey = new()
        {
            { KeyCodes.None, ImGuiKey.None },
            { KeyCodes.Tab, ImGuiKey.Tab },
            { KeyCodes.Left, ImGuiKey.LeftArrow },
            { KeyCodes.Right, ImGuiKey.RightArrow },
            { KeyCodes.Up, ImGuiKey.UpArrow },
            { KeyCodes.Down, ImGuiKey.DownArrow },
            { KeyCodes.PageUp, ImGuiKey.PageUp },
            { KeyCodes.PageDown, ImGuiKey.PageDown },
            { KeyCodes.Home, ImGuiKey.Home },
            { KeyCodes.End, ImGuiKey.End },
            { KeyCodes.Insert, ImGuiKey.Insert },
            { KeyCodes.Delete, ImGuiKey.Delete },
            { KeyCodes.Back, ImGuiKey.Backspace },
            { KeyCodes.Space, ImGuiKey.Space },
            { KeyCodes.Enter, ImGuiKey.Enter },
            { KeyCodes.Escape, ImGuiKey.Escape },
            { KeyCodes.LControl, ImGuiKey.LeftCtrl },
            { KeyCodes.LShift, ImGuiKey.LeftShift },
            { KeyCodes.AltModifier, ImGuiKey.LeftAlt },
            { KeyCodes.LWin, ImGuiKey.LeftSuper },
            { KeyCodes.RControl, ImGuiKey.RightCtrl },
            { KeyCodes.RShift, ImGuiKey.RightShift },
            { KeyCodes.RWin, ImGuiKey.RightSuper },
            { KeyCodes.Menu, ImGuiKey.Menu },
            { KeyCodes.A, ImGuiKey.A },
            { KeyCodes.B, ImGuiKey.B },
            { KeyCodes.C, ImGuiKey.C },
            { KeyCodes.D, ImGuiKey.D },
            { KeyCodes.E, ImGuiKey.E },
            { KeyCodes.F, ImGuiKey.F },
            { KeyCodes.G, ImGuiKey.G },
            { KeyCodes.H, ImGuiKey.H },
            { KeyCodes.I, ImGuiKey.I },
            { KeyCodes.J, ImGuiKey.J },
            { KeyCodes.K, ImGuiKey.K },
            { KeyCodes.L, ImGuiKey.L },
            { KeyCodes.M, ImGuiKey.M },
            { KeyCodes.N, ImGuiKey.N },
            { KeyCodes.O, ImGuiKey.O },
            { KeyCodes.P, ImGuiKey.P },
            { KeyCodes.Q, ImGuiKey.Q },
            { KeyCodes.R, ImGuiKey.R },
            { KeyCodes.S, ImGuiKey.S },
            { KeyCodes.T, ImGuiKey.T },
            { KeyCodes.U, ImGuiKey.U },
            { KeyCodes.V, ImGuiKey.V },
            { KeyCodes.W, ImGuiKey.W },
            { KeyCodes.X, ImGuiKey.X },
            { KeyCodes.Y, ImGuiKey.Y },
            { KeyCodes.Z, ImGuiKey.Z },
            { KeyCodes.D0, ImGuiKey._0 },
            { KeyCodes.D1, ImGuiKey._1 },
            { KeyCodes.D2, ImGuiKey._2 },
            { KeyCodes.D3, ImGuiKey._3 },
            { KeyCodes.D4, ImGuiKey._4 },
            { KeyCodes.D5, ImGuiKey._5 },
            { KeyCodes.D6, ImGuiKey._6 },
            { KeyCodes.D7, ImGuiKey._7 },
            { KeyCodes.D8, ImGuiKey._8 },
            { KeyCodes.D9, ImGuiKey._9 },
            { KeyCodes.F1, ImGuiKey.F1 },
            { KeyCodes.F2, ImGuiKey.F2 },
            { KeyCodes.F3, ImGuiKey.F3 },
            { KeyCodes.F4, ImGuiKey.F4 },
            { KeyCodes.F5, ImGuiKey.F5 },
            { KeyCodes.F6, ImGuiKey.F6 },
            { KeyCodes.F7, ImGuiKey.F7 },
            { KeyCodes.F8, ImGuiKey.F8 },
            { KeyCodes.F9, ImGuiKey.F9 },
            { KeyCodes.F10, ImGuiKey.F10 },
            { KeyCodes.F11, ImGuiKey.F11 },
            { KeyCodes.F12, ImGuiKey.F12 },
            { KeyCodes.OemComma, ImGuiKey.Comma },
            { KeyCodes.OemMinus, ImGuiKey.Minus },
            { KeyCodes.OemPeriod, ImGuiKey.Period },
            { KeyCodes.OemSemicolon, ImGuiKey.Semicolon },
            { KeyCodes.OemOpenBrackets, ImGuiKey.LeftBracket },
            { KeyCodes.OemBackslash, ImGuiKey.Backslash },
            { KeyCodes.OemCloseBrackets, ImGuiKey.RightBracket },
            { KeyCodes.CapsLock, ImGuiKey.CapsLock },
            { KeyCodes.Scroll, ImGuiKey.ScrollLock },
            { KeyCodes.NumLock, ImGuiKey.NumLock },
            { KeyCodes.PrintScreen, ImGuiKey.PrintScreen },
            { KeyCodes.Pause, ImGuiKey.Pause },
            { KeyCodes.NumPad0, ImGuiKey.Keypad0 },
            { KeyCodes.NumPad1, ImGuiKey.Keypad1 },
            { KeyCodes.NumPad2, ImGuiKey.Keypad2 },
            { KeyCodes.NumPad3, ImGuiKey.Keypad3 },
            { KeyCodes.NumPad4, ImGuiKey.Keypad4 },
            { KeyCodes.NumPad5, ImGuiKey.Keypad5 },
            { KeyCodes.NumPad6, ImGuiKey.Keypad6 },
            { KeyCodes.NumPad7, ImGuiKey.Keypad7 },
            { KeyCodes.NumPad8, ImGuiKey.Keypad8 },
            { KeyCodes.NumPad9, ImGuiKey.Keypad9 },
            { KeyCodes.Decimal, ImGuiKey.KeypadDecimal },
            { KeyCodes.Divide, ImGuiKey.KeypadDivide },
            { KeyCodes.Multiply, ImGuiKey.KeypadMultiply },
            { KeyCodes.Subtract, ImGuiKey.KeypadSubtract },
            { KeyCodes.Add, ImGuiKey.KeypadAdd }
        };

        internal static string CurrentlySelectedText = "";

        private delegate void SetClipboardTextFn(void* user_data, char* text);
        private static GCHandle _hndl; // <- do not delete, needed to prevent GC of this method!

        private bool disposedValue;

        public static void InitImGuiInput(GameWindow gw)
        {
            var io = ImGui.GetIO();

            gw.FocusedChanged += (FocusedChangedEventArgs e) => io.AddFocusEvent(e.IsFocused);
            gw.TextInput += (TextInputEventArgs c) => io.AddInputCharacter((uint)c.Unicode);

            // pin new a instance of SetClipboardTextFn, do not garbage collect
            _hndl = GCHandle.Alloc(new SetClipboardTextFn((_, text) =>
            {
                var copiedStr = Marshal.PtrToStringUTF8((IntPtr)text);
                gw.ClipboardString = copiedStr;
            }));

            if (_hndl.Target != null)
            {
                /// overwrite clipboard copy (Strg+C). Use OpenTK <see cref="GameWindow"/> implementation
                io.SetClipboardTextFn =
                    Marshal.GetFunctionPointerForDelegate<SetClipboardTextFn>((SetClipboardTextFn)_hndl.Target);
            }

            Input.Keyboard.ButtonValueChanged += (s, e) =>
            {
                if (_translateKeyCodeToImGuiKey.TryGetValue((KeyCodes)e.Button.Id, out var imGuiKey))
                {
                    var io = ImGui.GetIO();
                    var isDown = e.Pressed;
                    io.AddKeyEvent(imGuiKey, isDown);
                }
            };
        }

        public static void UpdateImGuiInput(Vector2 scaleFactor)
        {
            var io = ImGui.GetIO();
            io.ClearInputCharacters();

            io.AddMousePosEvent(Input.Mouse.X / scaleFactor.X, Input.Mouse.Y / scaleFactor.Y);
            io.AddMouseButtonEvent((int)ImGuiMouseButton.Left, Input.Mouse.LeftButton);
            io.AddMouseButtonEvent((int)ImGuiMouseButton.Middle, Input.Mouse.MiddleButton);
            io.AddMouseButtonEvent((int)ImGuiMouseButton.Right, Input.Mouse.RightButton);

            io.AddMouseWheelEvent(0, Input.Mouse.WheelVel * 0.01f);

            io.AddKeyEvent(ImGuiKey.ModShift, Input.Keyboard.GetKey(KeyCodes.LShift) || Input.Keyboard.GetKey(KeyCodes.RShift));
            io.AddKeyEvent(ImGuiKey.ModCtrl, Input.Keyboard.GetKey(KeyCodes.LControl) || Input.Keyboard.GetKey(KeyCodes.RControl));
            io.AddKeyEvent(ImGuiKey.ModAlt, Input.Keyboard.GetKey(KeyCodes.LMenu) || Input.Keyboard.GetKey(KeyCodes.RMenu));
            io.AddKeyEvent(ImGuiKey.ModSuper, Input.Keyboard.GetKey(KeyCodes.LWin) || Input.Keyboard.GetKey(KeyCodes.RWin));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                _hndl.Free();
                disposedValue = true;
            }
        }

        ~ImGuiInputImp()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}