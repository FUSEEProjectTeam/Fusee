using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Engine;
using Fusee.Math;

namespace Test_Anwendung
{
   public class Program : RenderCanvas
   {
       protected string _vs = @"
            // #version 120

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
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

       protected string _ps = @"
            // #version 120

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

       private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
       
       protected Mesh _mesh, _meshFace;
       protected IShaderParam _vColorParam;

       static float _x = 1;
       static float _y = 1;
       static float _z = 1;
       private static float _speed = 0.5F;


       public override void Init()
       {
           Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Face.obj.model"));
           _mesh = geo.ToMesh();

           Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Donut.obj.model"));
           _meshFace = geo2.ToMesh();

           _angleHorz = 0;
           _rotationSpeed = 10.0f;
           ShaderProgram sp = RC.CreateShader(_vs, _ps);
           RC.SetShader(sp);
           _vColorParam = sp.GetShaderParam("vColor");


           RC.ClearColor = new float4(0.5F, 0.5F, 0.5F, 1);
       }

       public override void RenderAFrame()
       {
           
           RC.Clear(ClearFlags.Color | ClearFlags.Depth);

           

           if (In.IsButtonDown(MouseButtons.Left))
           {
               _angleVelHorz = _rotationSpeed * In.GetAxis(InputAxis.MouseX) * (float)DeltaTime;
               _angleVelVert = _rotationSpeed * In.GetAxis(InputAxis.MouseY) * (float)DeltaTime;
           }
           else
           {
               _angleVelHorz *= _damping;
               _angleVelVert *= _damping;
           }
           _angleHorz += _angleVelHorz;
           _angleVert += _angleVelVert;

           if (In.IsKeyDown(KeyCodes.Left))
           {
               _x += -1;
           }
           if (In.IsKeyDown(KeyCodes.Right))
           {
               _x += 1;
           }
           if (In.IsKeyDown(KeyCodes.Up))
           {
               _z += -1; ;
           }
           if (In.IsKeyDown(KeyCodes.Down))
           {
               _z += 1;
           }

           _y += _speed;

           if( _y > 300)
           {
               _speed *= -1;
           }

           if (_y < -300)
           {
               _speed *= -1;
           }

           float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
           float4x4 mtxCam = float4x4.LookAt(0, 200, 800, 0, 50, 0, 0, 1, 0);

           RC.ModelView = mtxRot * float4x4.CreateTranslation(_x, _y, _z) * mtxCam;
           RC.SetShaderParam(_vColorParam, new float4( 0.2f, 0.4f, 0.8f, 1));
           RC.Render(_mesh);

           RC.ModelView =  float4x4.CreateTranslation(_x, 0, _z) * mtxCam;
           RC.SetShaderParam(_vColorParam, new float4(0.4f, 0.8f, 0.2f, 1));
           RC.Render(_meshFace);
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
           Program app = new Program();
           app.Run();
       }

   }
}

