using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class Renderer : Component
    {
        public Mesh mesh;
        public Material material;
        public float4 color;
        public Renderer()
        {
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            mesh = geo.ToMesh();
            material = new Material();
            color = new float4(1,0,0,1);
        }

    }
}
