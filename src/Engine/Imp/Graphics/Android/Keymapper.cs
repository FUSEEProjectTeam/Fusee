using System.Collections.Generic;
using Fusee.Engine.Common;
using Android.Views;

namespace Fusee.Engine.Imp.Graphics.Android
{
    internal class Keymapper : Dictionary<Keycode, ButtonDescription>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Keycode
        /// </summary>
        internal Keymapper()
        {
            this.Add(Keycode.Escape, new ButtonDescription {Name = KeyCodes.Escape.ToString(), Id = (int)KeyCodes.Escape});

            // Function keys
            for (int i = 0; i < 12; i++)
            {
                this.Add(Keycode.F1 + i, new ButtonDescription { Name = $"F{i}", Id = (int)KeyCodes.F1 + i });
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Keycode.Num0 + i, new ButtonDescription { Name = $"D{i}", Id = (int)0x30 + i } );
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(Keycode.A + i, new ButtonDescription { Name = ((KeyCodes)(0x41 + i)).ToString(), Id = (int)(0x41 + i) });
            }

            this.Add(Keycode.Tab,  new ButtonDescription {Name = KeyCodes.Tab.ToString(), Id = (int)KeyCodes.Tab});
            this.Add(Keycode.CapsLock,  new ButtonDescription {Name = KeyCodes.Capital.ToString(), Id = (int)KeyCodes.Capital});
            this.Add(Keycode.CtrlLeft,  new ButtonDescription {Name = KeyCodes.LControl.ToString(), Id = (int)KeyCodes.LControl});
            this.Add(Keycode.ShiftLeft,  new ButtonDescription {Name = KeyCodes.LShift.ToString(), Id = (int)KeyCodes.LShift});
            this.Add(Keycode.MetaLeft,  new ButtonDescription {Name = KeyCodes.LWin.ToString(), Id = (int)KeyCodes.LWin});
            this.Add(Keycode.AltLeft,  new ButtonDescription {Name = KeyCodes.LMenu.ToString(), Id = (int)KeyCodes.LMenu});
            this.Add(Keycode.Space,  new ButtonDescription {Name = KeyCodes.Space.ToString(), Id = (int)KeyCodes.Space});
            this.Add(Keycode.AltRight,  new ButtonDescription {Name = KeyCodes.RMenu.ToString(), Id = (int)KeyCodes.RMenu});
            this.Add(Keycode.MetaRight,  new ButtonDescription {Name = KeyCodes.RWin.ToString(), Id = (int)KeyCodes.RWin});
            this.Add(Keycode.Menu,  new ButtonDescription {Name = KeyCodes.Apps.ToString(), Id = (int)KeyCodes.Apps});
            this.Add(Keycode.CtrlRight,  new ButtonDescription {Name = KeyCodes.RControl.ToString(), Id = (int)KeyCodes.RControl});
            this.Add(Keycode.ShiftRight,  new ButtonDescription {Name = KeyCodes.RShift.ToString(), Id = (int)KeyCodes.RShift});
            this.Add(Keycode.Enter,  new ButtonDescription {Name = KeyCodes.Return.ToString(), Id = (int)KeyCodes.Return});
            this.Add(Keycode.Back,  new ButtonDescription {Name = KeyCodes.Back.ToString(), Id = (int)KeyCodes.Back});

            /* OEM stuff inherited from windows
            this.Add(Keycode.Semicolon,  new ButtonDescription {Name = KeyCodes.Oem1.ToString(), Id = (int)KeyCodes.Oem1});
            this.Add(Keycode.Slash,  new ButtonDescription {Name = KeyCodes.Oem2.ToString(), Id = (int)KeyCodes.Oem2});
            this.Add(Keycode.Tilde,  new ButtonDescription {Name = KeyCodes.Oem3.ToString(), Id = (int)KeyCodes.Oem3});
            this.Add(Keycode.BracketLeft,  new ButtonDescription {Name = KeyCodes.Oem4.ToString(), Id = (int)KeyCodes.Oem4});
            this.Add(Keycode.BackSlash,  new ButtonDescription {Name = KeyCodes.Oem5.ToString(), Id = (int)KeyCodes.Oem5});
            this.Add(Keycode.BracketRight,  new ButtonDescription {Name = KeyCodes.Oem6.ToString(), Id = (int)KeyCodes.Oem6});
            this.Add(Keycode.Quote,  new ButtonDescription {Name = KeyCodes.Oem7.ToString(), Id = (int)KeyCodes.Oem7});
            this.Add(Keycode.Plus,  new ButtonDescription {Name = KeyCodes.OemPlus.ToString(), Id = (int)KeyCodes.OemPlus});
            this.Add(Keycode.Comma,  new ButtonDescription {Name = KeyCodes.OemComma.ToString(), Id = (int)KeyCodes.OemComma});
            this.Add(Keycode.Minus,  new ButtonDescription {Name = KeyCodes.OemMinus.ToString(), Id = (int)KeyCodes.OemMinus});
            this.Add(Keycode.Period,  new ButtonDescription {Name = KeyCodes.OemPeriod.ToString(), Id = (int)KeyCodes.OemPeriod});
            */

