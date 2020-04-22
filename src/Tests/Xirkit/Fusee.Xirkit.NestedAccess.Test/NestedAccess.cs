using Fusee.Xirkit;
using Xunit;

namespace Fusee.Test.Xirkit.NestedAccess
{
    public class CNodeOb
    {
        public int Num;
        public string Txt;

        public SNestedOuter Outer;

        public CNestedInner _propBacker;
        public CNestedInner Prop
        {
            get
            {
                return _propBacker;
            }
            set
            {
                _propBacker = value;
                NotifySet();
            }
        }

        public bool PropIsSet = false;
        private void NotifySet()
        {
            PropIsSet = true;
        }
    }



    public struct SNestedOuter
    {
        public SNestedInner Inner;
    }

    public struct SNestedInner
    {
        public string Text;
        public int Number;
    }

    public class CNestedInner
    {
        public string Text;
        public int Number;
    }



    public class Nicetries
    {
        [Fact]
        public void NestedProperties()
        {
            var src = new CNodeOb();
            var srcNode = new Node(src);

            var dst = new CNodeOb();
            var dstNode = new Node(dst);

            var circuit = new Circuit();
            circuit.AddRoot(srcNode);
            circuit.AddNode(dstNode);

            // Nested -> Simple field
            src.Outer.Inner.Text = "Some Text";
            srcNode.Attach("Outer.Inner.Text", dstNode, "Txt");

            // Simple Field -> Nested
            src.Num = 42;
            srcNode.Attach("Num", dstNode, "Outer.Inner.Number");

            circuit.Execute();

            Assert.Equal("Some Text", dst.Txt);
            Assert.Equal(42, dst.Outer.Inner.Number);
        }

        [Fact]
        public void NotifyNestedProperty()
        {
            var ob = new CNodeOb();

            ob.Prop = new CNestedInner { Number = 44, Text = "Howdy" };

            ob.Prop.Number = 33;



        }
    }
}
