using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class RenderCamera : RenderJob
    {
        private float4x4 _matrix;

        public RenderCamera(float4x4 matrix)
        {
            _matrix = matrix;
        }

        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.View=_matrix;
        }
    }
}
