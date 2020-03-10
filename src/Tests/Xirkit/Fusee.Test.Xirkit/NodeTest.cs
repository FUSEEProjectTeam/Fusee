using System;
using Xunit;
using System.Linq;
using Fusee.Xirkit;
using static Fusee.Test.Xirkit.HelperClasses;


namespace Fusee.Test.Xirkit
{
    public class NodeTest
    {
        [Fact]
        public void TestInAndOutPins_AndRemove_Object()
        {
            TestClass obj1 = new TestClass(0, 0);
            TestClass obj2 = new TestClass(1, 1);

            Node node1 = new Node(obj1);
            Node node2 = new Node(obj2);

            node1.Attach("x", node2, "y");

            Assert.Collection<IInPin>(node2.InPins, pin => Assert.True(pin.Member == "y", "Member is " + pin.Member + " but should be y"));
            Assert.Collection<IOutPin>(node1.OutPins, pin => Assert.True(pin.Member == "x", "Member is " + pin.Member + " but should be x"));

            node1.RemoveAllPins();
            node2.RemoveAllPins();

            Assert.True(node1.OutPins.Count() == 0, "OutPins count should be 0 but is " + node1.OutPins.Count());
            Assert.True(node2.InPins.Count() == 0, "InPins count should be 0 but is " + node2.OutPins.Count());
        }

        [Fact]
        public void TestInAndOutPins_AndRemove_Struct()
        {
            TestStruct struct1 = new TestStruct(0, 0);
            TestStruct struct2 = new TestStruct(1, 1);

            Node node1 = new Node(struct1);
            Node node2 = new Node(struct2);

            node1.Attach("x", node2, "y");

            Assert.Collection<IInPin>(node2.InPins, pin => Assert.True(pin.Member == "y", "Member is " + pin.Member + " but should be y"));
            Assert.Collection<IOutPin>(node1.OutPins, pin => Assert.True(pin.Member == "x", "Member is " + pin.Member + " but should be x"));

            node1.RemoveAllPins();
            node2.RemoveAllPins();

            Assert.True(node1.OutPins.Count() == 0, "OutPins count should be 0 but is " + node1.OutPins.Count());
            Assert.True(node2.InPins.Count() == 0, "InPins count should be 0 but is " + node2.OutPins.Count());
        }

        [Fact]
        public void TestPropagate_Object()
        {
            TestClass obj1 = new TestClass(0, 0);
            TestClass obj2 = new TestClass(1, 1);

            Node node1 = new Node(obj1);
            Node node2 = new Node(obj2);

            node1.Attach("x", node2, "y");

            node1.Propagate();

            Assert.True(obj2.y == obj1.x, "Node2.y is " + obj2.y + " but should be " + obj1.x + ".");
        }

        [Fact]
        public void TestPropagate_Struct()
        {
            TestStruct struct1 = new TestStruct(0, 0);
            TestStruct struct2 = new TestStruct(1, 1);

            Node node1 = new Node(struct1);
            Node node2 = new Node(struct2);

            node1.Attach("x", node2, "y");

            node1.Propagate();

            Assert.True(struct2.y == struct1.x, "Node2.y is " + struct2.y + " but should be " + struct1.x + ".");
        }
    }
}
