﻿using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This class contains information about the scene of the picked point.
    /// </summary>
    public class RayCastResult
    {
        /// <summary>
        /// The scene node container of the result.
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
                float2 uva = Mesh.UVs[(int)Mesh.Triangles[Triangle]];
                float2 uvb = Mesh.UVs[(int)Mesh.Triangles[Triangle + 1]];
                float2 uvc = Mesh.UVs[(int)Mesh.Triangles[Triangle + 2]];

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
            a = Mesh.Vertices[(int)Mesh.Triangles[Triangle + 0]];
            b = Mesh.Vertices[(int)Mesh.Triangles[Triangle + 1]];
            c = Mesh.Vertices[(int)Mesh.Triangles[Triangle + 2]];
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
        /// The <see cref="RayF"/> to check intersections with.
        /// </summary>
        public RayF Ray { get; private set; }

        /// <summary>
        /// The <see cref="Cull"/> mode to use by the SceneRayCaster
        /// </summary>
        public Cull CullMode { get; private set; }

        /// <summary>
        /// The <see cref="SceneContainer"/>, containing the scene that gets rendered.
        /// </summary>
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
        /// <param name="ray">The ray to test.</param>
        /// <returns>A collection of <see cref="RayCastResult"/> that can be iterated over.</returns>
        public IEnumerable<RayCastResult> RayCast(RayF ray)
        {
            Ray = ray;
            return Viserate();
        }

        /// <summary>
        /// Returns a collection of objects that are hit by the ray and that can be iterated over.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="pickPos"></param>
        /// <returns></returns>
        public IEnumerable<RayCastResult> RayPick(RenderContext rc, float2 pickPos)
        {
            PrePassVisitor.PrePassTraverse(_sc);
            var cams = PrePassVisitor.CameraPrepassResults;

            float2 pickPosClip;

            if (cams.Count == 0)
            {
                pickPosClip = (pickPos * new float2(2.0f / rc.ViewportWidth, -2.0f / rc.ViewportHeight)) + new float2(-1, 1);
                Ray = new RayF(pickPosClip, rc.View, rc.Projection);
                return Viserate();
            }

            CameraResult pickCam = default;
            Rectangle pickCamRect = new();

            foreach (var camRes in cams)
            {
                Rectangle camRect = new()
                {
                    Left = (int)(camRes.Camera.Viewport.x * rc.ViewportWidth / 100),
                    Top = (int)(camRes.Camera.Viewport.y * rc.ViewportHeight / 100)
                };
                camRect.Right = ((int)(camRes.Camera.Viewport.z * rc.ViewportWidth) / 100) + camRect.Left;
                camRect.Bottom = ((int)(camRes.Camera.Viewport.w * rc.ViewportHeight) / 100) + camRect.Top;

                if (!float2.PointInRectangle(new float2(camRect.Left, camRect.Top), new float2(camRect.Right, camRect.Bottom), pickPos)) continue;

                if (pickCam == default || camRes.Camera.Layer > pickCam.Camera.Layer)
                {
                    pickCam = camRes;
                    pickCamRect = camRect;
                }
            }

            // Calculate pickPosClip
            pickPosClip = ((pickPos - new float2(pickCamRect.Left, pickCamRect.Top)) * new float2(2.0f / pickCamRect.Width, -2.0f / pickCamRect.Height)) + new float2(-1, 1);
            Ray = new RayF(pickPosClip, pickCam.View, pickCam.Camera.GetProjectionMat(rc.ViewportWidth, rc.ViewportHeight, out _));

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
                var a = new float3(mesh.Vertices[(int)mesh.Triangles[i + 0]]);
                a = float4x4.Transform(State.Model, a);

                var b = new float3(mesh.Vertices[(int)mesh.Triangles[i + 1]]);
                b = float4x4.Transform(State.Model, b);

                var c = new float3(mesh.Vertices[(int)mesh.Triangles[i + 2]]);
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