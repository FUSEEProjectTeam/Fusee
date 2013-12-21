using System;
using ProtoBuf.Meta;
using ProtoBufTest;

namespace MySerializer
{
    public class Program
    {
        Program()
        {
            var model = TypeModel.Create();

            
            model.Add(typeof(Person), true);
            model.Compile("MySerializer", "MySerializer.dll");
        }

      
        static void Main(string[] args)
        {
            var hans = new Program();
        }
    }
}
