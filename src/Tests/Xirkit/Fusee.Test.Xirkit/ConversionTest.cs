using System;
using Xunit;
using System.Collections.Generic;
using Fusee.Xirkit;
using Fusee.Math.Core;
using static Fusee.Test.Xirkit.HelperClasses;

namespace Fusee.Test.Xirkit
{
    public class ConversionTest
    {
        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromInt(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.i = i;

            root.Attach("i", node, "i");
            root.Attach("i", node, "f");
            root.Attach("i", node, "d");
            root.Attach("i", node, "isTrue");
            root.Attach("i", node, "text");
            root.Attach("i", node, "d2");
            root.Attach("i", node, "d3");
            root.Attach("i", node, "d4");
            root.Attach("i", node, "d4x4");
            root.Attach("i", node, "f2");
            root.Attach("i", node, "f3");
            root.Attach("i", node, "f4");
            root.Attach("i", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromFloat(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.f = f;

            root.Attach("f", node, "i");
            root.Attach("f", node, "f");
            root.Attach("f", node, "d");
            root.Attach("f", node, "isTrue");
            root.Attach("f", node, "text");
            root.Attach("f", node, "d2");
            root.Attach("f", node, "d3");
            root.Attach("f", node, "d4");
            root.Attach("f", node, "d4x4");
            root.Attach("f", node, "f2");
            root.Attach("f", node, "f3");
            root.Attach("f", node, "f4");
            root.Attach("f", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromDouble(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.d = d;

            root.Attach("d", node, "i");
            root.Attach("d", node, "f");
            root.Attach("d", node, "d");
            root.Attach("d", node, "isTrue");
            root.Attach("d", node, "text");
            root.Attach("d", node, "d2");
            root.Attach("d", node, "d3");
            root.Attach("d", node, "d4");
            root.Attach("d", node, "d4x4");
            root.Attach("d", node, "f2");
            root.Attach("d", node, "f3");
            root.Attach("d", node, "f4");
            root.Attach("d", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromBool(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = isTrue.ToString();

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.isTrue = isTrue;

            root.Attach("isTrue", node, "i");
            root.Attach("isTrue", node, "f");
            root.Attach("isTrue", node, "d");
            root.Attach("isTrue", node, "isTrue");
            root.Attach("isTrue", node, "text");
            root.Attach("isTrue", node, "d2");
            root.Attach("isTrue", node, "d3");
            root.Attach("isTrue", node, "d4");
            root.Attach("isTrue", node, "d4x4");
            root.Attach("isTrue", node, "f2");
            root.Attach("isTrue", node, "f3");
            root.Attach("isTrue", node, "f4");
            root.Attach("isTrue", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromString(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = text;

            root.Attach("text", node, "i");
            root.Attach("text", node, "f");
            root.Attach("text", node, "d");
            root.Attach("text", node, "text");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
        }

        [Fact]
        public void ConvertFromStringToBool()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "true";
            root.Attach("text", node, "isTrue");
            circuit.Execute();

            Assert.True(expected.isTrue, "Error when parsing to bool: Should be true but is " + expected.isTrue + ".");

            source.text = "false";
            circuit.Execute();

            Assert.False(expected.isTrue, "Error when parsing to bool: Should be false but is " + expected.isTrue + ".");
        }

        [Fact]
        public void ConvertFromStringToDouble2()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1)";
            root.Attach("text", node, "d2");
            circuit.Execute();

            Assert.True(expected.d2 == new double2(1, 1), "Error when parsing to double2: Should be " + source.text + " but is " + expected.d2 + ".");
        }

        [Fact]
        public void ConvertFromStringToDouble3()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1)";
            root.Attach("text", node, "d3");
            circuit.Execute();

            Assert.True(expected.d3 == new double3(1, 1, 1), "Error when parsing to double3: Should be " + source.text + " but is " + expected.d3 + ".");
        }

        [Fact]
        public void ConvertFromStringToDouble4()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1, 1)";
            root.Attach("text", node, "d4");
            circuit.Execute();

            Assert.True(expected.d4 == new double4(1, 1, 1, 1), "Error when parsing to double4: Should be " + source.text + " but is " + expected.d4 + ".");
        }

        [Fact]
        public void ConvertFromStringToDouble4x4()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)";
            root.Attach("text", node, "d4x4");
            circuit.Execute();

