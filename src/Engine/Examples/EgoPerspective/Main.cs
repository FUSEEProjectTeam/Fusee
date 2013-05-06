using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.EgoPerspective
{
    public class EgoPerspective : RenderCanvas
    {
        //At first we have to define the shader.
        protected string VsSimpleTexture = @"
            #ifndef GL_ES
               #version 120
            #endif

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
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        protected string PsSimpleTexture = @"
           #ifndef GL_ES
              #version 120
           #endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            //The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;
            //The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              //The most basic texturing function, expecting the above mentioned parameters   
              gl_FragColor = texture2D(texture1, vUV);        
            }";

        private World _world;
        protected ShaderProgram Sp;
        protected IShaderParam[] Param;
        protected ShaderMaterial M;
        protected IShaderParam Texture1Param;


        public override void Init()
        {
            _world = new World(RC);
            Geometry geo1 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));

            Sp = RC.CreateShader(VsSimpleTexture, PsSimpleTexture);
            M = new ShaderMaterial(Sp);
            RC.SetShader(Sp);

            Texture1Param = Sp.GetShaderParam("texture1");

            ImageData imgData = RC.LoadImage("Assets/cube_tex.jpg");

            ITexture iTex = RC.CreateTexture(imgData);

            RC.SetShaderParamTexture(Texture1Param, iTex);

            _world.AddObject(geo1, M, 0, 0, 500);

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
            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new EgoPerspective();
            app.Run();
        }

    }
}
