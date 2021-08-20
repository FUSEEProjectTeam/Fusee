using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Jometri;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                Components = new List<SceneComponent>(),
                Children = new ChildList()
            };

            Transform parentTrans = new()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            parentNode.Components.Add(parentTrans);

            SceneNode sceneNodeCOne = new() { Components = new List<SceneComponent>() };

            Mesh meshCOne = new()
            {
                Vertices = meshOne.Vertices,
                Triangles = meshOne.Triangles,
                Normals = meshOne.Normals,
            };

            Transform tranC = new()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            sceneNodeCOne.Components.Add(tranC);
            sceneNodeCOne.Components.Add(meshCOne);
            ///////////////////////////////////////////////////////////
            SceneNode sceneNodeCCube = new() { Components = new List<SceneComponent>() };

            Mesh meshCCube = new()
            {
                Vertices = cube.Vertices,
                Triangles = cube.Triangles,
                Normals = cube.Normals,
            };
            Transform tranCube = new()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(-2, -1, 0)
            };

            sceneNodeCCube.Components.Add(tranCube);
            sceneNodeCCube.Components.Add(meshCCube);
            //////////////////////////////////////////////////////////////////
            SceneNode sceneNodeCTri = new() { Components = new List<SceneComponent>() };

            Mesh meshCTri = new()
            {
                Vertices = triangle.Vertices,
                Triangles = triangle.Triangles,
                Normals = triangle.Normals,
            };
            Transform tranTri = new()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(1.5f, -1, 0)
            };

            sceneNodeCTri.Components.Add(tranTri);
            sceneNodeCTri.Components.Add(meshCTri);
            //////////////////////////////////////////////////////////////////

            parentNode.Children.Add(sceneNodeCTri);
            parentNode.Children.Add(sceneNodeCOne);
            parentNode.Children.Add(sceneNodeCCube);
            var sc = new SceneContainer { Children = new List<SceneNode> { parentNode } };

            _renderer = new SceneRendererForward(sc);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 1, 1, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the back buffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            float2 speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.0001f;
                _beta -= speed.y * 0.0001f;
            }

            // Create the camera matrix and set it as the current ModelView transformation
            float4x4 mtxRot = float4x4.CreateRotationX(_beta) * float4x4.CreateRotationY(_alpha);
            float4x4 mtxCam = float4x4.LookAt(0, 0, -3, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

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