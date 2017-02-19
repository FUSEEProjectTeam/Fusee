#define GUI_SIMPLE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
#if GUI_SIMPLE
using Fusee.Engine.Core.GUI;
#endif

namespace Fusee.Engine.Examples.Simple.Core
{

    [FuseeApplication(Name = "Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = MathHelper.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

#if GUI_SIMPLE
        private GUIHandler _guiHandler;

        private GUIButton _guiFuseeLink;
        private GUIImage _guiFuseeLogo;
        private FontMap _guiLatoBlack;
        private GUIText _guiSubText;
        private float _subtextHeight;
        private float _subtextWidth;
#endif

        // Init is called on startup. 
        public override void Init()
        {
#if GUI_SIMPLE
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
            _guiFuseeLogo = new GUIImage(AssetStorage.Get<ImageData>("FuseeLogo150.png"), 10, 10, -5, 150, 80);
            _guiHandler.Add(_guiFuseeLogo);
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            fontLato.UseKerning = true;
            _guiLatoBlack = new FontMap(fontLato, 18);
            _guiSubText = new GUIText("Simple FUSEE Example", _guiLatoBlack, 100, 100);
            _guiSubText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);
            _guiHandler.Add(_guiSubText);
            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);
#endif

            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.2f, 0.2f, 0.2f, 1);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("WuggyLand.fus");

            var cube = new Cube();
            
            _rocketScene.Children.Add(new SceneNodeContainer
            {
                Children = new List<SceneNodeContainer>(),
                Components = new List<SceneComponentContainer>
                {
                    new LightComponent
                    {
                        Active = true,
                        AmbientCoefficient = 1f,
                        Attenuation = 0.9f,
                        Color = new float3(0.9f,0.9f,0.9f),
                        ConeAngle = 45f,
                        ConeDirection = new float3(0,0,1),
                        Position = new float3(0, 0, 0),
                       Type = LightType.Point
                    },
                    new TransformComponent
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One * 100f,
                        Translation = float3.Zero
                    },
                    new MeshComponent
                    {
                        Normals = cube.Normals,
                        Triangles = cube.Triangles,
                        UVs = cube.UVs,
                        Vertices = cube.Vertices
                    },
                    new MaterialComponent
                    {
                        Diffuse = new MatChannelContainer
                        {
                            Color = float3.One
                        }
                    }
                }

            });



            _rocketScene.Children[0].Children[0].Name = "cube";

            // Wrap a SceneRenderer around the model.
            // Shadow
            //_sceneRenderer = new SceneRenderer(_rocketScene, LightningCalculationMethod.SIMPLE, true);
            // Deferred
            _sceneRenderer = new SceneRenderer(_rocketScene, LightningCalculationMethod.SIMPLE, true);
        
         
            _rocketScene.Children[0].Children[0].Components[2].Name = "debug";

        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            // No backbuffer cleared here
            RC.Clear(ClearFlags.Color);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
            }


            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var transform = _rocketScene.Children[8].GetComponent<TransformComponent>();
            transform.Translation = new float3(transform.Translation.x + Keyboard.ADAxis * 2.5f, transform.Translation.y + Keyboard.WSAxis * 2.5f, transform.Translation.z + Keyboard.UpDownAxis * 2.5f);
            var light = _rocketScene.Children[8].GetComponent<LightComponent>();
            light.Position = new float3(light.Position.x + Keyboard.ADAxis * 2.5f, light.Position.y + Keyboard.WSAxis * 2.5f, light.Position.z + Keyboard.UpDownAxis * 2.5f);


            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -3000, 0, -100, 0, 0, 1, 0);
            var mtxScale = float4x4.CreateScale(1f);

            RC.ModelView = mtxCam * mtxRot * mtxScale; 


            
            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

#if GUI_SIMPLE
            _guiHandler.RenderGUI();
#endif

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 200000);
            RC.Projection = projection;

#if GUI_SIMPLE
            _guiSubText.PosX = (int)((Width - _subtextWidth) / 2);
            _guiSubText.PosY = (int)(Height - _subtextHeight - 3);

            _guiHandler.Refresh();
#endif

        }

#if GUI_SIMPLE
        private void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderWidth = 0;
            SetCursor(CursorType.Standard);
        }

        private void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
            _guiFuseeLink.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea)
        {
            OpenLink("http://fusee3d.org");
        }
#endif
    }
}