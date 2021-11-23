using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace FuseeApp
{
    [FuseeApplication(Name = "Fusee_App", Description = "Yet another FUSEE App.")]
    public class Fusee_App : RenderCanvas
    {
        // Horizontal and vertical rotation Angles for the displayed object 
        private static float _angleHorz = M.PiOver4, _angleVert;

        // Horizontal and vertical angular speed
        private static float _angleVelHorz, _angleVelVert;

        // Overall speed factor. Change this to adjust how fast the rotation reacts to input
        private const float RotationSpeed = 7;

        // Damping factor 
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private bool _keys;

        private bool _isLoaded = false;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Dispatch async loading (this is only needed if you actually want to use this application in Blazor)
            InitAsync();
        }

        private async void InitAsync()
        {
            // Load the rocket model
            _rocketScene = await AssetStorage.GetAsync<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);

            _isLoaded = true;
        }

        // Update is called 60 times a second
        public override void Update()
        {
        }

        // RenderAFrame is called once per frame
        public override void RenderAFrame()
        {
            if (!_isLoaded) return;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

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

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current View transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 2, -8, 0, 1.5f, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

            // Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}