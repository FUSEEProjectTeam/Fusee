<<<<<<< HEAD
﻿using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Simple
{
    public class Simple : RenderCanvas
    {
        private RenderQueue _queue = new RenderQueue();
        public List<RenderJob> RenderJobs = new List<RenderJob>(); 
        public List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        public float4x4 Camera = float4x4.LookAt(0, 200, 2000, 0, 50, 0, 0, 1, 0);
        private Material _material = new Material();
       
        //TestZone
        private SceneEntity TestEntity = new SceneEntity();
        private Renderer testrenderer = new Renderer();
        private TestBehaviour testscript = new TestBehaviour();

        // Child
        private SceneEntity ChildEntity = new SceneEntity();
        private Renderer Childrenderer = new Renderer();
        private ChildAction Childscript = new ChildAction();
        

        protected float4 _farbe = new float4(1, 0, 0, 1);
        protected IShaderParam _vColorParam;

        public override void Init()
        {
           
            // Parent
            TestEntity.AddComponent(testrenderer);
            TestEntity.AddComponent(testscript);
            testscript.Init(TestEntity);
            TestEntity.AddChild(ChildEntity); // Als Child hinzugefuegt
            _queue.SceneMembers.Add(TestEntity);
            
            // Child
            ChildEntity.AddComponent(Childrenderer);
            ChildEntity.AddComponent(Childscript);
            Childscript.Init(ChildEntity);
            Childscript.Start();
            testscript.Start();

            ShaderProgram sp = RC.CreateShader(_material._vs, _material._ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("vColor");
            RC.ClearColor = new float4(1, 1, 1, 1);
            RC.SetShaderParam(_vColorParam, _farbe);
        }


        public override void  RenderAFrame()
        {
            _queue.Traverse(this, RC, Camera);
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
=======
﻿using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.Simple
{
    public class Simple : RenderCanvas 
    {
        protected string Vs = @"
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

        protected string Ps = @"
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
        protected Mesh Mesh, MeshFace;
        protected IShaderParam VColorParam;

        public override void Init()
        {
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
            Mesh = geo.ToMesh();

            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Face.obj.model"));
            MeshFace = geo2.ToMesh();

            _angleHorz = 0;
            _rotationSpeed = 10.0f;
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            VColorParam = sp.GetShaderParam("vColor");


            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color| ClearFlags.Depth);

            
            if (In.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed * In.GetAxis(InputAxis.MouseX) * (float) DeltaTime;
                _angleVelVert = _rotationSpeed * In.GetAxis(InputAxis.MouseY) * (float) DeltaTime;
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

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            RC.SetShaderParam(VColorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.Render(Mesh);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
            RC.SetShaderParam(VColorParam, new float4(0.8f, 0.5f, 0, 1));
            RC.Render(MeshFace);
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
            float3[] verts = new float3[1000000];
            
            double t1 = Diagnostics.Timer;
            for (int i= 0; i < verts.Length; i++)
            {
                verts[i].x = i;
                verts[i].y = i+1;
                verts[i].z = i-1;
            }
            double t2 = Diagnostics.Timer;
            Diagnostics.Log("Initializing " + verts.Length + " float3 objects took " + (t2 - t1) + " ms.");

            Simple app = new Simple();
            app.Run();
        }

    }
}
>>>>>>> remotes/origin/develop
