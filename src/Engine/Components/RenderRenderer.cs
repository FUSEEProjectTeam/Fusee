using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.SceneManagement
{
    public class RenderRenderer : RenderJob
    {
        private Renderer _renderer;
        public RenderRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }
        public override Renderer GetRenderer()
        {
            return _renderer;
        }
    }
}
