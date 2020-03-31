using CommandLine;
using Fusee.Engine.Common;
using Fusee.Serialization;
using System;
using System.IO;

namespace Fusee.Tools.fuseeCmdLine
{
    [Verb("protoschema", HelpText = "Output the protobuf schema for the .fus file format.")]
    internal class ProtoSchema
    {
        [Option('o', "output", HelpText = "Path of .proto file to be written. \".proto\" extension will be added if not present.")]
        public string Output { get; set; }

        public int Run()
        {
            var schema = ProtoBuf.Serializer.GetProto<FusFile>();

            // Check and open output file
            if (!String.IsNullOrEmpty(Output))
            {
                if (Path.GetExtension(Output).ToLower() != ".proto")
                {
                    Output += ".proto";
                }
                try
                {
                    using (var output = new StreamWriter(File.Open(Output, System.IO.FileMode.Create)))
                    {
                        // Is added with Protobuf 2.4.0: output.WriteLine("syntax = \"proto2\";");
                        output.Write(schema);
                    }
                    Console.Error.WriteLine(
                        $"SUCCESS: Protobuf schema for .fus files written to '{Output}'");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"ERROR creating output file '{Output}':");
                    Console.Error.WriteLine(ex);
                    Environment.Exit((int)ErrorCode.OutputFile);
                }
            }
            else
            {
                Console.WriteLine(schema);
            }
            return 0;
        }
    }
}
