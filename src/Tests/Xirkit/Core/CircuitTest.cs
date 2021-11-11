using Fusee.Xirkit;
using System;
using Xunit;
using static Fusee.Tests.Xirkit.HelperClasses;

namespace Fusee.Tests.Xirkit
{
    public class CircuitTest
    {
        [Fact]
        public void CreateTwoNodesAndChangeSimpleValue()
        {
            SimpleClass obj1 = new(0, 0);
            SimpleClass obj2 = new(1, 1);

            Circuit circuit = new();
            Node node1 = new(obj1);
            Node node2 = new(obj2);

            circuit.AddNode(node1);
            circuit.AddRoot(node1);
            circuit.AddNode(node2);

            node1.Attach("x", node2, "x");

            circuit.Execute();

            Assert.True(obj2.x == obj1.x, "Node2.x is " + obj2.x + " but it should be " + obj1.x + ".");
        }

        [Fact]
        public void NestedProperties()
        {
            NestedClass obj1 = new(new SimpleStruct(1, 1));
            NestedClass obj2 = new(new SimpleStruct(0, 0));
            Circuit circuit = new();
            Node node1 = new(obj1);
            Node node2 = new(obj2);

            circuit.AddNode(node1);
            circuit.AddRoot(node1);
            circuit.AddNode(node2);

            node1.Attach("str.x", node2, "str.y");

            circuit.Execute();

            Assert.True(obj2.str.y == obj1.str.x, "Node2.str.y is " + obj2.str.y + " but should be " + obj1.str.x + ".");
        }

        [Fact]
        public void PerformCalculationOnExecute()
        {
            CalculationClass obj1 = new(1, 1);
            SimpleClass obj2 = new(0, 0);
            Circuit circuit = new();
            Node node1 = new(obj1);
            Node node2 = new(obj2);

            circuit.AddNode(node1);
            circuit.AddRoot(node1);
            circuit.AddNode(node2);

            node1.Attach("x", node2, "y");

            circuit.Execute();

            Assert.True(obj2.y == 2, "Obj2.y is " + obj2.y + " but should be " + obj1.x + ".");
            Assert.True(obj1.x == 2, "Obj1.x is " + obj1.x + " but should be 2.");
        }
    }
}