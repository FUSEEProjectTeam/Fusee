using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples
{
    public class Simple : RenderCanvas
    {

        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f, _moveX = 0.0f, _moveY = 0.0f, _moveZ = 0.0f;
        private World _world;
        protected ShaderProgram Sp3;
        protected IShaderParam VColorParam;
        
        public override void Init()
        {
            _world = new World(RC,In);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Hut_obj2.obj.model"));
            _world.addObject(geo,0,0,0);
            Geometry user = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Ninja.obj.model"));

            Sp3 = Shaders.GetShader("name",RC);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;
            VColorParam = Sp3.GetShaderParam("vColor");
            RC.ClearColor = new float4(0, 0, 0, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            //if (In.IsButtonDown(MouseButtons.Left))
            //{
            _angleVelHorz += _rotationSpeed*In.GetAxis(InputAxis.MouseX);//  *(float)DeltaTime;
              //  _angleVelVert = _rotationSpeed * In.GetAxis(InputAxis.MouseY) * (float)DeltaTime;
            //}
            //else
            //{
            //    _angleVelHorz *= _damping;
            //    _angleVelVert *= _damping;
            //}
            //_angleHorz += _angleVelHorz;
            //_angleVert += _angleVelVert;

            if (In.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.W))
            {
                _moveZ += _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.S))
            {
                _moveZ -= _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.A))
            {
                _moveX += _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.D))
            {
                _moveX -= _rotationSpeed * (float)DeltaTime;
            }
            RC.SetShader(Sp3);
            RC.SetShaderParam(VColorParam, new float4(0.5f, 0.0f, 0.0f, 1));
            RC.ModelView = float4x4.CreateRotationY((float)0) * float4x4.CreateRotationX(0) * float4x4.CreateTranslation(_moveX, _moveY, _moveZ + 80) * float4x4.LookAt(0, 50, 200, _angleVelHorz, 50, 0, 0, 1, 0) * float4x4.CreateTranslation(0, 0, -80);

            RC.SetShaderParam(VColorParam, new float4(0.0f, 0.0f, 0.8f, 1));
            RC.ModelView = float4x4.CreateRotationY((float)Math.PI * 3 / 2) * float4x4.CreateRotationX(0) * float4x4.CreateTranslation(0,0,0) * float4x4.LookAt(0, 80, 80, 0, 50, 0, 0, 1, 0);
            
            
            _world.RenderWorld(_angleVert);
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
            Simple app = new Simple();
            app.Run();
        }

    
    }
}
