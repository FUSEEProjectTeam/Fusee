using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace MyTestGame
{
    public class MyTestGame : RenderCanvas
    {
        // GLSL
        protected string _vs = @"
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

        protected string _ps = @"
            #version 120

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            varying vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vec4(0.5, 0.15, 0.17, 1.0) * dot(vNormal, vec3(0, 0, 1));
            }";

        // Variablen
        protected Mesh _mesh;

        private static int cubeX = 0;
        private static int orientY = 0;
        private static float rotateY = 0.0f;

        private static bool animActive = false;

        // Konstanten
        private const float piHalf = (float)Math.PI / 2.0f;           // Pi halbe = 90°

        // Init()
        public override void Init()
        {
            // Todo: Funktion basteln: MeshReader.loadObj("Datei");
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Cube.obj.model"));  
            _mesh = geo.ToMesh();

            ShaderProgram sp = RC.CreateShader(_vs, _ps);

            RC.SetShader(sp);
            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        // RenderAFrame()
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // aktuelle X-Position des Wuerfels
            int cubeXtmp = cubeX;

            if (In.IsKeyDown(KeyCodes.Left))
            {
                // wenn noch keine Animation aktiv
                if (!animActive)
                {
                    orientY++;
                    animActive = true;
                }
            }

            // wenn eine Animation aktiv
            if (animActive)
            {
                float rotateTarg = (float)orientY * piHalf;       // Drehungsziel (aktuell + 90°)

                // wenn Drehnung noch nicht abgeschlossen
                if (rotateY < rotateTarg)
                {
                    // Rotation des Cubes erhoehen
                    rotateY += 2.0f * (float)DeltaTime;

                    // Fortschritt der Drehung, damit der Cube sich gleichmaessig bewegt
                    float progr = 1 - ((rotateTarg - rotateY) / piHalf);
                    float curPos = progr * 200;

                    cubeXtmp = cubeX - (int)curPos;
                }

                // wenn Drehung abgeschlossen
                if (rotateY >= rotateTarg)
                {
                    orientY %= 4;                                   // 0, 1, 2, 3, 0, 1, 2, ...
                    rotateY = (orientY == 0) ? 0 : rotateTarg;      // 0°, 90°, 180°, 270°, 360°, 90°, ...

                    cubeX -= 200;
                    cubeXtmp = cubeX;

                    animActive = false;
                }
            }

            // Rotation ist um die Y-Achse, Bewegung aber in X-Richtung
            float4x4 mtxObjRot = float4x4.CreateRotationY(-rotateY) * float4x4.CreateRotationX(0) * float4x4.CreateRotationZ(0);
            float4x4 mtxObjPos = float4x4.CreateTranslation(cubeXtmp, 0, 0);

            // Kamera ausrichten
            float4x4 mtxCamPos = float4x4.CreateTranslation(0, 0, 0);
            float4x4 mtxCamRot = float4x4.LookAt(0, 0, 4000, 0, 0, 0, 0, 1, 0);
        
            // Rendern
            RC.ModelView = mtxObjRot * mtxObjPos * mtxCamPos * mtxCamRot;
            RC.Render(_mesh);

            Present();  // <-- ohne ergibt Endlosschleife, völlige Überlastung...
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            MyTestGame app = new MyTestGame();
            app.Run();
        }
    }
}