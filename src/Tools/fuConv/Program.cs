using System;
using System.IO;
using Assimp;
using CommandLine;
using CommandLine.Text;
using Fusee.Serialization;

namespace Fusee.Tools.fuConv
{
    enum ErrorCode : int
    {
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
    }

    class Program
    {
        static AssimpContext Assimp => _assimpCtx ?? (_assimpCtx = new AssimpContext());
        private static AssimpContext _assimpCtx;

        [Verb("scene", HelpText = "Convert 3D scene input into .fus.")]
        public class SceneOptions
        {
            [Value(0, HelpText = "Input scene file in a recognized format. Use the 'inputsceneformats' command to retrieve a list of supported formats.", MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Path of .fus file to be written. \".fus\" extension will be added if not present.")]
            public string Output { get; set; }

            [Option('f', "format", HelpText = "Input file format overriding the file extension (if any). For example 'obj' for a Wavefront .obj file.")]
            public string Format { get; set; }

        }

        [Verb("inputsceneformats", HelpText = "List the supported input scene formats.")]

        public class InputSceneFormats
        {
        }

        [Verb("protoschema", HelpText = "Output the protobuf schema for the .fus file format.")]
        public class ProtoSchema
        {
            [Option('o', "output", HelpText = "Path of .proto file to be written. \".proto\" extension will be added if not present.")]
            public string Output { get; set; }

        }


        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<SceneOptions, InputSceneFormats, ProtoSchema>(args)
                
                // Called with the SCENE verb
                .WithParsed<SceneOptions>(opts =>
                {
                    Stream input = null, output=null;

                    // Check and open input file
                    string inputFormat = Path.GetExtension(opts.Input).ToLower();
                    if (string.IsNullOrEmpty(inputFormat))
                    {
                        if (string.IsNullOrEmpty(opts.Format))
                        {
                            Console.Error.WriteLine($"ERROR: No input format specified.");
                            Environment.Exit((int)ErrorCode.InputFormat);
                        }
                        inputFormat = opts.Format.ToLower();
                        if (inputFormat[0] != '.')
                            inputFormat = inputFormat.Insert(0, ".");
                    }
                    if (!Assimp.IsImportFormatSupported(inputFormat))
                    {
                        Console.Error.WriteLine($"ERROR: Unsupported input format {inputFormat}.");
                        Environment.Exit((int)ErrorCode.InputFormat);
                    }
                    try
                    {
                        input = File.Open(opts.Input, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR opening input file '{opts.Input}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.InputFile);
                    }

                    // Check and open output file
                    if (string.IsNullOrEmpty(opts.Output))
                    {
                        string inp = Path.GetFullPath(opts.Input);
                        opts.Output = Path.Combine(
                            Path.GetPathRoot(inp),
                            Path.GetDirectoryName(inp), 
                            Path.GetFileNameWithoutExtension(inp) + ".fus");

                    }
                    if (Path.GetExtension(opts.Output).ToLower() != ".fus")
                    {
                        opts.Output += ".fus";
                    }
                    try
                    {
                        output = File.Open(opts.Output, FileMode.Create);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR creating output file '{opts.Output}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int)ErrorCode.OutputFile);
                    }

                    Console.WriteLine($"Converting from {opts.Input} to {Path.GetFileName(opts.Output)}");
                    var assimpScene = Assimp.ImportFileFromStream(input,
                                    PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | 
                                    PostProcessSteps.JoinIdenticalVertices, inputFormat);

                    SceneContainer fuseeScene = Assimp2Fusee.FuseefyScene(assimpScene);

                    var ser = new Serializer();
                    ser.Serialize(output, fuseeScene);
                    output.Flush();
                    output.Close();
                })

                // Called with the INPUTSCENEFORMATS verb
                .WithParsed<InputSceneFormats>(opts =>
                {
                    Console.WriteLine("Supported input formats for scene files:");
                    foreach (var inFormat in Assimp.GetSupportedImportFormats())
                    {
                        Console.Write(inFormat + "; ");
                    }
                    Console.WriteLine();
                })

                // Called with the PROTOSCHEMA verb
                .WithParsed<ProtoSchema>(opts =>
                {
                    var schema = ProtoBuf.Serializer.GetProto<SceneContainer>();

                    // Check and open output file
                    if (!string.IsNullOrEmpty(opts.Output))
                    {
                        if (Path.GetExtension(opts.Output).ToLower() != ".proto")
                        {
                            opts.Output += ".proto";
                        }
                        try
                        {
                            using (var output = new StreamWriter(File.Open(opts.Output, FileMode.Create)))
                                output.Write(schema);
                            Console.Error.WriteLine($"Protobuf schema for .fus files successfully written to '{opts.Output}'");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"ERROR creating output file '{opts.Output}':");
                            Console.Error.WriteLine(ex);
                            Environment.Exit((int) ErrorCode.OutputFile);
                        }
                    }
                    else
                    {
                        Console.WriteLine(schema);
                    }

                })

