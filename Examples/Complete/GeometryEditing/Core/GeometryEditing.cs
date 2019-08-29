using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Geometry = Fusee.Jometri.Geometry;


namespace Fusee.Examples.GeometryEditing.Core
{

    [FuseeApplication(Name = "FUSEE Geometry Editing Example", Description = "Example App to show basic geometry editing in FUSEE")]
    public class GeometryEditing : RenderCanvas
    {
        private readonly float4 _selectedColor = new float4(0.7f, 0.3f, 0,1.0f);
        private readonly float4 _defaultColor = new float4(0.5f, 0.5f, 0.5f,1.0f);

        // angle and camera variables 
        private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom = 8, _xPos, _yPos;
        private static float2 _offset;
        private static float2 _offsetInit;
        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private readonly float4x4 _sceneScale = float4x4.CreateScale(1);
        private float4x4 _projection;
        private float _keyTimeout = 1;

        private bool _twoTouchRepeated;

        private SceneNodeContainer _parentNode;
        private SceneContainer _scene;
        private SceneRenderer _renderer;

        private Dictionary<int, Geometry> _activeGeometrys;

        Random rng = new Random();

        //picking
        private float2 _pickPos;
        private ScenePicker _scenePicker;
        private PickResult _currentPick;

        private SceneNodeContainer _selectedNode;
        private bool _isTranslating;
        private bool _isScaling;

        // Init is called on startup. 
        public override void Init()
        {

            ////////////////// Fill SceneNodeContainer ////////////////////////////////
            _parentNode = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>(),
                Children = new ChildList()
            };

            var parentTrans = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };
            _parentNode.Components.Add(parentTrans);


            _scene = new SceneContainer { Children = new List<SceneNodeContainer> { _parentNode } };

            var projComp = new ProjectionComponent(ProjectionMethod.PERSPECTIVE, 1, 5000, M.PiOver4);
            AddResizeDelegate(delegate
            {
                projComp.Resize(Width, Height);
                RC.Viewport(0, 0, Width, Height);
            });
            _scene.Children[0].Components.Insert(0, projComp);

