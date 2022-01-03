using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Jometri;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Geometry = Fusee.Jometri.Geometry;
using Transform = Fusee.Engine.Core.Scene.Transform;

namespace Fusee.Examples.GeometryEditing.Core
{
    [FuseeApplication(Name = "FUSEE Geometry Editing Example", Description = "Example App to show basic geometry editing in FUSEE")]
    public class GeometryEditing : RenderCanvas
    {
        private readonly float4 _selectedColor = new float4(0.7f, 0.3f, 0, 1.0f).LinearColorFromSRgb();
        private readonly float4 _defaultColor = new float4(0.5f, 0.5f, 0.5f, 1.0f).LinearColorFromSRgb();

        // angle and camera variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert, _zoomVel, _zoom = 8, _xPos, _yPos = 2;

        private static float2 _offset;
        private static float2 _offsetInit;
        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private float _keyTimeout = 1;

        private bool _twoTouchRepeated;

        private SceneNode _parentNode;
        private SceneContainer _scene;
        private SceneRendererForward _renderer;
        private SceneRendererForward _uiRenderer;

        private Dictionary<int, Geometry> _activeGeometrys;

        private readonly Random rng = new();

        //picking
        private float2 _pickPos;

        private ScenePicker _scenePicker;
        private PickResult _currentPick;

        private SceneNode _selectedNode;
        private bool _isTranslating;
        private bool _isScaling;

        private readonly Camera _mainCam = new(ProjectionMethod.Perspective, 0.1f, 1000, M.PiOver4)
        {
            BackgroundColor = new float4(.4f, .6f, .7f, 1)
        };
        private Transform _camTransform;

