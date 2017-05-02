using System;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;


// For this demo the MaterialPBRComponent Cache needs to be disabled in order to see live changes with the material properties.
// To do so comment line 1073 and line 1077 in SceneRenderer.cs
// line 1073: if (_pbrComponent.TryGetValue(mc, out mat)) return mat;
// line 1077: _pbrComponent.Add(mc, mat);
// 
// today is 02.05.2017

namespace Fusee.Engine.Examples.PBRMatComp.Core
{
    internal enum CurrentManipulatedItem
    {
        Roughness,
        DiffFraction,
        Fresnel
    }

    [FuseeApplication(Name = "PBRMaterialComponent Example", Description = "A simple PBRMaterial example.")]
    // ReSharper disable once InconsistentNaming
    public class PBRMatComp : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private GUIHandler _guiHandler;
        private FontMap _guiLatoBlack;
        private GUIText _guiSubText;
        private float _subtextHeight;
        private float _subtextWidth;

        private string _text;

        private float _roughness = 0.5f;
        private float _diffFraction = 0.5f;
        private float _fresnel = 0.5f;

        private CurrentManipulatedItem _currentItem;
        

        // Init is called on startup. 
        public override void Init()
        {
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            fontLato.UseKerning = true;
            _guiLatoBlack = new FontMap(fontLato, 18);

            _text = $"(F1) Roughness: {System.Math.Round(_roughness, 3)}  " +
                    $"(F3) Diffuse Fraction: {System.Math.Round(_diffFraction, 3)}  " +
                    $"(F2) Fresnel Reflectance: {System.Math.Round(_fresnel, 3)}  Current Item Manipulated: {_currentItem}";

            _guiSubText = new GUIText(_text, _guiLatoBlack, 0, 0) {TextColor = new float4(0, 0, 0, 1.0f)};
            _guiHandler.Add(_guiSubText);
            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);

            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Create MaterialPBRComponent
            // PBRMatComp inherits from MaterialComponent
            // Roughness: This factor defines the fraction of microfacets that are oriented in the same way as the halfway vector v
            // FresnelReflectance: The fresnel factor defines what fraction of the incoming light is reflected and what fraction is transmitted.
            // DiffuseFraction: Fraction of diffuse reflection (specular reflection = 1 - k)
            var pbrComponent = new MaterialPBRComponent
            {
                Diffuse = new MatChannelContainer
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    Color = ColorUint.ForestGreen.Tofloat3()
                },
                Specular = new SpecularChannelContainer
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    Color = ColorUint.White.Tofloat3()
                },
                DiffuseFraction = _diffFraction,
                FresnelReflectance = _fresnel,
                RoughnessValue = _roughness
            };

            // Insert into SceneGraph
            // Replace MaterialComponent with MaterialPBRComponent
            // MaterialPBRComponent needs to be in the same order as any MaterialComponent:
            // TransformComponent, Material, MeshComponent, ....
            _rocketScene.Children[0].Components[1] = pbrComponent;

            // Wrap a SceneRenderer around the model.
            // Set LightingCalculationMethod to Advanced, Cook-Torrance.
            _sceneRenderer = new SceneRenderer(_rocketScene, LightingCalculationMethod.ADVANCED);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (System.Math.Abs(Keyboard.LeftRightAxis) > 0.1f || System.Math.Abs(Keyboard.UpDownAxis) > 0.1)
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
                   // _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                   // _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                    switch (_currentItem)
                    {
                        case CurrentManipulatedItem.Roughness:
                            _roughness += Keyboard.UpDownAxis * 0.001f ;
                            break;
                        case CurrentManipulatedItem.DiffFraction:
                            _diffFraction += Keyboard.UpDownAxis * 0.001f;
                            break;
                        case CurrentManipulatedItem.Fresnel:
                            _fresnel += Keyboard.UpDownAxis * 0.001f;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                  
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            if (Keyboard.GetKey(KeyCodes.F1))
                _currentItem = CurrentManipulatedItem.Roughness;
            if (Keyboard.GetKey(KeyCodes.F2))
                _currentItem = CurrentManipulatedItem.Fresnel;
            if (Keyboard.GetKey(KeyCodes.F3))
                _currentItem = CurrentManipulatedItem.DiffFraction;



            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -600, 0, 150, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            // update Text
            _guiSubText.Text = $"(F1) Roughness: {System.Math.Round(_roughness, 3)}  " +
                               $"(F2) Fresnel Reflectance: {System.Math.Round(_fresnel, 3)}  " +
                               $"(F3) Diffuse Fraction: {System.Math.Round(_diffFraction, 3)}  Current Item Manipulated: {_currentItem}";

            // update Vars
            MaterialPBRComponent pbrMat = _rocketScene.Children[0].Components[1] as MaterialPBRComponent;
            if (pbrMat != null)
            {
                pbrMat.DiffuseFraction = _diffFraction;
                pbrMat.FresnelReflectance = _fresnel;
                pbrMat.RoughnessValue = _roughness;
            }
            _rocketScene.Children[0].Components[1] = pbrMat;

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiHandler.RenderGUI();
            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
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
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;

            _guiSubText.PosX = (int)((Width - _subtextWidth) / 2);
            _guiSubText.PosY = (int)(_subtextHeight + 20);

            _guiHandler.Refresh();
        }
    }
}