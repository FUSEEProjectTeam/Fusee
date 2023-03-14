using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This class contains information about the scene of the picked point.
    /// </summary>
    public class RayCastResult : PickResult
    {
        /// <summary>
        /// The mesh.
        /// </summary>
        public Mesh? Mesh;
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

        private readonly IEnumerable<CameraResult> _prePassResults;

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
        /// <param name="prePassCameraResults">The collected <see cref="IEnumerable{CameraResult}"/> from the <see cref="PrePassVisitor.PrePassTraverse(SceneContainer)"/> functionality.</param>
        /// <param name="cullMode">The <see cref="Cull"/> mode to use.</param>
        public SceneRayCaster(SceneContainer scene, IEnumerable<CameraResult> prePassCameraResults, Cull cullMode = Cull.None)
            : base(scene.Children)
        {
            CullMode = cullMode;
            _prePassResults = prePassCameraResults;
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
        /// <param name="ray">The <see cref="RayF"/> which should travel through the scene</param>
        /// <returns></returns>
        public IEnumerable<RayCastResult>? Traverse(RayF ray)
        {
            Ray = ray;
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
            if (mesh == null) return;
            if (!mesh.Active) return;
            if (mesh.Vertices == null) return;
            if (mesh.Triangles == null) return;

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
                            Model = State.Model,
                            DistanceFromOrigin = distance
                        });
                    }
                }
            }
        }



        #endregion
    }
}