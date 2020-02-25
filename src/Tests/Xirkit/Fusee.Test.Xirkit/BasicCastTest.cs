using System;
using Xunit;
using Fusee.Xirkit;

namespace Fusee.Test.Xirkit
{
    public class BasicCastTest
    {

        struct number()
        [Fact]
        public void CastsNode1ToNote2()
        {
            

            
            circuit.AddRoot(node1);
            circuit.AddNode(node2);
            node1.Attach("x",node2,"x");

            circuit.Execute();

            Assert.True(obj2 == obj1, "Node2.x = " + obj2 + " but it should be " + obj1 + ".");
        }

        
    }
}
