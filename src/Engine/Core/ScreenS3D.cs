using System;
using Fusee.Math.Core;
using Fusee.Engine.Common;
using Fusee.Serialization;


namespace Fusee.Engine.Core
{
    /// <summary>
    /// This class is used to store all parameters the user can adjust for a ScreenS3D object
    /// Can be used to serialize / de-serialize into a cinfig file to store adjustments / load adustments on application start (i.e. using JSON)
    /// </summary>
    public class ScreenConfig
    {
        public ScreenConfig()
        {
            Transform = float4x4.Identity;
            ScaleDepth = 1;
            ScaleSize = 1;
            Hit = 0;
        }

        public float4x4 Transform { get; set; }
        public float ScaleDepth { get; set; }
        public int ScaleSize { get; set; }
        public float Hit { get; set; }
    }

    /// <summary>
    /// This stuct contains all four textures used for the next render call.
    /// </summary>
    public struct ScreenS3DTextures
    {
        public ITexture Left;
        public ITexture LeftDepth;
        public ITexture Right;
        public ITexture RightDepth;
    }

    /// <summary>
    /// ScreenS3D can be used as a (video) screen to diplay steroscopic videos or images in addition with depth maps to simulate depth in the scene using the videos/images content
    /// </summary>
    /// TOTO: Implement class
    public class ScreenS3D
    {
        


        #region S3D-Shader + Depth
        /// <summary>
        /// S3D-Shader + Depth
        /// </summary>

        // GLSL
        private const string VsS3dDepth = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;        

            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec4 FuVertex;

            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_IMV;

            void main()
            {               
               
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);                
                
                FuVertex = vec4(fuVertex, 1.0);              
                
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string PsS3dDepth = @"
            #ifdef GL_ES
                precision highp float;
            #endif
           
            //SahderParams
            uniform sampler2D vTexture;
            uniform sampler2D textureDepth;
            uniform float scale;
            uniform int invert;
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_MVP;
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec4 FuVertex;
            float coordZ;
         
            void main()
            {
                //Read Texture Value (RGB)
                vec4 colTex = texture(vTexture, vUV);    
                //Read Texture Value (Grey/Depth)                  
                float depthTexValue = texture(textureDepth, vUV);
                if(invert == 1)
                {
                    depthTexValue = 1- depthTexValue;
                }   
                //homogenous vertex coordinates               
                vec4 vertex = FuVertex;             
    
                if(depthTexValue >0.9)          
                {                
                    discard;
                }
                else
                {          
                    //Add offest from 'textureDepth' with scaling value;               
                    vertex.z += ((depthTexValue*2)-1)*scale;
                    //trnasform to ClipSpace 
                    vec4 clip = FUSEE_P*FUSEE_MV*vertex;                     
                    //Noramlized Device Coordinates   
                    float ndcDepth = (clip.z/clip.w);                    
                    //Viewport transformation
                    coordZ  = (gl_DepthRange.diff)*0.5*ndcDepth+(gl_DepthRange.diff)*0.5; 
                    //Fragment Depth Value

                    gl_FragDepth =  coordZ;              
                }
                //write color 
                gl_FragColor = colTex /** dot(vNormal, vec3(0, 0, -1))*/;                            
            }";

        #endregion

        private RenderContext _rc;

        private Mesh _screenMesh;

        public ScreenConfig Config { get; private set; }

        public ScreenS3DTextures TexturesLR_DLR;// { get; set; }

        //Shader variables
        private ShaderProgram _stereo3DShaderProgram;
        private IShaderParam _colorTextureShaderParam;
        private IShaderParam _depthTextureShaderParam;
        private IShaderParam _depthShaderParamScale;
        private IShaderParam _invertDepthShaderParam;

        //Constructor
        public ScreenS3D(RenderContext rc, ScreenS3DTextures textures)
        {
            _rc = rc;
            _screenMesh = CreatePlaneMesh();
            TexturesLR_DLR = textures;
            InitializeShader();
            Config = new ScreenConfig();            
        }

        private void InitializeShader()
        {
            _stereo3DShaderProgram = _rc.CreateShader(VsS3dDepth, PsS3dDepth);
            _colorTextureShaderParam = _stereo3DShaderProgram.GetShaderParam("vTexture");
            _depthTextureShaderParam = _stereo3DShaderProgram.GetShaderParam("textureDepth");
            _depthShaderParamScale = _stereo3DShaderProgram.GetShaderParam("scale");
            _invertDepthShaderParam = _stereo3DShaderProgram.GetShaderParam("invert");
        }

        //TODO: Hit and Setting config stuff

        public void SetHit(float offset)
        {
            Config.Hit +=offset;
        }
  
        /// <summary>
        /// Creates a mesh used for the screen
        /// </summary>
        private Mesh CreatePlaneMesh()
        {
            var mesh = new Mesh();
            var vertecies = new[]
            {
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f}
            };

            var triangles = new ushort[]
            {
                // front face
                1,2,0,2,3,0
            };

            var normals = new[]
            {
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1)
            };
            var uVs = new[]
            {
                new float2(0, 0),
                new float2(0, 1),
                new float2(1, 1),                
                new float2(1, 0)
            };

            mesh.Vertices = vertecies;
            mesh.Triangles = triangles;
            mesh.Normals = normals;
            mesh.UVs = uVs;
            return mesh;
        }


        /// <summary>
        /// Rednder ScreenS3D Object
        /// </summary>
        /// <param name="cameraRig">The StereoCamerarig used in the Scene <see cref="StereoCameraRig" /></param>
        /// <param name="mtx">The camera matrix</param>
        public void Render(StereoCameraRig cameraRig, float4x4 mtx)
        {
            float hit = 0;
            ITexture textureColor = null;
            ITexture textureDepth = null;
            switch (cameraRig.CurrentEye)
            {
                case Stereo3DEye.Left:
                    textureColor = TexturesLR_DLR.Left;
                    textureDepth = TexturesLR_DLR.LeftDepth;
                    hit = Config.Hit == 0 ? 0: (-Config.Hit / 2);
                    break;
                case Stereo3DEye.Right:
                    textureColor = TexturesLR_DLR.Right;
                    textureDepth = TexturesLR_DLR.RightDepth;
                    hit = Config.Hit == 0 ? 0 : (Config.Hit / 2);
                    break;
            }

            if (textureColor != null && textureDepth != null)
            {
                //_rc.SetShader(_stereo3DShaderProgram);
                _rc.SetShaderParamTexture(_colorTextureShaderParam, textureColor);
                _rc.SetShaderParamTexture(_depthTextureShaderParam, textureDepth);
                _rc.SetShaderParam(_depthShaderParamScale, Config.ScaleDepth);
                _rc.SetShaderParam(_invertDepthShaderParam, 1);
               
                var mv = mtx * Config.Transform * float4x4.CreateTranslation(hit, 0, 0) * float4x4.CreateScale(Config.ScaleSize) 
                    * float4x4.CreateRotationY(-(float)System.Math.PI/4);

                _rc.ModelView = mv;
                _rc.Render(_screenMesh);
            }
        }
    }
}