            _renderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);

            //////////////////////////////////////////////////////////////////////////

            RC.ClearColor = new float4(.7f, .7f, .7f, 1);

            _activeGeometrys = new Dictionary<int, Geometry>();

            //Create Geometry
            //Geometry sphere = CreateGeometry.CreateSpehreGeometry(2,22,11);
            //sphere = SubdivisionSurface.CatmullClarkSubdivision(sphere);
            //AddGeometryToSceneNode(sphere, new float3(0,0,0));

            //Geometry cuboid = CreateGeometry.CreateCuboidGeometry(5, 2, 5);
            //AddGeometryToSceneNode(cuboid, new float3(-5,0,0));         
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            HandleCameraAndPicking();
            InteractionHandler();
            _renderer.Render(RC);
            RC.ClearColor = new float4(.7f, .7f, .7f, 1);

            Present();
        }

        private void SelectGeometry(SceneNodeContainer selectedNode)
        {
            if (selectedNode != _selectedNode && selectedNode != null)
            {
                if (_selectedNode != null)
                {
                    _selectedNode.GetMaterial().Diffuse.Color = _defaultColor;
                }
                _selectedNode = selectedNode;
                _selectedNode.GetMaterial().Diffuse.Color = _selectedColor;
            }
        }

        private void InteractionHandler()
        {
            //Add new Geometry
            if (Keyboard.GetKey(KeyCodes.D1) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                var geometry = CreatePrimitiveGeometry.CreateCuboidGeometry(1, 1, 1);
                AddGeometryToSceneNode(geometry, new float3(0, 0, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D2) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                var geometry = CreatePrimitiveGeometry.CreatePyramidGeometry(1, 1, 1);
                AddGeometryToSceneNode(geometry, new float3(0, 0, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D3) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                var geometry = CreatePrimitiveGeometry.CreateConeGeometry(1, 1, 15);
                AddGeometryToSceneNode(geometry, new float3(0, 0, 0));
            }
            if (Keyboard.GetKey(KeyCodes.D4) && _keyTimeout < 0)
            {
                _keyTimeout = 1;
                var geometry = CreatePrimitiveGeometry.CreateSpehreGeometry(1, 30, 15);
                AddGeometryToSceneNode(geometry, new float3(0, 0, 0));
            }
            _keyTimeout -= DeltaTime;

            //following actions are only allowed if something is selected
            if (_selectedNode == null) return;

            //Translate Geometry
            if (Keyboard.GetKey(KeyCodes.G))
            {
                _isTranslating = true;
            }
            if (_isTranslating)
            {
                var worldPos = new float3(Mouse.Velocity.x * .0001f, Mouse.Velocity.y * -.0001f, Mouse.WheelVel * .001f);
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

                var zwerg = new Dictionary<int, Geometry>();
                foreach (var key in _activeGeometrys.Keys)
                {
                    if(key > currentGeometryIndex)
                    {
                        var test = _activeGeometrys[key];                        
                        zwerg.Add(key-1, test);
                    }
                    else { zwerg.Add(key, _activeGeometrys[key]); }
                }

                _activeGeometrys.Clear();
                foreach (var item in zwerg)
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
                var currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                var currentSelection = _parentNode.Children[currentGeometryIndex];
                var currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry.InsetFace(rng.Next(4, currentSelectedGeometry.GetAllFaces().Count()), .5f);
                var copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                var geometryMesh = new JometriMesh(currentSelectedGeometry);
                var meshComponent = new Mesh
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
                var currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                var currentSelection = _parentNode.Children[currentGeometryIndex];
                var currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry.ExtrudeFace(rng.Next(4, currentSelectedGeometry.GetAllFaces().Count()), 1);
                var copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                var geometryMesh = new JometriMesh(currentSelectedGeometry);
                var meshComponent = new Mesh
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
                var currentGeometryIndex = _parentNode.Children.IndexOf(_selectedNode);
                var currentSelection = _parentNode.Children[currentGeometryIndex];
                var currentSelectedGeometry = _activeGeometrys[currentGeometryIndex];

                currentSelectedGeometry = SubdivisionSurface.CatmullClarkSubdivision(currentSelectedGeometry);
                var copy = currentSelectedGeometry.CloneGeometry();
                _activeGeometrys[currentGeometryIndex] = copy;
                currentSelectedGeometry.Triangulate();

                var geometryMesh = new JometriMesh(currentSelectedGeometry);
                var meshComponent = new Mesh
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
            var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

            //Camera Rotation
            if (Mouse.MiddleButton && !Keyboard.GetKey(KeyCodes.LShift))
            {
                _angleVelHorz = -RotationSpeed * Mouse.XVel * 0.00002f;
                _angleVelVert = RotationSpeed * Mouse.YVel * 0.00002f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * 0.00002f;
                _angleVelVert = RotationSpeed * touchVel.y * 0.00002f;
            }

            // Zoom & Roll
            if (Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _angleRollInit = Touch.TwoPointAngle - _angleRoll;
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.001f;
                _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
            }
            else if (!_isTranslating)
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.005f;
                _angleRoll *= curDamp * 0.8f;
                _offset *= curDamp * 0.8f;
            }
            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 2)
                _zoom = 2;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);

            //Camera Translation
            if (Keyboard.GetKey(KeyCodes.LShift) && Mouse.MiddleButton)
            {
                _xPos += -RotationSpeed * Mouse.XVel * 0.00002f;
                _yPos += RotationSpeed * Mouse.YVel * 0.00002f;
            }

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(_xPos, _yPos, -_zoom, _xPos, _yPos, 0, 0, 1, 0);

            var viewMatrix = mtxCam * mtxRot * _sceneScale;

            //Picking
            if (Mouse.RightButton)
            {
                _pickPos = Mouse.Position;
                Diagnostics.Log(_pickPos);
                var pickPosClip = _pickPos * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                _scenePicker.View = viewMatrix;
                _scenePicker.Projection = _projection;
                var newPick = _scenePicker.Pick(pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (newPick != null)
                    {
                        SelectGeometry(newPick.Node);
                    }
                    _currentPick = newPick;
                }
            }

            RC.View = viewMatrix;
            //var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
            RC.Projection = /*mtxOffset **/ _projection;
        }

        private void AddGeometryToSceneNode(Geometry geometry, float3 position)
        {
            Geometry newGeo = geometry.CloneGeometry();
            newGeo.Triangulate();
            var geometryMesh = new JometriMesh(newGeo);

            var sceneNodeContainer = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshComponent = new Mesh
            {
                Vertices = geometryMesh.Vertices,
                Triangles = geometryMesh.Triangles,
                Normals = geometryMesh.Normals,
            };
            var translationComponent = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = new float3(1, 1, 1),
                Translation = position
            };
            var materialComponent = new MaterialComponent
            {
                Diffuse = new MatChannelContainer(),
                Specular = new SpecularChannelContainer(),
            };
            materialComponent.Diffuse.Color = _defaultColor;
            sceneNodeContainer.Components.Add(translationComponent);
            sceneNodeContainer.Components.Add(materialComponent);
            sceneNodeContainer.Components.Add(meshComponent);
            
            _parentNode.Children.Add(sceneNodeContainer);
            _activeGeometrys.Add(_parentNode.Children.IndexOf(sceneNodeContainer), geometry);
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
           
        }

    }
}