using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.Test.GameWindow.Desktop;
using System;
using Xunit;

namespace Fusee.Tests.GameWindow.Desktop
{
    public class GameWindowTests
    {
        [Fact]
        public void WindowHandleTest()
        {
            Program.Example = new Fusee.Examples.Simple.Core.Simple() { };
            Program.Init();

            Assert.NotEqual(IntPtr.Zero, ((WindowHandle)Program.Example.CanvasImplementor.WindowHandle).Handle);

            Program.Example.CloseGameWindow();
        }
    }
}