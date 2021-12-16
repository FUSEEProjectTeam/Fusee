using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This class contains information about the scene of the picked point.
    /// </summary>
    public class RayCastResult
    {
        /// <summary>
        /// The scene node containter of the result.
        /// </summary>
        public SceneNode Node;

        /// <summary>
        /// The picked mesh.
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// The index of the triangle in which the intersection of ray and mesh happened.
        /// </summary>
        public int Triangle;

        /// <summary>
        /// The barycentric u, v coordinates within the picked triangle.
        /// </summary>
        public float U, V;

        /// <summary>
        /// The (texture-) UV coordinates of the picked point.
        /// </summary>
        public float2 UV
        {
            get
            {
                float2 uva = Mesh.UVs[Mesh.Triangles[Triangle]];
                float2 uvb = Mesh.UVs[Mesh.Triangles[Triangle + 1]];
                float2 uvc = Mesh.UVs[Mesh.Triangles[Triangle + 2]];

                return float2.Barycentric(uva, uvb, uvc, U, V);
            }
        }

        /// <summary>
        /// The model matrix.
        /// </summary>
        public float4x4 Model;

        /// <summary>
        /// Gets the triangles of the picked mesh.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void GetTriangle(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Vertices[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Vertices[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Vertices[Mesh.Triangles[Triangle + 2]];
        }

        /// <summary>
        /// Returns the barycentric triangle coordinates.
        /// </summary>
        public float3 TriangleBarycentric
        {
            get
            {
                GetTriangle(out var a, out var b, out var c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }

        /// <summary>
        /// Returns the model position.
        /// </summary>
        public float3 ModelPos => TriangleBarycentric;

        /// <summary>
        /// Returns the world position of the intersection.
        /// </summary>
        public float3 WorldPos => float4x4.TransformPerspective(Model, ModelPos);

        /// <summary>
        /// Returns the distance between ray origin and the intersection.
        /// </summary>
        public float DistanceFromOrigin;
    }

    /// <summary>
    /// Implements the scene raycaster.
    /// </summary>
    public class SceneRayCaster : Viserator<RayCastResult, SceneRayCaster.RayCasterState, SceneNode, SceneComponent>
    {
        /// <summary>
        /// The <see cref="Rayf"/> to check intersections with.
        /// </summary>
        public Rayf Ray { get; private set; }

        /// <summary>
        /// The <see cref="Cull"/> mode to use by the SceneRayCaster
        /// </summary>
        public Cull CullMode { get; private set; }


        protected SceneContainer _sc;

        internal PrePassVisitor PrePassVisitor { get; private set; }

        #region State
        /// <summary>
        /// The raycaster state upon scene traversal.
        /// </summary>
        public class RayCasterState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _model = new();

            /// <summary>
            /// The registered model.
            /// </summary>
            public float4x4 Model
            {
                set => _model.Tos = value;
                get => _model.Tos;
            }

            /// <summary>
            /// The default constructor for the <see cref="RayCasterState"/> class, which registers state stacks for the model./>
            /// </summary>
            public RayCasterState()
            {
                RegisterState(_model);
            }
        }
        #endregion

        /// <summary>
        /// The constructor to initialize a new SceneRayCaster.
        /// </summary>
        /// <param name="scene">The <see cref="SceneContainer"/> to use.</param>
        /// <param name="cullMode">The <see cref="Cull"/> mode to use.</param>
        public SceneRayCaster(SceneContainer scene, Cull cullMode = Cull.None)
            : base(scene.Children)
        {
            CullMode = cullMode;

            PrePassVisitor = new PrePassVisitor();
            _sc = scene;
        }

        /// <summary>
        /// This method is called when traversal starts to initialize the traversal state.
        /// </summary>
        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
        }

        /// <summary>
        /// Returns a collection of objects that are hit by the ray and that can be iterated over.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns>A collection of <see cref="RayCastResult"/> that can be iterated over.</returns>
        public IEnumerable<RayCastResult> RayCast(Rayf ray)
        {
            Ray = ray;
            return Viserate();
        }

        public IEnumerable<RayCastResult> RayPick(RenderContext rc, float2 pickPos)
        {
            PrePassVisitor.PrePassTraverse(_sc, rc);
            var cams = PrePassVisitor.CameraPrepassResults;

            float2 pickPosClip = float2.Zero;

            if (cams.Count == 0)
            {
                pickPosClip = (pickPos * new float2(2.0f / rc.ViewportWidth, -2.0f / rc.ViewportHeight)) + new float2(-1, 1);
                Ray = new Rayf(pickPosClip, rc.View, rc.Projection);
                return Viserate();
            }

            Tuple<SceneNode, CameraResult> pickCam = null;
            Rectangle pickCamRect = new Rectangle();

            foreach (var cam in cams)
            {
                Rectangle camRect = new Rectangle();
                camRect.Left = (int)((cam.Item2.Camera.Viewport.x * rc.ViewportWidth) / 100);
                camRect.Top = (int)((cam.Item2.Camera.Viewport.y * rc.ViewportHeight) / 100);
                camRect.Right = (int)((cam.Item2.Camera.Viewport.z * rc.ViewportWidth) / 100) + camRect.Left;
                camRect.Bottom = (int)((cam.Item2.Camera.Viewport.w * rc.ViewportHeight) / 100) + camRect.Top;


                if (!float2.PointInRectangle(new float2(camRect.Left, camRect.Top), new float2(camRect.Right, camRect.Bottom), pickPos)) continue;

                if (pickCam == null || cam.Item2.Camera.Layer > pickCam.Item2.Camera.Layer)
                {
                    pickCam = cam;
                    pickCamRect = camRect;
                }
            }

            // Calculate pickPosClip
            pickPosClip = ((pickPos - new float2(pickCamRect.Left, pickCamRect.Top)) * new float2(2.0f / pickCamRect.Width, -2.0f / pickCamRect.Height)) + new float2(-1, 1);
            Ray = new Rayf(pickPosClip, float4x4.Invert(pickCam.Item1.GetTransform().Matrix), pickCam.Item2.Camera.GetProjectionMat(rc.ViewportWidth, rc.ViewportHeight, out _));

            return Viserate();
        }

        #region Visitors

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RayCasterState"/> is updated.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(Transform transform)
        {
            State.Model *= transform.Matrix;
        }

        /// <summary>
        /// Creates a raycast result from a given mesh if it is hit by the ray.
        /// </summary>
        /// <param name="mesh">The given mesh.</param>
        [VisitMethod]
        public void HitMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

            AABBf box = State.Model * mesh.BoundingBox;
            if (!box.IntersectRay(Ray)) return;

            for (int i = 0; i < mesh.Triangles.Length; i += 3)
            {
                // Vertices of the picked triangle in world space
                var a = new float3(mesh.Vertices[mesh.Triangles[i + 0]]);
                a = float4x4.Transform(State.Model, a);

                var b = new float3(mesh.Vertices[mesh.Triangles[i + 1]]);
                b = float4x4.Transform(State.Model, b);

                var c = new float3(mesh.Vertices[mesh.Triangles[i + 2]]);
                c = float4x4.Transform(State.Model, c);

                // Normal of the plane defined by a, b, and c.
                var n = float3.Normalize(float3.Cross(a - c, b - c));

                // Distance between "Origin" and the plane abc when following the Direction.
                var distance = -float3.Dot(Ray.Origin - a, n) / float3.Dot(Ray.Direction, n);

                if (distance < 0)
                    continue;

                // Position of the intersection point between ray and plane.
                var point = Ray.Origin + Ray.Direction * distance;

                if (float3.PointInTriangle(a, b, c, point, out float u, out float v))
                {
                    if (CullMode == Cull.None || (CullMode == Cull.Clockwise) == (float3.Dot(n, Ray.Direction) < 0))
                    {
                        YieldItem(new RayCastResult
                        {
                            Mesh = mesh,
                            Node = CurrentNode,
                            Triangle = i,
                            Model = State.Model,
                            U = u,
                            V = v,
                            DistanceFromOrigin = distance
                        });
                    }
                }
            }
        }



        #endregion
    }
}