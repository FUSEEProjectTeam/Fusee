using Fusee.Base.Core;
using Fusee.Engine.Core;

namespace Fusee.Engine.Examples.SoundTest.Web
{
    public class SoundTest
    {
        public static void Main()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            // We're using *Desktop*.IOImp here because JSIL can (still) xcompile it.
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            var app = new Core.SoundTest();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderCanvasImp();
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Web.RenderCanvasInputDriverImp(app.CanvasImplementor));
            app.AudioImplementor = new Fusee.Engine.Imp.Sound.Web.AudioImp();
            // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Web.NetworkImp();
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Web.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            // Start the app
            app.Run();
        }
    }
}
