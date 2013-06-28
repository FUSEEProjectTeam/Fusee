using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class CubeAndTiles : RenderCanvas
    {
        #region Shader

        // GLSL
        private const string Vs = @"
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
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string PsStart = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D vTexture;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                vec4 colTex = vColor * texture2D(vTexture, vUV);
            ";

        private const string PsAnaglyph = @"
                vec4 _redBalance = vec4(0.1, 0.65, 0.25, 0);
                float _redColor = (colTex.r * _redBalance.r + colTex.g * _redBalance.g + colTex.b * _redBalance.b) * 1.5;
                gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * vec4(_redColor, colTex.g, colTex.b, 1) * dot(vNormal, vec3(0, 0, 1)) * 1.4;
            }";

        private const string PsNonAnaglyph = @"
                gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * colTex * dot(vNormal, vec3(0, 0, 1));
            }";

        #endregion

        // variables
        private static Level _exampleLevel;
        private static Anaglyph3D _anaglyph3D;

        private ShaderProgram _spAnaglyph;
        private ShaderProgram _spNonAnaglyph;

        private static float _angleHorz = 0.4f;
        private static float _angleVert = -1.0f;
        private static float _angleVelHorz, _angleVelVert;

        private static bool _topView;
        private static KeyCodes _lastKey = KeyCodes.None;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // Init()
        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);
            
            _spNonAnaglyph = RC.CreateShader(Vs, PsStart + PsNonAnaglyph);
            _spAnaglyph = RC.CreateShader(Vs, PsStart + PsAnaglyph);

            RC.SetShader(_spNonAnaglyph);

            _anaglyph3D = new Anaglyph3D(RC);
            _exampleLevel = new Level(RC, _spNonAnaglyph, _anaglyph3D);
        }

        // RenderAFrame()
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // keyboard
            if (_lastKey == KeyCodes.None)
            {
                if (Input.Instance.IsKeyPressed(KeyCodes.V))
                {
                    _angleVelHorz = 0.0f;
                    _angleVelVert = 0.0f;

                    if (_topView)
                    {
                        _angleHorz = 0.4f;
                        _angleVert = -1.0f;

                        _topView = false;
                    }
                    else
                    {
                        _angleHorz = 0.0f;
                        _angleVert = 0.0f;
                        _topView = true;
                    }

                    _lastKey = KeyCodes.V;
                }

                if (Input.Instance.IsKeyPressed(KeyCodes.S))
                {
                    Audio.Instance.SetVolume(Audio.Instance.GetVolume() > 0 ? 0 : 100);
                    _lastKey = KeyCodes.S;
                }

                if (Input.Instance.IsKeyPressed(KeyCodes.C))
                {
                    RC.SetShader(_exampleLevel.UseAnaglyph3D ? _spNonAnaglyph : _spAnaglyph);

                    _exampleLevel.UseAnaglyph3D = !_exampleLevel.UseAnaglyph3D;
                    _lastKey = KeyCodes.C;
                }
            }
            else if (!Input.Instance.IsKeyPressed(_lastKey))
                _lastKey = KeyCodes.None;

            if (Input.Instance.IsKeyPressed(KeyCodes.Left))
                _exampleLevel.MoveCube(Level.Directions.Left);

            if (Input.Instance.IsKeyPressed(KeyCodes.Right))
                _exampleLevel.MoveCube(Level.Directions.Right);

            if (Input.Instance.IsKeyPressed(KeyCodes.Up))
                _exampleLevel.MoveCube(Level.Directions.Forward);

            if (Input.Instance.IsKeyPressed(KeyCodes.Down))
                _exampleLevel.MoveCube(Level.Directions.Backward);

            // mouse
            if (Input.Instance.GetAxis(InputAxis.MouseWheel) > 0)
                _exampleLevel.ZoomCamera(50);

            if (Input.Instance.GetAxis(InputAxis.MouseWheel) < 0)
                _exampleLevel.ZoomCamera(-50);

            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float) Math.Exp(-Damping*Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.CreateRotationZ(_angleHorz)*float4x4.CreateRotationX(_angleVert);

            _exampleLevel.Render(mtxRot);

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
            var app = new CubeAndTiles();
            app.Run();
        }
    }
}