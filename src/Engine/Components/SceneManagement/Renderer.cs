using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Renderer is derived from Component. Creates a usable ingame object.
    /// </summary>
    public class Renderer : Component
    {
        #region public fields
        public Mesh mesh;
        public Material material;
        public float4 color;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class. Creates usable ingame object from an obj.model file, and a new material.
        /// </summary>
        public Renderer()
        {

            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            mesh = geo.ToMesh();
            material = new Material();
            color = new float4(1,0,0,1);
        }
        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((Renderer)this);
        }
        #endregion
    }
}