        // Init is called on startup.
        public override void Init()
        {
            var gui = CreateUi();//FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE Camera Example");
            _uiRenderer = new SceneRendererForward(gui);

            ////////////////// Fill SceneNodeContainer ////////////////////////////////
            var checkerboardTex = new Texture(AssetStorage.Get<ImageData>("checkerboard.jpg"), true, TextureFilterMode.LinearMipmapLinear);
            _parentNode = new SceneNode
            {
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Rotation = new float3(0,0,0),
                        Scale = float3.One,
                        Translation = new float3(0, 0, 0)
                    }
                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = $"Plane",
                        Components = new List<SceneComponent>{
                            new Transform()
                            {
                                Rotation = new float3(M.DegreesToRadians(90), 0, 0),
                                Translation = new float3(0, 0, 0),
                                Scale = new float3(50, 50,0.1f)
                            },
                            MakeEffect.FromDiffuse(float4.One, 0, float3.Zero, checkerboardTex, 1f, new float2(2,2)),
                            new Plane()
                        }
                    }
                }
            };

            _camTransform = new Transform()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(_xPos, _yPos, -_zoom)
            };
            var camNode = new SceneNode
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _mainCam
                }
            };
            
            _parentNode.Children.Add(camNode);

            _scene = new SceneContainer { Children = new List<SceneNode> { _parentNode } };

            _renderer = new SceneRendererForward(_scene);
            _scenePicker = new ScenePicker(_scene);

            //////////////////////////////////////////////////////////////////////////

            _activeGeometrys = new Dictionary<int, Geometry>();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            _renderer.Render(RC);
            HandleCameraAndPicking();
            InteractionHandler();
            _uiRenderer.Render(RC);
            Present();
        }

        private void SelectGeometry(SceneNode selectedNode)
        {
            if (selectedNode != _selectedNode && selectedNode != null)
            {
                if (_selectedNode != null)
                {
                    _selectedNode.GetComponent<SurfaceEffect>().SurfaceInput.Albedo = _defaultColor;
                }
                _selectedNode = selectedNode;
                _selectedNode.GetComponent<SurfaceEffect>().SurfaceInput.Albedo = _selectedColor;
            }
        }

        private void InteractionHandler()
        {
            //Add new Geometry
            if (Keyboard.GetKey(KeyCodes.D1) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                Geometry geometry = CreatePrimitiveGeometry.CreateCuboidGeometry(1, 1, 1);
                AddGeometryToSceneNode(geometry, new float3(0, 0.5f, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D2) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                Geometry geometry = CreatePrimitiveGeometry.CreatePyramidGeometry(1, 1, 1);
                AddGeometryToSceneNode(geometry, new float3(0, 0.5f, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D3) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                Geometry geometry = CreatePrimitiveGeometry.CreateConeGeometry(1, 1, 15);
                AddGeometryToSceneNode(geometry, new float3(0, 0.5f, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D4) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                Geometry geometry = CreatePrimitiveGeometry.CreateSphereGeometry(1, 30, 15);
                AddGeometryToSceneNode(geometry, new float3(0, 0.5f, 0));
            }
            _keyTimeout -= DeltaTime;

            //following actions are only allowed if something is selected
            if (_selectedNode == null)
            {
                return;
            }

            //Translate Geometry
            if (Keyboard.GetKey(KeyCodes.G))
            {
                _isTranslating = true;
            }
            if (_isTranslating)
            {
                float3 worldPos = new(Mouse.Velocity.x * .0001f, Mouse.Velocity.y * -.0001f, Mouse.WheelVel * .001f);
                _selectedNode.GetTransform().Translation += worldPos.xyz;

                if (Mouse.LeftButton)
                {
                    _isTranslating = false;
                }
            }

            //Scaling Geometry
            if (Keyboard.GetKey(KeyCodes.S))
            {
                _isScaling = true;
            }
            if (_isScaling)
            {
                _selectedNode.GetTransform().Scale += new float3(Mouse.Velocity.y, Mouse.Velocity.y, Mouse.Velocity.y) * .0001f;
                if (Mouse.LeftButton)
                {
                    _isScaling = false;
                }
            }

            //DeleteGeom
            if (Keyboard.GetKey(KeyCodes.Delete) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                int currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                _activeGeometrys.Remove(currentGeometryIndex);

                Dictionary<int, Geometry> zwerg = new();
                foreach (int key in _activeGeometrys.Keys)
                {
                    if (key > currentGeometryIndex)
                    {
                        Geometry test = _activeGeometrys[key];
                        zwerg.Add(key - 1, test);
                    }
                    else { zwerg.Add(key, _activeGeometrys[key]); }
                }

                _activeGeometrys.Clear();
                foreach (KeyValuePair<int, Geometry> item in zwerg)
                {
                    _activeGeometrys.Add(item.Key, item.Value);
                }

                _parentNode.Children.RemoveAt(currentGeometryIndex);
                _selectedNode = null;
                _currentPick = null;
            }

            //Insert
            if (Keyboard.GetKey(KeyCodes.I) && _keyTimeout < 0)
            {
                _keyTimeout = .25f;
                int currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                SceneNode currentSelection = _parentNode.Children[currentGeometryIndex];
                Geometry currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry.InsetFace(rng.Next(4, currentSelectedGeometry.GetAllFaces().Count()), .5f);
                Geometry copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                JometriMesh geometryMesh = new(currentSelectedGeometry);
                Mesh meshComponent = new()
                {
                    Vertices = geometryMesh.Vertices,
                    Triangles = geometryMesh.Triangles,
                    Normals = geometryMesh.Normals,
                };
                currentSelection.Components[2] = meshComponent;
            }

            //Extrude
            if (Keyboard.GetKey(KeyCodes.E) && _keyTimeout < 0)
            {
                _keyTimeout = .25f;
                int currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                SceneNode currentSelection = _parentNode.Children[currentGeometryIndex];
                Geometry currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry.ExtrudeFace(rng.Next(4, currentSelectedGeometry.GetAllFaces().Count()), 1);
                Geometry copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                JometriMesh geometryMesh = new(currentSelectedGeometry);
                Mesh meshComponent = new()
                {
                    Vertices = geometryMesh.Vertices,
                    Triangles = geometryMesh.Triangles,
                    Normals = geometryMesh.Normals,
                };
                currentSelection.Components[2] = meshComponent;
            }

            //Add Catmull-Clark
            if (Keyboard.GetKey(KeyCodes.C) && _keyTimeout < 0)
            {
                _keyTimeout = .25f;
                int currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                SceneNode currentSelection = _parentNode.Children[currentGeometryIndex];
                Geometry currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry = SubdivisionSurface.CatmullClarkSubdivision(currentSelectedGeometry);
                Geometry copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                JometriMesh geometryMesh = new(currentSelectedGeometry);
                Mesh meshComponent = new()
                {
                    Vertices = geometryMesh.Vertices,
                    Triangles = geometryMesh.Triangles,
                    Normals = geometryMesh.Normals,
                };
                currentSelection.Components[2] = meshComponent;
            }
        }

        private void HandleCameraAndPicking()
        {
            float curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

            //Camera Rotation
            if (Mouse.MiddleButton && !Keyboard.GetKey(KeyCodes.LShift))
            {
                _angleVelHorz = RotationSpeed * Mouse.XVel * 0.00002f;
                _angleVelVert = RotationSpeed * Mouse.YVel * 0.00002f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * 0.00002f;
                _angleVelVert = RotationSpeed * touchVel.y * 0.00002f;
            }

            // Zoom & Roll
            if (Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.001f;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
            }
            else if (!_isTranslating)
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.005f;
                _offset *= curDamp * 0.8f;
            }
            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 2)
            {
                _zoom = 2;
            }

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            //Camera Translation
            if (Keyboard.GetKey(KeyCodes.LShift) && Mouse.MiddleButton)
            {
                _xPos += -RotationSpeed * Mouse.XVel * 0.00002f;
                _yPos += RotationSpeed * Mouse.YVel * 0.00002f;
            }

            _camTransform.Translation = new float3(_xPos, _yPos, -_zoom);
            _camTransform.RotationMatrix = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);

            //Picking
            if (Mouse.RightButton)
            {
                _pickPos = Mouse.Position;
                Diagnostics.Debug(_pickPos);
                float2 pickPosClip = _pickPos * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                PickResult newPick = _scenePicker.Pick(RC, pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (newPick != null)
                    {
                        SelectGeometry(newPick.Node);
                    }
                    _currentPick = newPick;
                }
            }
        }

        private void AddGeometryToSceneNode(Geometry geometry, float3 position)
        {
            Geometry newGeo = geometry.CloneGeometry();
            newGeo.Triangulate();
            JometriMesh geometryMesh = new(newGeo);

            SceneNode sceneNodeContainer = new() { Components = new List<SceneComponent>() };

            Mesh meshComponent = new()
            {
                Vertices = geometryMesh.Vertices,
                Triangles = geometryMesh.Triangles,
                Normals = geometryMesh.Normals,
            };
            Transform translationComponent = new()
            {
                Rotation = float3.Zero,
                Scale = new float3(1, 1, 1),
                Translation = position
            };

            sceneNodeContainer.Components.Add(translationComponent);
            sceneNodeContainer.Components.Add(MakeEffect.FromDiffuseSpecular(_defaultColor));
            sceneNodeContainer.Components.Add(meshComponent);

            _parentNode.Children.Add(sceneNodeContainer);
            _activeGeometrys.Add(_parentNode.Children.IndexOf(sceneNodeContainer), geometry);
        }

        private SceneContainer CreateUi()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 14);

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

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
                    TextNode.Create(
                    "\n" +
                    "Camera\n" +
                    "\n" +
                    "Click mouse middle: Look around\n" +
                    "Shift + mouse middle: Move left & right\n" +
                    "Mouse wheel: Move forward & backward\n" +
                    "\n" +
                    "Geometry\n" +
                    "\n" +
                    "Key 1: Create Cuboid\n" +
                    "Key 2: Create Pyramid\n" +
                    "Key 3: Create Cone\n" +
                    "Key 4: Create Sphere\n" +
                    "Select: Right Click\n" +
                    "\n" +
                    "Only affecting selected geometry:\n" +
                    "\n" +
                    "G: Translate (click to confirm)\n" +
                    "S: Scale (click to confirm)\n" +
                    "Del: Delete\n" +
                    "I: Insert random face\n" +
                    "E: Extrude random face\n" +
                    "C: Subdivide geometry",
                    "AppTitle",
                    GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                    GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0.25f, canvasHeight - 16.25f), canvasHeight, canvasWidth, new float2(16, 16)),
                    guiLatoBlack,
                    (float4)ColorUint.White,
                    HorizontalTextAlignment.Left,
                    VerticalTextAlignment.Top)
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, 0),
                                Rotation = float3.Zero,
                                Scale = float3.One
                            },
                            new Camera(ProjectionMethod.Orthographic, 0.01f, 500, M.PiOver4)
                            {

                                ClearColor = false
                            }
                        }
                    },
                    canvas
                }
            };
        }

    }
}