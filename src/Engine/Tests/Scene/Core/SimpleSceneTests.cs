using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Fusee.Engine.SimpleScene;
using Fusee.Math;
using Fusee.Serialization;
using NUnit.Framework;

namespace Tests.Scene.Core
{
    [TestFixture]
    class VisitorTests
    {

        public class MySceneVisitor : SceneVisitor
        {
            [VisitMethod]
            void HowzitMesh(MeshComponent mesh)
            {
                Diagnostics.Log("Here's a mesh with " + mesh.Vertices.Length + " vertices shaping " + mesh.Triangles.Length + " triangles.");
            }

            [VisitMethod]
            void GDayMaterial(MaterialComponent material)
            {
                float3 col = material.Diffuse.Color;
                Diagnostics.Log("Here's a material showing (" + col.r + ", " + col.g + ", " + col.b + ").");
            }

            [VisitMethod]
            void HelloNode(SceneNodeContainer node)
            {
                Diagnostics.Log("A node called " + node.Name + ".");
            }
        }

        public static SceneNodeContainer CreateTestHierarchy()
        {
            var aMesh = new MeshComponent
            {
                Name = "Cube",
                Vertices = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Normals = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Triangles = new ushort[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 1,
                    4, 5, 6,
                    4, 6, 7,
                    4, 7, 5,
                }
            };

            var aChild = new SceneNodeContainer()
            {
                Name = "A Child",

                Components = new List<SceneComponentContainer>(new SceneComponentContainer[]
                {
                    new TransformComponent { Rotation = new float3(0, 0, 0), Translation = new float3(0.22f, 0.221f, 0), Scale = new float3(1, 1, 1) },
                    aMesh
                })
            };

            var aGrandChild = new SceneNodeContainer
            {
                Name = "Test",

                Components = new List<SceneComponentContainer>(new SceneComponentContainer[]
                {
                    new TransformComponent { Rotation = new float3(0, 0, 0), Translation = new float3(0.33f, 0.33f, 0), Scale = new float3(1, 1, 1) },
                })
            };

            var anotherChild = new SceneNodeContainer
            {
                Name = "Test",
                Components = new List<SceneComponentContainer>(new SceneComponentContainer[]
                {
                    new TransformComponent { Rotation = new float3(0, 0, 0), Translation = new float3(0.11f, 0.11f, 0), Scale = new float3(1, 1, 1) },
                    aMesh
                }),
                Children = new List<SceneNodeContainer>(new[] { aGrandChild })
            };


            var parent = new SceneNodeContainer
            {
                Name = "Root",
                Components = new List<SceneComponentContainer> {
                    new TransformComponent
                    {
                        Rotation = new float3(0, 0, 0),
                        Translation = new float3(0, 0, 0),
                        Scale = new float3(1, 1, 1)
                    },
                },

                Children = new List<SceneNodeContainer>(new[]
                {
                    aChild,
                    anotherChild
                })
            };
            return parent;
        }


        public static void SimpleVisitorTest()
        {
            var root = CreateTestHierarchy();
            MySceneVisitor visitor = new MySceneVisitor();
            visitor.Traverse(root);
        }


        public static void EnumeratorTests()
        {
            var root = CreateTestHierarchy();

            // Enumerate all nodes called "Test"
            foreach (SceneNodeContainer node in root.Children.FindNodes(node => node.Name == "Test"))
            {
                Diagnostics.Log("Got a node with " + (node.Children == null ? 0 : node.Children.Count) + " children and " + (node.Components == null ? 0 : node.Components.Count) + " components.");
            }

            SceneNodeContainer firstTest = root.FindNodes(node => node.Name == "Test").FirstOrDefault();

            // Enumerate all components containing a Mesh component with more than 10 vertices
            foreach (var m in root.FindComponents<MeshComponent>(mesh => mesh.Triangles.Length > 10))
            {
                Diagnostics.Log("Got a mesh with " + m.Triangles.Length + " triangle indices.");
            }


            /*
            // Enumerate all "special" nodes (SpecialNode derived from Node)
            foreach (SceneNodeContainer node in root.FindNode<SpecialNode>(snode  => snode.SpecialProperty == Specialvalue))
            {
            }


            // Enumerate all nodes containing a component with a spot light
            foreach (SceneNodeContainer node in root.FindNodeWithComponent<LightComponent>(light => light.Type == LightType.Spot))
            {
            }

            // Enumerate all special nodes containing a special component with a certain match
            foreach (SceneNodeContainer node in root.FindNodeWithComponent<SpecialNode, SpecialComponent>(snode => snode.SpecialProperty == Specialvalue, scomp => scomp.Abc == Xyz))
            {
            }

            // Enumerate all nodes containing a Mesh component with more than 10 vertices
            foreach (SceneComponentContainer node in root.FindComponent<MeshContainer>(mesh => mesh.Vertices.Depth > 10))
            {
            }
            */
        }


        public struct Tri
        {
            public float2 A;
            public float2 B;
            public float2 C;
        }

        public class TestViserator : Viserator<Tri, StandardState>
        {
            private Tri _t;
            public override Tri Current
            {
                get { return _t; }
            }

            protected override void InitState()
            {
                base.InitState();
                State.Model = float4x4.Identity;
                State.View = float4x4.Identity;
                State.Projection = float4x4.Identity;
            }

            [VisitMethod]
            public void Test(MeshComponent mc)
            {
                if (mc.Triangles != null && mc.Triangles.Length > 2)
                {
                    float4x4 mvp = State.Projection * State.View * State.Model;
                    float4 a = mvp * new float4(mc.Vertices[mc.Triangles[0]], 1);
                    float4 b = mvp * new float4(mc.Vertices[mc.Triangles[1]], 1);
                    float4 c = mvp * new float4(mc.Vertices[mc.Triangles[2]], 1);

                    _t = new Tri {A = a.xy/a.w, B = b.xy/b.w, C = c.xy/c.w};
                    YieldOnCurrentComponent = true;
                }
            }

            [VisitMethod]
            public void Test(TransformComponent tc)
            {
                State.Model = tc.Matrix();
            }

        }

        public static void ViseratorTests()
        {
            var root = CreateTestHierarchy();
            foreach (var tri in root.Viserate<TestViserator, Tri>())
            {
               Diagnostics.Log("{a:" + tri.A + "; b:" + tri.B + "; c:" + tri.C + "}");
            }
        }
    }
}
