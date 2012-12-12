using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.EgoPerspective
{
    public class EgoPerspective : RenderCanvas
    {
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f, _moveX = 0.0f, _moveY = 0.0f, _moveZ = 0.0f;
        private World _world;
        protected ShaderProgram Sp3;
        protected IShaderParam[] Param;
        protected ShaderMaterial m, m2;

        public override void Init()
        {
            _world = new World(RC, In);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/castle.obj.model"));
            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/ground.obj.model"));
            Sp3 = MoreShaders.GetShader("chess", RC);
            RC.SetShader(Sp3);
            RC.SetLightPosition(0, new float3(0, 1000, 1000));
            m = new ShaderMaterial(Sp3);
           
            m.SetValue("chessSize", 1000);
            m.SetValue("smoothFactor", 1);
            m.SetValue("darkColor", new float3(1, 0, 0));
            m.SetValue("brightColor", new float3(0, 0, 0));

            //Param = new IShaderParam[4];
            //Param[0] = RC.GetShaderParam(Sp3, "brightColor");
            //Param[1] = RC.GetShaderParam(Sp3, "darkColor");
            //Param[2] = RC.GetShaderParam(Sp3, "smoothFactor");
            //Param[3] = RC.GetShaderParam(Sp3, "chessSize");
            //RC.SetShaderParam(Param[0], new float3(1,1,1));
            //RC.SetShaderParam(Param[1], new float3(0, 0, 0));
            //RC.SetShaderParam(Param[2], 1);
            //RC.SetShaderParam(Param[3], 100);

            
            

            _world.addObject(geo2, m, 0, -100, 1000);
          //  _world.addObject(geo2, m, 0, -100, 1000);

            RC.ClearColor = new float4(0.6f, 0.8f, 1, 1);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;

        }

        public override void RenderAFrame()
        {
            
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            Random zufall = new Random();
            float number = zufall.Next(1000) / 1000;
            _world.RenderWorld(_angleVert);
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new EgoPerspective();
            app.Run();
        }

    }
}
