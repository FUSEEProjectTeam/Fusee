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
        protected IShaderParam VFrameCounterParam;
        protected float Timer = 0;
        
        public override void Init()
        {
            _world = new World(RC,In);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Hut_obj2.obj.model"));
            _world.addObject(geo,0,0,0);
            Geometry user = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Ninja.obj.model"));

            Sp3 = Shaders.GetShader("flat",RC);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;
            VColorParam = Sp3.GetShaderParam("vColor");
            //VFrameCounterParam = Sp3.GetShaderParam("vTimer");
            RC.ClearColor = new float4(0, 0, 0, 1);
        }

        public override void RenderAFrame()
        {
            Timer += 1;

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.SetShader(Sp3);
            RC.SetShaderParam(VFrameCounterParam, Timer);
            //RC.SetShaderParam(VColorParam, new float4(0.5f, 0.0f, 0.0f, 1));
            
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
