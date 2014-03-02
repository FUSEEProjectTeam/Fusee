using System;
using System.Diagnostics;
using Fusee.Engine;

using Fusee.Math;

namespace Examples.PhysicsTest
{
    public class PhysicsTest : RenderCanvas
    {
        #region shader
        private const string Vs = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string Ps = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1)) *50;
            }";



        private const string Vt = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string Pt = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            
            uniform sampler2D vTexture;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                
                gl_FragColor = texture2D(vTexture, vUV) * dot(vNormal, vec3(0, 0, 1))*1.5;
            }";



        private const string VLin = @"
             attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string PLin = @"
             #ifdef GL_ES
                precision highp float;
            #endif
        
            
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1)) * 0.5;
            }";
        #endregion shader




        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshTea, _meshCube, _meshSphere, _meshCylinder, _meshPlatinic;

        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;
        private ShaderProgram _spLinda;

        private IShaderParam _colorParam;
        private IShaderParam _colorLinda;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        //Physic
        private static Physics _physic;
        
        public override void Init()
        {
            // is called on startup
            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            _meshSphere = MeshReader.LoadMesh(@"Assets/Sphere.obj.model");
            _meshCylinder = MeshReader.LoadMesh(@"Assets/Cylinder.obj.model");
            _meshPlatinic = MeshReader.LoadMesh(@"Assets/Platonic.obj.model");
            //RC.CreateShader(Vs, Ps);
            _spColor = RC.CreateShader(Vs, Ps); //MoreShaders.GetShader("simple", RC);
            _spTexture = RC.CreateShader(Vt, Pt);//MoreShaders.GetShader("texture", RC);
            _spLinda = RC.CreateShader(VLin, PLin);
            _colorParam = _spColor.GetShaderParam("vColor");
            _colorLinda = _spLinda.GetShaderParam("vColor");
            _textureParam = _spTexture.GetShaderParam("vTexture");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);

            _physic = new Physics();
        }


        public override void RenderAFrame()
        {
            var rb1 = _physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 3);
            var rb2 = _physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 1);
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _physic.World.StepSimulation((float) Time.Instance.DeltaTime, Time.Instance.FramePerSecondSmooth);
            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

           

            // move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.NumPad1))
            {
                _physic.World.Dispose();
                _physic.InitScene1();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.NumPad2))
            {
                _physic.World.Dispose();
                _physic.InitScene2();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.NumPad3))
            {
                _physic.World.Dispose();
                _physic.InitScene3();
            }


            if (Input.Instance.IsKeyDown(KeyCodes.NumPad4))
            {
               // var rb = _physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 3);
                rb1.ApplyCentralImpulse = new float3(-10, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.NumPad6))
            {
                //var rb = _physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 3);
                rb1.ApplyCentralImpulse = new float3(10, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                //_physic.World.GetRigidBody(_physic.World.NumberRigidBodies() -1).ApplyCentralImpulse = new float3(-10, 0, 0);
                rb2.ApplyCentralImpulse = new float3(-10, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                //_physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 1).ApplyCentralImpulse = new float3(10, 0, 0);
                rb2.ApplyCentralImpulse = new float3(10, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _physic.World.GetRigidBody(_physic.World.NumberRigidBodies() - 1).ApplyCentralImpulse = new float3(0, 10, 0);

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _physic.World.GetRigidBody(_physic.World.NumberRigidBodies()-1).ApplyCentralImpulse = new float3(0,-10,0);
                    // _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;
           
            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = mtxRot * float4x4.LookAt(0, 20, 70, 0, 0, 0, 0, 1, 0);


            if (Input.Instance.IsKey(KeyCodes.Space))
            {
                Debug.WriteLine(Width - Input.Instance.GetMousePos().x);
                Debug.WriteLine(Input.Instance.GetAxis(InputAxis.MouseX));

                _physic.Shoot(new float3(0, 15, 15), new float3(0, 0, 0));
            }
           
            //Render all RigidBodies


            /*var ground = _physic.World.GetRigidBody(0);
            var groundShape = (BoxShape) ground.CollisionShape;
            var ma = ground.WorldTransform;
            RC.ModelView = float4x4.Scale(groundShape.HalfExtents.x / 100, groundShape.HalfExtents.y / 100, groundShape.HalfExtents.z / 100) * ma * mtxCam;
            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.0f, 0.0f, 1.0f, 1));
            RC.Render(_meshCube);*/

            //Debug.WriteLine("FramePerSecond: " +Time.Instance.FramePerSecond);
            for (int i = 0; i < _physic.World.NumberRigidBodies(); i++)
            {
                var rb = _physic.World.GetRigidBody(i);
                var matrix = rb.WorldTransform;
                
                if (rb.CollisionShape.GetType().ToString() == "Fusee.Engine.BoxShape")
                {
                    var shape = (BoxShape) rb.CollisionShape;
                    RC.ModelView = float4x4.Scale(shape.HalfExtents.x/100, shape.HalfExtents.y/100, shape.HalfExtents.z /100) * matrix * mtxCam;
                    RC.SetShader(_spColor);
                    RC.SetShaderParam(_colorParam, new float4(0.9f, 0.9f, 0.0f, 1));
                    RC.Render(_meshCube);
                }
                else if (rb.CollisionShape.GetType().ToString() == "Fusee.Engine.SphereShape")
                {
                    var shape = (SphereShape) rb.CollisionShape;
                    RC.ModelView = float4x4.Scale(shape.Radius)*matrix*mtxCam;
                    RC.SetShader(_spTexture);
                    RC.SetShaderParamTexture(_textureParam, _iTex);
                    RC.Render(_meshSphere);
                }
                else if (rb.CollisionShape.GetType().ToString() == "Fusee.Engine.CylinderShape")
                {
                    var shape = (CylinderShape)rb.CollisionShape;
                    RC.ModelView = float4x4.Scale(4) * matrix * mtxCam;
                    RC.SetShader(_spLinda);
                    RC.SetShaderParam(_colorLinda, new float4(0.1f, 0.1f, 0.9f, 1));
                    RC.Render(_meshCylinder);
                }
                else if (rb.CollisionShape.GetType().ToString() == "Fusee.Engine.ConvexHullShape")
                {
                    Debug.WriteLine("ConvexHullShape");
                    var shape = (ConvexHullShape)rb.CollisionShape;
                    RC.ModelView = float4x4.Scale(1.0f)*matrix * mtxCam;
                    
                    //RC.SetShader(_spTexture);
                    //RC.SetShaderParamTexture(_textureParam, _iTex);
                    RC.SetShader(_spLinda);
                    RC.SetShaderParam(_colorLinda, new float4(2f, 2f, 2, 1));
                    RC.Render(_meshPlatinic);
                }

                
                //RC.SetShader(_spTexture);
                //RC.SetShaderParamTexture(_textureParam, _iTex);
               // RC.Render(_meshCube);
            }
           
            

            #region RenderSimple
            //first mesh
            /*RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            RC.Render(_meshTea);

            // second mesh
            RC.ModelView = mtxRot*float4x4.CreateTranslation(150, 0, 0)*mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            RC.Render(_meshCube);*/
            #endregion RenderSimple
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
            var app = new PhysicsTest();
            app.Run();
            _physic.World.Dispose();
        }

    }
}
