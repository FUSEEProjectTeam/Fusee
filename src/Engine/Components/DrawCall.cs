using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace SceneManagement
{
    public struct DrawCall
    {
        public Transformation _Transform;
        public Renderer _Renderer;
        public Mesh _Mesh;

        public DrawCall(Transformation transform, Renderer renderer, Mesh mesh)
        {
            _Transform = transform;
            _Renderer = renderer;
            _Mesh = mesh;
        }
    }
}
