using System;
using Xunit;
using System.Collections.Generic;
using Fusee.Xirkit;
using static Fusee.Test.Xirkit.HelperClasses;

namespace Fusee.Test.Xirkit
{
    public class PinTest
    {
        [Fact]
        public void TestMemberAndN()
        {
            SimpleClass obj = new SimpleClass(1, 1);
            Node node = new Node(obj);
            Pin pin = new Pin(node, "x");

            Assert.Equal("x", pin.Member);
            Assert.Equal(pin.N, node);
        }

        [Fact]
        public void InPin_TestSetValue_GetType()
        {
            SimpleClass obj1 = new SimpleClass(1, 1);
            SimpleClass obj2 = new SimpleClass(0, 0);
            Node node1 = new Node(obj1);
            Node node2 = new Node(obj2);

            node1.Attach("x", node2, "x");

            IEnumerator<IInPin> pins = node2.InPins.GetEnumerator();
            pins.MoveNext();

            InPin<int> pin = (InPin<int>)pins.Current;

            Assert.Equal(typeof(int), pin.GetPinType());

            pin.SetValue(1);

            Assert.True(obj2.x == 1, "obj2.x is " + obj2.x + " but should be 1 after changing it with SetValue.");
        }

        //TODO: Test OutPin Attach, Detach, GetValue, Propagate
    }
}