            Assert.True(expected.d4x4 == new double4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), "Error when parsing to double4x4: Should be " + source.text + " but is " + expected.d4x4 + ".");
        }

        [Fact]
        public void ConvertFromStringToFloat2()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1)";
            root.Attach("text", node, "f2");
            circuit.Execute();

            Assert.True(expected.f2 == new float2(1, 1), "Error when parsing to float2: Should be " + source.text + " but is " + expected.f2 + ".");
        }

        [Fact]
        public void ConvertFromStringToFloat3()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1)";
            root.Attach("text", node, "f3");
            circuit.Execute();

            Assert.True(expected.f3 == new float3(1, 1, 1), "Error when parsing to float3: Should be " + source.text + " but is " + expected.f3 + ".");
        }

        [Fact]
        public void ConvertFromStringToFloat4()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1, 1)";
            root.Attach("text", node, "f4");
            circuit.Execute();

            Assert.True(expected.f4 == new float4(1, 1, 1, 1), "Error when parsing to float4: Should be " + source.text + " but is " + expected.f4 + ".");
        }

        [Fact]
        public void ConvertFromStringToFloat4x4()
        {
            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.text = "(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)";
            root.Attach("text", node, "f4x4");
            circuit.Execute();

            Assert.True(expected.f4x4 == new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), "Error when parsing to float4x4: Should be " + source.text + " but is " + expected.f4x4 + ".");
        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromDouble2(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = d2.ToString();

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.d2 = d2;

            root.Attach("d2", node, "i");
            root.Attach("d2", node, "f");
            root.Attach("d2", node, "d");
            root.Attach("d2", node, "isTrue");
            root.Attach("d2", node, "text");
            root.Attach("d2", node, "d2");
            root.Attach("d2", node, "d3");
            root.Attach("d2", node, "d4");
            root.Attach("d2", node, "d4x4");
            root.Attach("d2", node, "f2");
            root.Attach("d2", node, "f3");
            root.Attach("d2", node, "f4");
            root.Attach("d2", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromDouble3(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = d3.ToString();

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.d3 = d3;

            root.Attach("d3", node, "i");
            root.Attach("d3", node, "f");
            root.Attach("d3", node, "d");
            root.Attach("d3", node, "isTrue");
            root.Attach("d3", node, "text");
            root.Attach("d3", node, "d2");
            root.Attach("d3", node, "d3");
            root.Attach("d3", node, "d4");
            root.Attach("d3", node, "d4x4");
            root.Attach("d3", node, "f2");
            root.Attach("d3", node, "f3");
            root.Attach("d3", node, "f4");
            root.Attach("d3", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromDouble4(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = d4.ToString();
            d4x4 = new double4x4(d4, new double4(0, 0, 0, 0), new double4(0, 0, 0, 0), new double4(0, 0, 0, 0));
            f4x4 = new float4x4(f4, new float4(0, 0, 0, 0), new float4(0, 0, 0, 0), new float4(0, 0, 0, 0));

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.d4 = d4;

            root.Attach("d4", node, "i");
            root.Attach("d4", node, "f");
            root.Attach("d4", node, "d");
            root.Attach("d4", node, "isTrue");
            root.Attach("d4", node, "text");
            root.Attach("d4", node, "d2");
            root.Attach("d4", node, "d3");
            root.Attach("d4", node, "d4");
            root.Attach("d4", node, "d4x4");
            root.Attach("d4", node, "f2");
            root.Attach("d4", node, "f3");
            root.Attach("d4", node, "f4");
            root.Attach("d4", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromDouble4x4(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = d4x4.ToString();
            d4 = d4x4.Row0;
            f4 = f4x4.Row0;

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.d4x4 = d4x4;

            root.Attach("d4x4", node, "i");
            root.Attach("d4x4", node, "f");
            root.Attach("d4x4", node, "d");
            root.Attach("d4x4", node, "isTrue");
            root.Attach("d4x4", node, "text");
            root.Attach("d4x4", node, "d2");
            root.Attach("d4x4", node, "d3");
            root.Attach("d4x4", node, "d4");
            root.Attach("d4x4", node, "d4x4");
            root.Attach("d4x4", node, "f2");
            root.Attach("d4x4", node, "f3");
            root.Attach("d4x4", node, "f4");
            root.Attach("d4x4", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromFloat2(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = f2.ToString();

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.f2 = f2;

            root.Attach("f2", node, "i");
            root.Attach("f2", node, "f");
            root.Attach("f2", node, "d");
            root.Attach("f2", node, "isTrue");
            root.Attach("f2", node, "text");
            root.Attach("f2", node, "d2");
            root.Attach("f2", node, "d3");
            root.Attach("f2", node, "d4");
            root.Attach("f2", node, "d4x4");
            root.Attach("f2", node, "f2");
            root.Attach("f2", node, "f3");
            root.Attach("f2", node, "f4");
            root.Attach("f2", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromFloat3(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = f3.ToString();

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.f3 = f3;

            root.Attach("f3", node, "i");
            root.Attach("f3", node, "f");
            root.Attach("f3", node, "d");
            root.Attach("f3", node, "isTrue");
            root.Attach("f3", node, "text");
            root.Attach("f3", node, "d2");
            root.Attach("f3", node, "d3");
            root.Attach("f3", node, "d4");
            root.Attach("f3", node, "d4x4");
            root.Attach("f3", node, "f2");
            root.Attach("f3", node, "f3");
            root.Attach("f3", node, "f4");
            root.Attach("f3", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromFloat4(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = f4.ToString();
            d4x4 = new double4x4(d4, new double4(0, 0, 0, 0), new double4(0, 0, 0, 0), new double4(0, 0, 0, 0));
            f4x4 = new float4x4(f4, new float4(0, 0, 0, 0), new float4(0, 0, 0, 0), new float4(0, 0, 0, 0));

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.f4 = f4;

            root.Attach("f4", node, "i");
            root.Attach("f4", node, "f");
            root.Attach("f4", node, "d");
            root.Attach("f4", node, "isTrue");
            root.Attach("f4", node, "text");
            root.Attach("f4", node, "d2");
            root.Attach("f4", node, "d3");
            root.Attach("f4", node, "d4");
            root.Attach("f4", node, "d4x4");
            root.Attach("f4", node, "f2");
            root.Attach("f4", node, "f3");
            root.Attach("f4", node, "f4");
            root.Attach("f4", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }

        [Theory]
        [MemberData(nameof(GetConversions))]
        public void ConvertFromFloat4x4(int i, float f, double d, bool isTrue, string text, double2 d2, double3 d3, double4 d4, double4x4 d4x4, float2 f2, float3 f3, float4 f4, float4x4 f4x4)
        {
            text = f4x4.ToString();
            d4 = d4x4.Row0;
            f4 = f4x4.Row0;

            ConverterClass source = new ConverterClass();
            ConverterClass expected = new ConverterClass();
            Node root = new Node(source);
            Node node = new Node(expected);
            Circuit circuit = new Circuit();

            circuit.AddNode(root);
            circuit.AddNode(node);
            circuit.AddRoot(root);

            source.f4x4 = f4x4;

            root.Attach("f4x4", node, "i");
            root.Attach("f4x4", node, "f");
            root.Attach("f4x4", node, "d");
            root.Attach("f4x4", node, "isTrue");
            root.Attach("f4x4", node, "text");
            root.Attach("f4x4", node, "d2");
            root.Attach("f4x4", node, "d3");
            root.Attach("f4x4", node, "d4");
            root.Attach("f4x4", node, "d4x4");
            root.Attach("f4x4", node, "f2");
            root.Attach("f4x4", node, "f3");
            root.Attach("f4x4", node, "f4");
            root.Attach("f4x4", node, "f4x4");

            circuit.Execute();

            Assert.True(expected.i == i, "Error when parsing to int: Should be " + i + " but is " + expected.i + ".");
            Assert.True(expected.f == f, "Error when parsing to float: Should be " + f + " but is " + expected.f + ".");
            Assert.True(expected.d == d, "Error when parsing to double: Should be " + d + " but is " + expected.d + ".");
            Assert.True(expected.isTrue == isTrue, "Error when parsing to bool: Should be " + isTrue + " but is " + expected.isTrue + ".");
            Assert.True(expected.text == text, "Error when parsing to string: Should be " + text + " but is " + expected.text + ".");
            Assert.True(expected.d2 == d2, "Error when parsing to double2: Should be " + d2 + " but is " + expected.d2 + ".");
            Assert.True(expected.d3 == d3, "Error when parsing to double3: Should be " + d3 + " but is " + expected.d3 + ".");
            Assert.True(expected.d4 == d4, "Error when parsing to double4: Should be " + d4 + " but is " + expected.d4 + ".");
            Assert.True(expected.d4x4 == d4x4, "Error when parsing to double4x4: Should be " + d4x4 + " but is " + expected.d4x4 + ".");
            Assert.True(expected.f2 == f2, "Error when parsing to float2: Should be " + f2 + " but is " + expected.f2 + ".");
            Assert.True(expected.f3 == f3, "Error when parsing to float3: Should be " + f3 + " but is " + expected.f3 + ".");
            Assert.True(expected.f4 == f4, "Error when parsing to float4: Should be " + f4 + " but is " + expected.f4 + ".");
            Assert.True(expected.f4x4 == f4x4, "Error when parsing to float4x4: Should be " + f4x4 + " but is " + expected.f4x4 + ".");

        }


        public static IEnumerable<object[]> GetConversions()
        {
            yield return new object[]
            {
                1,
                1.0f,
                1.0d,
                true,
                "1",
                new double2(1.0d, 0),
                new double3(1.0d, 0, 0),
                new double4(1.0d, 0, 0, 1.0d),
                new double4x4(1.0d, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                new float2(1.0f, 0),
                new float3(1.0f, 0, 0),
                new float4(1.0f, 0, 0, 1.0f),
                new float4x4(1.0f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            };
            yield return new object[]
            {
                0,
                0.0f,
                0.0d,
                false,
                "0",
                new double2(0, 0),
                new double3(0, 0, 0),
                new double4(0, 0, 0, 1),
                new double4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                new float2(0, 0),
                new float3(0, 0, 0),
                new float4(0, 0, 0, 1),
                new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            };
        }
    }
}