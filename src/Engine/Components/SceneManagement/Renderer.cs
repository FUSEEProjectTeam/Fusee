using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Renderer is derived from Component. Creates a drawable ingame object.
    /// </summary>
    public class Renderer : Component
    {
        #region Fields
        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh mesh;
        /// <summary>
        /// The material.
        /// </summary>
        public Material material;
        /// <summary>
        /// The color.
        /// </summary>
        public float4 color;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class. Creates usable ingame object from an obj.model file, and a new material.
        /// </summary>
        public Renderer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="sp">The Shaderprogram is used to initialize the Material<see cref="Material"/> of this Renderer.</param>
        public Renderer (ShaderProgram sp)
        {
            material = new Material(sp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="geo">The geometry information is parsed to a Mesh<see cref="Mesh"/> object upon initilization of Renderer.</param>
        public Renderer(Geometry geo)
        {
            mesh = geo.ToMesh();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="mesh">The mesh of this Renderer.</param>
        public Renderer(Mesh mesh)
        {
            this.mesh = mesh;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="mesh">The Mesh of this Renderer.</param>
        /// <param name="sp">The Shaderprogram is used to initialize the Material of the Renderer.</param>
        public Renderer(Mesh mesh, ShaderProgram sp)
        {
            this.mesh = mesh;
            material = new Material(sp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="geo">The geo.</param>
        /// <param name="shaderProgram">The shader program.</param>
        public Renderer(Geometry geo, ShaderProgram shaderProgram)
        {
            mesh = geo.ToMesh();
            material = new Material(shaderProgram);
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Passes the Component to the SceneVisitor which decides what to do with that Component.
        /// </summary>
        /// <param name="sv">The SceneVisitor that traverses through the information of the renderer during runtime.</param>
        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((Renderer)this);
        }
        #endregion
    }
}
