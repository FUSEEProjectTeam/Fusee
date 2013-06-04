using System;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class LightTypeTest : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _mesh1, _mesh2, _meshSky;

        // variables for shader
        private ShaderProgram _spDiffuse;
        private ShaderProgram _spSpecular;
        private ShaderProgram _spBump;
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colColor, _SpecShininess, _SpecLevel,_BumpShininess, _BumpSpecLevel;
        private IShaderParam _texTexture, _difTex1, _specTex1, _bumpTex1, _bumpTex2;

        private ITexture _iTex, _iTex2;

        private float _time, _time2;

        // is called on startup
        public override void Init()
        {
            _time = 5;
            _time2 = 0;
            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _mesh2 = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            _meshSky = MeshReader.LoadMesh(@"Assets/spacebox.obj.model");

            _spColor = MoreShaders.GetShader("color", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);
            _spDiffuse = MoreShaders.GetShader("diffuse", RC);
            _spSpecular = MoreShaders.GetShader("specular", RC);
            _spBump = MoreShaders.GetShader("bump", RC);

            // load texture
            var imgData = RC.LoadImage("Assets/metall2.jpg");
            _iTex = RC.CreateTexture(imgData);
            var imgData2 = RC.LoadImage("Assets/normal2.jpg");
            _iTex2 = RC.CreateTexture(imgData2);

            //Params for Color-Shader
            RC.SetShader(_spColor);
            _colColor = RC.GetShaderParam(_spColor, "color");
            RC.SetShaderParam(_colColor, new float4(1, 0, 0, 1));

            ////Params for Texture-Shader
            RC.SetShader(_spTexture);
            _texTexture = RC.GetShaderParam(_spTexture, "texture1");
            RC.SetShaderParamTexture(_texTexture, _iTex);

            //Params for Diffuse-Shader
            RC.SetShader(_spDiffuse);
            _difTex1 = RC.GetShaderParam(_spDiffuse, "texture1");
            RC.SetShaderParamTexture(_difTex1, _iTex);

            //Params for Specular-Shader
            RC.SetShader(_spSpecular);
            _specTex1 = RC.GetShaderParam(_spSpecular, "texture1");
            _SpecShininess = RC.GetShaderParam(_spSpecular, "shininess");
            _SpecLevel = RC.GetShaderParam(_spSpecular, "specularLevel");
            RC.SetShaderParamTexture(_specTex1, _iTex);
            RC.SetShaderParam(_SpecLevel, 512.0f);
            RC.SetShaderParam(_SpecShininess, 8.0f);

            //Params for Bump-Shader
            RC.SetShader(_spBump);
            _bumpTex1 = RC.GetShaderParam(_spBump, "texture1");
            _bumpTex2 = RC.GetShaderParam(_spBump, "normalTex");
            _SpecShininess = RC.GetShaderParam(_spBump, "shininess");
            _SpecLevel = RC.GetShaderParam(_spBump, "specularLevel");
            RC.SetShaderParamTexture(_bumpTex1, _iTex);
            RC.SetShaderParamTexture(_bumpTex2, _iTex2);
            RC.SetShaderParam(_SpecLevel, 512.0f);
            RC.SetShaderParam(_SpecShininess, 8.0f);

            //Directional Light
            //RC.SetLightActive(0, 1.0f);
            //RC.SetLightDiffuse(0, new float4(0.7f, 0.7f, 0.7f, 1));
            //RC.SetLightAmbient(0, new float4(0.3f, 0.3f, 0.3f, 1));
            //RC.SetLightSpecular(0, new float4(0.2f, 0.2f, 0.2f, 1));
            //RC.SetLightDirection(0, new float3(0, -1, 0));

            //Point Light
            //RC.SetLightActive(1, 2);
            //RC.SetLightDiffuse(1, new float4(0.7f, 0.7f, 0.7f, 1));
            //RC.SetLightAmbient(1, new float4(0.3f, 0.3f, 0.3f, 1));
            //RC.SetLightSpecular(1, new float4(0.2f, 0.2f, 0.2f, 1));
            //RC.SetLightPosition(1, new float3(0, 0, 0));

            //Spot Light
            RC.SetLightActive(2, 3.0f);
            RC.SetLightDiffuse(2, new float4(0.7f, 0.7f, 0.7f, 1));
            RC.SetLightAmbient(2, new float4(0.3f, 0.3f, 0.3f, 1));
            RC.SetLightSpecular(2, new float4(0.2f, 0.2f, 0.2f, 1));
            RC.SetLightDirection(2, new float3(0, 0, -1));
            RC.SetLightPosition(2, new float3(0, 0, 0));
            RC.SetLightSpotAngle(2, 5.0f);
            


        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.SetLightPosition(2, new float3(_time2, _time2, 0));
            RC.SetLightSpotAngle(2, _time);

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _time -= 0.01f;
            

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _time += 0.01f;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _time2 += 5;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _time2 -= 5;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            //RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;

            RC.SetShader(_spBump);
            //RC.SetShaderParamTexture(_DifTex1, _iTex);
            RC.Render(_mesh2);

            // second mesh
            RC.ModelView = mtxRot * float4x4.CreateTranslation(0, 0, 0) * mtxCam;


            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new LightTypeTest();
            app.Run();
        }

    }
}