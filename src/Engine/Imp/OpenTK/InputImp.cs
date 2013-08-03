using System;
using OpenTK;
using OpenTK.Input;

namespace Fusee.Engine
{
    public class InputImp : IInputImp
    {
        protected GameWindow _gameWindow;
        internal Keymapper KeyMapper;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");

            _gameWindow = ((RenderCanvasImp) renderCanvas)._gameWindow;
            if (_gameWindow != null)
            {
                _gameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
                _gameWindow.Keyboard.KeyUp += OnGameWinKeyUp;
                _gameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
                _gameWindow.Mouse.ButtonUp += OnGameWinMouseUp;
            }
            else
            {
                // Todo

            }

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
            if (_gameWindow != null)
                return new Point{x = _gameWindow.Mouse.X, y = _gameWindow.Mouse.Y};
            return new Point{x=0, y=0};
        }

        public int GetMouseWheelPos()
        {
            if (_gameWindow != null)
                return _gameWindow.Mouse.Wheel;
            return 0;
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
