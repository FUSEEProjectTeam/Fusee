using System;
using OpenTK;
using OpenTK.Input;

namespace Fusee.Engine
{
    public class InputImp : IInputImp
    {
        protected GameWindow GameWindow;
        internal Keymapper KeyMapper;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");

            GameWindow = ((RenderCanvasImp) renderCanvas)._gameWindow;
            GameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
            GameWindow.Keyboard.KeyUp += OnGameWinKeyUp;
            GameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
            GameWindow.Mouse.ButtonUp += OnGameWinMouseUp;

            KeyMapper = new Keymapper();
        }

        public void FrameTick(double time)
        {
            // do nothing
        }

        public Point SetMouseToCenter()
        {
            var ctrPoint = new Point
                {
                    x = GameWindow.Bounds.Left + (GameWindow.Width/2),
                    y = GameWindow.Bounds.Top + (GameWindow.Height/2)
                };

            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(ctrPoint.x, ctrPoint.y);

            return ctrPoint;
        }

        public bool CursorVisible
        {
            get { return GameWindow.CursorVisible; }
            set { GameWindow.CursorVisible = value; }
        }

        public void SetMousePos(Point pos)
        {
            Mouse.SetPosition(pos.x, pos.y);
        }

        public Point GetMousePos()
        {
            return new Point{x = GameWindow.Mouse.X, y = GameWindow.Mouse.Y};
        }

        public int GetMouseWheelPos()
        {
            return GameWindow.Mouse.Wheel;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;

        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonDown != null)
            {
                var mb = MouseButtons.Unknown;

                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                }

                MouseButtonDown(this, new MouseEventArgs
                    {
                        Button = mb,
                        Position = new Point {x = mouseArgs.X, y = mouseArgs.Y}
                    });
            }
        }

        public event EventHandler<MouseEventArgs> MouseButtonUp;

        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonUp != null)
            {
                var mb = MouseButtons.Unknown;

                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                }

                MouseButtonUp(this, new MouseEventArgs
                    {
                        Button = mb,
                        Position = new Point {x = mouseArgs.X, y = mouseArgs.Y}
                    });
            }
        }
      
        public event EventHandler<KeyEventArgs> KeyDown;

        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyDown != null && KeyMapper.ContainsKey(key.Key))
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyDown(this, new KeyEventArgs
                    {
                        Alt = false,
                        Control = false,
                        Shift = false,
                        KeyCode = KeyMapper[key.Key],
                    });
            }
        }

        public event EventHandler<KeyEventArgs> KeyUp;

        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyUp != null && KeyMapper.ContainsKey(key.Key))
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyUp(this, new KeyEventArgs
                    {
                        Alt = false,
                        Control = false,
                        Shift = false,
                        KeyCode = KeyMapper[key.Key],
                    });
            }
        }
    }
}
