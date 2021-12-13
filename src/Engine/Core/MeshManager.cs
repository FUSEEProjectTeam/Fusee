using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    internal class MeshManager : IDisposable
    {
        private readonly IRenderContextImp _renderContextImp;
        private readonly Stack<IMeshImp> _toBeDeletedMeshImps = new();
        private readonly Dictionary<Suid, IMeshImp> _identifierToMeshImpDictionary = new();

        private void Remove(IMeshImp meshImp)
        {
            if (meshImp == null) return;
            if (meshImp.VerticesSet)
                _renderContextImp.RemoveVertices(meshImp);

            if (meshImp.NormalsSet)
                _renderContextImp.RemoveNormals(meshImp);

            if (meshImp.ColorsSet)
                _renderContextImp.RemoveColors(meshImp);

            if (meshImp.ColorsSet1)
                _renderContextImp.RemoveColors1(meshImp);

            if (meshImp.ColorsSet2)
                _renderContextImp.RemoveColors2(meshImp);

            if (meshImp.UVsSet)
                _renderContextImp.RemoveUVs(meshImp);

            if (meshImp.TrianglesSet)
                _renderContextImp.RemoveTriangles(meshImp);

            if (meshImp.BoneWeightsSet)
                _renderContextImp.RemoveBoneWeights(meshImp);

            if (meshImp.BoneIndicesSet)
                _renderContextImp.RemoveBoneIndices(meshImp);

            if (meshImp.TangentsSet)
                _renderContextImp.RemoveTangents(meshImp);

            if (meshImp.BiTangentsSet)
                _renderContextImp.RemoveBiTangents(meshImp);

            // Force collection
            GC.Collect();
        }

        private void DisposeMesh(object sender, MeshChangedEventArgs meshDataEventArgs)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(meshDataEventArgs.Mesh.SessionUniqueIdentifier, out IMeshImp toBeUpdatedMeshImp))
                throw new KeyNotFoundException("Mesh is not registered.");

            // Add the meshImp to the toBeDeleted Stack...#
            _toBeDeletedMeshImps.Push(toBeUpdatedMeshImp);

            // remove the meshImp from the dictionary, the meshImp data now only resides inside the gpu and will be cleaned up on bottom of Render(Mesh mesh)
            _identifierToMeshImpDictionary.Remove(meshDataEventArgs.Mesh.SessionUniqueIdentifier);
        }

        private void MeshChanged(object sender, MeshChangedEventArgs meshDataEventArgs)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(meshDataEventArgs.Mesh.SessionUniqueIdentifier, out IMeshImp toBeUpdatedMeshImp))
                throw new KeyNotFoundException("Mesh is not registered.");

            var mesh = (Mesh)meshDataEventArgs.Mesh;

            switch (meshDataEventArgs.ChangedEnum)
            {
                case MeshChangedEnum.Vertices:
                    _renderContextImp.SetVertices(toBeUpdatedMeshImp, mesh.Vertices);
                    mesh.BoundingBox = new AABBf(mesh.Vertices);
                    break;
                case MeshChangedEnum.Triangles:
                    _renderContextImp.SetTriangles(toBeUpdatedMeshImp, mesh.Triangles);
                    break;
                case MeshChangedEnum.Colors:
                    _renderContextImp.SetColors(toBeUpdatedMeshImp, mesh.Colors);
                    break;
                case MeshChangedEnum.Colors1:
                    _renderContextImp.SetColors(toBeUpdatedMeshImp, mesh.Colors1);
                    break;
                case MeshChangedEnum.Colors2:
                    _renderContextImp.SetColors(toBeUpdatedMeshImp, mesh.Colors2);
                    break;
                case MeshChangedEnum.Normals:
                    _renderContextImp.SetNormals(toBeUpdatedMeshImp, mesh.Normals);
                    break;
                case MeshChangedEnum.Uvs:
                    _renderContextImp.SetUVs(toBeUpdatedMeshImp, mesh.UVs);
                    break;
                case MeshChangedEnum.BoneIndices:
                    _renderContextImp.SetBoneIndices(toBeUpdatedMeshImp, mesh.BoneIndices);
                    break;
                case MeshChangedEnum.BoneWeights:
                    _renderContextImp.SetBoneWeights(toBeUpdatedMeshImp, mesh.BoneWeights);
                    break;
                case MeshChangedEnum.Tangents:
                    _renderContextImp.SetTangents(toBeUpdatedMeshImp, mesh.Tangents);
                    break;
                case MeshChangedEnum.BiTangents:
                    _renderContextImp.SetBiTangents(toBeUpdatedMeshImp, mesh.BiTangents);
                    break;
            }
        }

        public void RegisterNewMesh(GpuMesh mesh, float3[] vertices, ushort[] triangles = null, float2[] uvs = null,
            float3[] normals = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null,
            float4[] tangents = null, float3[] bitangents = null, float4[] boneIndices = null, float4[] boneWeights = null)
        {
            _renderContextImp.SetVertexArrayObject(mesh);

            if (vertices != null)
                _renderContextImp.SetVertices(mesh, vertices);

            if (uvs != null)
                _renderContextImp.SetUVs(mesh, uvs);

            if (normals != null)
                _renderContextImp.SetNormals(mesh, normals);

            if (colors != null)
                _renderContextImp.SetColors(mesh, colors);

            if (colors1 != null)
                _renderContextImp.SetColors1(mesh, colors1);

            if (colors2 != null)
                _renderContextImp.SetColors2(mesh, colors2);

            if (boneIndices != null)
                _renderContextImp.SetBoneIndices(mesh, boneIndices);

            if (boneWeights != null)
                _renderContextImp.SetBoneWeights(mesh, boneWeights);

            if (triangles != null)
                _renderContextImp.SetTriangles(mesh, triangles);

            if (tangents != null)
                _renderContextImp.SetTangents(mesh, tangents);

            if (bitangents != null)
                _renderContextImp.SetBiTangents(mesh, bitangents);

            mesh.DisposeData += DisposeMesh;

            _identifierToMeshImpDictionary.Add(mesh.SessionUniqueIdentifier, mesh);
        }

        // Configure newly created MeshImp to reflect Mesh's properties on GPU (allocate buffers)
        private IMeshImp RegisterNewMesh(Mesh mesh)
        {
            var meshImp = _renderContextImp.CreateMeshImp();

            _renderContextImp.SetVertexArrayObject(meshImp);

            if (mesh.VerticesSet)
                _renderContextImp.SetVertices(meshImp, mesh.Vertices);

            if (mesh.UVsSet)
                _renderContextImp.SetUVs(meshImp, mesh.UVs);

            if (mesh.NormalsSet)
                _renderContextImp.SetNormals(meshImp, mesh.Normals);

            if (mesh.ColorsSet)
                _renderContextImp.SetColors(meshImp, mesh.Colors);

            if (mesh.ColorsSet1)
                _renderContextImp.SetColors1(meshImp, mesh.Colors1);

            if (mesh.ColorsSet2)
                _renderContextImp.SetColors2(meshImp, mesh.Colors2);

            if (mesh.BoneIndicesSet)
                _renderContextImp.SetBoneIndices(meshImp, mesh.BoneIndices);

            if (mesh.BoneWeightsSet)
                _renderContextImp.SetBoneWeights(meshImp, mesh.BoneWeights);

            if (mesh.TrianglesSet)
                _renderContextImp.SetTriangles(meshImp, mesh.Triangles);

            if (mesh.TangentsSet)
                _renderContextImp.SetTangents(meshImp, mesh.Tangents);

            if (mesh.BiTangentsSet)
                _renderContextImp.SetBiTangents(meshImp, mesh.BiTangents);

            mesh.MeshChanged += MeshChanged;
            mesh.DisposeData += DisposeMesh;

            meshImp.MeshType = mesh.MeshType;

            _identifierToMeshImpDictionary.Add(mesh.SessionUniqueIdentifier, meshImp);

            return meshImp;
        }

        /// <summary>
        /// Creates a new Instance of MeshManager. Th instance is handling the memory allocation and deallocation on the GPU by observing Mesh.cs objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation. See RegisterMesh.</param>
        public MeshManager(IRenderContextImp renderContextImp)
        {
            _renderContextImp = renderContextImp;
        }

        public IMeshImp GetMeshImpFromMesh(Mesh m)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(m.SessionUniqueIdentifier, out IMeshImp foundMeshImp))
            {
                return RegisterNewMesh(m);
            }
            return foundMeshImp;
        }

        public IMeshImp GetMeshImpFromMesh(GpuMesh m)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(m.SessionUniqueIdentifier, out IMeshImp foundMeshImp))
            {
                throw new ArgumentException("GpuMesh not found, make sure you created it first.");
            }
            return foundMeshImp;
        }

        /// <summary>
        /// Call this method on the main thread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            if (_toBeDeletedMeshImps == null || _toBeDeletedMeshImps.Count == 0)
            {
                return;
            }
            while (_toBeDeletedMeshImps.Count > 0)
            {
                var tobeDeletedMeshImp = _toBeDeletedMeshImps.Pop();
                Remove(tobeDeletedMeshImp);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                Cleanup();

                for (int i = 0; i < _identifierToMeshImpDictionary.Count; i++)
                {
                    var meshItem = _identifierToMeshImpDictionary.ElementAt(i);
                    Remove(meshItem.Value);
                    _identifierToMeshImpDictionary.Remove(meshItem.Key);
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~MeshManager()
        {
            Dispose(disposing: false);
        }
    }
}