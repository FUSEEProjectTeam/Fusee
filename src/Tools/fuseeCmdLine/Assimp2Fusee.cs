using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Fusee.Serialization;
using Assimp;
using Fusee.Math.Core;
using Fusee.Xene;
using AsQuaternion = Assimp.Quaternion;
using FuQuaternion = Fusee.Math.Core.Quaternion;

namespace Fusee.Tools.fuseeCmdLine
{
    public class Assimp2Fusee
    {
        // Context used while traversing
        private Scene _assimpScene;
        private readonly StateStack<Node> _currentAssimpNode;
        private readonly Dictionary<int, MaterialComponent> _matCache;
        private readonly Dictionary<int, MeshComponent> _meshCache;

        public static SceneContainer FuseefyScene(Scene assimpScene)
        {
            var userName = Environment.UserName;

            // Create an instance of this class to keep state during recursive traversal
            var inst = new Assimp2Fusee {_assimpScene = assimpScene};

            SceneContainer fuScene = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = userName,
                    Generator = "fuConv FUSEE conversion tool",
                    Version = 1,
                    CreationDate = DateTime.Now.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US")),
                },

                // Assimp scenes contain ONE root node (not a list of root notes)
                Children = new List<SceneNodeContainer>(new [] {inst.FuseefyNode(assimpScene.RootNode)})
            };

            // Evaluate and add all Lights to rootNode
            // There is a meshcount but no lightcount per node. :/
            // TODO: Merge changes from moritzhfu first - see function below
            /*
            foreach (var light in assimpScene.Lights)
            {
                var currentLight = GetLight(light);
                fuScene.Children[0].AddComponent(currentLight);
            }*/

