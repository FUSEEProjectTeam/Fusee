﻿using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri;
using Fusee.Math.Core;
using Fusee.Serialization;
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
        public override async Task<bool> Init()
        {
            var outlineOne = new PolyBoundary //CCW!!
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

            var outlineOneHole = new PolyBoundary //CW!!
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

            var outlineTwo = new PolyBoundary //CCW!!
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

            var outlineThree = new PolyBoundary //CCW!!
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(1, 0, 1),
                    new float3(0, 0.5f, 0.5f)
                },
                IsOuter = true
            };

            var geomOutlinesOne = new List<PolyBoundary> { outlineOne, outlineOneHole };
            var geomOne = new Geometry(geomOutlinesOne);
            geomOne.Extrude2DPolygon(0.5f, true);
            geomOne.Triangulate();
            var meshOne = new JometriMesh(geomOne);

            var geomCubeOutlines = new List<PolyBoundary> { outlineTwo };
            var geomCube = new Geometry(geomCubeOutlines);
            geomCube.Extrude2DPolygon(1, false);
            //geomCube.Extrude2DPolygon(1, true);
            geomCube.Triangulate();
            var cube = new JometriMesh(geomCube);

            var geomTriangleOutlines = new List<PolyBoundary> { outlineThree };
            var geomTri = new Geometry(geomTriangleOutlines);
            geomTri.Triangulate();
            var triangle = new JometriMesh(geomTri);

            ////////////////// Fill SceneNodeContainer ////////////////////////////////
            var parentNode = new SceneNodeContainer
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

            parentNode.Components.Add(parentTrans);

            var sceneNodeCOne = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCOne = new Mesh
            {
                Vertices = meshOne.Vertices,
                Triangles = meshOne.Triangles,
                Normals = meshOne.Normals,
            };

            var tranC = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            sceneNodeCOne.Components.Add(tranC);
            sceneNodeCOne.Components.Add(meshCOne);
            ///////////////////////////////////////////////////////////
            var sceneNodeCCube = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCCube = new Mesh
            {
                Vertices = cube.Vertices,
                Triangles = cube.Triangles,
                Normals = cube.Normals,
            };
            var tranCube = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(-2, -1, 0)
            };

            sceneNodeCCube.Components.Add(tranCube);
            sceneNodeCCube.Components.Add(meshCCube);
            //////////////////////////////////////////////////////////////////
            var sceneNodeCTri = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCTri = new Mesh
            {
                Vertices = triangle.Vertices,
                Triangles = triangle.Triangles,
                Normals = triangle.Normals,
            };
            var tranTri = new TransformComponent
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
            var sc = new SceneContainer { Children = new List<SceneNodeContainer> { parentNode } };

            var projComp = new ProjectionComponent(ProjectionMethod.PERSPECTIVE, 1, 5000, M.PiOver4);
            sc.Children[0].Components.Insert(0, projComp);

            _renderer = new SceneRendererForward(sc);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 1, 1, 1);

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the back buffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            var speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.0001f;
                _beta -= speed.y * 0.0001f;
            }

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_beta) * float4x4.CreateRotationY(_alpha);
            var mtxCam = float4x4.LookAt(0, 0, -3, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

            _renderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
        }
    }
}