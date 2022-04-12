using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fusee.Tests.Xene
{
    public class SimpleXene
    {

        [Fact]
        public void SingleNodeEnumeratorThrowsOnReset()
        {
            TestNode node = new();

            var singleNodeEnumerator = VisitorHelpers.SingleRootEnumerator(node);

            // See https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerator.reset?view=netcore-3.1:
            // "The Reset method is provided for COM interoperability. It does not necessarily need to be implemented;
            // instead, the implementer can simply throw a NotSupportedException."
            Assert.Throws<NotSupportedException>(() => singleNodeEnumerator.Reset());
        }

        [Fact]
        public void SingleNodeEnumerableForEach()
        {
            TestNode node = new();

            var singleNodeEnumerable = VisitorHelpers.SingleRootEnumerable(node);

            int i = 0;
            foreach (var curNode in singleNodeEnumerable)
            {
                Assert.Equal(node, curNode);
                i++;
            }

            Assert.Equal(1, i);
        }

        [Fact]
        public void SingleNodeEnumerableForEachTwoTimes()
        {
            // to test the enumeration without the usage of reset()
            // (enumerable should yield a new enumerator every time it's called)
            TestNode node = new();

            var singleNodeEnumerable = VisitorHelpers.SingleRootEnumerable(node);

            int i = 0;
            foreach (var curNode in singleNodeEnumerable)
            {
                Assert.Equal(node, curNode);
                i++;
            }

            Assert.Equal(1, i);

            i = 0;
            foreach (var curNode in singleNodeEnumerable)
            {
                Assert.Equal(node, curNode);
                i++;
            }

            Assert.Equal(1, i);
        }

        [Fact]
        public void FindNodesSingleRoot()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestNode> findResultChild = tree.FindNodes(node => node.Name.Contains("Child"));
            Assert.Equal(2, findResultChild.Count());

            IEnumerable<TestNode> findResultNode = tree.FindNodes(node => node.Name.Contains("Node"));
            Assert.Equal(2, findResultNode.Count());

        }

        [Fact]
        public void FindNodesListOfRoots()
        {
            var twoRoots = TwoTestTrees();

            IEnumerable<TestNode> findResultChild = twoRoots.FindNodes(node => node.Name.Contains("Child"));

            Assert.Equal(4, findResultChild.Count());
        }

        [Fact]
        public void FindComponentsSingleRootDirect()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestComponent> findComponentsResult2 = tree.FindComponents<TestNode, TestComponent>(comp => comp.Num % 2 == 0);
            Assert.Equal(3, findComponentsResult2.Count());

            IEnumerable<TestComponent> findComponentsResult3 = tree.FindComponents<TestNode, TestComponent>(comp => comp.Num % 3 == 0);
            Assert.Equal(2, findComponentsResult3.Count());
        }

        [Fact]
        public void FindComponentsSingleRootExtension()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestComponent> findComponentsResult2 = tree.FindComponents(comp => comp.Num % 2 == 0);
            Assert.Equal(3, findComponentsResult2.Count());

            IEnumerable<TestComponent> findComponentsResult3 = tree.FindComponents(comp => comp.Num % 3 == 0);
            Assert.Equal(2, findComponentsResult3.Count());
        }


        [Fact]
        public void FindComponentsByTypeSingleRootDirect()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<SpecialTestComponent> findComponentsResult2 = tree.FindComponents<SpecialTestComponent, TestNode, TestComponent>(comp => comp.SpecialId.Contains("special"));
            Assert.Equal(2, findComponentsResult2.Count());

            IEnumerable<SomeOtherTestComponent> findComponentsResult3 = tree.FindComponents<SomeOtherTestComponent, TestNode, TestComponent>(comp => comp.SomeOtherValue == 42);
            Assert.Single(findComponentsResult3);
        }

        [Fact]
        public void FindComponentsByTypeSingleRootExtension()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<SpecialTestComponent> findComponentsResult2 = tree.FindComponents<SpecialTestComponent>(comp => comp.SpecialId.Contains("special"));
            Assert.Equal(2, findComponentsResult2.Count());

            IEnumerable<SomeOtherTestComponent> findComponentsResult3 = tree.FindComponents<SomeOtherTestComponent>(comp => comp.SomeOtherValue == 42);
            Assert.Single(findComponentsResult3);
        }

        [Fact]
        public void FindNodesWhereComponentSingleRootDirect()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestNode> findNodesResult = tree.FindNodesWhereComponent<TestNode, TestComponent>(comp => comp.Num == 3);
            Assert.Collection(findNodesResult, node => Assert.Equal("ChildNode", node.Name));

            IEnumerable<TestNode> findNodesResult2 = tree.FindNodesWhereComponent<TestNode, TestComponent>(comp => comp.Num == 6);
            Assert.Collection(findNodesResult2, node => Assert.Equal("RootNode", node.Name));
        }

        [Fact]
        public void FindNodesWhereComponentSingleRootExtension()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestNode> findNodesResult = tree.FindNodesWhereComponent(comp => comp.Num == 3);
            Assert.Collection(findNodesResult, node => Assert.Equal("ChildNode", node.Name));

            IEnumerable<TestNode> findNodesResult2 = tree.FindNodesWhereComponent(comp => comp.Num == 6);
            Assert.Collection(findNodesResult2, node => Assert.Equal("RootNode", node.Name));
        }

        [Fact]
        public void FindNodesWhereComponentByTypeSingleRootDirect()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestNode> findNodesResult = tree.FindNodesWhereComponent<SpecialTestComponent, TestNode, TestComponent>(comp => comp.SpecialId.Contains("special"));
            Assert.Collection(findNodesResult,
                node => Assert.Equal("RootNode", node.Name),
                node => Assert.Equal("ChildNode", node.Name)
            );

            IEnumerable<TestNode> findNodesResult2 = tree.FindNodesWhereComponent<SomeOtherTestComponent, TestNode, TestComponent>(comp => comp.SomeOtherValue == 3.1415f);
            Assert.Collection(findNodesResult2,
                node => Assert.Equal("RootNode", node.Name)
            );
        }

        [Fact]
        public void FindNodesWhereComponentByTypeSingleRootExtension()
        {
            TestNode tree = CreateSimpleTestTree();

            IEnumerable<TestNode> findNodesResult = tree.FindNodesWhereComponent<SpecialTestComponent>(comp => comp.SpecialId.Contains("special"));
            Assert.Collection(findNodesResult,
                node => Assert.Equal("RootNode", node.Name),
                node => Assert.Equal("ChildNode", node.Name)
            );

            IEnumerable<TestNode> findNodesResult2 = tree.FindNodesWhereComponent<SomeOtherTestComponent>(comp => comp.SomeOtherValue == 3.1415f);
            Assert.Collection(findNodesResult2,
                node => Assert.Equal("RootNode", node.Name)
            );
        }

        [Fact]
        public void SingleNodeViserator()
        {
            TestNode tree = CreateSimpleTestTree();

            var resultSet = tree.Viserate<TestViserator, TestResult, TestNode, TestComponent>();
            Assert.Collection(resultSet,
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[2]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[6]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNode/[3]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNumberTwo/[4]" }, resultItem)
            );
        }

        [Fact]
        public void ListOfRootsViserator()
        {
            var twoRoots = TwoTestTrees();


            var resultSet = twoRoots.Viserate<TestViserator, TestResult, TestNode, TestComponent>();

            Assert.Collection(resultSet,
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[2]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[6]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNode/[3]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNumberTwo/[4]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[2]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 1, Path = "/RootNode/[6]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNode/[3]" }, resultItem),
                resultItem => Assert.Equal(new TestResult { Depth = 2, Path = "/RootNode/ChildNumberTwo/[4]" }, resultItem)
            );
        }


        private static IEnumerable<TestNode> TwoTestTrees()
        {
            yield return CreateSimpleTestTree();
            yield return CreateSimpleTestTree();
        }

        private static TestNode CreateSimpleTestTree()
        {
            return new TestNode
            {
                Name = "RootNode",
                Children = new TestNode[]
                {
                    new TestNode
                    {
                        Name="ChildNode",
                        Components = new TestComponent[]
                        {
                            new SpecialTestComponent{Num = 3 , SpecialId = "I am special"},
                        }
                    },
                    new TestNode
                    {
                        Name="ChildNumberTwo",
                        Components = new TestComponent[]
                        {
                            new SomeOtherTestComponent{Num = 4, SomeOtherValue = 42},
                        }
                    },
                },
                Components = new TestComponent[]
                {
                    new SomeOtherTestComponent{Num = 2, SomeOtherValue = 3.1415f},
                    new SpecialTestComponent{Num = 6, SpecialId = "Especially for this test"}
                }
            };
        }
    }

    internal class TestNode : INode
    {
        public string Name;
        public TestNode[] Children;

        public TestComponent[] Components;

        public IEnumerable<INode> EnumChildren => Children;

        public IEnumerable<IComponent> EnumComponents => Components;
    }

    internal class TestComponent : IComponent
    {
        public int Num;

        public bool Active { get; set; } = true;
    }

    internal class SpecialTestComponent : TestComponent
    {
        public string SpecialId;
    }

    internal class SomeOtherTestComponent : TestComponent
    {
        public float SomeOtherValue;
    }




    internal static class TestExtensions
    {
        // Find Components by predicate only
        public static IEnumerable<TestComponent> FindComponents(this TestNode root, Predicate<TestComponent> match)
            => root.FindComponents<TestNode, TestComponent>(match);

        public static IEnumerable<TestComponent> FindComponents(this IEnumerable<TestNode> roots, Predicate<TestComponent> match)
            => roots.FindComponents<TestNode, TestComponent>(match);

        // Find Components by type (and predicate)
        public static IEnumerable<TComponentToFind> FindComponents<TComponentToFind>(this TestNode root, Predicate<TComponentToFind> match)
            where TComponentToFind : TestComponent
            => root.FindComponents<TComponentToFind, TestNode, TestComponent>(match);

        public static IEnumerable<TComponentToFind> FindComponents<TComponentToFind>(this IEnumerable<TestNode> roots, Predicate<TComponentToFind> match)
            where TComponentToFind : TestComponent
            => roots.FindComponents<TComponentToFind, TestNode, TestComponent>(match);

        // Find nodes containing matching components by predicate only
        public static IEnumerable<TestNode> FindNodesWhereComponent(this TestNode root, Predicate<TestComponent> match)
            => root.FindNodesWhereComponent<TestNode, TestComponent>(match);

        public static IEnumerable<TestNode> FindNodesWhereComponent(this IEnumerable<TestNode> roots, Predicate<TestComponent> match)
            => roots.FindNodesWhereComponent<TestNode, TestComponent>(match);

        // Find nodes containing matching components by type (and predicate)
        public static IEnumerable<TestNode> FindNodesWhereComponent<TComponentToFind>(this TestNode root, Predicate<TComponentToFind> match)
            where TComponentToFind : TestComponent
            => root.FindNodesWhereComponent<TComponentToFind, TestNode, TestComponent>(match);

        public static IEnumerable<TestNode> FindNodesWhereComponent<TComponentToFind>(this IEnumerable<TestNode> roots, Predicate<TComponentToFind> match)
            where TComponentToFind : TestComponent
            => roots.FindNodesWhereComponent<TComponentToFind, TestNode, TestComponent>(match);
    }

    internal struct TestResult
    {
        public int Depth;
        public string Path;

        public override string ToString()
        {
            return $"Depth: {Depth}, Path: \"{Path}\"";
        }
    }

    internal class TestState : StateStack<string>
    {
        public TestState() : base()
        {
        }
    }

    internal class TestViserator : Viserator<TestResult, TestState, TestNode, TestComponent>
    {
        [VisitMethod]
        public void VisitNode(TestNode node)
        {
            State.Tos += "/" + node.Name;
        }

        [VisitMethod]
        public void VisitComponent(TestComponent comp)
        {
            YieldItem(new TestResult
            {
                Depth = State.Depth,
                Path = State.Tos + $"/[{comp.Num}]",
            });
        }
    }
}