using System;

namespace Fusee.Engine
{
    // A class implementing an Interface
    public class AParamClass
    {
        protected int i;
        public void DoSomething() { i = 3; }
        public void DoSomethingElse() { i++; }
        public int GetI() { return i; }
    };

    public class MyClass : IMyInterface
    {
        #region Methods
        public int SimpleMethod(double d)
        {
            return (int)d;
        }

        public int StringMethod(String str)
        {
            return int.Parse(str);
        }

        public double StructMethod(MyParamStruct s)
        {
            return s.x + s.y + s.z + int.Parse(s.str);
        }

        public double BlittableMethod(MyBlittableStruct s)
        {
            return s.x + s.y + s.z;
        }

        public int ClassMethod(AParamClass c)
        {
            c.DoSomething();
            c.DoSomethingElse();
            return c.GetI();
        }

        public MyParamStruct ReturnStruct(int i)
        {
            MyParamStruct ret = new MyParamStruct();
            ret.x = ret.y = ret.z = i;
            ret.str = String.Format("i contains {0:d}", i);
            return ret;
        }

        public int InterfaceMethod(double d)
        {
            return (int)d * 6;
        }
        #endregion

        #region Delegate
        public MyDelegateType _delegateMethod;
        public void SetDelegate(MyDelegateType delegateMeth)
        {
            _delegateMethod = delegateMeth;
        }

        public int CallDelegate(double d)
        {
            return _delegateMethod(d) + 1;
        }
        #endregion

        #region Field
        protected int _i;
        public int I
        {
            get { return _i; }
            set { _i = value; }
        }

        #endregion
    }


    public class MyCaller
    {
        public int InvokeCall(IMyInterface mc, double d)
        {
            return mc.InterfaceMethod(d);
        }
    };

    // Declaration of an Interface
    public interface IMyInterface
    {
        int InterfaceMethod(double d);
    }

    // Declaration of a delegate type
    public delegate int MyDelegateType(double d);


    // Declaration of a parameter struct
    public struct MyParamStruct
    {
        public int x;
        public int y;
        public int z;
        public String str;
    };

    public struct MyBlittableStruct
    {
        public int x;
        public double y;
        public byte z;
    };


    // Cyclic dependency:
    public class CCC
    {
        public int Get() { return 333; }

        protected BBB bbb;
        public BBB GetBBB() { return bbb; }
    };

    public class BBB
    {
        public int Get() { return 222; }

        protected AAA aaa;
        public AAA GetAAA() { return aaa; }
    };


    public class AAA
    {
        public AAA() { }
        public AAA(int a) { a = 3; }
        public AAA(CCC cc) { ccc = cc; }
        public int Get() { return 111; }

        protected CCC ccc;
        public CCC GetCCC() { return ccc; }
    };

}