            return fuScene;
        }

        private Assimp2Fusee()
        {
            _currentAssimpNode = new StateStack<Node>();
            _matCache = new Dictionary<int, MaterialComponent>();
            _meshCache = new Dictionary<int, MeshComponent>();
        }

        private SceneNodeContainer FuseefyNode(Node assimpNode)
        {
            _currentAssimpNode.Tos = assimpNode;

            SceneNodeContainer fuNode;
            if (assimpNode.HasMeshes)
            {
                if (assimpNode.MeshIndices.Count == 1)
                {
                    // We can create a 1:1 Fusee node for this assimp node
                    fuNode = CreateFuseeNode(assimpNode.Name, GetXForm(assimpNode.Transform),
                        GetMaterial(_assimpScene.Meshes[assimpNode.MeshIndices[0]].MaterialIndex),
                        GetMesh(assimpNode.MeshIndices[0]));
                }
                else
                {
                    // Create a group node to contain all meshes as child nodes.
                    fuNode = new SceneNodeContainer {Name = assimpNode.Name};
                    fuNode.AddComponent(GetXForm(assimpNode.Transform));
                    foreach (var meshIndex in assimpNode.MeshIndices)
                    {
                        fuNode.Children.Add(CreateFuseeNode($"{assimpNode.Name}_mat{meshIndex}",
                            MakeIdentityXForm(),
                            GetMaterial(_assimpScene.Meshes[assimpNode.MeshIndices[meshIndex]].MaterialIndex),
                            GetMesh(assimpNode.MeshIndices[meshIndex])));
                    }
                }
            }
            else
            {
                // Create a simple group node
                fuNode = CreateFuseeNode(assimpNode.Name, GetXForm(assimpNode.Transform), null, null);
            }

            if (assimpNode.HasChildren)
            {
                fuNode.Children = new List<SceneNodeContainer>(assimpNode.ChildCount);
                _currentAssimpNode.Push();
                // Recursively convert all children.
                foreach (var child in assimpNode.Children)
                    fuNode.Children.Add(FuseefyNode(child));
                _currentAssimpNode.Pop();
            }

            return fuNode;
        }

        private TransformComponent MakeIdentityXForm()
        {
            return new TransformComponent
            {
                Translation = float3.Zero,
                Rotation = float3.Zero,
                Scale = float3.One
            };
        }

        // TODO: Check if transform is right, or lefthanded and transform it accordingly, to match imported scene and scene display in FUSEE
        private static TransformComponent GetXForm(Matrix4x4 transform)
        {
            Vector3D scaling, translation;
            AsQuaternion rotation;
            transform.Decompose(out scaling, out rotation, out translation); 
            FuQuaternion fuRot = new FuQuaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            float3 ypr = FuQuaternion.QuaternionToEuler(fuRot);
            return new TransformComponent
            {
                Translation = new float3(translation.X, translation.Y, translation.Z),
                Rotation = ypr,
                Scale = new float3(scaling.X, scaling.Y, scaling.Z) 
            };
        }

        private MaterialComponent GetMaterial(int materialIndex)
        {
            MaterialComponent fuMat;
            if (_matCache.TryGetValue(materialIndex, out fuMat))
                return fuMat;

            // Cache miss.
            // TODO: Check for any anisotropic material and decide if material - or a materialpbrComponent is needed
            // Annotation: As far as I know, Assimp has no ability to decode physically based materials
            // This needs to be implmented to use the new MaterialPBRComponent
            // fuMat = new MaterialPBRComponent();
            fuMat = new MaterialComponent();

            var asMat = _assimpScene.Materials[materialIndex];
            if (asMat.HasName)
                fuMat.Name = asMat.Name;

            if (asMat.HasColorDiffuse || asMat.HasTextureDiffuse)
            {
                fuMat.Diffuse = new MatChannelContainer();
                if (asMat.HasColorDiffuse)
                {
                    fuMat.Diffuse.Color = new float3(asMat.ColorDiffuse.R, asMat.ColorDiffuse.G, asMat.ColorDiffuse.B);

                    if (asMat.HasTextureDiffuse)
                    {
                        fuMat.Diffuse.Texture = Path.GetFileName(asMat.TextureDiffuse.FilePath);
                        fuMat.Diffuse.Mix = 1.0f;
                    }
                }
                if (asMat.HasColorSpecular)
                {
                    fuMat.Specular = new SpecularChannelContainer
                    {
                        Color = new float3(asMat.ColorSpecular.R, asMat.ColorSpecular.G, asMat.ColorSpecular.B),
                        Shininess = asMat.Shininess,
                        Intensity = asMat.ShininessStrength // * 0.01f TODO: Play around with variables here, as these are sometimes too small, or large
                    };
                    if (asMat.HasTextureSpecular)
                    {
                        fuMat.Specular.Texture = asMat.TextureSpecular.FilePath;
                        fuMat.Specular.Mix = 1.0f;
                    }
                }
                // TODO: Evaluate and set roughness fresnel etc. for PBR Component 
            }

            // Add to cache
            _matCache.Add(materialIndex, fuMat);

            return fuMat;
        }

        /* TODO: Merge changes from morithfu/FUSEE3D branch: feat_AdvShaderCodeBuilder to FUSEE3D develop branch, to be able to use this function
        private static LightComponent GetLight(Light light)
        {
            var asLight = light;

            var asAmbient = new float3(asLight.ColorAmbient.R, asLight.ColorAmbient.G, asLight.ColorAmbient.B);
            var asPosition = new float3(asLight.Position.X, asLight.Position.Y, asLight.Position.Z);
            var asConeAngle = asLight.AngleOuterCone;
            var asAttenuation = asLight.AttenuationConstant;
            var asColor = new float3(asLight.ColorDiffuse.R, asLight.ColorDiffuse.G, asLight.ColorDiffuse.B);
            var asConeDirection = new float3(asLight.Direction.X, asLight.Direction.Y, asLight.Direction.Z);
            var asLightType = asLight.LightType; // Undefined = 0, Directional = 1, Point = 2, Spot = 3

            var fuLightType = LightType.Parallel;

            switch ((int) asLightType)
            {
                case 1:
                    fuLightType = LightType.Point;
                    break;
                case 3:
                    fuLightType = LightType.Spot;
                    break;
            }

            return new LightComponent
            {
                Active = true,
                Position = asPosition,
                ConeAngle = asConeAngle,
                ConeDirection = asConeDirection,
                Attenuation = asAttenuation,
                AmbientCoefficient = asAmbient.Length,
                Color = asColor,
                Type = fuLightType
            };
        } */

        private MeshComponent GetMesh(int meshIndex)
        {
            MeshComponent fuMesh;
            if (_meshCache.TryGetValue(meshIndex, out fuMesh))
                return fuMesh;

            // no mesh in cache
            var assimpMesh = _assimpScene.Meshes[meshIndex];

            var meshVertices = assimpMesh.Vertices;
            var meshNormals = assimpMesh.Normals;
            var meshTexCords = assimpMesh.TextureCoordinateChannels;
            var meshFaces = assimpMesh.Faces;

            var fuMeshVerticies = new float3[meshVertices.Count];
            var fuMeshNormals = new float3[meshNormals.Count];
            var fuMeshTexCords = new float2[meshTexCords.Length * 2];
            var fuMeshTriangles = new ushort[meshFaces.Count * 3];

            for (var i = 0; i < meshVertices.Count; i++)
            {
                // Evaluate mesh and ...
                var vertex = new float3(meshVertices[i].X, meshVertices[i].Y, meshVertices[i].Z);
                var normal = new float3(meshNormals[i].X, meshNormals[i].Y, meshNormals[i].Z);
                var texCord = new float2[meshTexCords.Length * meshTexCords[0].Count];

                // ... evaluate and set UVs
                foreach (var meshTexCord in meshTexCords)
                {
                    for (var k = 0; k < meshTexCord.Count; k++)
                    {
                        texCord[k] = new float2(meshTexCord[k].X, meshTexCord[k].Y);
                    }
                }

                // ... add it to components of MeshComponent
                fuMeshVerticies[i] = vertex;
                fuMeshNormals[i] = normal;
                fuMeshTexCords = texCord;

            }

            var count = 0;

            // Evaluate all faces and add them all to one ushort[]
            // TODO: This allocation is reversed (2, 1, 0) to satisfy FUSEE's rendering
            // TODO: Change this if your vertex is culled!!
            foreach (var face in meshFaces)
            {
                fuMeshTriangles[count] = (ushort)face.Indices[2];
                fuMeshTriangles[++count] = (ushort)face.Indices[1];
                fuMeshTriangles[++count] = (ushort)face.Indices[0];
                ++count;
            }

            float3 min;
            float3 max;

            // TODO: Test if this method works!
            EvaluateAABB(fuMeshVerticies, out min, out max);

            // Create new MeshComponent
            var fuMeshComponent =  new MeshComponent
            {
                Name = assimpMesh.Name,
                Vertices = fuMeshVerticies,
                BoundingBox = new AABBf(min, max),
                Normals = fuMeshNormals,
                Triangles = fuMeshTriangles,
                UVs = fuMeshTexCords
            };

            // Add to cache
            _meshCache.Add(meshIndex, fuMeshComponent);

            return fuMeshComponent;
        }

        // ReSharper disable once InconsistentNaming
        // TODO: See above, test this.
        private static void EvaluateAABB(IList<float3> fuMeshVerticies, out float3 min, out float3 max)
        {
            var minVert = fuMeshVerticies[0];
            var maxVert = minVert;

            foreach (var vertex in fuMeshVerticies)
            {
                minVert = float3.Min(minVert, vertex);
                maxVert = float3.Max(maxVert, vertex);
                // Debuging:
                // Console.WriteLine($"Current MinVert {minVert}, MaxVert {maxVert}");
            }

            // TODO: If bone transform is added in further release we need to take this into account!
            // e.g.: 
            // var minWithBone = float3.Transform(minVert, m_transforms[mesh.ParentBone.Index]);
            // min = float3.Min(minWithBone,minVert);

            min = minVert;
            max = maxVert;
        }

        private static SceneNodeContainer CreateFuseeNode(string name, TransformComponent xform, MaterialComponent mat, MeshComponent mesh)
        {
            var fuNode = new SceneNodeContainer { Name = name };

            if (xform != null)
                fuNode.AddComponent(xform);

            if (mat != null)
                fuNode.AddComponent(mat);

            if (mesh != null)
                fuNode.AddComponent(mesh);

            return fuNode;
        }
    }
}
