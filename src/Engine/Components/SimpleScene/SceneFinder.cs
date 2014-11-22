using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Fusee.Math;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{

    public class SimpleSceneTest
    {
        public class SomeClass
        {
            public void SomeMethod(int i)
            {
                Diagnostics.Log("Here I am " + i);
            }
        }

        public class SomeGeneric<T>
        {
            public void SomeMethod(T o)
            {
                Diagnostics.Log("Here I am " + o);
            }
        }

        public static void JSILReflectionInvocationTest()
        {
            // Call SomeClass.SomeMethod via reflection
            object sc = new SomeClass();
            Type type1 = sc.GetType();
            MethodInfo mi1 = type1.GetMethod("SomeMethod");
            mi1.Invoke(sc, new[] { (object)42} );

            // Call SomeGeneric<int>.SomeMethod via reflection
            object sg = new SomeGeneric<int>();
            Type type2 = sg.GetType();
            MethodInfo mi2 = type2.GetMethod("SomeMethod");
            mi2.Invoke(sg, new[] { (object)42 });
        }

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
                Transform = new TransformContainer() { Rotation = new float3(0, 0, 0), Translation = new float3(0.22f, 0.221f, 0), Scale = new float3(1, 1, 1) },
                Components = new List<SceneComponentContainer>(new []{aMesh})
            };

            var aGrandChild = new SceneNodeContainer()
            {
                Name = "Test",
                Transform = new TransformContainer() { Rotation = new float3(0, 0, 0), Translation = new float3(0.33f, 0.33f, 0), Scale = new float3(1, 1, 1) }
            };

            var anotherChild= new SceneNodeContainer()
            {
                Name = "Test",
                Transform = new TransformContainer() { Rotation = new float3(0, 0, 0), Translation = new float3(0.11f, 0.11f, 0), Scale = new float3(1, 1, 1) },
                Components = new List<SceneComponentContainer>(new []{aMesh}),
                Children = new List<SceneNodeContainer>(new []{aGrandChild})
            };


            /*
            var parent = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    Version = 1,
                    Generator = "Test Code",
                    CreatedBy = "Test Person"
                },
                Children = new List<SceneNodeContainer>(new SceneNodeContainer[]
                {
                    aChild,
                    anotherChild,
                }),
            };
            */
            var parent = new SceneNodeContainer()
            {
                Transform =
                    new TransformContainer()
                    {
                        Rotation = new float3(0, 0, 0),
                        Translation = new float3(0, 0, 0),
                        Scale = new float3(1, 1, 1)
                    },
                Name = "Root",
                Children = new List<SceneNodeContainer>(new []
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
                Diagnostics.Log("Got a node with " + (node.Children == null ? 0 : node.Children.Count) + " children and " + (node.Components == null ? 0 : node.Components.Count) +  " components.");
            }

            SceneNodeContainer firstTest = root.FindNodes(node => node.Name == "Test").FirstOrDefault();

            // Enumerate all components containing a Mesh component with more than 10 vertices
            foreach (var m in root.FindComponents<MeshComponent>(mesh => mesh.Triangles.Length > 10))
            {
                Diagnostics.Log("Got a mesh with "  +  m.Triangles.Length + " triangle indices.");
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
    }

    public static class SceneFinderExtensions
    {
        // Unfortunate construct, but there seems no other way. What we really needed here is a MixIn to make 
        // a SceneNodeContainer or SceneContainer implement IEnumerable (afterwards). All C# offers us is to 
        // define ExtensionMethods returning an IEnumerable<>.
        // Thus we need some class to implement that. Here it is - tada:
        internal class SceneNodeEnumerable<TNode> : IEnumerable<TNode> where TNode : SceneNodeContainer
        {
            internal Predicate<TNode> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TNode> GetEnumerator() { return new SceneNodeFinder<TNode>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        internal class SceneComponentEnumerable<TComponent> : IEnumerable<TComponent> where TComponent : SceneComponentContainer
        {
            internal Predicate<TComponent> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TComponent> GetEnumerator() { return new SceneComponentFinder<TComponent>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        internal class SceneNodeWhereComponentEnumerable<TNode, TComponent> : IEnumerable<TNode> where TNode : SceneNodeContainer where TComponent : SceneComponentContainer
        {
            internal Predicate<TComponent> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TNode> GetEnumerator() { return new SceneNodeWhereComponentFinder<TNode, TComponent>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        #region FindNodes
        /// <summary>
        /// Creates an enumerable traversing the tree starting with the given node.
        /// </summary>
        /// <param name="root">The root node where to start the traversal.</param>
        /// <param name="match">The matching predicate. Enumeration will yield on every matching node.</param>
        /// <returns>An enumerable that can be used in foreach statements.</returns>
        public static IEnumerable<SceneNodeContainer> FindNodes(this SceneNodeContainer root, Predicate<SceneNodeContainer> match)
        {
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<TNode> FindNodes<TNode>(this SceneNodeContainer root, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> {_match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root)};
        }

        public static IEnumerable<SceneNodeContainer> FindNodes(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneNodeContainer> match)
        {
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        public static IEnumerable<TNode> FindNodes<TNode>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

        #region FindComponents
        public static IEnumerable<SceneComponentContainer> FindComponents(this SceneNodeContainer root, Predicate<SceneComponentContainer> match) 
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<TComponent> FindComponents<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<SceneComponentContainer> FindComponents(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<TComponent> FindComponents<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

        #region FindNodesWhereComponent
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this SceneNodeContainer root, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this SceneNodeContainer root, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

    }



    /// <summary>
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene nodes.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TNode">The concrete type of nodes to look for.</typeparam>
    public class SceneNodeFinder<TNode> : SceneFinderBase<TNode>, IEnumerator<TNode> where TNode : SceneNodeContainer
    {
        public SceneNodeFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TNode> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNextNoComponent();}

        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

        [VisitMethod]
        protected void MatchNode(TNode node)
        {
            if (_match != null && _match(node))
            {
                YieldOnCurrentNode = true;
            }
        }
        
    }

    /// <summary>
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene components.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    public class SceneComponentFinder<TComponent> : SceneFinderBase<TComponent>, IEnumerator<TComponent> where TComponent : SceneComponentContainer
    {
        public SceneComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNext(); }

        public TComponent Current { get { return (TComponent) CurrentComponent; } }
        object IEnumerator.Current { get { return Current; } }

        [VisitMethod]
        protected void MatchComponent(TComponent component)
        {
            if (_match != null && _match(component))
            {
                YieldOnCurrentComponent = true;
            }
        }
    
    }

    /// <summary>
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene components.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    public class SceneNodeWhereComponentFinder<TNode, TComponent> : SceneFinderBase<TComponent>, IEnumerator<TNode>
        where TNode : SceneNodeContainer
        where TComponent : SceneComponentContainer
    {
        public SceneNodeWhereComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNext(); }

        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

        [VisitMethod]
        protected void MatchComponent(TComponent component)
        {
            if (_match != null && _match(component))
            {
                YieldOnCurrentComponent = true;
            }
        }
    }


    /// <summary>
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates.
    /// Results are returned as enumerator.
    /// </summary>
    public class SceneFinderBase<TSceneElementType> : SceneVisitor
    {
        protected IEnumerator<SceneNodeContainer> _rootList;
        protected Predicate<TSceneElementType> _match;

        public SceneFinderBase(IEnumerator<SceneNodeContainer> rootList, Predicate<TSceneElementType> match)
        {
            _match = match;
            _rootList = rootList;
            EnumInit(_rootList);
        }

        public void Dispose()
        {
            _rootList = null;
            _match = null;
        }

        public void Reset()
        {
            EnumInit(_rootList);
        }
    }
}
