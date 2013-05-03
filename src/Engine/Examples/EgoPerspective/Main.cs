using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.EgoPerspective
{
    public class EgoPerspective : RenderCanvas
    {
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _rotationSpeed = 10.0f; //_angleVelHorz = 0, _angleVelVert = 0, _damping = 0.95f, _moveX = 0.0f, _moveY = 0.0f, _moveZ = 0.0f;
        private World _world;
        protected ShaderProgram Sp;
        protected IShaderParam[] Param;
        protected ShaderMaterial m, m2;

        protected IShaderParam _specularLevel;
        protected IShaderParam _specularSize;
        protected IShaderParam _texture1Param;
        protected IShaderParam _texture2Param;
        protected float x, z, time;

        public override void Init()
        {
            //_world = new World(RC, Input.Instance);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));
            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));

            Sp = MoreShaders.GetShader("bump", RC);
            RC.SetShader(Sp);
            m = new ShaderMaterial(Sp);

            x = 500;
            z = 0;
            time = 0;


            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(500, 0, 0));
            RC.SetLightAmbient(0, new float4(0.1f, 0.1f, 0.1f, 1));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1));
            RC.SetLightDiffuse(0, new float4(1.0f, 1.0f, 1.0f, 1));
            RC.SetLightDirection(0, new float3(-1, 0, 0));

            //RC.SetLightActive(6, 1);
            //RC.SetLightPosition(6, new float3(-500, 100, 0));
            //RC.SetLightAmbient(6, new float4(0.1f, 0.1f, 0.1f, 1));
            //RC.SetLightSpecular(6, new float4(0.2f, 0.2f, 0.2f, 0));
            //RC.SetLightDiffuse(6, new float4(1.0f, 1.0f, 1.0f, 1));
            //RC.SetLightDirection(6, new float3(5, -1, 0));

            //RC.SetLightActive(2, 1);
            //RC.SetLightPosition(2, new float3(0, 500, 0));
            //RC.SetLightAmbient(2, new float4(0.1f, 0.1f, 0.1f, 1));
            //RC.SetLightSpecular(2, new float4(0, 0, 0.2f, 0));
            //RC.SetLightDiffuse(2, new float4(0.0f, 1.0f, 0.0f, 1));
            //RC.SetLightDirection(2, new float3(0, -1, 0));

            //RC.SetLightPosition(2, new float3(1000, 1000, 1000));
            //RC.SetLightAmbient(2, new float4(0.1f, 0.1f, 0.1f, 1));
            //RC.SetLightSpecular(2, new float4(0, 0.3f, 0, 0));
            //RC.SetLightDiffuse(2, new float4(0.0f, 0.0f, 0.7f, 1));
            //RC.SetLightDirection(2, new float3(-1, -1, -1));

            //RC.SetLightPosition(3, new float3(1000, 1000, 1000));
            //RC.SetLightAmbient(3, new float4(0.1f, 0.1f, 0.1f, 1));
            //RC.SetLightSpecular(3, new float4(0, 0.3f, 0, 0));
            //RC.SetLightDiffuse(3, new float4(0.0f, 0.0f, 0.7f, 1));
            //RC.SetLightDirection(3, new float3(-1, -1, -1));

            //_texture1Param = Sp.GetShaderParam("texture1");
            //_texture2Param = Sp.GetShaderParam("normalTex");
            //_texture2Param = Sp.GetShaderParam("normalTex");


            //ImageData imgData2 = RC.LoadImage("Assets/normal2.jpg");
            //RC.SetShaderParamTexture(_texture2Param, iTex2);

            //ITexture iTex = RC.CreateTexture(imgData);
            // iTex2 = RC.CreateTexture(imgData2);
            //RC.SetShaderParamTexture(_texture1Param, iTex);
            //RC.SetShaderParamTexture(_texture2Param, iTex2);
            //RC.SetShaderParamTexture(_texture2Param, iTex2);




           // _world.addObject(geo2, m, 0, -100, 700);
            _world.addObject(geo, m, 500, -100, 700);

            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;

        }

        public override void RenderAFrame()
        {
            x = (float)Math.Cos(time) * 500;
            z = (float)Math.Sin(time) * 500;
            time += 0.01f;
            RC.SetLightPosition(0, new float3(x, 0, z));
            RC.SetLightDirection(0, new float3(-x, 0, -z));



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
