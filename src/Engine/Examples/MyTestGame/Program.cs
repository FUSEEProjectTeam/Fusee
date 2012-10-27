using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class MyTestGame : RenderCanvas
    {
        // GLSL
        protected string Vs = @"
            #version 120

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
        
            varying vec4 vColor;
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

        protected string Ps = @"
            #version 120

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1));
            }";

        // Variablen
        private Level _exampleLevel;
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 10.0f;
        private const float Damping = 0.95f;

        // Init()
        public override void Init()
        {
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            
            RC.SetShader(sp);
            _exampleLevel = new Level(4, 4, RC, sp);
            RC.ClearColor = new float4(0, 0, 0, 1);
            
        }

        // RenderAFrame()
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            
            if (In.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * In.GetAxis(InputAxis.MouseX) * (float)DeltaTime;
                _angleVelVert = RotationSpeed * In.GetAxis(InputAxis.MouseY) * (float)DeltaTime;
            }
            else
            {
                _angleVelHorz *= Damping;
                _angleVelVert *= Damping;
            }
            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);

            
            _exampleLevel.Render(mtxRot);


            Present();  // <-- ohne ergibt Endlosschleife, völlige Überlastung...
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new MyTestGame();
            app.Run();
        }
    }
}