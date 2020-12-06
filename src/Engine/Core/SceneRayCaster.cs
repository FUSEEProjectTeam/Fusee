using System;
using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;

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
        /// Returns the (absolute) distance between ray origin and the intersection.
        /// </summary>
        public float DistanceFromOrigin;
    }

    /// <summary>
    /// Implements the scene raycaster.
    /// </summary>
    public class SceneRayCaster : Viserator<RayCastResult, SceneRayCaster.RayCasterState, SceneNode, SceneComponent>
    {
        /// <summary>
        /// The origin of the ray (in world space).
        /// </summary>
        public float3 Origin { get; private set; }

        /// <summary>
        /// The direction of the ray (in world space).
        /// </summary>
        public float3 Direction { get; private set; }

        #region State
        /// <summary>
        /// The raycaster state upon scene traversal.
        /// </summary>
        public class RayCasterState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

            /// <summary>
            /// The registered model.
            /// </summary>
            public float4x4 Model
            {
                set => _model.Tos = value;
                get => _model.Tos;
            }

            /// <summary>
            /// The default constructor for the <see cref="RayCasterState" class, which registers state stacks for the model./>
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
        public SceneRayCaster(SceneContainer scene)
            : base(scene.Children)
        {

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
        /// <param name="origin">The origin of the ray (in world space).</param>
        /// <param name="direction">The direction of the ray (in world space).</param>
        /// <returns>A collection of <see cref="RayCastRestult"/> that can be iterated over.</returns>
        public IEnumerable<RayCastResult> RayCast(float3 origin, float3 direction)
        {
            Origin = origin;
            Direction = direction;
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
            State.Model *= transform.Matrix();
        }

        /// <summary>
        /// Creates a raycast result from a given mesh if it is hit by the ray.
        /// </summary>
        /// <param name="mesh">The given mesh.</param>
        [VisitMethod]
        public void HitMesh(Mesh mesh)
        {
            if (!mesh.Active) return;

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
                var distance = -float3.Dot(Origin - a, n) / float3.Dot(Direction, n);

                if (distance < 0) return;

                // Position of the intersection point between ray and plane.
                var point = Origin + Direction * distance;

                if (float3.PointInTriangle(a, b, c, point, out float u, out float v))
                {
                    YieldItem(new RayCastResult
                    {
                        Mesh = mesh,
                        Node = CurrentNode,
                        Triangle = i,
                        Model = State.Model,
                        U = u,
                        V = v,
                        DistanceFromOrigin = System.Math.Abs(distance)
                    });
                }
            }
        }

        #endregion
    }
}