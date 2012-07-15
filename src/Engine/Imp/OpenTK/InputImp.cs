using System;
using OpenTK;
using OpenTK.Input;

namespace Fusee.Engine
{
    public class InputImp : IInputImp
    {
        protected GameWindow _gameWindow;
        internal Keymapper _keyMapper;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");
            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");
            _gameWindow = ((RenderCanvasImp) renderCanvas)._gameWindow;
            _gameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
            _gameWindow.Keyboard.KeyUp += OnGameWinKeyUp;
            _gameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
            _gameWindow.Mouse.ButtonUp += OnGameWinMouseUp;

            _keyMapper = new Keymapper();
        }

        public void FrameTick(double time)
        {
            // Do Nothing
        }

        public Point GetMousePos()
        {
            return new Point{x = _gameWindow.Mouse.X, y = _gameWindow.Mouse.Y};
        }

        public int GetMouseWheelPos()
        {
            return _gameWindow.Mouse.Wheel;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;

        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonDown != null)
            {
                MouseButtons mb = MouseButtons.Unknown;
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
                                             Position = new Point{x=mouseArgs.X, y=mouseArgs.Y}
                                          });   
            }
        }

        public event EventHandler<MouseEventArgs> MouseButtonUp;

        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonUp != null)
            {
                MouseButtons mb = MouseButtons.Unknown;
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
                    Position = new Point { x = mouseArgs.X, y = mouseArgs.Y }
                });
            }
        }
      
        public event EventHandler<KeyEventArgs> KeyDown;

        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyDown != null)
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyDown(this, new KeyEventArgs
                                  {
                                      Alt = false,
                                      Control = false,
                                      Shift = false,
                                      KeyCode = _keyMapper[key.Key],
                                  });
            }
        }

        public event EventHandler<KeyEventArgs> KeyUp;

        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyUp != null)
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyUp(this, new KeyEventArgs
                {
                    Alt = false,
                    Control = false,
                    Shift = false,
                    KeyCode = _keyMapper[key.Key],
                });
            }
        }
    }
}
