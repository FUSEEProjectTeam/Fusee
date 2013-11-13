using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;


namespace Examples.Simple
{
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private DynamicWorld world;
        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshTea, _meshFace, _meshCube;

        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        // is called on startup
        public override void Init()
        {
            world = new DynamicWorld();

            //create ground
            int size = 3;
            for (int b = -size; b < size; b++)
            {
                for (int c = -size; c < size; c++)
                {
                    world.AddRigidBody(0, new float3(b * 5, 0, c * 5), _meshFace, new float3(1, 1, 1));
                }
            }
            
            //Falling Tower
            int k, h, j;
            for (k = -2; k < 2; k++)
            {
                for (h = -2; h < 2; h++)
                {
                    for (j = -2; j < 2; j++)
                    {
                        var pos = new float3(4*h, 100+(k*4), 4*j);


                        world.AddRigidBody(1, pos, _meshTea, new float3(1, 1, 1));//ApplyCentralForce = new float3(50,0,0);

                    }
                }
            }

            world.GetRigidBody(15).ApplyTorque = new float3(10,0,0);
            
            //add Boxes
            /*    world.AddRigidBody(1, new float3(0, 100, 0), new float3(0, 0, 0));
                world.AddRigidBody(1, new float3(0, 200, 0), new float3(0, 0, 0));
                world.AddRigidBody(1, new float3(3, 300, 0), new float3(0, 0, 0));
                world.AddRigidBody(1, new float3(0, 500, 0), new float3(0, 0, 0));
            */

                //Set Restitution:
                /*world.GetRigidBody(0).Bounciness = 1.0f;
            world.GetRigidBody(1).Bounciness = 2.0f;
            world.GetRigidBody(2).Bounciness = 0.5f;
            world.GetRigidBody(3).Bounciness = 0.5f;
            world.GetRigidBody(4).Bounciness = 1.5f;*/

                world.GetRigidBody(1).ApplyCentralImpulse = new float3(10, 0, 0);
                // world.GetRigidBody(2).LinearVelocity = new float3(30,30,30);
                // world.GetRigidBody(2).ApplyForce(new float3(0,0,500), new float3(20,0,0) );
                // RigidBody body = idwi.GetRigidBody(0);
                int n = world.NumberRigidBodies();
                world.GetRigidBody(n - 1).ApplyCentralForce = new float3(20, 0, 0);
                Debug.WriteLine("Anzahl RigidBodies/CollisionsObjects: " + n);



            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");

            _spColor = MoreShaders.GetShader("simple", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);

            _colorParam = _spColor.GetShaderParam("vColor");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);
        }


        public virtual void ShootBox(float3 startPos, float3 destination)
        {

            world.AddRigidBody(1, startPos, _meshTea, new float3(1,1,1));
            int n = world.NumberRigidBodies();
            world.GetRigidBody(n-1).ApplyCentralForce = new float3(20,0,0);

        }

        // is called once a frame
        public override void RenderAFrame()
        {
            world.StepSimulation((float) Time.Instance.DeltaTime, 1/60, 1/60);
            //Debug.WriteLine("Frames Per Second: " + Time.Instance.FramePerSecond);
            
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float) Math.Exp(-Damping*Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += RotationSpeed*(float) Time.Instance.DeltaTime;
            /*if (Input.Instance.OnKeyDown(KeyCodes.Space))
                ShootBox(new float3(0,100,100), new float3(0,50,0) );*/
            if (Input.Instance.OnKeyDown(KeyCodes.I))
            {
                for (int i = 1; i < 5; i++)
                {
                    var myObject = world.GetRigidBody(world.NumberRigidBodies() - 10-i);
                    myObject.ApplyCentralImpulse = new float3(50, 50, 50);
                    Debug.WriteLine("ApplyCentralImpulse");
                }
                
            }
            if (Input.Instance.IsKeyDown(KeyCodes.F))
            {
                var myObject = world.GetRigidBody(world.NumberRigidBodies() - 1);
                myObject.ApplyCentralForce = new float3(0, 50,50);
                Debug.WriteLine("ApplyCentralForce");
            }
      

            var mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            var mtxCam = mtxRot * float4x4.LookAt(0, 100, 200, 0, 0, 0, 0, 1, 0);
            
            for (int i = 0; i < world.NumberRigidBodies(); i++)
            {
                var body = world.GetRigidBody(i);
                
                var matrix = world.GetRigidBody(i).WorldTransform;
                RC.ModelView = float4x4.Scale(0.025f) * matrix * mtxCam;
                
                RC.SetShader(_spTexture);
                RC.SetShaderParamTexture(_textureParam, _iTex);

                RC.Render(_meshCube);
            }
               

            

#region Render Simple
            /* first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            RC.Render(_meshTea);

            // second mesh
            RC.ModelView = mtxRot*float4x4.CreateTranslation(150, 0, 0)*mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            RC.Render(_meshFace);*/
#endregion

            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new Simple();
            app.Run();
        }
    }
}