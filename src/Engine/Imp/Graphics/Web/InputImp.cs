// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    public class InputImp : IInputImp
    {
        [JSExternal]
        public InputImp(IRenderCanvasImp canvasImplementor)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void FrameTick(double time)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void SetMousePos(Point pos)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public Point SetMouseToCenter()
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public bool CursorVisible { get; set; }

        [JSExternal]
        public Point GetMousePos()
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public int GetMouseWheelPos()
        {
            throw new NotImplementedException();
        }

#pragma warning disable 0067 // events are used in external javascript
        public event EventHandler<MouseEventArgs> MouseButtonDown;
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
#pragma warning restore 0067
    }
}
