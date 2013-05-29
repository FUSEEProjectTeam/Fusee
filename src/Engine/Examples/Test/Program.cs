using System.Diagnostics;
using Fusee.Math;
using Fusee.Xirkit;
using System;


namespace Test
{
    //Classes which be linked together
    public class One
    {
        public float4x4  I { get; set; }
        public string S { get; set; }
    }

    public class Two
    {
        public float J; // { get; set; }
        public string S { get; set; }
    }
    //Class1: add A and B Result is R
    public class Calc : ICalculationPerformer
    {
        public int A { get; set; }
        public int B { get; set; }

        public int R { get; set; }

        public void PerformCalculation()
        {
            R = A + B;
        }
    }
    //End Class1

    //Class2: multiply C by D Result is R2
    class Calc2 : ICalculationPerformer
    {
        public int C { get; set; }
        public int D { get; set; }

        public int R2 { get; set; }

        public void PerformCalculation()
        {
            R2 = C * D;
        }
    }
    //End Class2

    //Class3: divide E by F Result is R3
    class Calc3 : ICalculationPerformer
    {
        public int E { get; set; }
        public int F { get; set; }

        public int R3 { get; set; }

        public void PerformCalculation()
        {
            R3 = E / F;
        }
    }
    //End Class2

    class Program
    {
        static void Main(string[] args)
        {




            // TODO: Set up a couple of Circuits of sample objects in code

            //Circuit with the new Objects
            
            One aOne = new One();
            Two  aTwo = new Two();
            //Calc  object3 = new Calc();
            //Calc2 object4 = new Calc2() ;

            Circuit circuit = new Circuit();
            Node nOne = new Node(aOne);
            Node nTwo = new Node(aTwo);
            //Node n12 = new Node(object3);
            //Node n13 = new Node(object4);

            nOne.Attach("I", nTwo, "J");

            circuit.AddRoot(nOne);
            circuit.AddNode(nTwo);


            aOne.I = new float4x4(13f, 10f,10f,10f,10f,10f,10f,10f,10f,10f,10f,10f,10f,10f,10f,10f);
            circuit.Execute();

            if (aTwo.J == 13 )
            {
                // success
                Debug.WriteLine("win");
            }
            else
            {
                // no success
            }

            ////n12.Attach("Output", n11, "Input");

            ////a.AddRoot(n10);
            ////a.AddNode(n10);
            ////a.AddNode(n11);
            ////a.AddNode(n12);
            //////a.AddNode(n13);
            ////a.AddRoot(n11);
            ////object4.C  = 2;
            ////object3.A  = 33;
            

            //Circuit c = new Circuit();
            //Node n1 = new Node(object1);
            //Node n2 = new Node(object2 );

            //Node calc = new Node(new Calc());
            //Node r = new Node(new Two());

            //n1.Attach("I", calc, "A");
            //n2.Attach("J", calc, "B");
            //calc.Attach("R", r, "I");

            //c.AddNode(n1);
            //c.AddNode(n2);
            //c.AddNode(calc);
            //c.AddNode(r);
            //c.AddRoot(n1);
            //c.AddRoot(n2);
            ////Circuit 1 end

            ////Circuit 2 begin
            //One Objekt1_1 = new One();
            //Two Objekt2_1 = new Two();
            //One result2 = new One();


            //Circuit d = new Circuit();
            //Node n3 = new Node(Objekt1_1);
            //Node n4 = new Node(Objekt2_1);

            //Node calc2 = new Node(new Calc2());
            //Node r2 = new Node(result2);

            //n3.Attach("I", calc2, "C");
            //n4.Attach("J", calc2, "D");
            //calc2.Attach("R2", r2, "I");

            //d.AddNode(n3);
            //d.AddNode(n4);
            //d.AddNode(calc2);
            //d.AddNode(r2);
            //d.AddRoot(n3);
            //d.AddRoot(n4);
            ////Circuit 2 end

            ////Circuit 3 begin
            //One Objekt1_2 = new One();
            //Two Objekt2_2 = new Two();
            //One result3 = new One();


            //Circuit e = new Circuit();
            //Node n5 = new Node(Objekt1_2);
            //Node n6 = new Node(Objekt2_2);

            //Node calc3 = new Node(new Calc3());
            //Node r3 = new Node(result3);

            //n5.Attach("I", calc3, "E");
            //n6.Attach("J", calc3, "F");
            //calc3.Attach("R3", r3, "I");

            //e.AddNode(n5);
            //e.AddNode(n6);
            //e.AddNode(calc3);
            //e.AddNode(r3);
            //e.AddRoot(n5);
            //e.AddRoot(n6);
            ////Circuit 3 end

            //// TODO: Test that the circuit works (Set values on the root properties and call Execute on the circuit).

            ////Data Circuit 1
            //object1.I = 6;
            //object2.J = 2;
            //c.Execute();

            ////Data Circuit 2
            //Objekt1_1.I = 6;
            //Objekt2_1.J = 2;
            //d.Execute();

            ////Data Circuit 3
            //Objekt1_2.I = 27;
            //Objekt2_2.J = 2;
            //e.Execute();

            ////   Debug.Assert(result.I == 5);

            //// TODO: Serialize and Deserialize the Circuit to XML

            ////Traverse.traverseCircuitFromXML(Traverse.DeserializeFromXML());
            ////Traverse.traverseCircuitToXml(a);
            ////Traverse.SerializeToXML(Traverse.traverseCircuitToXml(a));

            ////traverseCircuit(DeserializeFromXML());
            ////traverseCircuitToXml(c);
            ////SerializeToXML(traverseCircuitToXml(c));

            //// TODO: Traverse the circuit and print its contents to the console

            //for (int i = 1; i <= 5; i++)
            //{
            //    //Print Curcuit 1
            //    Console.WriteLine("Rechnung fuer Circuit 1:\r\n" + object1.I + "+" + object2.J);
            //    Console.WriteLine("Ergebnis von " + result3 + " = " + result3.I);
            //    Console.WriteLine("Circuitausgabe: " + c);

            //    Console.WriteLine();

            //    //Print Curcuit 2
            //    Console.WriteLine("Rechnung fuer Circuit 2:\r\n" + Objekt1_1.I + "*" + Objekt2_1.J);
            //    Console.WriteLine("Ergebnis von " + result2 + " = " + result2.I);
            //    Console.WriteLine("Circuitausgabe: " + d);

            //    Console.WriteLine();

            //    //Print Curcuit 3
            //    Console.WriteLine("Rechnung fuer Circuit 3:\r\n" + Objekt1_2.I + "/" + Objekt2_2.J);
            //    Console.WriteLine("Ergebnis von " + result3 + " = " + result3.I);
            //    Console.WriteLine("Circuitausgabe: " + e);

            //    break;
            //}

            System.Threading.Thread.Sleep(50000);

        }
    }
}