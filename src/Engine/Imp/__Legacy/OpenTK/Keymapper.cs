using System;
using System.Collections.Generic;
using OpenTK.Input;

namespace Fusee.Engine
{
    internal class Keymapper : Dictionary<OpenTK.Input.Key, Fusee.Engine.KeyCodes>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal Keymapper()
        {
            this.Add(Key.Escape, KeyCodes.Escape);

            // Function keys
            for (int i = 0; i < 24; i++)
            {
                this.Add(Key.F1 + i, (KeyCodes)((int)KeyCodes.F1 + i));
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.Number0 + i, (KeyCodes)(0x30 + i));
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(Key.A + i, (KeyCodes)(0x41 + i));
            }

            this.Add(Key.Tab,          KeyCodes.Tab);
            this.Add(Key.CapsLock,     KeyCodes.Capital);
            this.Add(Key.ControlLeft,  KeyCodes.LControl);
            this.Add(Key.ShiftLeft,    KeyCodes.LShift);
            this.Add(Key.WinLeft,      KeyCodes.LWin);
            this.Add(Key.AltLeft,      KeyCodes.LMenu);
            this.Add(Key.Space,        KeyCodes.Space);
            this.Add(Key.AltRight,     KeyCodes.RMenu);
            this.Add(Key.WinRight,     KeyCodes.RWin);
            this.Add(Key.Menu,         KeyCodes.Apps);
            this.Add(Key.ControlRight, KeyCodes.RControl);
            this.Add(Key.ShiftRight,   KeyCodes.RShift);
            this.Add(Key.Enter,        KeyCodes.Return);
            this.Add(Key.BackSpace,    KeyCodes.Back);

            this.Add(Key.Semicolon,    KeyCodes.Oem1);
            this.Add(Key.Slash,        KeyCodes.Oem2);
            this.Add(Key.Tilde,        KeyCodes.Oem3);
            this.Add(Key.BracketLeft,  KeyCodes.Oem4);
            this.Add(Key.BackSlash,    KeyCodes.Oem5);
            this.Add(Key.BracketRight, KeyCodes.Oem6);
            this.Add(Key.Quote,        KeyCodes.Oem7);
            this.Add(Key.Plus,         KeyCodes.OemPlus);
            this.Add(Key.Comma,        KeyCodes.OemComma);
            this.Add(Key.Minus,        KeyCodes.OemMinus);
            this.Add(Key.Period,       KeyCodes.OemPeriod);

            this.Add(Key.Home,        KeyCodes.Home);
            this.Add(Key.End,         KeyCodes.End);
            this.Add(Key.Delete,      KeyCodes.Delete);
            this.Add(Key.PageUp,      KeyCodes.Prior);
            this.Add(Key.PageDown,    KeyCodes.Next);
            this.Add(Key.PrintScreen, KeyCodes.Print);
            this.Add(Key.Pause,       KeyCodes.Pause);
            this.Add(Key.NumLock,     KeyCodes.NumLock);

            this.Add(Key.ScrollLock,  KeyCodes.Scroll);
            // Do we need to do something here?? this.Add(Key.PrintScreen, KeyCodes.Snapshot);
            this.Add(Key.Clear,       KeyCodes.Clear);
            this.Add(Key.Insert,      KeyCodes.Insert);

            this.Add(Key.Sleep, KeyCodes.Sleep);

            // Keypad
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.Keypad0 + i, (KeyCodes)((int)KeyCodes.NumPad0 + i));
            }
            this.Add(Key.KeypadDecimal,  KeyCodes.Decimal);
            this.Add(Key.KeypadAdd,      KeyCodes.Add);
            this.Add(Key.KeypadSubtract, KeyCodes.Subtract);
            this.Add(Key.KeypadDivide,   KeyCodes.Divide);
            this.Add(Key.KeypadMultiply, KeyCodes.Multiply);

            // Navigation
            this.Add(Key.Up,    KeyCodes.Up);
            this.Add(Key.Down,  KeyCodes.Down);
            this.Add(Key.Left,  KeyCodes.Left);
            this.Add(Key.Right, KeyCodes.Right);
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
}
