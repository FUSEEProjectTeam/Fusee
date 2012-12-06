using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class RenderJob
    {
        virtual public float4x4 GetMatrix()
        {
            return float4x4.Identity;
        }
        virtual public Mesh GetMesh()
        {
            return null;
        }
        virtual public Renderer GetRenderer()
        {
            return null;
        }

    }
}
