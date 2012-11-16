using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace SceneManagement
{
    public class RenderMesh : RenderJob
    {
        private Mesh _mesh;

        public RenderMesh(Mesh mesh)
        {
            _mesh = mesh;
        }
        public override Mesh GetMesh()
        {
            return _mesh;
        }
    }
}
