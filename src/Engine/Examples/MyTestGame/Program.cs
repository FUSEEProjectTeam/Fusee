using System;
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
                gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * vColor * dot(vNormal, vec3(0, 0, 1));
            }";

        // Variablen
        private Level _exampleLevel;


        private static float _angleHorz = 0.4f;
        private static float _angleVert = -1.0f;
        private static float _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 10.0f;
        private const float Damping = 0.95f;

        // Init()
        public override void Init()
        {
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            
            RC.SetShader(sp);
            RC.ClearColor = new float4(0, 0, 0, 1);

            _exampleLevel = new Level(RC, sp);
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

            //_angleHorz = Math.Max(0.3f, Math.Min(_angleHorz + _angleVelHorz, 0.5f));
            //_angleVert =  Math.Max(-1.1f, Math.Min(_angleVert + _angleVelVert, 0.9f));

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
            

            if (In.IsKeyDown(KeyCodes.Left))
                _exampleLevel.MoveCube(Level.Directions.Left);

            if (In.IsKeyDown(KeyCodes.Right))
                _exampleLevel.MoveCube(Level.Directions.Right);

            if (In.IsKeyDown(KeyCodes.Up))
                _exampleLevel.MoveCube(Level.Directions.Forward);

            if (In.IsKeyDown(KeyCodes.Down))
                _exampleLevel.MoveCube(Level.Directions.Backward);

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationZ(0.5f);
            _exampleLevel.Render(mtxRot, DeltaTime);

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