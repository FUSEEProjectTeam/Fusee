using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Examples.BoneAnimation.Core
{
    [FuseeApplication(Name = "FUSEE Bone Animation Example", Description = "Quick bone animation example")]
    public class Bone : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f,
                             _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;

        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private float4x4 _sceneCenter;
        private float4x4 _sceneScale;
        private bool _twoTouchRepeated;

        private bool _keys;

        private float _maxPinchSpeed;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        // Init is called on startup.
        public override void Init()
        {
            Diagnostics.Warn("[05/2020] Bone animation is disabled for now due to the Blender exporter not be able to export bones!");

            // Initial "Zoom" value (it's rather the distance in view direction, not the camera's focal distance/opening angle)
            _zoom = 400;

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = float4.One;

            // Load the standard model

            _scene = AssetStorage.Get<SceneContainer>("BoneAnim.fus");
            _gui = CreateGui();

            #region LEGACY CODE - REFERENCE ONLY!
            /*
            =====================================================================================
            // then add a weightcomponent with weight matrices etc:
            // binding matrices is the start point of every transformation
            // as many entries as vertices are present in current model
            var cube = _scene.Children[0].GetComponent<Mesh>();
            var vertexCount = cube.Vertices.Length;

            var bindingMatrices = new List<float4x4>();
            for (var i = 0; i < vertexCount; i++)
            {
                bindingMatrices.Add(float4x4.Identity);
            }
            Mesh mesh = _scene.Children[1].Children[2].GetComponent<Mesh>();
            Weight wm = _scene.Children[1].Children[2].GetComponent<Weight>();
            List<VertexWeightList> WeightMap = new List<VertexWeightList>();
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                WeightMap.Add(new VertexWeightList
                {
                    VertexWeights = new List<VertexWeight>
                    {
                        new VertexWeight
                        {
                            JointIndex = 0,
                            Weight = (mesh.Vertices[i].y > 0 ? 1: 0f)
                        },
                        new VertexWeight()
                        {
                            JointIndex = 1,
                            Weight = (mesh.Vertices[i].y <= 0 ? 1f: 0f)
                        }
                    }
                });
            }
            wm.WeightMap = WeightMap;
            SceneComponent weightMapFromScene = _scene.Children[1].Children[2].Components[1];

            _scene.Children.Insert(0, new SceneNode()
            {
                Name = "BoneContainer1",
                Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent()
                    {
                        Translation = new float3(0, 2, 0),
                        Scale = new float3(1, 1, 1)
                    },

                },
                Children = new ChildList
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent
                            {
                                Translation = new float3(0, -1f, 0),
                                Scale = new float3(1, 2, 1)
                            },
                            new BoneComponent(),
                            new Cube()
                        }
                    },

                    new SceneNode()
                    {
                        Name = "BoneContainer2",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent
                            {
                                Translation = new float3(0, -2, 0),
                                Scale = new float3(1, 2, 1)
                            },

                        },

                        Children = new ChildList
                        {
                            new SceneNode
                            {
                            Components = new List<SceneComponentContainer>()
                            {
                                new TransformComponent()
                                {
                                    Translation = new float3(0, -0.5f, 0),
                                    Scale = new float3(1,1,1)
                                },
                                new BoneComponent(),
                                new Cube()
                            }
                            }
                        }

                    }
                }

            });

            _scene.Children[1].Components.Insert(1, new WeightComponent
            {
                BindingMatrices = bindingMatrices,
                WeightMap = WeightMap
                // Joints are added automatically during scene conversion (ConvertSceneGraph)
            });

            */
            #endregion

            AABBCalculator aabbc = new AABBCalculator(_scene);
            AABBf? bbox = aabbc.GetBox();
            if (bbox != null)
            {
                // If the model origin is more than one third away from its bounding box,
                // recenter it to the bounding box. Do this check individually per dimension.
                // This way, small deviations will keep the model's original center, while big deviations
                // will make the model rotate around its geometric center.
                var bbCenter = bbox.Value.Center;
                var bbSize = bbox.Value.Size;
                var center = float3.Zero;
                if (System.Math.Abs(bbCenter.x) > bbSize.x * 0.3)
                    center.x = bbCenter.x;
                if (System.Math.Abs(bbCenter.y) > bbSize.y * 0.3)
                    center.y = bbCenter.y;
                if (System.Math.Abs(bbCenter.z) > bbSize.z * 0.3)
                    center.z = bbCenter.z;
                _sceneCenter = float4x4.CreateTranslation(-center);

                // Adjust the model size
                var maxScale = System.Math.Max(bbSize.x, System.Math.Max(bbSize.y, bbSize.z));
                if (maxScale != 0)
                    _sceneScale = float4x4.CreateScale(200.0f / maxScale);
                else
                    _sceneScale = float4x4.Identity;
            }

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            float curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);

            // Zoom & Roll
            if (Input.Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _angleRollInit = Input.Touch.TwoPointAngle - _angleRoll;
                    _offsetInit = Input.Touch.TwoPointMidPoint - _offset;
                    _maxPinchSpeed = 0;
                }
                _zoomVel = Input.Touch.TwoPointDistanceVel * -0.01f;
                _angleRoll = Input.Touch.TwoPointAngle - _angleRollInit;
                _offset = Input.Touch.TwoPointMidPoint - _offsetInit;
                var pinchSpeed = Input.Touch.TwoPointDistanceVel;
                if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed; // _maxPinchSpeed is used for debugging only.
            }
            else
            {
                _twoTouchRepeated = false;
                _zoomVel = Input.Mouse.WheelVel * -0.5f;
                _angleRoll *= curDamp * 0.8f;
                _offset *= curDamp * 0.8f;
            }

            // UpDown / LeftRight rotation
            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Input.Mouse.XVel * 0.000002f;
                _angleVelVert = -RotationSpeed * Input.Mouse.YVel * 0.000002f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
                _angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Input.Keyboard.LeftRightAxis * 0.002f;
                    _angleVelVert = -RotationSpeed * Input.Keyboard.UpDownAxis * 0.002f;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 80)
                _zoom = 80;
            if (_zoom > 2000)
                _zoom = 2000;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);

            // Create the camera matrix and set it as the current ModelView transformation
            float4x4 mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            float4x4 mtxCam = float4x4.LookAt(0, 0, -_zoom, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot * _sceneScale * _sceneCenter;
            float4x4 mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
            RC.Projection = mtxOffset * RC.Projection;

            // TODO: rewrite for new scene when BONES are exported from Blender again
            //Transform translation = _scene.Children[1].Children[1].GetComponent<Transform>();
            //translation.Rotation.x -= Input.Keyboard.ADAxis * 0.05f;
            //translation.Rotation.y += Input.Keyboard.WSAxis * 0.05f;

            //Diagnostics.Log(_scene.Children[0].GetComponent<TransformComponent>().Translation);

            // Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Animate();
            _sceneRenderer.Render(RC);

            RC.Projection = float4x4.CreateOrthographic(Width, Height, 0.1f, 1000);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "FUSEE Picking Example",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var canvas = new CanvasNode(
                "Canvas",
                CanvasRenderMode.Screen,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                })
            {
                Children = new ChildList()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}