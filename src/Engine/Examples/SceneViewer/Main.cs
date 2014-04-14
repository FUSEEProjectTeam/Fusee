using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.Serialization;
using Fusee.Engine.SimpleScene;

namespace Examples.SceneViewer
{
    public class SceneViewer : RenderCanvas
    {
        private float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private float _zVel, _zVal;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;
        private float _subtextWidth;
        private float _subtextHeight;
        private float4x4 _modelScaleOffset;

        // model variables
        private Mesh _meshFace;
        private Mesh _meshTea;
        private SceneRenderer _sr;
        private ShaderProgram _sColor;
        private IShaderParam _colorParam;

        // GUI stuff
        private GUIHandler _guiHandler;
        private GUIImage _guiFuseeLogo;
        private GUIButton _guiFuseeLink;
        private GUIText _guiSubText;
        private IFont _guiLatoBlack;

        public void AdjustModelScaleOffset()
        {
            AABBf? box = null;
            if (_sr == null || (box = _sr.GetAABB()) == null)
            {
                _modelScaleOffset = float4x4.Identity;
            }
            var bbox = ((AABBf) box);
            float scale = Math.Max(Math.Max(bbox.Size.x, bbox.Size.y), bbox.Size.z);
            _modelScaleOffset = float4x4.CreateScale(200.0f/scale)*float4x4.CreateTranslation(-bbox.Center);
        }

        // is called on startup
        public override void Init()
        {
            //TestSerialize();
            //TestDeserialize();
            
            // GUI initialization
            _zVal = 500;
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            _guiFuseeLink = new GUIButton(6, 6, 157, 87);
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderColor = new float4(0, 0.6f, 0.2f, 1);
            _guiFuseeLink.BorderWidth = 0;
            _guiFuseeLink.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            _guiFuseeLink.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            _guiFuseeLink.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            _guiHandler.Add(_guiFuseeLink);

            _guiFuseeLogo = new GUIImage("Assets/FuseeLogo150.png", 10, 10, -5, 150, 80);
            _guiHandler.Add(_guiFuseeLogo);

            _guiLatoBlack = RC.LoadFont("Assets/Lato-Black.ttf", 14);
            _guiSubText = new GUIText("FUSEE 3D Scene Viewer", _guiLatoBlack, 100, 100);
            _guiSubText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);
            _guiHandler.Add(_guiSubText);

            // Scene loading
            SceneContainer scene;
            var ser = new Serializer();
            using (var file = File.OpenRead(@"Assets/Model.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            _sr = new SceneRenderer(scene, "Assets");
            AdjustModelScaleOffset();
            _guiSubText.Text = "FUSEE 3D Scene";
            if (scene.Header.CreatedBy != null || scene.Header.CreationDate != null)
            {
                _guiSubText.Text += " created";
                if (scene.Header.CreatedBy != null)
                {
                    _guiSubText.Text += " by " + scene.Header.CreatedBy;
                }
                if (scene.Header.CreationDate != null)
                {
                    _guiSubText.Text += " on " + scene.Header.CreationDate;
                }
            }

            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);

            _sColor = MoreShaders.GetDiffuseColorShader(RC);
            RC.SetShader(_sColor);
            _colorParam = _sColor.GetShaderParam("color");
            RC.SetShaderParam(_colorParam, new float4(1, 1, 1, 1));
            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        private void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderWidth = 0;
            SetCursor(CursorType.Standard);
        }


        private void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
            _guiFuseeLink.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            OpenLink("http://fusee3d.org");
        }


        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * 30 * (float)Time.Instance.DeltaTime * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * 30 * (float)Time.Instance.DeltaTime * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            if (Input.Instance.GetAxis(InputAxis.MouseWheel) == 0)
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);
                _zVel *= (curDamp * curDamp * curDamp);
            }
            else
            {
                _zVel = -100000 * Input.Instance.GetAxis(InputAxis.MouseWheel) * (float)Time.Instance.DeltaTime;
            }

            _angleHorz -= _angleVelHorz;
            _angleVert -= _angleVelVert;
            _zVal = Math.Max(100, Math.Min(_zVal + _zVel, 1000));

            // move per keyboard
            if (Input.Instance.IsKey(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 200, -_zVal, 0, 0, 0, 0, 1, 0);

            
            // first mesh
            //RC.ModelView = mtxCam * mtxRot /* float4x4.CreateScale(100) * */;
            //RC.SetShader(_spColor);
            //RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));
            //RC.Render(_meshTea);

            RC.ModelView = mtxCam * mtxRot * _modelScaleOffset;
            _sr.Render(RC);
            _guiHandler.RenderGUI();

            // swap buffers
              Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            _guiSubText.PosX = (int)((Width - _subtextWidth) / 2);
            _guiSubText.PosY = (int)(Height - _subtextHeight - 3);
            _guiHandler.Refresh();        
        }

        private void TestSerialize()
        {
            /**/
            var aMesh = new MeshContainer
            {
                Vertices = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Normals = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Triangles = new ushort[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 1,
                    4, 5, 6,
                    4, 6, 7,
                    4, 7, 5,
                }
            };

            var aChild = new SceneObjectContainer()
            {
                Mesh = aMesh,
                Transform = new TransformContainer(){Rotation = new float3(0, 0, 0), Translation = new float3(0.11f, 0.11f, 0), Scale = new float3(1, 1, 1)}
            };

            var parent = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    Version = 1,
                    Generator = "Fusee.SceneViewer",
                    CreatedBy = "FuseeProjetTeam"
                },
                Children = new List<SceneObjectContainer>(new SceneObjectContainer[]
                {
                    aChild,
                    aChild,
                    new SceneObjectContainer()
                    {
                        Mesh = aMesh,
                        Transform = new TransformContainer(){Rotation = new float3(0, 0, 0), Translation = new float3(0.22f, 0.22f, 0), Scale = new float3(1, 1, 1)}
                    },
                }),
            };
            var ser = new Serializer();
            using (var file = File.Create(@"Assets/Test.fus"))
            {
                ser.Serialize(file, parent);
            }
        }

        private void TestDeserialize()
        {
            var ser = new Serializer();
            SceneContainer mc2;
            using (var file = File.OpenRead(@"Assets/Test.fus"))
            {
                mc2 = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            /**/            
            Diagnostics.Log(mc2.ToString());
        }


        public static void Main()
        {
            var app = new SceneViewer();
            app.Run();
        }
    }
}
