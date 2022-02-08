using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Jometri;
using Fusee.Math.Core;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using Geometry = Fusee.Jometri.Geometry;

namespace Fusee.Examples.MeshingAround.Core
{
    [FuseeApplication(Name = "FUSEE Meshing Example", Description = "Meshing around...")]
    public class MeshingAround : RenderCanvas
    {
        private float _alpha;
        private float _beta;

        private SceneRendererForward _renderer;
        private Transform _camPivotTransform;

        // Init is called on startup.
        public override void Init()
        {
            PolyBoundary outlineOne = new()
            {
                Points = new List<float3>
                {
                    new float3(1, 0, 0),
                    new float3(1.25f, 0.5f, 0.5f),
                    new float3(1, 1, 1),
                    new float3(0, 1, 1),
                    new float3(-0.25f, 0.5f, 0.5f),
                    new float3(0, 0, 0)
                },
                IsOuter = true
            };

            PolyBoundary outlineOneHole = new()
            {
                Points = new List<float3>
                {
                    new float3(0.75f, 0.25f, 0.25f),
                    new float3(0.25f, 0.25f, 0.25f),
                    new float3(0.25f, 0.75f, 0.75f),
                    new float3(0.75f, 0.75f, 0.75f)
                },
                IsOuter = false
            };

            PolyBoundary outlineTwo = new()
            {
                Points = new List<float3>
                {
                    new float3(1, 0, 0),
                    new float3(1, 0.707f, 0.707f),
                    new float3(0, 0.707f, 0.707f),
                    new float3(0, 0, 0)
                },
                IsOuter = true
            };

            PolyBoundary outlineThree = new()
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(1, 0, 1),
                    new float3(0, 0.5f, 0.5f)
                },
                IsOuter = true
            };

            List<PolyBoundary> geomOutlinesOne = new() { outlineOne, outlineOneHole };
            Geometry geomOne = new(geomOutlinesOne);
            geomOne.Extrude2DPolygon(0.5f, true);
            geomOne.Triangulate();
            JometriMesh meshOne = new(geomOne);

            List<PolyBoundary> geomCubeOutlines = new() { outlineTwo };
            Geometry geomCube = new(geomCubeOutlines);
            geomCube.Extrude2DPolygon(1, false);
            //geomCube.Extrude2DPolygon(1, true);
            geomCube.Triangulate();
            JometriMesh cube = new(geomCube);

            List<PolyBoundary> geomTriangleOutlines = new() { outlineThree };
            Geometry geomTri = new(geomTriangleOutlines);
            geomTri.Triangulate();
            JometriMesh triangle = new(geomTri);

            ////////////////// Fill SceneNode ////////////////////////////////
            SceneNode parentNode = new()
            {
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(0, 0, 0)
                    }
                },
                Children = new ChildList()
            };

            SceneNode sceneNodeOne = new()
            {
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(0, 0, 0)
                    },
                    new Mesh()
                    {
                        Vertices = meshOne.Vertices,
                        Triangles = meshOne.Triangles,
                        Normals = meshOne.Normals,
                    }
                }
            };
            ///////////////////////////////////////////////////////////
            SceneNode sceneNodeCube = new() 
            { 
                Components = new List<SceneComponent>() 
                {
                    new Transform()
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(-2, -1, 0)
                    },
                     new Mesh()
                    {
                        Vertices = cube.Vertices,
                        Triangles = cube.Triangles,
                        Normals = cube.Normals,
                    }
                    
                }
            };
            //////////////////////////////////////////////////////////////////
            SceneNode sceneNodeCTri = new() 
            { 
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(1.5f, -1, 0)
                    },
                    new Mesh()
                    {
                        Vertices = triangle.Vertices,
                        Triangles = triangle.Triangles,
                        Normals = triangle.Normals,
                    }
                }
            };
            //////////////////////////////////////////////////////////////////

            parentNode.Children.Add(sceneNodeCTri);
            parentNode.Children.Add(sceneNodeOne);
            parentNode.Children.Add(sceneNodeCube);

            _camPivotTransform = new Transform();

            var sc = new SceneContainer 
            { 
                Children = new List<SceneNode> 
                {
                    new SceneNode()
                    {
                        Components =
                        {
                            _camPivotTransform,
                        },
                        Children =
                        {
                            new SceneNode()
                            {
                                Name = "MainCam",
                                Components = new List<SceneComponent>
                                {
                                    new Transform()
                                    {
                                        Translation = new float3(0, 0, -3)
                                    },
                                    new Camera(ProjectionMethod.Perspective, 0.1f, 100, M.PiOver4)
                                    {
                                        BackgroundColor = float4.One
                                    }
                                }
                            }
                        }
                    },
                    parentNode 
                } 
            };

            _renderer = new SceneRendererForward(sc);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            float2 speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.0001f;
                _beta -= speed.y * 0.0001f;
            }

            _camPivotTransform.RotationQuaternion = QuaternionF.FromEuler(_beta, _alpha, 0);

            _renderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
        }
    }
}