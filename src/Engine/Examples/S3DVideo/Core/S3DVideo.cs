﻿#define GUI_SIMPLE

using System;
using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Emgu.CV;
#if GUI_SIMPLE
using Fusee.Engine.Core.GUI;
#endif

namespace Fusee.Engine.Examples.S3DVideo.Core
{

    [FuseeApplication(Name = "S3DVideo Example", Description = "A very simple example.")]
    public class S3DVideo : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = MathHelper.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private StereoCameraRig _stereoCam;
        private ScreenS3D _screen;
        private IVideoStreamImp vl, vr, vld, vrd;
        Capture capL, capR, capLD, capRD;

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
            _guiSubText = new GUIText("FUSEE Example", _guiLatoBlack, 100, 100);
            _guiSubText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);
            _guiHandler.Add(_guiSubText);
            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);
            #endif

            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            //Create StereoCam for S3D rendering
            _stereoCam = new StereoCameraRig(Stereo3DMode.Anaglyph, Width, Height, 6.5f);
            _stereoCam.AttachToContext(RC);

            //Create ScreenS3DTextures object holding the 4 textures to be used with the ScreenS3D object
            ScreenS3DTextures screenTex = new ScreenS3DTextures();
            screenTex.Left = RC.CreateTexture(AssetStorage.Get<ImageData>("left.png"));
            screenTex.LeftDepth = RC.CreateTexture(AssetStorage.Get<ImageData>("depthLeft.png"));
            screenTex.Right = RC.CreateTexture(AssetStorage.Get<ImageData>("right.png"));
            screenTex.RightDepth = RC.CreateTexture(AssetStorage.Get<ImageData>("depthRight.png"));
            //Create ScreenS3D Object using the ScreenS3Dtextures object from above
            _screen = new ScreenS3D(RC, screenTex);
            //Set the config fort the Screen objet. This can also be doene using a whole ScreenConfig object and assiging direktly to the ScreenS3D object
            _screen.Config.ScaleSize = 1000;
            _screen.Config.ScaleDepth = 5;
            _screen.Config.Transform = float4x4.CreateTranslation(0,200,0);
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_rocketScene);


            // vl = AssetStorage.Get<VideoStream>("left.mkv");
            capL = new Capture("Assets/left.mkv");
            capLD = new Capture("Assets/depthLeft.mkv");
            capR = new Capture("Assets/right.mkv");
            capRD = new Capture("Assets/depthRight.mkv");

        }

        int count = 0;
        private void UpdateVideos()
        {
            if (count < 300)
            {
                var frameL = capL.QueryFrame();
                var imgData = new ImageData();
                imgData.Width = frameL.Width;
                imgData.Height = frameL.Height;
                imgData.PixelFormat = ImagePixelFormat.RGB;
                imgData.Stride = 3;

                imgData.PixelData = frameL.GetData();
                _screen.TexturesLR_DLR.Left = RC.CreateTexture(imgData);

                imgData.PixelData = capLD.QueryFrame().GetData();
                _screen.TexturesLR_DLR.LeftDepth = RC.CreateTexture(imgData);

                imgData.PixelData = capR.QueryFrame().GetData();
                _screen.TexturesLR_DLR.Right = RC.CreateTexture(imgData);

                imgData.PixelData = capRD.QueryFrame().GetData();
                _screen.TexturesLR_DLR.RightDepth = RC.CreateTexture(imgData);
                //_screen.TexturesLR_DLR.Left = RC.CreateTexture(vl.GetCurrentFrame());
                count++;
            }
            else
            {
                capL.Dispose();
                capLD.Dispose();
                capR.Dispose();
                capRD.Dispose();

                capL= null;
                capLD= null;
                capR= null;
                capRD= null;

                capL = new Capture("Assets/left.mkv");
                capLD = new Capture("Assets/depthLeft.mkv");
                capR = new Capture("Assets/right.mkv");
                capRD = new Capture("Assets/depthRight.mkv");
                count = 0;
            }
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            UpdateVideos();

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
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }
            if (Input.Keyboard.IsKeyDown(KeyCodes.PageUp) == true)
            {
                _screen.SetHit(10);
            }
            if(Input.Keyboard.IsKeyDown(KeyCodes.PageDown) == true)
            {
                _screen.SetHit(-10);
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);

            // 3d mode
            var eyeF = new float3(0, 110, -1000);
            var targetF = new float3(0, 110, 0);
            var upF = new float3(0, 1, 0);

            _stereoCam.Prepare(Stereo3DEye.Left);
            for (var x = 0; x < 2; x++)
            {
                var lookAt = _stereoCam.LookAt3D(_stereoCam.CurrentEye, eyeF, targetF, upF);
                var mtx = lookAt * mtxRot;
              //  var mtxCam = float4x4.LookAt(0, 20, -600, 0, 150, 0, 0, 1, 0);
                RC.ModelView = mtx * float4x4.CreateTranslation(new float3(0,0,0));

                // Render the scene loaded in Init()
                    //Render FUSEE Rocket
                _sceneRenderer.Render(RC);
                //Render ScreenS3D object
                _screen.Render(_stereoCam, lookAt * mtx);

                _stereoCam.Save();
                if (x == 0)
                {
                    _stereoCam.Prepare(Stereo3DEye.Right);
                }                
            }
            _stereoCam.Display();


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
            var aspectRatio = Width/(float) Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            // var projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 20000);
            // RC.Projection = projection;
          
            _stereoCam.UpdateOnResize(Width, Height);         
            _stereoCam.SetFrustums(RC, MathHelper.PiOver4, aspectRatio, 1,20000, 10000);

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