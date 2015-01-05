using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.SimpleScene;
using Fusee.Math;
using Fusee.Serialization;
using NUnit.Framework;

namespace Fusee.Tests.Scene.Core
{
    [TestFixture]
    public class VisitorTests
    {
        public class MySceneVisitor : SceneVisitor
        {
            public bool MeshSeen;
            public int MeshVerts;
            public int MeshTris;

            public bool MaterialSeen;
            public float3 MaterialColor;

            public bool NodeSeen;
            public string NodeName;

            [VisitMethod]
            void HowzitMesh(MeshComponent mesh)
            {
                MeshSeen = true;
                MeshVerts = mesh.Vertices.Length;
                MeshTris = mesh.Triangles.Length;
            }

            [VisitMethod]
            void GDayMaterial(MaterialComponent material)
            {
                MaterialSeen = true;
                MaterialColor = material.Diffuse.Color;
            }

            [VisitMethod]
            void HelloNode(SceneNodeContainer node)
            {
                NodeSeen = true;
                NodeName = node.Name;
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
                    new MaterialComponent { Diffuse = new MatChannelContainer{ Color = new float3(0.1f, 0.2f, 0.4f)}}, 
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


        [Test]
        public static void BasicVisitorTest()
        {
            var root = CreateTestHierarchy();
            MySceneVisitor visitor = new MySceneVisitor();
            visitor.Traverse(root);

            Assert.IsTrue(visitor.MeshSeen);
            Assert.AreEqual(18, visitor.MeshTris);
            Assert.AreEqual(8, visitor.MeshVerts);

            Assert.IsTrue(visitor.MaterialSeen);
            Assert.AreEqual(0.1f, visitor.MaterialColor.r);
            Assert.AreEqual(0.2f, visitor.MaterialColor.g);
            Assert.AreEqual(0.4f, visitor.MaterialColor.b);

            Assert.IsTrue(visitor.NodeSeen);
            StringAssert.AreEqualIgnoringCase("Test", visitor.NodeName);
        }

        [Test]
        public static void BasicEnumeratorTests()
        {
            var root = CreateTestHierarchy();

            // Enumerate all nodes called "Test"
            int nNodes = 0;
            foreach (SceneNodeContainer node in root.FindNodes(node => node.Name == "Test"))
            {
                StringAssert.AreEqualIgnoringCase("Test", node.Name);
                nNodes++;
            }
            Assert.AreEqual(2, nNodes);


            // Enumerate all nodes called "Test". Start directly on the child list.
            nNodes = 0;
            foreach (SceneNodeContainer node in root.Children.FindNodes(node => node.Name == "Test"))
            {
                StringAssert.AreEqualIgnoringCase("Test", node.Name);
                nNodes++;
            }
            Assert.AreEqual(2, nNodes);



            SceneNodeContainer firstTest = root.FindNodes(node => node.Name == "Test").FirstOrDefault();
            Assert.NotNull(firstTest);

            SceneNodeContainer firstPest = root.FindNodes(node => node.Name == "Pest").FirstOrDefault();
            Assert.IsNull(firstPest);

            // Enumerate all components containing a Mesh component with more than 10 vertices
            nNodes = 0;
            foreach (var m in root.FindComponents<MeshComponent>(mesh => mesh.Triangles.Length > 10))
            {
                Assert.IsTrue(m.Triangles.Length > 10);
                nNodes++;
            }
            Assert.AreEqual(2, nNodes);


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

                    YieldItem(new Tri {A = a.xy/a.w, B = b.xy/b.w, C = c.xy/c.w});
                }
            }

            [VisitMethod]
            public void Test(TransformComponent tc)
            {
                State.Model = tc.Matrix();
            }

        }

        [Test]
        public static void BasicViseratorTest()
        {
            int nTris = 0;
            var root = CreateTestHierarchy();
            foreach (var tri in root.Viserate<TestViserator, Tri>())
            {
                nTris++;
                // TODO: Better check stack pushing and popping
                string ts = "{a:" + tri.A + "; b:" + tri.B + "; c:" + tri.C + "}";
            }
            Assert.AreEqual(2, nTris);
        }
    }
}


/* test scratch

        private void TestDictionaryNoCast()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict["one"] = 1;
            dict["two"] = 2;
            dict["three"] = 3;

            foreach (KeyValuePair<string, int> pair in dict)
            {
                Diagnostics.Log("Key: " + pair.Key + ", Value: " + pair.Value);
            }
        }

        private void TestDictionaryDoCast()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict["one"] = 1;
            dict["two"] = 2;
            dict["three"] = 3;

            IEnumerable enuObject = dict;
            foreach (object pairOb in enuObject)
            {
                KeyValuePair<string, int> pair = (KeyValuePair<string, int>) pairOb;
                Diagnostics.Log("Key: " + pair.Key + ", Value: " + pair.Value);
            }
        }


        IEnumerable enuObject = (IEnumerable) dict;
        DoTheTest(enuObject);

            foreach (KeyValuePair<string, int> pair in dict)
            {
                Diagnostics.Log("Key: " + pair.Key + ", Value: " + pair.Value);
            }

            IEnumerable enuObject = dict;
            IEnumerable<KeyValuePair<string, int>> enuTypeSafe = (IEnumerable<KeyValuePair<string, int>>)enuObject;

            foreach (KeyValuePair<string, int> pair in enuTypeSafe)
            {
                Diagnostics.Log("Key: " + pair.Key + "Value: " + pair.Value);
            }


        public void DoTheTest(IEnumerable enuObject)
        {
            foreach (object pairOb in enuObject)
            {
                KeyValuePair<string, int> pair = (KeyValuePair<string, int>) pairOb;
                Diagnostics.Log("Key: " + pair.Key + ", Value: " + pair.Value);
            }
        }
            */