            this.Add(Keycode.Home,  new ButtonDescription {Name = KeyCodes.Home.ToString(), Id = (int)KeyCodes.Home});
            this.Add(Keycode.MoveEnd,  new ButtonDescription {Name = KeyCodes.End.ToString(), Id = (int)KeyCodes.End});
            this.Add(Keycode.Del,  new ButtonDescription {Name = KeyCodes.Delete.ToString(), Id = (int)KeyCodes.Delete});
            this.Add(Keycode.PageUp,  new ButtonDescription {Name = KeyCodes.Prior.ToString(), Id = (int)KeyCodes.Prior});
            this.Add(Keycode.PageDown,  new ButtonDescription {Name = KeyCodes.Next.ToString(), Id = (int)KeyCodes.Next});
            // this.Add(Keycode.PrintScreen,  new ButtonDescription {Name = KeyCodes.Print.ToString(), Id = (int)KeyCodes.Print});
            // this.Add(Keycode.Pause,  new ButtonDescription {Name = KeyCodes.Pause.ToString(), Id = (int)KeyCodes.Pause});
            this.Add(Keycode.NumLock,  new ButtonDescription {Name = KeyCodes.NumLock.ToString(), Id = (int)KeyCodes.NumLock});

            this.Add(Keycode.ScrollLock,  new ButtonDescription {Name = KeyCodes.Scroll.ToString(), Id = (int)KeyCodes.Scroll});
            // Do we need to do something here?? this.Add(Keycode.PrintScreen,  new ButtonDescription {Name = KeyCodes.Snapshot.ToString(), Id = (int)KeyCodes.Snapshot});
            this.Add(Keycode.Clear,  new ButtonDescription {Name = KeyCodes.Clear.ToString(), Id = (int)KeyCodes.Clear});
            this.Add(Keycode.Insert,  new ButtonDescription {Name = KeyCodes.Insert.ToString(), Id = (int)KeyCodes.Insert});

            this.Add(Keycode.Sleep,  new ButtonDescription {Name = KeyCodes.Sleep.ToString(), Id = (int)KeyCodes.Sleep});

            // Keypad
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Keycode.Numpad0 + i, new ButtonDescription { Name = $"Numpad{i}", Id = (int)KeyCodes.NumPad0 + i});
            }

            this.Add(Keycode.NumpadDot,  new ButtonDescription {Name = KeyCodes.Decimal.ToString(), Id = (int)KeyCodes.Decimal});
            this.Add(Keycode.NumpadAdd,  new ButtonDescription {Name = KeyCodes.Add.ToString(), Id = (int)KeyCodes.Add});
            this.Add(Keycode.NumpadSubtract,  new ButtonDescription {Name = KeyCodes.Subtract.ToString(), Id = (int)KeyCodes.Subtract});
            this.Add(Keycode.NumpadDivide,  new ButtonDescription {Name = KeyCodes.Divide.ToString(), Id = (int)KeyCodes.Divide});
            this.Add(Keycode.NumpadMultiply,  new ButtonDescription {Name = KeyCodes.Multiply.ToString(), Id = (int)KeyCodes.Multiply});

            // Navigation
            this.Add(Keycode.DpadUp,  new ButtonDescription {Name = KeyCodes.Up.ToString(), Id = (int)KeyCodes.Up});
            this.Add(Keycode.DpadDown,  new ButtonDescription {Name = KeyCodes.Down.ToString(), Id = (int)KeyCodes.Down});
            this.Add(Keycode.DpadLeft,  new ButtonDescription {Name = KeyCodes.Left.ToString(), Id = (int)KeyCodes.Left});
            this.Add(Keycode.DpadRight,  new ButtonDescription {Name = KeyCodes.Right.ToString(), Id = (int)KeyCodes.Right});
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
