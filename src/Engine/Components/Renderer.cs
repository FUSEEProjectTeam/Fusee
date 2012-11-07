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
        private readonly int _id = 3;
        public Mesh mesh;
        public Material material;

        public Renderer()
        {
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Cube.obj.model"));
            mesh = geo.ToMesh();
        }
        public override int GETID()
        {
            return _id;
        }
    }
}
