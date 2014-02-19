using System;

namespace Fusee.Engine
{
    
    /// <summary>
    /// This class is used to return the correct implementations of <see cref="RenderCanvasImp" />, <see cref="RenderContextImp" /> and <see cref="InputImp" />.
    /// The output depends on the underlying plattform.
    /// This class is instantiated dynamically (by reflection).
    /// </summary>
    public class RenderingImplementor
    {
        /// <summary>
        /// Creates the render canvas implementation.
        /// </summary>
        /// <returns>A new instance of <see cref="RenderCanvasImp" /></returns>
        public static IRenderCanvasImp CreateRenderCanvasImp()
        {
            return new RenderCanvasImp();
        }

        /// <summary>
        /// Creates the render context implementation.
        /// </summary>
        /// <param name="rci">The RenderCanvas implementation interface derivate.</param>
        /// <returns>A new instance of <see cref="RenderContextImp" /></returns>
        public static IRenderContextImp CreateRenderContextImp(IRenderCanvasImp rci)
        {
            return new RenderContextImp(rci);
        }

        /// <summary>
        /// Creates the input implementation.
        /// </summary>
        /// <param name="rci">The RenderCanvas implementation interface derivate.</param>
        /// <returns>A new instance of <see cref="InputImp" /></returns>
        public static IInputImp CreateInputImp(IRenderCanvasImp rci)
        {
            return new InputImp(rci);
        }
    }
}
