using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Test.Xirkit
{
    class HelperClasses
    {
        public class TestClass
        {
            public int x;
            public int y;

            public TestClass(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public struct TestStruct
        {
            public int x;
            public int y;

            public TestStruct(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
