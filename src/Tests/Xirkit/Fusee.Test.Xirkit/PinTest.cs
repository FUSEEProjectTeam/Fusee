﻿using System;
using Xunit;
using Fusee.Xirkit;
using static Fusee.Test.Xirkit.HelperClasses;

namespace Fusee.Test.Xirkit
{
    public class PinTest
    {
        [Fact]
        public void TestMemberAndN()
        {
            TestClass obj = new TestClass(1, 1);
            Node node = new Node(obj);
            Pin pin = new Pin(node, "x");

            Assert.Equal("x", pin.Member);
            Assert.Equal(pin.N, node);
        }

        [Fact]
        public void InPin_TestSetValue()
        {
            TestClass obj = new TestClass(1, 1);
            Node node = new Node(obj);

        }
    }
}
