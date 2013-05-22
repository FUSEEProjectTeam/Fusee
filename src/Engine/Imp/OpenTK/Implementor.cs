using System;

namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class RenderingImplementor
    {
        public static IRenderCanvasImp CreateRenderCanvasImp()
        {
            return new RenderCanvasImp();
        }

        public static IRenderContextImp CreateRenderContextImp(IRenderCanvasImp rci)
        {
            return new RenderContextImp(rci);
        }

        public static IInputImp CreateInputImp(IRenderCanvasImp rci)
        {
            return new InputImp(rci);
        }
    }
}
