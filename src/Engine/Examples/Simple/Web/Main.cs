using System.Collections;
using System.Collections.Generic;
using Fusee.Base.Core;

namespace Fusee.Engine.Examples.Simple.Web
{
    class Simple
    {
        public static void Main()
        {
            //var daStack = new Stack<int>();
            //daStack.Push(2);
            //daStack.Push(3);
            //daStack.Push(4);
            //daStack.Push(5);
            // var eni = daStack.GetEnumerator();
            // eni.MoveNext();
            // var ons = eni.Current;

            // Inject Fusee.Engine.Base InjectMe dependencies
            // We're using *Desktop*.IOImp here because JSIL can (still) xcompile it.
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            var app = new global::Examples.Simple.Simple();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderCanvasImp();
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderContextImp(app.CanvasImplementor);
            app.InputImplementor = new Fusee.Engine.Imp.Graphics.Web.InputImp(app.CanvasImplementor);
            // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Web.AudioImp();
            // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Web.NetworkImp();
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Web.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            // Start the app
            app.Run();
        }
    }
}
