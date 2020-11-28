using System;
using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    public class RayCastResult
    {
        public SceneNode Node;

        public Mesh Mesh;

        public int Triangle;

        public float U, V;

        public float4x4 Model;

        public void GetTriangle(out float3 a, out float3 b, out float3 c)
        {
            a = Mesh.Vertices[Mesh.Triangles[Triangle + 0]];
            b = Mesh.Vertices[Mesh.Triangles[Triangle + 1]];
            c = Mesh.Vertices[Mesh.Triangles[Triangle + 2]];
        }

        public float3 TriangleBarycentric
        {
            get
            {
                GetTriangle(out var a, out var b, out var c);
                return float3.Barycentric(a, b, c, U, V);
            }
        }

        public float3 ModelPos => TriangleBarycentric;

        public float3 WorldPos => float4x4.TransformPerspective(Model, ModelPos);

    }

    public class SceneRayCaster : Viserator<RayCastResult, SceneRayCaster.RayCasterState, SceneNode, SceneComponent>
    {
        public float3 Origin { get; private set; }
        public float3 Direction { get; private set; }
        
        #region State
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

            public RayCasterState()
            {
                RegisterState(_model);
            }
        }
        #endregion

        public SceneRayCaster (SceneContainer scene) 
            : base(scene.Children)
        {
            // TODO: Implement constructor
        }

        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
        }

        public IEnumerable<RayCastResult> RayCast(float3 origin, float3 direction)
        {
            Console.WriteLine("Starting Traversal!");

            Origin = origin;
            Direction = direction;
            return Viserate();
        }

        #region Visitors

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(Transform transform)
        {
            State.Model *= transform.Matrix();
        }

        [VisitMethod]
        public void IntersectMesh(Mesh mesh)
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
                        V = v
                    }) ;
                }

            }
        }

        #endregion
    }
}

/*
// Camera and therefore ray origin in world space
var origin = _mainCamTransform.Translation;

// Direction of the ray in world space
var ray_clip = new float4(pickPosClip.x, pickPosClip.y, 1, 1);
var ray_cam = float4x4.Transform(RC.InvProjection, ray_clip);
ray_cam = new float4(ray_cam.x, ray_cam.y, 1, 0);
var ray_world = float4x4.Transform(RC.InvView, ray_cam).xyz;
ray_world = float3.Normalize(ray_world);

// Vertices of the picked triangle in world space
var a = new float3(newPick.Mesh.Vertices[newPick.Triangle + 0]);
a = float4x4.Transform(_triangleTransform.Matrix(), a);

var b = new float3(newPick.Mesh.Vertices[newPick.Triangle + 1]);
b = float4x4.Transform(_triangleTransform.Matrix(), b);

var c = new float3(newPick.Mesh.Vertices[newPick.Triangle + 2]);
c = float4x4.Transform(_triangleTransform.Matrix(), c);


var n = float3.Normalize(float3.Cross(a - c, b - c));

var distance = -float3.Dot(origin - a, n) / float3.Dot(ray_world, n);


var worldPos = origin + ray_world * distance;
*/