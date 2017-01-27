using System;
using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri.DCEL;
using Fusee.Jometri.Manipulation;
using Fusee.Jometri.Triangulation;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using Geometry = Fusee.Jometri.DCEL.Geometry;


namespace Fusee.Engine.Examples.MeshingAround.Core
{

    [FuseeApplication(Name = "Meshing Example", Description = "Meshing around...")]
    public class MeshingAround : RenderCanvas
    {
        private float _alpha;
        private float _beta;

        private SceneRenderer _renderer;

        // Init is called on startup. 
        public override void Init()
        {

            var outlineTest = new PolyBoundary //CCW!!
            {
                Points = new List<float3>
                {
                    new float3(1, 0, 0),
                    new float3(1, 1, 1),
                    new float3(0, 1, 1),
                    new float3(0, 0, 0)
                },
                IsOuter = true
            };

            var outlineTestHole = new PolyBoundary //CW!!
            {
                Points = new List<float3>
                {
                    new float3(0.25f, 0.25f, 0.25f),
                    new float3(0.25f, 0.75f, 0.75f),
                    new float3(0.75f, 0.75f, 0.75f),
                    new float3(0.75f, 0.25f, 0.25f)
                },
                IsOuter = false
            };


            ////////////////////// Mesh creation ///////////////////////////////
            var outlineOne = new PolyBoundary //CCW!!
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(0, 0.5f, 0),
                    new float3(-0.5f, 0.5f, 0),
                    new float3(-0.5f, 0, 0)
                },
                IsOuter = true
            };

            var outlineOneHole = new PolyBoundary //CW = hole!!
            {
                Points = new List<float3>
                {
                    new float3(-0.125f, 0.125f, 0),
                    new float3(-0.375f, 0.125f, 0),
                    new float3(-0.375f, 0.375f, 0),
                    new float3(-0.125f, 0.375f, 0)

                },
                IsOuter = true
            };

            var outlineTwo = new PolyBoundary
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(0.5f, 0, 0),
                    new float3(0.5f, 0.5f, 0),
                    new float3(0, 0.5f, 0)
                },
                IsOuter = true
            };

            var outlineThree = new PolyBoundary
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(-0.5f, 0, 0),
                    new float3(-0.5f, -0.5f, 0),
                    new float3(0, -0.5f, 0)
                },
                IsOuter = true
            };

            var geomOutlines = new List<PolyBoundary> { outlineOne,outlineOneHole};
            //var geomOutlines = new List<PolyBoundary> {outlineTest, outlineTestHole };
            var geom = new Geometry(geomOutlines); //2D
            geom.Extrude2DPolygon(1);
            geom.Triangulate();
            var mesh = new JometriMesh(geom);

            ////////////////// Fill SceneNodeContainer ////////////////////////////////
            var parentNode = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>(),
                Children = new List<SceneNodeContainer>()
            };

            var parentTrans = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            parentNode.Components.Add(parentTrans);

            var sceneNodeC = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };


            var meshC = new MeshComponent
            {
                Vertices = mesh.Vertices,
                Triangles = mesh.Triangles,
                Normals = mesh.Normals,
            };


            var tranC = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            sceneNodeC.Components.Add(tranC);
            sceneNodeC.Components.Add(meshC);

            parentNode.Children.Add(sceneNodeC);

            var sc = new SceneContainer { Children = new List<SceneNodeContainer> { parentNode } };

            _renderer = new SceneRenderer(sc);

            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 1, 1, 1);

        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
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
            RC.ModelView = mtxCam * mtxRot;

            _renderer.Render(RC);

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
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 2000000);
            RC.Projection = projection;

        }

    }
}