using CommandLine;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Serialization;
using JsonSubTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fusee.Tools.CmdLine.Verbs
{
    enum ConversionType
    {
        Json
    }

    [Verb("convert", HelpText = "Convert a given *.fus or *.fuz scene to specified human readable output format.")]
    internal class Convert
    {
        [Value(0, HelpText = "Specify input file. Must be *.fus or *.fuz", MetaName = "File", Required = true)]
        public string File { get; set; }

        [Option('f', "format", Default = ConversionType.Json, HelpText = "Specify output format, possible value is: 'Json'.")]
        public ConversionType ConvType { get; set; }


        public int Run()
        {
            if (!string.IsNullOrWhiteSpace(File) && (Path.GetExtension(File).ToLower().Equals(".fuz") || Path.GetExtension(File).ToLower().Equals(".fus")))
            {

                using var stream = System.IO.File.OpenRead(File);
                var scene = FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>(stream));

                var serialized = JsonSerializer.Serialize((Container)scene, new JsonSerializerOptions
                {
                    Converters = { new Float2Converter(), new Float3Converter(), new Float4Converter(), new Float4x4Converter() },
                    WriteIndented = true,
                    MaxDepth = 42
                });

                var outputFile = File + "_converted.json";

                System.IO.File.WriteAllText(outputFile, serialized);
                Console.WriteLine($"{File} sucessfully converted to {ConvType}, saved to: {outputFile}");

                return 0;
            }

            return -1;
        }

        #region JSON

        [Serializable]
        internal class Header
        {
            /// <summary>
            /// The generator used to create this scene.
            /// </summary>
            public string Generator { get; set; }

            /// <summary>
            /// The user who created this scene.
            /// </summary>
            public string CreatedBy { get; set; }

            /// <summary>
            /// The creation date of this scene.
            /// </summary>
            public string CreationDate { get; set; }

            public static implicit operator Header(SceneHeader hdr)
            {
                return new Header
                {
                    CreatedBy = hdr.CreatedBy,
                    CreationDate = hdr.CreationDate,
                    Generator = hdr.Generator
                };
            }
        }

        [Serializable]
        internal class Container
        {
            public Header Header { get; set; }

            public List<Scene> Scenes { get; set; }

            public static implicit operator Container(SceneContainer scn)
            {
                return new Container
                {
                    Header = scn.Header,
                    Scenes = scn.Children.Select(x => (Scene)x).ToList()
                };
            }
        }

        [Serializable]
        internal class Scene
        {
            public string Name { get; set; }
            public List<object> Components { get; set; }

            public List<Node> Children { get; set; }

            public static implicit operator Scene(SceneNode node)
            {
                return new Scene
                {
                    Children = node.Children.Select(x => (Node)x).ToList(),
                    Components = ConvertComponents(node.Components).ToList(),
                    Name = node.Name
                };
            }
        }

        [Serializable]
        internal class Component
        {
            public string Name { get; set; }

            public string Type { get; set; }
        }

        [Serializable]
        internal class TransformComponent : Component
        {
            public float4x4 Transformation { get; set; }
        }

        [Serializable]
        internal class EffectComponent : Component
        {
            public List<string> ParamDeclarations { get; set; }
        }

        [Serializable]
        internal class MeshComponent : Component
        {
            public List<float4> BoneWeights { get; set; }
            public List<float4> BoneIndices { get; set; }
            public List<float4> Tangents { get; set; }

            public List<float3> BiTangents { get; set; }
            public List<float3> Vertices { get; set; }
            public List<float3> Normals { get; set; }

            public List<float2> Uvs { get; set; }

            public List<ushort> Triangles { get; set; }
            public List<uint> Colors { get; set; }
        }

        [Serializable]
        internal class Node
        {
            public string Name { get; set; }

            public List<Node> Children { get; set; }

            public List<object> Components { get; set; }


            public static explicit operator Node(SceneNode comp)
            {
                return new Node
                {
                    Children = comp.Children.Select(x => (Node)x).ToList(),
                    Components = ConvertComponents(comp.Components).ToList(),
                    Name = comp.Name
                };
            }
        }

        private static IEnumerable<object> ConvertComponents(List<SceneComponent> comps)
        {

            foreach (var obj in comps)
            {
                if (obj is Transform)
                {
                    var t = obj as Transform;
                    yield return new TransformComponent
                    {
                        Name = t.Name,
                        Transformation = t.Matrix,
                        Type = "Transformation"
                    };
                }

                if (obj is Effect)
                {
                    var eff = obj as Effect;
                    var allParamDecls = eff.ParamDecl.Select((pair) => pair.Key).ToList();
                    yield return new EffectComponent
                    {
                        Name = eff.Name,
                        Type = "Effect",
                        ParamDeclarations = allParamDecls
                    };
                }

                if (obj is Mesh)
                {
                    var eff = obj as Mesh;
                    yield return new MeshComponent
                    {
                        Name = eff.Name,
                        Type = "Mesh",
                        BiTangents = eff.BiTangentsSet ? eff.BiTangents.ToList() : new List<float3>(),
                        BoneIndices = eff.BoneIndicesSet ? eff.BoneIndices.ToList() : new List<float4>(),
                        BoneWeights = eff.BoneWeightsSet ? eff.BoneWeights.ToList() : new List<float4>(),
                        Colors = eff.ColorsSet ? eff.Colors.ToList() : new List<uint>(),
                        Normals = eff.NormalsSet ? eff.Normals.ToList() : new List<float3>(),
                        Tangents = eff.TangentsSet ? eff.Tangents.ToList() : new List<float4>(),
                        Triangles = eff.TrianglesSet ? eff.Triangles.ToList() : new List<ushort>(),
                        Uvs = eff.UVsSet ? eff.UVs.ToList() : new List<float2>(),
                        Vertices = eff.VerticesSet ? eff.Vertices.ToList() : new List<float3>(),
                    };

                }

                yield return new Component
                {
                    Name = obj.Name,
                    Type = "Unkown"
                };
            }
        }

        #region CustomConverter

        public class Float2Converter : JsonConverter<float2>
        {
            public override float2 Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return new float2();
            }

            public override void Write(
                Utf8JsonWriter writer,
                float2 value,
                JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.x);
                writer.WriteNumberValue(value.y);
                writer.WriteEndArray();             
            }
        }

        public class Float3Converter : JsonConverter<float3>
        {
            public override float3 Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return new float3();
            }

            public override void Write(
                Utf8JsonWriter writer,
                float3 value,
                JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.x);
                writer.WriteNumberValue(value.y);
                writer.WriteNumberValue(value.z);
                writer.WriteEndArray();
            }
        }

        public class Float4Converter : JsonConverter<float4>
        {
            public override float4 Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return new float4();
            }

            public override void Write(
                Utf8JsonWriter writer,
                float4 value,
                JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.x);
                writer.WriteNumberValue(value.y);
                writer.WriteNumberValue(value.z);
                writer.WriteNumberValue(value.w);
                writer.WriteEndArray();


            }
        }

        public class Float4x4Converter : JsonConverter<float4x4>
        {
            public override float4x4 Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return new float4x4();
            }

            public override void Write(
                Utf8JsonWriter writer,
                float4x4 value,
                JsonSerializerOptions options)
            {
                writer.WriteStartArray();

                writer.WriteStartArray();
                writer.WriteNumberValue(value.Column0.x);
                writer.WriteNumberValue(value.Column0.y);
                writer.WriteNumberValue(value.Column0.z);
                writer.WriteNumberValue(value.Column0.w);
                writer.WriteEndArray();


                writer.WriteStartArray();
                writer.WriteNumberValue(value.Column1.x);
                writer.WriteNumberValue(value.Column1.y);
                writer.WriteNumberValue(value.Column1.z);
                writer.WriteNumberValue(value.Column1.w);
                writer.WriteEndArray();

                writer.WriteStartArray();
                writer.WriteNumberValue(value.Column2.x);
                writer.WriteNumberValue(value.Column2.y);
                writer.WriteNumberValue(value.Column2.z);
                writer.WriteNumberValue(value.Column2.w);
                writer.WriteEndArray();

                writer.WriteStartArray();
                writer.WriteNumberValue(value.Column3.x);
                writer.WriteNumberValue(value.Column3.y);
                writer.WriteNumberValue(value.Column3.z);
                writer.WriteNumberValue(value.Column3.w);
                writer.WriteEndArray();

                writer.WriteEndArray();
            }
        }


        #endregion

        #endregion
    }
}
