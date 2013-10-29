using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.WorldRendering
{
    [FuseeApplication(Name = "World Rendering", Description = "Shows some application of texture functionality inlcuding text rendering.")]
    public class WorldRendering : RenderCanvas
    {
        #region Shader

        // At first we have to define the shader.
        protected string VsSimpleTexture = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;

            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        protected string PsSimpleTexture = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            // The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;

            // The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              // The most basic texturing function, expecting the above mentioned parameters   
              gl_FragColor = texture2D(texture1, vUV);        
            }";

        #endregion

        private World _world;

        protected IShaderParam[] Param;
        protected IShaderParam Texture1Param;

        public override void Init()
        {
            _world = new World(RC);

            // load mesh as geometry
            var geo1 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));

            // create and set shader
            var sp = RC.CreateShader(VsSimpleTexture, PsSimpleTexture);
            RC.SetShader(sp);

            var material = new ShaderMaterial(sp);

            // load a texture and write a text on it
            var imgData = RC.LoadImage("Assets/cube_tex.jpg");
            imgData = RC.TextOnImage(imgData, "Verdana", 80f, "FUSEE rocks!", "Black", 0, 30);

            var iTex = RC.CreateTexture(imgData);

            Texture1Param = sp.GetShaderParam("texture1");
            RC.SetShaderParamTexture(Texture1Param, iTex);

            // add object with material
            _world.AddObject(geo1, material, 0, 0, 500);

            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _world.RenderWorld();
            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new WorldRendering();
            app.Run();
        }
    }
}