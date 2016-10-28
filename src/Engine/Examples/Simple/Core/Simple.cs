//#define GUI_SIMPLE

using System;
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
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("WuggyLand.fus");
            //_rocketScene = AssetStorage.Get<SceneContainer>("RocketModel.fus");
            RC.RenderDeferred = false;
            /* //// Legacy Mode
             // Wrap a SceneRenderer around the model.
             _sceneRenderer = new SceneRenderer(_rocketScene); */

            /* //// Legacy Mode with Lightning Calculation given. For Cook Torrance the calculation is aproximated from material
             // Wrap a SceneRenderer around the model.
             _sceneRenderer = new SceneRenderer(_rocketScene, LightningCalculationMethod.ADVANCED); */

            /*   //// LightComponents in Scene
               ///// Light is calculated with given Lightning Method & Light or SIMPLE as standard 
            Random rnd = new Random();

            for (var i = 0; i < 6; i++)
            {
                _rocketScene.Children[1].AddComponent(new LightComponent
                {
                    Active = true,
                    AmbientCoefficient = 0.01f,
                    Attenuation = 1000f,
                    Color = new float3((float) rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
                    ConeAngle = 35.0f,
                    ConeDirection = new float3(0f, 1f, 0f),
                    Name = "Light1",
                    Position = new float3(0f, 0f, -1f),
                    Type = LightType.Point
                });
            }  */
            _rocketScene.Children[0].AddComponent(new LightComponent
                {
                    Active = true,
                    AmbientCoefficient = 0.9f,
                    Attenuation = 1000f,
                    Color = new float3(1f,1f,1f),
                    ConeAngle = 45f,
                    ConeDirection = new float3(0f, 1f, 1f),
                    Name = "Light1",
                    Position = new float3(896f, 283.5f, 1455.25f),
                    Type = LightType.Spot
                });

           // var lightCone = AssetStorage.Get<SceneContainer>("Cube.fus");
           // _rocketScene.Children.Add(lightCone.Children[0]);




            //var transform = _rocketScene.Children[0].GetComponent<TransformComponent>();
            //transform.Translation = _rocketScene.Children[0].GetComponent<LightComponent>().PositionWorldSpace;
            /*
            _rocketScene.Children[0].AddComponent(new LightComponent
            {
                Active = true,
                AmbientCoefficient = 1000f,
                Attenuation = 0.001f,
                Color = new float3(0f, 1f, 1f),
                ConeAngle = 20.0f,
                ConeDirection = new float3(-1f, -1f, 0f),
                Name = "Light1",
                Position = new float4(100, 1000, 700, 1),
                Type = LightType.Spot
            });*/


            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_rocketScene, LightningCalculationMethod.ADVANCED);
          


            /*     //// MaterialLightComponent in Scene, lightningcaluclation with given ApplyLight Method over all lights
            _rocketScene.Children[0].AddComponent(new LightComponent
            {
                Active = true,
                AmbientCoefficient = 1f,
                Attenuation = 1f,
                Color = new float3(1f, 0f, 0f),
                ConeAngle = 35.0f,
                ConeDirection = new float3(0f, 1f, 0f),
                Name = "Light1",
                Position = new float4(0f, 0f, -1f, 1f),
                Type = LightType.Point
            });

            _rocketScene.Children[0].Children[1].Components[1] = new MaterialLightComponent
                {
                    ApplyLightString = "vec3 ApplyLight(Light light) {return vec3(1.0,0.0,0.0);}"
                };

                _sceneRenderer = new SceneRenderer(_rocketScene);   

            /*   ///// Render with PBR Material and Light

            _rocketScene.Children[0].AddComponent(new LightComponent
              {
                  Active = true,
                  AmbientCoefficient = 0.9f,
                  Attenuation = 2000f,
                  Color = new float3(1f, 1f, 1f),
                  ConeAngle = 35.0f,
                  ConeDirection = new float3(0f, 0f, 1f),
                  Name = "Light1",
                  Position = new float4(1000, 0, 1f, 1f),
                  Type = LightType.Spot
              });

            /*  _rocketScene.Children[0].AddComponent(new LightComponent
              {
                  Active = true,
                  AmbientCoefficient = 0.1f,
                  Attenuation = 1f,
                  Color = new float3(1f, 1f, 0.1f),
                  ConeAngle = 35.0f,
                  ConeDirection = new float3(1f, 0f, 1f),
                  Name = "Light1",
                  Position = new float4(0, 0, 1f, 1f),
                  Type = LightType.Point
              });
         
              var material = _rocketScene.Children[0].Components[1] as MaterialComponent;

              if (material != null)
              {
                  var pbr =  new MaterialPBRComponent
                  {
                      Bump = material.Bump,
                      Diffuse = material.Diffuse,
                      Emissive = material.Emissive,
                      Specular = material.Specular,
                      RoughnessValue = 0.1F,
                      FresnelReflectance = 0.9F,
                      DiffuseFraction = 0.1F
                  };

                  _rocketScene.Children[0].Components[1] = pbr;
              }

             _sceneRenderer = new SceneRenderer(_rocketScene, LightningCalculationMethod.SIMPLE); */
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

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
                //Diagnostics.Log($"New ModelMatrix? {RC.Model}");
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



            var light = _rocketScene.Children[0].GetComponent<LightComponent>();
            light.Position += new float3(Keyboard.LeftRightAxis * 10f, Keyboard.UpDownAxis * 10f, Keyboard.WSAxis * 10f);
            _rocketScene.Children[0].Components[3] = light;

            //Diagnostics.Log($"Is light changed? : {SceneRenderer.AllLightResults[0].PositionWorldSpace}");

                //var light2 = SceneRenderer.AllLightResults[0];
                //Diagnostics.Log($"FIRST: {light.PositionWorldSpace}");
               // light.PositionWorldSpace = light.PositionWorldSpace;
               // SceneRenderer.AllLightResults[0] = light2;
               // Diagnostics.Log($"SECOND: {SceneRenderer.AllLightResults[0].PositionWorldSpace}");


            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(40, 600, -600, 0, 150, 0, 0, 1, 0);
           
            RC.ModelView = mtxCam  * mtxRot ;

         //   _rocketScene.Children[8].GetComponent<TransformComponent>().Scale =
          //  new float3(10, 10, 10);

//            _sceneRenderer.AccumulateLight(_rocketScene.Children[0].GetLight());

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);




#if GUI_SIMPLE
            _guiHandler.RenderGUI();
#endif

            /*
                        var param = RC.GetShaderParam(RC.CurrentShader, "allLights[0].position");
                        if (param != null)
                        {
                            var value = RC.GetParamValue(RC.CurrentShader, param);
                            Diagnostics.Log($"ShaderValue {value}");
                            RC.SetShaderParam(param, new float3(0, 0, 0));
                            value = RC.GetParamValue(RC.CurrentShader, param);
                            Diagnostics.Log($"ShaderValue2 {value}");
                        }

                        _sceneRenderer.Render(RC); */

            /*   var list = RC.GetShaderParamList(RC.CurrentShader);
               foreach (var shaderParamInfo in list)
               {
                   Diagnostics.Log(shaderParamInfo.Name);
               }
              */




            /*  .PositionWorldSpace = light.PositionWorldSpace;
              light.ConeDirection = _rc.InvModelView * light.ConeDirection;
              effect.SetEffectParam($"allLights[{position}].position", light.PositionWorldSpace);
              */
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
            var projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 20000);
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
 