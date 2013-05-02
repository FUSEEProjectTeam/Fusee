using System.IO;
using Fusee.Engine;
using Fusee.Math;
/* In this example 2 objects are rendered with textures on them.
 * The ways to use textures are 
 *  - 1. Load any image file from your hard drive and use it as a texture.
 *  - 2. Create a rectangular ImageData object that can be used as a texture.
 */
namespace Examples.Texture
{
    public class Texture : RenderCanvas
    {
        //At first we have to define the shader.
        protected string _vs = @"
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

        protected string _ps = @"
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

        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;
        
        //Two objects will be rendered, so we need two Mesh-bbjects which hold the information. 
        protected Mesh _meshSphere, _meshCube;
        //To work with the parameter set in the shader, we have to declare an IShaderparam
        protected IShaderParam _texture1Param;
        //The ImageData-objects hold the Pixel-data from the image we would like to use as a texture.
        protected ImageData _imgData1;
        protected ImageData _imgData2;
        //The Itexture-objects hold the Texturing-handle
        protected ITexture _iTex1;
        protected ITexture _iTex2;
        
        

        public override void Init()
        {
            //We load two object-model on which the textures will be displayed           
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            _meshSphere = geo.ToMesh();

            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));
            _meshCube = geo2.ToMesh();

            //The shader is created and our IShaderParam is connected with the parameter specified in the shader.
            ShaderProgram sp = RC.CreateShader(_vs, _ps);
            RC.SetShader(sp);
            _texture1Param = sp.GetShaderParam("texture1");
            
            //The first texture is a .jpg-file loaded from the hard drive 
            _imgData1 = RC.LoadImage("Assets/world_map.jpg");
            //The second texture is created by ourselves
            _imgData2 = RC.CreateImage(600, 600, "Green");
            //Another option is to put some text on a texture
            _imgData2 = RC.TextOnImage(_imgData2, "Verdana", 80f, "FUSEE rocks!", "Black", 0, 30);
            
            //At last the texture are created
            _iTex1 = RC.CreateTexture(_imgData1);
            _iTex2 = RC.CreateTexture(_imgData2);
        }

        public override void RenderAFrame()
        {
            //Some basic movement controls
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)System.Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;
            }

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            //The objects are placed on the canvas, textured and finally rendered
            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            RC.SetShaderParamTexture(_texture1Param, _iTex1);
            RC.Render(_meshSphere);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
            RC.SetShaderParamTexture(_texture1Param, _iTex2);
            RC.Render(_meshCube);

            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            Texture app = new Texture();
            app.Run();
        }

    }
}