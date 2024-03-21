using Android.Views;
using Fusee.Engine.Common;
using System.Collections.Generic;

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
            Add(Keycode.Escape, new ButtonDescription { Name = KeyCodes.Escape.ToString(), Id = (int)KeyCodes.Escape });

            // Function keys
            for (int i = 0; i < 12; i++)
            {
                Add(Keycode.F1 + i, new ButtonDescription { Name = $"F{i}", Id = (int)KeyCodes.F1 + i });
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                Add(Keycode.Num0 + i, new ButtonDescription { Name = $"D{i}", Id = 0x30 + i });
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                Add(Keycode.A + i, new ButtonDescription { Name = ((KeyCodes)(0x41 + i)).ToString(), Id = 0x41 + i });
            }

            Add(Keycode.Tab, new ButtonDescription { Name = KeyCodes.Tab.ToString(), Id = (int)KeyCodes.Tab });
            Add(Keycode.CapsLock, new ButtonDescription { Name = KeyCodes.Capital.ToString(), Id = (int)KeyCodes.Capital });
            Add(Keycode.CtrlLeft, new ButtonDescription { Name = KeyCodes.LControl.ToString(), Id = (int)KeyCodes.LControl });
            Add(Keycode.ShiftLeft, new ButtonDescription { Name = KeyCodes.LShift.ToString(), Id = (int)KeyCodes.LShift });
            Add(Keycode.MetaLeft, new ButtonDescription { Name = KeyCodes.LWin.ToString(), Id = (int)KeyCodes.LWin });
            Add(Keycode.AltLeft, new ButtonDescription { Name = KeyCodes.LMenu.ToString(), Id = (int)KeyCodes.LMenu });
            Add(Keycode.Space, new ButtonDescription { Name = KeyCodes.Space.ToString(), Id = (int)KeyCodes.Space });
            Add(Keycode.AltRight, new ButtonDescription { Name = KeyCodes.RMenu.ToString(), Id = (int)KeyCodes.RMenu });
            Add(Keycode.MetaRight, new ButtonDescription { Name = KeyCodes.RWin.ToString(), Id = (int)KeyCodes.RWin });
            Add(Keycode.Menu, new ButtonDescription { Name = KeyCodes.Apps.ToString(), Id = (int)KeyCodes.Apps });
            Add(Keycode.CtrlRight, new ButtonDescription { Name = KeyCodes.RControl.ToString(), Id = (int)KeyCodes.RControl });
            Add(Keycode.ShiftRight, new ButtonDescription { Name = KeyCodes.RShift.ToString(), Id = (int)KeyCodes.RShift });
            Add(Keycode.Enter, new ButtonDescription { Name = KeyCodes.Return.ToString(), Id = (int)KeyCodes.Return });
            Add(Keycode.Back, new ButtonDescription { Name = KeyCodes.Back.ToString(), Id = (int)KeyCodes.Back });

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

            Add(Keycode.Home, new ButtonDescription { Name = KeyCodes.Home.ToString(), Id = (int)KeyCodes.Home });
            Add(Keycode.MoveEnd, new ButtonDescription { Name = KeyCodes.End.ToString(), Id = (int)KeyCodes.End });
            Add(Keycode.Del, new ButtonDescription { Name = KeyCodes.Delete.ToString(), Id = (int)KeyCodes.Delete });
            Add(Keycode.PageUp, new ButtonDescription { Name = KeyCodes.Prior.ToString(), Id = (int)KeyCodes.Prior });
            Add(Keycode.PageDown, new ButtonDescription { Name = KeyCodes.Next.ToString(), Id = (int)KeyCodes.Next });
            // this.Add(Keycode.PrintScreen,  new ButtonDescription {Name = KeyCodes.Print.ToString(), Id = (int)KeyCodes.Print});
            // this.Add(Keycode.Pause,  new ButtonDescription {Name = KeyCodes.Pause.ToString(), Id = (int)KeyCodes.Pause});
            Add(Keycode.NumLock, new ButtonDescription { Name = KeyCodes.NumLock.ToString(), Id = (int)KeyCodes.NumLock });

            Add(Keycode.ScrollLock, new ButtonDescription { Name = KeyCodes.Scroll.ToString(), Id = (int)KeyCodes.Scroll });
            // Do we need to do something here?? this.Add(Keycode.PrintScreen,  new ButtonDescription {Name = KeyCodes.Snapshot.ToString(), Id = (int)KeyCodes.Snapshot});
            Add(Keycode.Clear, new ButtonDescription { Name = KeyCodes.Clear.ToString(), Id = (int)KeyCodes.Clear });
            Add(Keycode.Insert, new ButtonDescription { Name = KeyCodes.Insert.ToString(), Id = (int)KeyCodes.Insert });

            Add(Keycode.Sleep, new ButtonDescription { Name = KeyCodes.Sleep.ToString(), Id = (int)KeyCodes.Sleep });

            // Keypad
            for (int i = 0; i <= 9; i++)
            {
                Add(Keycode.Numpad0 + i, new ButtonDescription { Name = $"Numpad{i}", Id = (int)KeyCodes.NumPad0 + i });
            }

            Add(Keycode.NumpadDot, new ButtonDescription { Name = KeyCodes.Decimal.ToString(), Id = (int)KeyCodes.Decimal });
            Add(Keycode.NumpadAdd, new ButtonDescription { Name = KeyCodes.Add.ToString(), Id = (int)KeyCodes.Add });
            Add(Keycode.NumpadSubtract, new ButtonDescription { Name = KeyCodes.Subtract.ToString(), Id = (int)KeyCodes.Subtract });
            Add(Keycode.NumpadDivide, new ButtonDescription { Name = KeyCodes.Divide.ToString(), Id = (int)KeyCodes.Divide });
            Add(Keycode.NumpadMultiply, new ButtonDescription { Name = KeyCodes.Multiply.ToString(), Id = (int)KeyCodes.Multiply });

            // Navigation
            Add(Keycode.DpadUp, new ButtonDescription { Name = KeyCodes.Up.ToString(), Id = (int)KeyCodes.Up });
            Add(Keycode.DpadDown, new ButtonDescription { Name = KeyCodes.Down.ToString(), Id = (int)KeyCodes.Down });
            Add(Keycode.DpadLeft, new ButtonDescription { Name = KeyCodes.Left.ToString(), Id = (int)KeyCodes.Left });
            Add(Keycode.DpadRight, new ButtonDescription { Name = KeyCodes.Right.ToString(), Id = (int)KeyCodes.Right });
            /*
            catch (ArgumentException e)
            {
                //Debug.Print("Exception while creating keymap: '{0}'.", e.ToString());
                System.Windows.Forms.MessageBox.Show(
                    String.Format("Exception while creating keymap: '{0}'.", e.ToString()));
            }
           */
        }

        #endregion Constructors
    }
}