                // ERROR on the command line
                .WithNotParsed(errs =>
                {
                    /*foreach (var error in errs)
                    {
                        Console.Error.WriteLine(error);
                    }
                    */
                    Environment.Exit((int)ErrorCode.CommandLineSyntax);
                });
         }


        /*
         var fileName = Environment.CurrentDirectory + "\\" + filename;
         var assimpImporter = new AssimpContext();
         var scene = assimpImporter.ImportFile(fileName,
                         PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs |
                         PostProcessSteps.JoinIdenticalVertices);

         InitMaterials(scene);

         // for every mesh
         var allMeshesInScene = scene.Meshes;
         foreach (var mesh in allMeshesInScene)
         {
             // init and add to vertices
             InitMesh(mesh);

         }
         */



        /*
        private void InitMaterials(Scene scene)
        {
            for (var i = 0; i < scene.MaterialCount; i++)
            {
                var material = scene.Materials[i];

                if (material.GetMaterialTextureCount(TextureType.Diffuse) > 0)
                {
                    TextureSlot foundTexture;
                    if (material.GetMaterialTexture(TextureType.Diffuse, 0, out foundTexture))
                    {
                        _textures.Add(new Texture(TextureTarget.Texture2D, foundTexture.FilePath));
                        if (!_textures[i].Load())
                        {
                            Console.WriteLine("Error Loading texture!");
                        }
                    }
                }
                else
                {
                    _textures.Add(new Texture(TextureTarget.Texture2D, "white.png"));
                    _textures[i].Load();
                }
            }
        }

        private void InitMesh(Assimp.Mesh mesh)
        {
            _materialTextureIndex.Add(mesh.MaterialIndex);

            var vertices = new Vertex[mesh.Vertices.Count];
            var indices = new int[mesh.FaceCount * 3];

            var meshVertices = mesh.Vertices;
            var normals = mesh.Normals;
            var texCords = mesh.TextureCoordinateChannels;
            var faces = mesh.Faces;

            for (var i = 0; i < meshVertices.Count; i++)
            {
                var vertex = new Vector3(meshVertices[i].X, meshVertices[i].Y, meshVertices[i].Z);
                var normal = new Vector3(normals[i].X, normals[i].Y, normals[i].Z);
                var texCord = new Vector2(texCords[0][i].X, texCords[0][i].Y);

                var compiledVertex = new Vertex
                {
                    Vertices = vertex,
                    Normal = normal,
                    Uv = texCord
                };

                // Add vertex to list
                vertices[i] = compiledVertex;
            }

            var count = 0;

            foreach (var face in faces)
            {
                indices[count] = face.Indices[0];
                indices[++count] = face.Indices[1];
                indices[++count] = face.Indices[2];
                ++count;
            }


            // add all to tuple:
            _completeScene.Add(new Tuple<Vertex[], int[]>(vertices, indices));

            // init buffer
            InitGlBuffer(vertices, indices);
        }
        */

    }
}
