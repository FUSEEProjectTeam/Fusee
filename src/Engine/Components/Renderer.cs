using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace SceneManagement
{
    public class Renderer : Component
    {
        public Mesh mesh;
        public Material material;

        public Renderer()
        {
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Cube.obj.model"));
            mesh = geo.ToMesh();
        }
        public override void Traverse(ITraversalState _traversalState)
        {
            _traversalState.StoreMesh(mesh);
        }

    }
}
