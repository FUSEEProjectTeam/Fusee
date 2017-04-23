using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Assimp;
using CommandLine;
using Fusee.Serialization;
using System.Net.NetworkInformation;
using System.Net.Sockets;


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
        private static FuseeHttpServer _httpServer;

        [Verb("scene", HelpText = "Convert 3D scene input into .fus.")]
        public class SceneOptions
        {
            [Value(0,
                HelpText =
                    "Input scene file in a recognized format. Use the 'inputsceneformats' command to retrieve a list of supported formats.",
                MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output",
                HelpText = "Path of .fus file to be written. \".fus\" extension will be added if not present.")]
            public string Output { get; set; }

            [Option('f', "format",
                HelpText =
                    "Input file format overriding the file extension (if any). For example 'obj' for a Wavefront .obj file."
            )]
            public string Format { get; set; }
        }

        [Verb("inputsceneformats", HelpText = "List the supported input scene formats.")]
        public class InputSceneFormats
        {
        }

        [Verb("protoschema", HelpText = "Output the protobuf schema for the .fus file format.")]
        public class ProtoSchema
        {
            [Option('o', "output",
                HelpText = "Path of .proto file to be written. \".proto\" extension will be added if not present.")]
            public string Output { get; set; }
        }

        [Verb("web", HelpText = "Use an existing .fus-file to start a webserver.")]
        public class WebViewer
        {
            [Value(0, HelpText = "Input .fus-file.", MetaName = "Input", Required = true)]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Target Directoy")]
            public string Output { get; set; }

            [Option('l', "list", HelpText = "List of paths to texture files")]
            public string List { get; set; }
        }


        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<SceneOptions, InputSceneFormats, ProtoSchema, WebViewer>(args)

                // Called with the SCENE verb
                .WithParsed<SceneOptions>(opts =>
                {
                    Stream input = null, output = null;

                    // Check and open input file
                    string inputFormat = Path.GetExtension(opts.Input).ToLower();
                    if (String.IsNullOrEmpty(inputFormat))
                    {
                        if (String.IsNullOrEmpty(opts.Format))
                        {
                            Console.Error.WriteLine($"ERROR: No input format specified.");
                            Environment.Exit((int) ErrorCode.InputFormat);
                        }
                        inputFormat = opts.Format.ToLower();
                        if (inputFormat[0] != '.')
                            inputFormat = inputFormat.Insert(0, ".");
                    }
                    if (!Assimp.IsImportFormatSupported(inputFormat))
                    {
                        Console.Error.WriteLine($"ERROR: Unsupported input format {inputFormat}.");
                        Environment.Exit((int) ErrorCode.InputFormat);
                    }
                    try
                    {
                        input = File.Open(opts.Input, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR opening input file '{opts.Input}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int) ErrorCode.InputFile);
                    }

                    // Check and open output file
                    if (String.IsNullOrEmpty(opts.Output))
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
                        Environment.Exit((int) ErrorCode.OutputFile);
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
                    if (!String.IsNullOrEmpty(opts.Output))
                    {
                        if (Path.GetExtension(opts.Output).ToLower() != ".proto")
                        {
                            opts.Output += ".proto";
                        }
                        try
                        {
                            using (var output = new StreamWriter(File.Open(opts.Output, FileMode.Create)))
                                output.Write(schema);
                            Console.Error.WriteLine(
                                $"Protobuf schema for .fus files successfully written to '{opts.Output}'");
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

                // Called with the WEB verb
                .WithParsed<WebViewer>(opts =>
                {
                    List<string> textureFiles = new List<string>();
                    try
                    {
                        // Get list of paths to texturefiles
                        string fileList = opts.List;
                        Console.WriteLine($"FILELIST: {opts.List}");
                        textureFiles = fileList.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        Console.WriteLine($"FILES: {textureFiles}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"No texture filepaths set");
                    }

                    string htmlFileDir = opts.Output;
                    string thisPath = Assembly.GetExecutingAssembly().Location;
                    thisPath = thisPath.Remove(thisPath.LastIndexOf(Path.DirectorySeparatorChar));
                    string fuseePlayerDir = Path.Combine(thisPath, "Viewer");
                    Stream input = null, output = null;
                    string sceneFileDir = Path.Combine(htmlFileDir, "Assets");
                    if (File.Exists(sceneFileDir))
                    {
                        File.Delete(sceneFileDir);
                    }
                    string sceneFilePath = Path.Combine(sceneFileDir, "Model.fus");
                    string origHtmlFilePath = Path.Combine(htmlFileDir, "SceneViewer.html");
                    if (File.Exists(origHtmlFilePath))
                        File.Delete(origHtmlFilePath);

                    //Copy
                    DirCopy.DirectoryCopy(fuseePlayerDir, htmlFileDir, true, true);


                    // Check and open input file
                    string inputFormat = Path.GetExtension(opts.Input).ToLower();
                    if (!String.Equals(inputFormat, ".fus"))
                    {
                        Console.Error.WriteLine($"Input file needs to be a .fus file'{opts.Input}':");
                        Environment.Exit((int) ErrorCode.InputFile);
                    }

                    try
                    {
                        input = File.Open(opts.Input, FileMode.Open);
                        input.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR opening input file '{opts.Input}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int) ErrorCode.InputFile);
                    }

                    // Check and open output file
                    if (String.IsNullOrEmpty(opts.Output))
                    {
                        Console.Error.WriteLine($"You need to specify an outputpath");
                        Environment.Exit((int) ErrorCode.OutputFile);
                    }
                    try
                    {
                        if (File.Exists(sceneFilePath))
                            File.Delete(sceneFilePath);
                        File.Move(opts.Input, sceneFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR creating output file '{sceneFilePath}':");
                        Console.Error.WriteLine(ex);
                        Environment.Exit((int) ErrorCode.OutputFile);
                    }

                    //Manifest File + Textures
                    Console.WriteLine($"Moving File from {opts.Input} to {sceneFilePath}");
                    for (int i = 0; i < textureFiles.Count; i++)
                    {
                        string textureFile = Path.GetFileName(textureFiles[i]);
                        string texturePath = Path.Combine(sceneFileDir, textureFile);
                        if (!File.Exists(texturePath))
                        {
                            File.Move(textureFiles[i], texturePath);
                        }
                        textureFiles[i] = Path.Combine("Assets", textureFile);
                        Console.WriteLine($"TEXTUREFILES {textureFiles[i]}");
                    }
                    if (textureFiles != null)
                        AssetManifest.CreateAssetManifest(htmlFileDir, textureFiles);

                    //WebServer
                    if (_httpServer == null)
                    {
                        _httpServer = new FuseeHttpServer(htmlFileDir, 4655); // HEX: FU
                        Thread thread = new Thread(_httpServer.listen);
                        thread.Start();
                    }
                    else
                    {
                        _httpServer.HtDocsRoot = htmlFileDir;
                    }
                    Console.WriteLine($"Server running");
                    Process.Start("http://localhost:4655/" + origHtmlFilePath);
                })

                // ERROR on the command line
                .WithNotParsed(errs =>
                {
                    /*foreach (var error in errs)
                    {
                        Console.Error.WriteLine(error);
                    }
                    */
                    Environment.Exit((int) ErrorCode.CommandLineSyntax);
                });
        }
    }
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
