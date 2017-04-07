using System.Collections.Generic;
using System.Xml;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri.DCEL;
using Fusee.Jometri.Manipulation;
using Fusee.Jometri.Triangulation;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
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
            ///////////// Create PolyBoundaries ////////////////////////
            var outlineOctagon = new PolyBoundary //CCW
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

            var outlineOctaHole = new PolyBoundary //CW
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

            var outlineRectangle = new PolyBoundary //CCW
            {
                Points = new List<float3>
                {
                    new float3(-1, 1, -1),
                    new float3(-1, -1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, -1)
                },
                IsOuter = true
            };

            var outlineTriangle = new PolyBoundary //CCW
            {
                Points = new List<float3>
                {
                    new float3(0, 0, 0),
                    new float3(1, 0, 0),
                    new float3(0, 1, 0)
                },
                IsOuter = true
            };

            var outlineCustomStar = new PolyBoundary()
            {
                Points = new List<float3>
                {
                    new float3(0, 2, 0),
                    new float3(-2, 1, 0),
                    new float3(-2, -1, 0),
                    new float3(0, -2, 0),
                    new float3(2, -1, 0),
                    new float3(2, 1, 0),
                },
                IsOuter = true
            };
            ////////////////////////////////////////////////

            //var outlineCCube = new List<PolyBoundary> { outlineCustomStar };
            //var geomCCube = new Geometry(outlineCCube);
            //geomCCube.Extrude2DPolygon(2, true);
            //geomCCube = SubDivisionSurface.CatmullClarkSubDivision(geomCCube);
            //geomCCube = SubDivisionSurface.CatmullClarkSubDivision(geomCCube);
            //geomCCube = SubDivisionSurface.CatmullClarkSubDivision(geomCCube);
            //geomCCube.Triangulate();
            //var customCube = new JometriMesh(geomCCube);

            //var outlinesOcta = new List<PolyBoundary> { outlineOctagon, outlineOctaHole };
            //var geomOcta = new Geometry(outlinesOcta);
            //geomOcta.Extrude2DPolygon(0.5f, true);
            //geomOcta.Triangulate();
            //var octagon = new JometriMesh(geomOcta);

            var outlinesCube = new List<PolyBoundary> { outlineRectangle };
            var geomCube = new Geometry(outlinesCube);
            geomCube.Extrude2DPolygon(2, false);
            geomCube.ExtrudeFace(2, 1);
            geomCube.ExtrudeFace(4, 1);
            geomCube.ExtrudeFace(5, 1);
            geomCube.ExtrudeFace(6, 1);
            geomCube.ExtrudeFace(7, 1);
            geomCube.ExtrudeFace(8, 1);

            geomCube = SubDivisionSurface.CatmullClarkSubDivision(geomCube);
            geomCube = SubDivisionSurface.CatmullClarkSubDivision(geomCube);
            geomCube = SubDivisionSurface.CatmullClarkSubDivision(geomCube);
            //geomCube = SubDivisionSurface.CatmullClarkSubDivision(geomCube);



            geomCube.Triangulate();
            var cube = new JometriMesh(geomCube);

            var outlinesTriangle = new List<PolyBoundary> { outlineTriangle };
            var geomTri = new Geometry(outlinesTriangle);
            geomTri.Triangulate();
            var triangle = new JometriMesh(geomTri);
            //var triangleSD = SubDivisionSurface.ReifSubDivision(geomTri);

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

            ////Custom
            //var sceneNodeCustomStar = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };
            //var meshCustomStar = new MeshComponent
            //{
            //    Vertices = customCube.Vertices,
            //    Triangles = customCube.Triangles,
            //    Normals = customCube.Normals,
            //};

            //var transCustomStar = new TransformComponent
            //{
            //    Rotation = float3.Zero,
            //    Scale = new float3(0.5f, 0.5f, 0.5f),
            //    Translation = new float3(0, 0, 0),
            //};

            //sceneNodeCustomStar.AddComponent(transCustomStar);
            //sceneNodeCustomStar.AddComponent(meshCustomStar);

            ////Octagon
            //var sceneNodeCOne = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };
            //var meshCOne = new MeshComponent
            //{
            //    Vertices = octagon.Vertices,
            //    Triangles = octagon.Triangles,
            //    Normals = octagon.Normals,
            //};

            //var tranC = new TransformComponent
            //{
            //    Rotation = float3.Zero,
            //    Scale = float3.One,
            //    Translation = new float3(0, 0, 0)
            //};

            //sceneNodeCOne.Components.Add(tranC);
            //sceneNodeCOne.Components.Add(meshCOne);

            //Cube
            var sceneNodeCCube = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCCube = new MeshComponent
            {
                Vertices = cube.Vertices,
                Triangles = cube.Triangles,
                Normals = cube.Normals,
            };
            var tranCube = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = new float3(0.5f,0.5f,0.5f),
                Translation = new float3(0, 0, 0)
            };

            sceneNodeCCube.Components.Add(tranCube);
            sceneNodeCCube.Components.Add(meshCCube);

            //Triangle
            var sceneNodeCTri = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCTri = new MeshComponent
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

            //parentNode.Children.Add(sceneNodeCTri);
            //parentNode.Children.Add(sceneNodeCOne);
            parentNode.Children.Add(sceneNodeCCube);
            //parentNode.Children.Add(sceneNodeCustomStar);

            var sc = new SceneContainer { Children = new List<SceneNodeContainer> { parentNode } };
            ///////////////////////////////////////////////////////////////////////////

            _renderer = new SceneRenderer(sc);

            RC.ClearColor = new float4(0, 0.61f, 0.88f, 1);

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

            Present();
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