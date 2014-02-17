using System;
using OpenTK;
using OpenTK.Input;

namespace Fusee.Engine
{
    /// <summary>
    /// This class accesses the underlying OpenTK adapter and is the implementation of the input interface <see cref="IInputImp" />.
    /// </summary>
    public class InputImp : IInputImp
    {
        #region Fields
        /// <summary>
        /// The game window where the content will be rendered to.
        /// </summary>
        protected GameWindow _gameWindow;
        internal Keymapper KeyMapper;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InputImp"/> class.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <exception cref="System.ArgumentNullException">renderCanvas</exception>
        /// <exception cref="System.ArgumentException">renderCanvas must be of type RenderCanvasImp;renderCanvas</exception>
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
                _gameWindow.Mouse.Move += OnGameWinMouseMove;
            }
            else
            {
                // Todo

            }

            KeyMapper = new Keymapper();
        }
        #endregion

        #region Members
        /// <summary>
        /// Implement this to receive callbacks once a frame if your implementation needs
        /// regular updates.
        /// </summary>
        /// <param name="time">The elapsed time since the last frame.</param>
        public void FrameTick(double time)
        {
            // do nothing
        }

        /// <summary>
        /// Sets the mouse cursor to the center of the GameWindow.
        /// </summary>
        /// <returns>A Point with x,y,z properties.</returns>
        public Point SetMouseToCenter()
        {
            var ctrPoint = GetMousePos();

            if (_gameWindow.Focused)
            {
                ctrPoint.x = _gameWindow.Bounds.Left + (_gameWindow.Width/2);
                ctrPoint.y = _gameWindow.Bounds.Top + (_gameWindow.Height/2);
                Mouse.SetPosition(ctrPoint.x, ctrPoint.y);
            }

            return ctrPoint;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the cursor is visible; otherwise, <c>false</c>.
        /// </value>
        public bool CursorVisible
        {
            get { return _gameWindow.CursorVisible; }
            set { _gameWindow.CursorVisible = value; }
        }

        /// <summary>
        /// Sets the mouse position by using X and Y values (in pixel units).
        /// </summary>
        /// <param name="pos">The point containing window X and Y values.</param>
        public void SetMousePos(Point pos)
        {
            Mouse.SetPosition(pos.x, pos.y);
        }

        /// <summary>
        /// Retrieve the position(x,y values in pixel units) of the Mouse.
        /// </summary>
        /// <returns>
        /// The point containing window X and Y values.
        /// If gamewindow is null 0,0 position is returned.
        /// </returns>
        public Point GetMousePos()
        {
            if (_gameWindow != null)
                return new Point{x = _gameWindow.Mouse.X, y = _gameWindow.Mouse.Y};
            return new Point{x=0, y=0};
        }

        /// <summary>
        /// Implement this to return the absolute mouse wheel position
        /// </summary>
        /// <returns>
        /// The mouse wheel position.
        /// </returns>
        public int GetMouseWheelPos()
        {
            if (_gameWindow != null)
                return _gameWindow.Mouse.Wheel;
            return 0;
        }

        /// <summary>
        /// Trigger this event on any mouse button pressed down (and held).
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonDown;

        /// <summary>
        /// Called when [game win mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Trigger this event on any mouse button release.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonUp;

        /// <summary>
        /// Called when mouse button is released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Trigger this event on any mouse movement.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Called when mouse is moving.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseArgs">The <see cref="MouseMoveEventArgs"/> instance containing the event data.</param>
        protected void OnGameWinMouseMove(object sender, MouseMoveEventArgs mouseArgs)
        {
            if (MouseMove != null)
            {
                MouseMove(this, new MouseEventArgs
                {
                    Button = MouseButtons.Unknown,
                    Position = new Point { x = mouseArgs.X, y = mouseArgs.Y }
                });
            }
        }

        /// <summary>
        /// Trigger this event once a key on the keyboard is pressed down.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Called when mouse button is pressed down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Trigger this event in your implementation once a key on the keyboard is released.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;

        /// <summary>
        /// Called when keyboard key has been released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The <see cref="KeyboardKeyEventArgs"/> instance containing the event data.</param>
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

        
        #endregion
    }
}
