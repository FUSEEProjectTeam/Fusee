using System;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.HandOnCanvas
{
    public class HandOnCanvas : RenderCanvas
    {
        // angle variables
        private float _angleHorz, _angleVert, _angleHorzAbs, _angleVertAbs;

        private const float RotationSpeed = 30f;
        private const float _maxRotChange = 40f;
        private const float Damping = 1f;

        // model variables
        private Mesh _meshTea;

        // variables for shader
        private ShaderProgram _spColor;
        private IShaderParam _colorParam;

        private ShaderEffect _shaderEffect;

        private float _normWidth;
        private float _normHeight;
        private float _lineWidth = 1.7f;

        private float _camDist = 500.0f;
        private float _squareScreenPxls = 2048.0f;
        private float _handScale = 0.5f;


        private void InitShader()
        {
            var imgData = RC.LoadImage("Assets/art_billard.jpg");
            var iTex = RC.CreateTexture(imgData);

            _shaderEffect = new ShaderEffect(
                new[]
                    {
                        new EffectPassDeclaration
                            {
                                VS = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            uniform vec2 uLineWidth;

            void main()
            {
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = normalize(vNormal);
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) ) + vec4(uLineWidth * vNormal.xy, 0, 0) + vec4(0, 0, 0.06, 0);
                vUV = fuUV;
            }",

                                PS = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 uLineColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = uLineColor;
            }",

                                StateSet = new RenderStateSet()
                                    {
                                        AlphaBlendEnable = false,
                                        ZEnable = true
                                    }
                            },

                        new EffectPassDeclaration
                            {
                                VS = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) );
                vNormal = normalize(mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal);
                vUV = fuUV;
            }",

                                PS = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D texture1;

            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                gl_FragColor = vec4(texture2D(texture1, vNormal.xy * 0.5 + vec2(0.5, 0.5)).rgb, 0.85);
            }",

                StateSet = new RenderStateSet()
                {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                }
            },
        }, 
        new[]
            {
                new EffectParameterDeclaration {Name = "uLineColor", Value = new float4(0, 0, 0, 1)},
                new EffectParameterDeclaration {Name = "texture1", Value = iTex},
                new EffectParameterDeclaration {Name = "uLineWidth", Value = new float2(5, 5)},
            });
            _shaderEffect.AttachToContext(RC);
        }

        // is called on startup
        public override void Init()
        {
             InitShader();
            RC.ClearColor = new float4(1f, 1f, 1f, 0.0f);
            _meshTea = MeshReader.LoadMesh(@"Assets/handhipolynorm.obj.model");
 
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            Point pt = Input.Instance.GetMousePos();
            float3 mousePosWorld = new float3(pt.x - this.Width / 2, this.Height / 2 - pt.y, 0);
            mousePosWorld = 2 * _camDist/_squareScreenPxls * mousePosWorld;

            var lineWidthFactor = 1.0f;
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                 lineWidthFactor = 3.0f;
                 _shaderEffect.SetEffectParam("uLineColor", new float4(1, 0.2f, 0.2f,  0.9f));            
            }
            else
            {
                _shaderEffect.SetEffectParam("uLineColor", new float4(0, 0, 0, 1));
            }
            _shaderEffect.SetEffectParam("uLineWidth", new float2(lineWidthFactor * _lineWidth / _normWidth, lineWidthFactor * _lineWidth / _normHeight));


            float curMaxRotChange = (float)(_maxRotChange * Math.Abs(Input.Instance.GetAxis(InputAxis.MouseX)) * Time.Instance.DeltaTime);
            float angleHorzDelta =
                Math.Min(Math.Max(RotationSpeed*-Input.Instance.GetAxis(InputAxis.MouseX) - _angleHorz, -curMaxRotChange), curMaxRotChange);
            _angleHorz = (float)Math.Max(-0.5f * Math.PI, Math.Min(_angleHorz + angleHorzDelta, 0.5f * Math.PI));

            curMaxRotChange = (float)(_maxRotChange * Math.Abs(Input.Instance.GetAxis(InputAxis.MouseY)) * Time.Instance.DeltaTime);
            float angleVertDelta =
                Math.Min(Math.Max(RotationSpeed * -Input.Instance.GetAxis(InputAxis.MouseY) - _angleVert, -curMaxRotChange), curMaxRotChange);
            _angleVert = (float)Math.Max(-0.7f * Math.PI, Math.Min(_angleVert + angleVertDelta, 0.2f * Math.PI));
            
            /* float angleVertDelta = 
                Math.Min(Math.Max(_angleVert - RotationSpeed * -Input.Instance.GetAxis(InputAxis.MouseX), -_maxRotChange), _maxRotChange);
            _angleVert += angleVertDelta;*/

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 0, _camDist, 0, 0, 0, 0, 1, 0);

            var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);
            _angleHorz *= curDamp;
            _angleVert *= curDamp;

            // first mesh
            RC.ModelView = float4x4.CreateRotationX((float)(-0.3*Math.PI)) * new float4x4(_handScale, 0, 0, 0, 0, _handScale, 0, 0, 0, 0, _handScale, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(mousePosWorld) * mtxCam;

            _shaderEffect.RenderMesh(_meshTea);

            // swap buffers
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            // var aspectRatio = Width / (float)Height;
            // RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000); 
            _normWidth = Width/_squareScreenPxls;
            _normHeight = Height/_squareScreenPxls;
            RC.Projection = float4x4.CreatePerspectiveOffCenter(-_normWidth, _normWidth, -_normHeight, _normHeight, 1, 10000);
            _shaderEffect.SetEffectParam("uLineWidth", new float2(_lineWidth / _normWidth, _lineWidth / _normHeight));
        }

        public static void Main()
        {
            var app = new HandOnCanvas();
            app.Run();
        }

    }
}
