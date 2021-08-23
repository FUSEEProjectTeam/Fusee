using Fusee.Math.Core;
using Fusee.Xirkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Tests.Xirkit
{
    class HelperClasses
    {
        public class SimpleClass
        {
            public int x;
            public int y;

            public SimpleClass(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class NestedClass
        {
            public SimpleStruct str;

            public NestedClass(SimpleStruct str)
            {
                this.str = str;
            }
        }

        public struct SimpleStruct
        {
            public int x;
            public int y;

            public SimpleStruct(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class CalculationClass : ICalculationPerformer
        {
            public int x;
            public int y;

            public CalculationClass(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public void PerformCalculation()
            {
                this.x += x;
            }
        }

        public class ConverterClass
        {
            public int i;
            public float f;
            public double d;
            public bool isTrue;
            public string text;
            public double2 d2;
            public double3 d3;
            public double4 d4;
            public double4x4 d4x4;
            public float2 f2;
            public float3 f3;
            public float4 f4;
            public float4x4 f4x4;

            public ConverterClass() { }
        }
    }
}