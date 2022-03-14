using Fusee.Xirkit;
using System.Collections.Generic;
using Xunit;
using static Fusee.Tests.Xirkit.HelperClasses;

namespace Fusee.Tests.Xirkit
{
    public class PinTest
    {
        [Fact]
        public void TestMemberAndN()
        {
            SimpleClass obj = new(1, 1);
            Node node = new(obj);
            Pin pin = new(node, "x");

            Assert.Equal("x", pin.Member);
            Assert.Equal(pin.N, node);
        }

        [Fact]
        public void InPin_TestSetValue_GetPinType()
        {
            SimpleClass obj1 = new(1, 1);
            SimpleClass obj2 = new(0, 0);
            Node node1 = new(obj1);
            Node node2 = new(obj2);

            node1.Attach("x", node2, "x");

            IEnumerator<IInPin> pins = node2.InPins.GetEnumerator();
            pins.MoveNext();

            InPin<int> pin = (InPin<int>)pins.Current;

            Assert.Equal(typeof(int), pin.GetPinType());

            pin.SetValue(1);

            Assert.True(obj2.x == 1, "obj2.x is " + obj2.x + " but should be 1 after changing it with SetValue.");
        }
    }
}