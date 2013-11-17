using System;
using System.Diagnostics;
using BulletSharp;
using Fusee.Engine;
using Fusee.Math;
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
        private Mesh _meshTea, _meshCube;

        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        //Physic
        private Physic _physic;
       
        public override void Init()
        {
            // is called on startup
            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");

            _spColor = MoreShaders.GetShader("simple", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);

            _colorParam = _spColor.GetShaderParam("vColor");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);

            _physic = new Physic();
        }

       

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _physic.World.StepSimulation((float)Time.Instance.DeltaTime, 1 / 60, 1 / 60);
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
            {
                Debug.WriteLine("ApplyImpulse");
                var rb = _physic.World.GetConstraint(0).RigidBodyB;
                rb.ApplyCentralImpulse(new Vector3(0,-50,0));
            }
                //_angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _physic.World.GetConstraint(0).RigidBodyA.ApplyTorque(new Vector3(30, 30, 0));
                //_angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                Debug.WriteLine("ApplyImpulse");
                var co = _physic.World.CollisionObjectArray[0];
                var rb = (RigidBody)co;
                _physic.World.GetConstraint(0).RigidBodyA.ApplyCentralImpulse(new Vector3(0, 20, 0));
                
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                var rb = _physic.World.GetConstraint(0).RigidBodyB;
                rb.ApplyCentralImpulse(new Vector3(0, -50, 0));
            }

           // if (Input.Instance.IsKeyDown(KeyCodes.Down))
                //_physic.ExitPhysics();
           
            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = mtxRot *float4x4.LookAt(0, 70, 250, 0, 0, 0, 0, 1, 0);

            #region
            for (int i = 0; i < _physic.World.CollisionObjectArray.Count; i++)
            {

                var colisionObject = _physic.World.CollisionObjectArray[i];
                var btRigidBody = (RigidBody) colisionObject;

                var converter = new Convert();
                var matrix = converter.ConvertMatrixTof4X4(btRigidBody.WorldTransform);
                //var matrix = _physic.World.GetRigidBody(i).WorldTransform;
                RC.ModelView = float4x4.Scale(0.025f) * matrix * mtxCam;
                RC.SetShader(_spTexture);
                RC.SetShaderParamTexture(_textureParam, _iTex);
                RC.Render(_meshCube);
            }
            #endregion

            #region RenderConstraint
            for (int i = 0; i < _physic.World.NumConstraints; i++)
            {
                //Debug.WriteLine("Render Constraints");
                var constraint = _physic.World.GetConstraint(i);
                //var btRigidBody = (RigidBody) colisionObject;

                var converter = new Convert();
                var matrixA = converter.ConvertMatrixTof4X4(constraint.RigidBodyA.WorldTransform);
                //var matrix = _physic.World.GetRigidBody(i).WorldTransform;
                RC.ModelView = float4x4.Scale(0.025f) * matrixA * mtxCam;
                RC.SetShader(_spTexture);
                RC.SetShaderParamTexture(_textureParam, _iTex);
                RC.Render(_meshCube); 
               

                var matrixB = converter.ConvertMatrixTof4X4(constraint.RigidBodyB.WorldTransform);
                //var matrix = _physic.World.GetRigidBody(i).WorldTransform;
                RC.ModelView = float4x4.Scale(0.025f) * matrixB * mtxCam;
                RC.SetShader(_spTexture);
                RC.SetShaderParamTexture(_textureParam, _iTex);
                RC.Render(_meshCube);

            }
            #endregion RenderConstraint

            #region RenderSimple
            /* //first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;

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
            var app = new BulletSharp();
            app.Run();
        }

    }

}
