using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Fusee.Serialization;
using Assimp;
using Fusee.Math.Core;
using Fusee.Xene;
using AsQuaternion = Assimp.Quaternion;
using FuQuaternion = Fusee.Math.Core.Quaternion;

namespace Fusee.Tools.fuConv
{
    public class Assimp2Fusee
    {
        // Context used while traversing
        private Scene _assimpScene;
        private StateStack<Node> _currentAssimpNode;
        private Dictionary<int, MaterialComponent> _matCache;

        public static SceneContainer FuseefyScene(Scene assimpScene)
        {
            String userName = Environment.UserName;

            // Create an instance of this class to keep state during recursive traversal
            var inst = new Assimp2Fusee();
            inst._assimpScene = assimpScene;

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
                Children = new List<SceneNodeContainer>(new [] {inst.FuseefyNode(assimpScene.RootNode)}),
            };

            return fuScene;
        }

        Assimp2Fusee()
        {
            _currentAssimpNode = new StateStack<Node>();
            _matCache = new Dictionary<int, MaterialComponent>();
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

        private TransformComponent GetXForm(Matrix4x4 transform)
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
            fuMat = new MaterialComponent();
            Material asMat = _assimpScene.Materials[materialIndex];
            if (asMat.HasName)
                fuMat.Name = asMat.Name;

            if (asMat.HasColorDiffuse || asMat.HasTextureDiffuse)
            {
                fuMat.Diffuse = new MatChannelContainer();
                if (asMat.HasColorDiffuse)
                {
                    /*
                    Color = new float3(asMat.ColorDiffuse.R, asMat.ColorDiffuse.G, asMat.ColorDiffuse.B),
                    Mix = 1.0f,
                    */

                };
            }
            return fuMat;
        }

        private MeshComponent GetMesh(int meshIndex)
        {
            return null;
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
