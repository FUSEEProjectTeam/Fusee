using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MageExample
{
    public class MageExample : RenderCanvas
    {
        private const string VsBump = @"

attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L2_AMBIENT;
uniform vec4 FUSEE_L3_AMBIENT;
uniform vec4 FUSEE_L4_AMBIENT;
uniform vec4 FUSEE_L5_AMBIENT;
uniform vec4 FUSEE_L6_AMBIENT;
uniform vec4 FUSEE_L7_AMBIENT;

uniform float FUSEE_L0_ACTIVE;
uniform float FUSEE_L1_ACTIVE;
uniform float FUSEE_L2_ACTIVE;
uniform float FUSEE_L3_ACTIVE;
uniform float FUSEE_L4_ACTIVE;
uniform float FUSEE_L5_ACTIVE;
uniform float FUSEE_L6_ACTIVE;
uniform float FUSEE_L7_ACTIVE;

varying vec2 vUV;
varying vec3 lightDir[8];
varying vec3 vNormal;
varying vec4 endAmbient;
varying vec3 eyeVector;

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal);

    eyeVector = mat3(FUSEE_MVP[0].xyz, FUSEE_MVP[1].xyz, FUSEE_MVP[2].xyz) * -fuVertex;
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        endAmbient += FUSEE_L7_AMBIENT;
    }

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";


private const string PsBump = @"
#ifdef GL_ES
    precision highp float;
#endif

uniform sampler2D texture1;
uniform sampler2D normalTex;
uniform float specularLevel;

uniform vec4 FUSEE_L0_SPECULAR;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec4 FUSEE_L2_SPECULAR;
uniform vec4 FUSEE_L3_SPECULAR;
uniform vec4 FUSEE_L4_SPECULAR;
uniform vec4 FUSEE_L5_SPECULAR;
uniform vec4 FUSEE_L6_SPECULAR;
uniform vec4 FUSEE_L7_SPECULAR;

uniform float FUSEE_L0_ACTIVE;
uniform float FUSEE_L1_ACTIVE;
uniform float FUSEE_L2_ACTIVE;
uniform float FUSEE_L3_ACTIVE;
uniform float FUSEE_L4_ACTIVE;
uniform float FUSEE_L5_ACTIVE;
uniform float FUSEE_L6_ACTIVE;
uniform float FUSEE_L7_ACTIVE;

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L2_POSITION;
uniform vec3 FUSEE_L3_POSITION;
uniform vec3 FUSEE_L4_POSITION;
uniform vec3 FUSEE_L5_POSITION;
uniform vec3 FUSEE_L6_POSITION;
uniform vec3 FUSEE_L7_POSITION;

uniform vec3 FUSEE_L0_DIRECTION;
uniform vec3 FUSEE_L1_DIRECTION;
uniform vec3 FUSEE_L2_DIRECTION;
uniform vec3 FUSEE_L3_DIRECTION;
uniform vec3 FUSEE_L4_DIRECTION;
uniform vec3 FUSEE_L5_DIRECTION;
uniform vec3 FUSEE_L6_DIRECTION;
uniform vec3 FUSEE_L7_DIRECTION;

varying vec3 vNormal;
varying vec2 vUV;
varying vec4 endAmbient;
varying vec3 eyeVector;
 
