using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
namespace Fusee.SceneManagement
{
    public class RenderMatrix : RenderJob
    {
        private float4x4 _matrix;
        public RenderMatrix(float4x4 matrix)
        {
            _matrix = matrix;
        }

        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.ModelView = _matrix*renderContext.Camera;
        }
    }
}
