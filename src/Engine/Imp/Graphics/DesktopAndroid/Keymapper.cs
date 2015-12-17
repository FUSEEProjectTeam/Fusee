using System.Collections.Generic;
using OpenTK.Input;
using Fusee.Engine.Common;



#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    internal class Keymapper : Dictionary<OpenTK.Input.Key, ButtonDescription>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal Keymapper()
        {
            this.Add(Key.Escape, new ButtonDescription {Name = KeyCodes.Escape.ToString(), Id = (int)KeyCodes.Escape});

            // Function keys
            for (int i = 0; i < 24; i++)
            {
                this.Add(Key.F1 + i, new ButtonDescription { Name = $"F{i}", Id = (int)KeyCodes.F1 + i });
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.Number0 + i, new ButtonDescription { Name = $"D{i}", Id = (int)0x30 + i } );
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(Key.A + i, new ButtonDescription { Name = ((KeyCodes)(0x41 + i)).ToString(), Id = (int)(0x41 + i) });
            }

            this.Add(Key.Tab,  new ButtonDescription {Name = KeyCodes.Tab.ToString(), Id = (int)KeyCodes.Tab});
            this.Add(Key.CapsLock,  new ButtonDescription {Name = KeyCodes.Capital.ToString(), Id = (int)KeyCodes.Capital});
            this.Add(Key.ControlLeft,  new ButtonDescription {Name = KeyCodes.LControl.ToString(), Id = (int)KeyCodes.LControl});
            this.Add(Key.ShiftLeft,  new ButtonDescription {Name = KeyCodes.LShift.ToString(), Id = (int)KeyCodes.LShift});
            this.Add(Key.WinLeft,  new ButtonDescription {Name = KeyCodes.LWin.ToString(), Id = (int)KeyCodes.LWin});
            this.Add(Key.AltLeft,  new ButtonDescription {Name = KeyCodes.LMenu.ToString(), Id = (int)KeyCodes.LMenu});
            this.Add(Key.Space,  new ButtonDescription {Name = KeyCodes.Space.ToString(), Id = (int)KeyCodes.Space});
            this.Add(Key.AltRight,  new ButtonDescription {Name = KeyCodes.RMenu.ToString(), Id = (int)KeyCodes.RMenu});
            this.Add(Key.WinRight,  new ButtonDescription {Name = KeyCodes.RWin.ToString(), Id = (int)KeyCodes.RWin});
            this.Add(Key.Menu,  new ButtonDescription {Name = KeyCodes.Apps.ToString(), Id = (int)KeyCodes.Apps});
            this.Add(Key.ControlRight,  new ButtonDescription {Name = KeyCodes.RControl.ToString(), Id = (int)KeyCodes.RControl});
            this.Add(Key.ShiftRight,  new ButtonDescription {Name = KeyCodes.RShift.ToString(), Id = (int)KeyCodes.RShift});
            this.Add(Key.Enter,  new ButtonDescription {Name = KeyCodes.Return.ToString(), Id = (int)KeyCodes.Return});
            this.Add(Key.BackSpace,  new ButtonDescription {Name = KeyCodes.Back.ToString(), Id = (int)KeyCodes.Back});

            this.Add(Key.Semicolon,  new ButtonDescription {Name = KeyCodes.Oem1.ToString(), Id = (int)KeyCodes.Oem1});
            this.Add(Key.Slash,  new ButtonDescription {Name = KeyCodes.Oem2.ToString(), Id = (int)KeyCodes.Oem2});
            this.Add(Key.Tilde,  new ButtonDescription {Name = KeyCodes.Oem3.ToString(), Id = (int)KeyCodes.Oem3});
            this.Add(Key.BracketLeft,  new ButtonDescription {Name = KeyCodes.Oem4.ToString(), Id = (int)KeyCodes.Oem4});
            this.Add(Key.BackSlash,  new ButtonDescription {Name = KeyCodes.Oem5.ToString(), Id = (int)KeyCodes.Oem5});
            this.Add(Key.BracketRight,  new ButtonDescription {Name = KeyCodes.Oem6.ToString(), Id = (int)KeyCodes.Oem6});
            this.Add(Key.Quote,  new ButtonDescription {Name = KeyCodes.Oem7.ToString(), Id = (int)KeyCodes.Oem7});
            this.Add(Key.Plus,  new ButtonDescription {Name = KeyCodes.OemPlus.ToString(), Id = (int)KeyCodes.OemPlus});
            this.Add(Key.Comma,  new ButtonDescription {Name = KeyCodes.OemComma.ToString(), Id = (int)KeyCodes.OemComma});
            this.Add(Key.Minus,  new ButtonDescription {Name = KeyCodes.OemMinus.ToString(), Id = (int)KeyCodes.OemMinus});
            this.Add(Key.Period,  new ButtonDescription {Name = KeyCodes.OemPeriod.ToString(), Id = (int)KeyCodes.OemPeriod});

            this.Add(Key.Home,  new ButtonDescription {Name = KeyCodes.Home.ToString(), Id = (int)KeyCodes.Home});
            this.Add(Key.End,  new ButtonDescription {Name = KeyCodes.End.ToString(), Id = (int)KeyCodes.End});
            this.Add(Key.Delete,  new ButtonDescription {Name = KeyCodes.Delete.ToString(), Id = (int)KeyCodes.Delete});
            this.Add(Key.PageUp,  new ButtonDescription {Name = KeyCodes.Prior.ToString(), Id = (int)KeyCodes.Prior});
            this.Add(Key.PageDown,  new ButtonDescription {Name = KeyCodes.Next.ToString(), Id = (int)KeyCodes.Next});
            this.Add(Key.PrintScreen,  new ButtonDescription {Name = KeyCodes.Print.ToString(), Id = (int)KeyCodes.Print});
            this.Add(Key.Pause,  new ButtonDescription {Name = KeyCodes.Pause.ToString(), Id = (int)KeyCodes.Pause});
            this.Add(Key.NumLock,  new ButtonDescription {Name = KeyCodes.NumLock.ToString(), Id = (int)KeyCodes.NumLock});

            this.Add(Key.ScrollLock,  new ButtonDescription {Name = KeyCodes.Scroll.ToString(), Id = (int)KeyCodes.Scroll});
            // Do we need to do something here?? this.Add(Key.PrintScreen,  new ButtonDescription {Name = KeyCodes.Snapshot.ToString(), Id = (int)KeyCodes.Snapshot});
            this.Add(Key.Clear,  new ButtonDescription {Name = KeyCodes.Clear.ToString(), Id = (int)KeyCodes.Clear});
            this.Add(Key.Insert,  new ButtonDescription {Name = KeyCodes.Insert.ToString(), Id = (int)KeyCodes.Insert});

            this.Add(Key.Sleep,  new ButtonDescription {Name = KeyCodes.Sleep.ToString(), Id = (int)KeyCodes.Sleep});

            // Keypad
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.Keypad0 + i, new ButtonDescription { Name = $"Numpad{i}", Id = (int)KeyCodes.NumPad0 + i});
            }

            this.Add(Key.KeypadDecimal,  new ButtonDescription {Name = KeyCodes.Decimal.ToString(), Id = (int)KeyCodes.Decimal});
            this.Add(Key.KeypadAdd,  new ButtonDescription {Name = KeyCodes.Add.ToString(), Id = (int)KeyCodes.Add});
            this.Add(Key.KeypadSubtract,  new ButtonDescription {Name = KeyCodes.Subtract.ToString(), Id = (int)KeyCodes.Subtract});
            this.Add(Key.KeypadDivide,  new ButtonDescription {Name = KeyCodes.Divide.ToString(), Id = (int)KeyCodes.Divide});
            this.Add(Key.KeypadMultiply,  new ButtonDescription {Name = KeyCodes.Multiply.ToString(), Id = (int)KeyCodes.Multiply});

            // Navigation
            this.Add(Key.Up,  new ButtonDescription {Name = KeyCodes.Up.ToString(), Id = (int)KeyCodes.Up});
            this.Add(Key.Down,  new ButtonDescription {Name = KeyCodes.Down.ToString(), Id = (int)KeyCodes.Down});
            this.Add(Key.Left,  new ButtonDescription {Name = KeyCodes.Left.ToString(), Id = (int)KeyCodes.Left});
            this.Add(Key.Right,  new ButtonDescription {Name = KeyCodes.Right.ToString(), Id = (int)KeyCodes.Right});
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



#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    internal class KeymapperOld : Dictionary<OpenTK.Input.Key, KeyCodes>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal KeymapperOld()
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
