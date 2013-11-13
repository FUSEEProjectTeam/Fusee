using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;
using BulletSharp;
using System.Diagnostics;
using System;

namespace Examples.BulletSharpScaledWorld
{
    public class BulletSharpScaledWorld : RenderCanvas
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

        ScaleDemo ScaleDemo = new ScaleDemo();
        MatrixConverter MatrixConverter = new MatrixConverter();

       

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

            ScaleDemo.World.StepSimulation((float)Time.Instance.DeltaTime);
            ScaleDemo.World.PerformDiscreteCollisionDetection();


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
                ScaleDemo.ShootBox(new Vector3(0, 500, 1000), new Vector3(0, 100, 0));

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = mtxRot * float4x4.LookAt(0, 150, 1500, 0, 0, 0, 0, 1, 0);



            /*for (int i = 0; i < 4; i++)
            {*/
                var ground = ScaleDemo.World.CollisionObjectArray[0];
                RC.ModelView = float4x4.Scale(0.099f, 0.001f, 0.099f) * float4x4.CreateTranslation(ground.WorldTransform.M41, ground.WorldTransform.M42, ground.WorldTransform.M43) * mtxCam;
                RC.SetShader(_spColor);
                RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f,  0.5f, 1));
                RC.Render(_meshCube);
           // }

            for (int i = 4; i < ScaleDemo.World.CollisionObjectArray.Count; i++)
            {
                var model = ScaleDemo.World.CollisionObjectArray[i];

                var matrx = MatrixConverter.BtMAtrixToF3dMatrix(model);
                RC.ModelView = float4x4.Scale(0.4f) * matrx * mtxCam;

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
            var app = new BulletSharpScaledWorld();
            app.Run();
        }

    }
}