void main()
{       
    float maxVariance = 2.0;
    float minVariance = maxVariance/2.0;
    vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 
    vec4 endSpecular = vec4(0,0,0,0);
    vec4 tempTexSpecular = texture2D(texture1, vUV);
    if(FUSEE_L0_ACTIVE == 1.0 ) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
        float L3NdotHV = max(min(dot(normalize(tempNormal), vHalfVector),1.0), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0 * tempTexSpecular.z;
        endSpecular += FUSEE_L0_SPECULAR * shine;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0 * tempTexSpecular.z;
        endSpecular += FUSEE_L1_SPECULAR * shine;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0 * tempTexSpecular.z;
        endSpecular += FUSEE_L2_SPECULAR * shine;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L3_SPECULAR * shine;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L4_SPECULAR * shine;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L5_SPECULAR * shine;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L6_SPECULAR * shine;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L7_SPECULAR * shine;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity += endAmbient; 
    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";



        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 1.5f, _damping = 0.92f;
        protected Mesh Body, GloveL, GloveR;
        protected IShaderParam VColorParam;
        //protected ShaderMaterial mBody, mGlove;
        private IShaderParam _texture1ParamBody;
        private IShaderParam _texture2ParamBody;
        private IShaderParam _specularLevelBody;
        private IShaderParam _texture1ParamGlove;
        private IShaderParam _texture2ParamGlove;
        private IShaderParam _specularLevelGlove;

        private ShaderProgram SpBody, SpGlove;
        private ITexture iTexGlove;
        private ITexture iTex2Glove;
        private ITexture iTex;
        private ITexture iTex2;

        public override void Init()
        {

            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageBodyOBJ.obj.model"));
            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageGloveLOBJ.obj.model"));
            Geometry geo3 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageGloveROBJ.obj.model"));

            Body = geo.ToMesh();
            GloveL = geo2.ToMesh();
            GloveR = geo3.ToMesh();

            ShaderProgram SpBody = RC.CreateShader(VsBump, PsBump);
            RC.SetShader(SpBody);

            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(0, new float4(0.2f, 0.2f, 0.2f, 1.0f));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(0, new float4(0.8f, 0.8f, 0.8f, 1.0f));
            RC.SetLightDirection(0, new float3(-1.0f, 0.0f, 0.0f));

            RC.SetLightActive(1, 1);
            RC.SetLightPosition(1, new float3(-5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(1, new float4(0.5f, 0.5f, 0.5f, 1.0f));
            RC.SetLightSpecular(1, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(1, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetLightDirection(1, new float3(1.0f, 0.0f, 0.0f));

            _texture1ParamBody = SpBody.GetShaderParam("texture1");
            _texture2ParamBody = SpBody.GetShaderParam("normalTex");
            _specularLevelBody = SpBody.GetShaderParam("specularLevel");

            ImageData imgDataGlove = RC.LoadImage("Assets/HandAOMap.jpg");
            ImageData imgData2Glove = RC.LoadImage("Assets/HandschuhNormalMap.jpg");
            iTexGlove = RC.CreateTexture(imgDataGlove);
            iTex2Glove = RC.CreateTexture(imgData2Glove);
            //RC.SetShader(SpGlove);
            //RC.SetShaderParamTexture(_texture1ParamGlove, iTexGlove);
            //RC.SetShaderParamTexture(_texture2ParamGlove, iTex2Glove);
            //RC.SetShaderParam(_specularLevelGlove, 64.0f);

            ImageData imgData = RC.LoadImage("Assets/TextureAtlas.jpg");
            ImageData imgData2 = RC.LoadImage("Assets/TextureAtlasNormal.jpg");
            iTex = RC.CreateTexture(imgData);
            iTex2 = RC.CreateTexture(imgData2);
            //RC.SetShader(SpBody);
            //RC.SetShaderParamTexture(_texture1ParamBody, iTex);
            //RC.SetShaderParamTexture(_texture2ParamBody, iTex2);
            //RC.SetShaderParam(_specularLevelBody, 64.0f);

            RC.ClearColor = new float4(0.3f, 0.3f, 0.3f, 1);

            _angleHorz = 0;
            _angleVert += 1.75f;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)System.Math.Exp(-_damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }

            _angleHorz -= 1.0f * (float)Time.Instance.DeltaTime;

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            float4x4 mtxCam = float4x4.LookAt(0, -10, 3, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(0, -2f, 0) * mtxCam;
            //RC.SetShaderParam(VColorParam, new float4(0.5f, 0.8f, 0, 1));

            
            RC.SetShaderParamTexture(_texture1ParamBody, iTex);
            RC.SetShaderParamTexture(_texture2ParamBody, iTex2);
            RC.SetShaderParam(_specularLevelBody, 64.0f);

            RC.Render(Body);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(0, -2f, 0) * mtxCam;

            RC.SetShaderParamTexture(_texture1ParamBody, iTexGlove);
            RC.SetShaderParamTexture(_texture2ParamBody, iTex2Glove);
            RC.SetShaderParam(_specularLevelBody, 64.0f);

            RC.Render(GloveL);
            RC.Render(GloveR);
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new MageExample();
            app.Run();
        }

    }
}
