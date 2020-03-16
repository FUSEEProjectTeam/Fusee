using System;
using Xunit;
using Fusee.Xirkit;
using static Fusee.Test.Xirkit.HelperClasses;

namespace Fusee.Test.Xirkit
{
    public class CircuitTest
    {
        [Fact]
        public void CreateTwoNodesAndChangeSimpleValue()
        {
            TestClass obj1 = new TestClass(0, 0);
            TestClass obj2 = new TestClass(1, 1);

            Circuit circuit = new Circuit();
            Node node1 = new Node(obj1);
            Node node2 = new Node(obj2);

            circuit.AddNode(node1);
            circuit.AddRoot(node1);
            circuit.AddNode(node2);

            Assert.True(circuit.RootList.Contains(node1), "Node1 should be in RootList of circuit but isn't.");
            Assert.True(circuit.NodeList.Contains(node1), "Node1 should be in NodeList of circuit but isn't.");
            Assert.True(circuit.NodeList.Contains(node2), "Node2 should be in NodeList of circuit but isn't.");

            node1.Attach("x", node2, "x");

            circuit.Execute();

            Assert.True(obj2.x == obj1.x, "Node2.x is " + obj2.x + " but it should be " + obj1.x + ".");
        }
    }
}
