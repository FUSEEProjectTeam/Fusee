using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class CubeAndTiles : RenderCanvas
    {
        // GLSL
        protected string Vs = @"
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

        protected string Ps = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D vTexture;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            uniform int vUseAnaglyph;

            void main()
            {
                vec4 colTex = vColor * texture2D(vTexture, vUV);

                if (vUseAnaglyph != 0)
                {
                    vec4 _redBalance = vec4(0.1, 0.65, 0.25, 0);
                    float _redColor = (colTex.r * _redBalance.r + colTex.g * _redBalance.g + colTex.b * _redBalance.b) * 1.5;

                    gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * vec4(_redColor, colTex.g, colTex.b, 1) * dot(vNormal, vec3(0, 0, 1)) * 1.4;
                } else {
                    gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * colTex * dot(vNormal, vec3(0, 0, 1));
                }
            }";

        // variables
        private static Level _exampleLevel;
        private static Anaglyph3D _anaglyph3D;

        private static float _angleHorz = 0.4f;
        private static float _angleVert = -1.0f;
        private static float _angleVelHorz, _angleVelVert;

        private static bool _topView;
        private static KeyCodes _lastKey = KeyCodes.None;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        private bool _clientConnected = false;

        // Init()
        public override void Init()
        {
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            
            RC.SetShader(sp);
            RC.ClearColor = new float4(0, 0, 0, 1);

            _anaglyph3D = new Anaglyph3D(RC);
            _exampleLevel = new Level(RC, sp, _anaglyph3D);
        }

        // RenderAFrame()
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // keyboard
            if (_lastKey == KeyCodes.None)
            {
                if (Input.Instance.IsKeyDown(KeyCodes.V))
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

                if (Input.Instance.IsKeyDown(KeyCodes.C))
                {
                    Network.Instance.OpenConnection(ConnectionType.CtClient, "127.0.0.1", 14242);
                    _lastKey = KeyCodes.C;
                }

                if (Input.Instance.IsKeyDown(KeyCodes.S))
                {
                    Network.Instance.OpenConnection(ConnectionType.CtServer, 14242);
                    _lastKey = KeyCodes.S;
                }

                var msg = Level.Directions.None;

                if (Input.Instance.IsKeyDown(KeyCodes.Left)) {
                    _exampleLevel.MoveCube(Level.Directions.Left);
                    _lastKey = KeyCodes.Left;
                    msg = Level.Directions.Left;
                }

                if (Input.Instance.IsKeyDown(KeyCodes.Right)) {
                    _exampleLevel.MoveCube(Level.Directions.Right);
                    _lastKey = KeyCodes.Right;
                    msg = Level.Directions.Right;
                }

               if (Input.Instance.IsKeyDown(KeyCodes.Up)) {
                    _exampleLevel.MoveCube(Level.Directions.Forward);
                    _lastKey = KeyCodes.Up;
                    msg = Level.Directions.Forward;
                }

                if (Input.Instance.IsKeyDown(KeyCodes.Down)) {
                    _exampleLevel.MoveCube(Level.Directions.Backward);
                    _lastKey = KeyCodes.Down;
                    msg = Level.Directions.Backward;
                }

                if (msg != Level.Directions.None)
                    if (Network.Instance.Connected || Network.Instance.NetType == ConnectionType.CtClient)
                        Network.Instance.SendMessage(System.Convert.ToString((int) msg));
            }
            else if (!Input.Instance.IsKeyDown(_lastKey))
                _lastKey = KeyCodes.None;

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
                var curDamp = (float) System.Math.Exp(-Damping*Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.CreateRotationZ(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            _exampleLevel.Render(mtxRot, Time.Instance.DeltaTime);

            while (Network.Instance.IncServerMsgCount > 0)
            {
                var msg = Network.Instance.IncServerMsg;

                if (msg.Type == MessageType.StatusChanged)
                    if (msg.Status == ConnectionStatus.Connected)
                        _exampleLevel.CubeColor = new float3(0f, 1f, 0);

                if (msg.Type == MessageType.Data)
                {
                    int move;
                    if (System.Int32.TryParse(msg.Message, out move))
                        if (move != (int) Level.Directions.None)
                            _exampleLevel.MoveCube((Level.Directions) move);
                }
            }

            while (Network.Instance.IncClientMsgCount > 0)
            {
                var msg = Network.Instance.IncClientMsg;

                if (msg.Type == MessageType.StatusChanged)
                    if (msg.Status == ConnectionStatus.Connected)
                        _exampleLevel.CubeColor = new float3(0f, 1f, 0);
            }

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
            var app = new CubeAndTiles();
            app.Run();
        }
    }
}