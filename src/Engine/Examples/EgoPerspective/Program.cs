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
        protected IShaderParam VLightDir;
        protected IShaderParam vMorph;
        
        public override void Init()
        {
            _world = new World(RC,In);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Teapot.obj.model"));
            _world.addObject(geo, 0, 0, 1000);

            Sp3 = Shaders.GetShader("",RC);
            //VLightDir = Sp3.GetShaderParam("vLightDir");
            VColorParam = Sp3.GetShaderParam("vColor");
            vMorph = Sp3.GetShaderParam("scale");
            RC.ClearColor = new float4(1, 1, 1, 1);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            RC.SetShader(Sp3);
            Random zufall = new Random();
            float number = zufall.Next(1000) / 1000;
            //RC.SetShaderParam(VColorParam, new float4(number, number, number, 1.0f));
            //RC.SetShaderParam(VLightDir, new float3(0.0f, 0.0f, 1));
            RC.SetShaderParam(vMorph, new float4(1.5f, 1.0f, 1.0f, 1.0f));
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
