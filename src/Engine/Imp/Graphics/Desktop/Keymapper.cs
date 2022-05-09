using Fusee.Engine.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    internal class Keymapper : Dictionary<Keys, ButtonDescription>
    {
        #region Constructors
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal Keymapper()
        {
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape, new ButtonDescription { Name = KeyCodes.Escape.ToString(), Id = (int)KeyCodes.Escape });

            // Function keys
            for (int i = 0; i < 24; i++)
            {
                this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F1 + i, new ButtonDescription { Name = $"F{i}", Id = (int)KeyCodes.F1 + i });
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D0 + i, new ButtonDescription { Name = $"D{i}", Id = (int)0x30 + i });
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A + i, new ButtonDescription { Name = ((KeyCodes)(0x41 + i)).ToString(), Id = (int)(0x41 + i) });
            }

            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Tab, new ButtonDescription { Name = KeyCodes.Tab.ToString(), Id = (int)KeyCodes.Tab });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.CapsLock, new ButtonDescription { Name = KeyCodes.Capital.ToString(), Id = (int)KeyCodes.Capital });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl, new ButtonDescription { Name = KeyCodes.LControl.ToString(), Id = (int)KeyCodes.LControl });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift, new ButtonDescription { Name = KeyCodes.LShift.ToString(), Id = (int)KeyCodes.LShift });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftSuper, new ButtonDescription { Name = KeyCodes.LWin.ToString(), Id = (int)KeyCodes.LWin });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftAlt, new ButtonDescription { Name = KeyCodes.LMenu.ToString(), Id = (int)KeyCodes.LMenu });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space, new ButtonDescription { Name = KeyCodes.Space.ToString(), Id = (int)KeyCodes.Space });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightAlt, new ButtonDescription { Name = KeyCodes.RMenu.ToString(), Id = (int)KeyCodes.RMenu });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightSuper, new ButtonDescription { Name = KeyCodes.RWin.ToString(), Id = (int)KeyCodes.RWin });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Menu, new ButtonDescription { Name = KeyCodes.Apps.ToString(), Id = (int)KeyCodes.Apps });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightControl, new ButtonDescription { Name = KeyCodes.RControl.ToString(), Id = (int)KeyCodes.RControl });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightShift, new ButtonDescription { Name = KeyCodes.RShift.ToString(), Id = (int)KeyCodes.RShift });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter, new ButtonDescription { Name = KeyCodes.Return.ToString(), Id = (int)KeyCodes.Return });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace, new ButtonDescription { Name = KeyCodes.Back.ToString(), Id = (int)KeyCodes.Back });

            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Semicolon, new ButtonDescription { Name = KeyCodes.Oem1.ToString(), Id = (int)KeyCodes.Oem1 });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Slash, new ButtonDescription { Name = KeyCodes.Oem2.ToString(), Id = (int)KeyCodes.Oem2 });
            //this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Tilde, new ButtonDescription { Name = KeyCodes.Oem3.ToString(), Id = (int)KeyCodes.Oem3 });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftBracket, new ButtonDescription { Name = KeyCodes.Oem4.ToString(), Id = (int)KeyCodes.Oem4 });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backslash, new ButtonDescription { Name = KeyCodes.Oem5.ToString(), Id = (int)KeyCodes.Oem5 });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightBracket, new ButtonDescription { Name = KeyCodes.Oem6.ToString(), Id = (int)KeyCodes.Oem6 });
            //this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Quote, new ButtonDescription { Name = KeyCodes.Oem7.ToString(), Id = (int)KeyCodes.Oem7 });
            //this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Plus, new ButtonDescription { Name = KeyCodes.OemPlus.ToString(), Id = (int)KeyCodes.OemPlus });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Comma, new ButtonDescription { Name = KeyCodes.OemComma.ToString(), Id = (int)KeyCodes.OemComma });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Minus, new ButtonDescription { Name = KeyCodes.OemMinus.ToString(), Id = (int)KeyCodes.OemMinus });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Period, new ButtonDescription { Name = KeyCodes.OemPeriod.ToString(), Id = (int)KeyCodes.OemPeriod });

            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Home, new ButtonDescription { Name = KeyCodes.Home.ToString(), Id = (int)KeyCodes.Home });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.End, new ButtonDescription { Name = KeyCodes.End.ToString(), Id = (int)KeyCodes.End });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Delete, new ButtonDescription { Name = KeyCodes.Delete.ToString(), Id = (int)KeyCodes.Delete });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageUp, new ButtonDescription { Name = KeyCodes.Prior.ToString(), Id = (int)KeyCodes.Prior });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageDown, new ButtonDescription { Name = KeyCodes.Next.ToString(), Id = (int)KeyCodes.Next });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.PrintScreen, new ButtonDescription { Name = KeyCodes.Print.ToString(), Id = (int)KeyCodes.Print });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Pause, new ButtonDescription { Name = KeyCodes.Pause.ToString(), Id = (int)KeyCodes.Pause });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.NumLock, new ButtonDescription { Name = KeyCodes.NumLock.ToString(), Id = (int)KeyCodes.NumLock });

            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.ScrollLock, new ButtonDescription { Name = KeyCodes.Scroll.ToString(), Id = (int)KeyCodes.Scroll });
            // Do we need to do something here?? this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.PrintScreen,  new ButtonDescription {Name = KeyCodes.Snapshot.ToString(), Id = (int)KeyCodes.Snapshot});
            //this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Clear, new ButtonDescription { Name = KeyCodes.Clear.ToString(), Id = (int)KeyCodes.Clear });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Insert, new ButtonDescription { Name = KeyCodes.Insert.ToString(), Id = (int)KeyCodes.Insert });

            //this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Sleep, new ButtonDescription { Name = KeyCodes.Sleep.ToString(), Id = (int)KeyCodes.Sleep });

            // KeyPad
            for (int i = 0; i <= 9; i++)
            {
                this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad0 + i, new ButtonDescription { Name = $"Numpad{i}", Id = (int)KeyCodes.NumPad0 + i });
            }

            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDecimal, new ButtonDescription { Name = KeyCodes.Decimal.ToString(), Id = (int)KeyCodes.Decimal });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadAdd, new ButtonDescription { Name = KeyCodes.Add.ToString(), Id = (int)KeyCodes.Add });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadSubtract, new ButtonDescription { Name = KeyCodes.Subtract.ToString(), Id = (int)KeyCodes.Subtract });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDivide, new ButtonDescription { Name = KeyCodes.Divide.ToString(), Id = (int)KeyCodes.Divide });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadMultiply, new ButtonDescription { Name = KeyCodes.Multiply.ToString(), Id = (int)KeyCodes.Multiply });

            // Navigation
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up, new ButtonDescription { Name = KeyCodes.Up.ToString(), Id = (int)KeyCodes.Up });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down, new ButtonDescription { Name = KeyCodes.Down.ToString(), Id = (int)KeyCodes.Down });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left, new ButtonDescription { Name = KeyCodes.Left.ToString(), Id = (int)KeyCodes.Left });
            this.Add(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right, new ButtonDescription { Name = KeyCodes.Right.ToString(), Id = (int)KeyCodes.Right });
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