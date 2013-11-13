using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;
using RigidBody = BulletSharp.RigidBody;


namespace Examples.BulletSharp
{
    public class BulletSharp : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshTea, _meshFace, _meshCube, _meshSphere;

        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        MyDemo MyDemo = new MyDemo();
        MatrixConverter MatrixConverter = new MatrixConverter();

        Clock Clock = new Clock();

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            _meshSphere = MeshReader.LoadMesh(@"Assets/Sphere.obj.model");

            _spColor = MoreShaders.GetShader("simple", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);

            _colorParam = _spColor.GetShaderParam("vColor");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);          
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            //MyDemo.OnUpdate();

            // MyDemo.World.StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps)
            // timeSteps = time since the last call
            // maxSubSteps =  maximum number of steps that Bullet is allowed to take each time you call it
            // fixedTimeStep = the size of that internal step.
            MyDemo.World.StepSimulation((float)Time.Instance.DeltaTime, Time.Instance.FramePerSecondSmooth);
            //MyDemo.World.StepSimulation((float)Time.Instance.DeltaTime, 2);
           // Debug.WriteLine("timeSptep: " +Time.Instance.DeltaTime);

            MyDemo.World.PerformDiscreteCollisionDetection();

            //Debug.WriteLine("maxSubSteps: " + Time.Instance.FramePerSecond);
            //Debug.WriteLine("fixedTimeStep: " + (1 / Time.Instance.FramePerSecond));
            Debug.WriteLine("Time.Instance.FramePerSecond: " + Time.Instance.FramePerSecond);

            // move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
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
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            Point pos = Input.Instance.GetMousePos();
            
            if (Input.Instance.OnKeyDown(KeyCodes.Space))
                MyDemo.ShootBox(new Vector3(0, 10, 20), new Vector3(0, 1, 0));

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = mtxRot * float4x4.LookAt(0, 40, 100, 0, 0, 0, 0, 1, 0);

           /* var rb = MyDemo.World.CollisionObjectArray[1];
            var body = rb as RigidBody;
            Matrix startTransform = Matrix.Translation(6,6,6);
            MotionState ms = new DefaultMotionState(startTransform);
            RigidBodyConstructionInfo conf = new RigidBodyConstructionInfo(5,ms, new BoxShape(1), new Vector3(33,33,33));
            RigidBody myBody = new RigidBody(conf);
            myBody.Gravity = new Vector3(0);*/

            int n = MyDemo.World.CollisionObjectArray.Count;
            var tea = MyDemo.World.CollisionObjectArray[n-1];


            var ma = MatrixConverter.BtMAtrixToF3dMatrix(tea);
            RC.ModelView = float4x4.Scale(0.05f) * ma * mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            RC.Render(_meshTea);


            for (int i = 0; i < 4; i++)
            {
                
                var ground = (RigidBody)MyDemo.World.CollisionObjectArray[i];
                
                RC.ModelView = float4x4.Scale(0.099f, 0.001f, 0.099f) * float4x4.CreateTranslation(ground.WorldTransform.M41, ground.WorldTransform.M42, ground.WorldTransform.M43) * mtxCam;
                RC.SetShader(_spColor);
                RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, i/10, 1));
                RC.Render(_meshCube);
            }
            
            for (int i = 4; i < MyDemo.World.CollisionObjectArray.Count-1; i++)
            {
                
                var model = MyDemo.World.CollisionObjectArray[i];


                var matrx = MatrixConverter.BtMAtrixToF3dMatrix(model);
                RC.ModelView = float4x4.Scale(0.04f) * matrx * mtxCam;
               
                RC.SetShader(_spTexture);
                RC.SetShaderParamTexture(_textureParam, _iTex);

                RC.Render(_meshCube);
 
            }

            
          
            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 2000);
        }

        public static void Main()
        {
            var app = new BulletSharp();
            app.Run();
        }

    }
}
