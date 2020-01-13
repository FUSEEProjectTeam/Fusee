using System;
using Xunit;
using Fusee.Xirkit;

namespace Fusee.Test.Xirkit
{
    public class BasicCircuitTest
    {
        [Fact]
        public void ChangesNode1_Node2ShouldBeEqual()
        {
            TestClass obj1 = new TestClass(0, 0);
            TestClass obj2 = new TestClass(1, 1);

            Circuit circuit = new Circuit();
            Node node1 = new Node(obj1);
            Node node2 = new Node(obj2);

            circuit.AddRoot(node1);
            circuit.AddNode(node2);
            node1.Attach("x", node2, "x");

            circuit.Execute();

            Assert.True(obj2.x == obj1.x, "Node2.x = " + obj2.x + " but it should be " + obj1.x + ".");
        }

        protected class TestClass
        {
            public int x;
            public int y;

            public TestClass(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
