using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    internal class MeshManager
    {
        private readonly IRenderContextImp _renderContextImp;
        private readonly Stack<IMeshImp> _toBeDeletedMeshImps = new();
        private readonly Stack<IInstanceDataImp> _toBeDeletedInstanceDataImps = new();
        private readonly Dictionary<Guid, (IMeshImp IMeshImp, Mesh? Mesh)> _identifierToMeshImpDictionary = new();

        private readonly Dictionary<Guid, IInstanceDataImp> _identifierToInstanceDataImpDictionary = new();

        /// <summary>
        /// Creates a new Instance of MeshManager. The instance is handling the memory allocation and deallocation on the GPU by observing Mesh objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation. See RegisterMesh.</param>
        public MeshManager(IRenderContextImp renderContextImp)
        {
            _renderContextImp = renderContextImp;
        }

        private void Remove(IMeshImp meshImp)
        {
            if (meshImp == null) return;

            if (meshImp.TrianglesSet)
                _renderContextImp.RemoveTriangles(meshImp);

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

            if (meshImp.BoneWeightsSet)
                _renderContextImp.RemoveBoneWeights(meshImp);

            if (meshImp.BoneIndicesSet)
                _renderContextImp.RemoveBoneIndices(meshImp);

            if (meshImp.TangentsSet)
                _renderContextImp.RemoveTangents(meshImp);

            if (meshImp.BiTangentsSet)
                _renderContextImp.RemoveBiTangents(meshImp);

            if (meshImp.FlagsSet)
                _renderContextImp.RemoveFlags(meshImp);
        }

        private void Remove(IInstanceDataImp instanceData)
        {
            _renderContextImp.RemoveInstanceData(instanceData);
        }

        private void DisposeMesh(object? sender, MeshChangedEventArgs meshDataEventArgs)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(meshDataEventArgs.Mesh.UniqueIdentifier, out var toBeUpdatedMeshImp))
                throw new KeyNotFoundException("Mesh is not registered.");

            // Add the meshImp to the toBeDeleted Stack...#
            _toBeDeletedMeshImps.Push(toBeUpdatedMeshImp.IMeshImp);

            // remove the meshImp from the dictionary, the meshImp data now only resides inside the gpu and will be cleaned up on bottom of Render(Mesh mesh)
            _ = _identifierToMeshImpDictionary.Remove(meshDataEventArgs.Mesh.UniqueIdentifier);
        }

        internal void UpdateAllMeshes()
        {
            foreach (var (kv, mesh) in from kv in _identifierToMeshImpDictionary
                                       let mesh = kv.Value.Mesh
                                       select (kv, mesh))
            {
                if (mesh == null || mesh.Vertices == null) continue;
                if (!mesh.UpdatePerFrame) continue;

                var meshImp = kv.Value.IMeshImp;
                if (mesh.Vertices.DirtyIndex)
                {
                    // update all vertices
                    // TODO: use BufferSubData() if possible, implement in interface in implement in imp
                    _renderContextImp.SetVertices(meshImp, mesh.Vertices.AsReadOnlySpan);
                    mesh.BoundingBox = new AABBf(mesh.Vertices.AsReadOnlySpan);
                }

                if (mesh.Triangles != null && mesh.Triangles.DirtyIndex)
                {
                    _renderContextImp.SetTriangles(meshImp, mesh.Triangles.AsReadOnlySpan);
                }

                if (mesh.Normals != null && mesh.Normals.DirtyIndex)
                {
                    _renderContextImp.SetNormals(meshImp, mesh.Normals.AsReadOnlySpan);
                }

                if (mesh.UVs != null && mesh.UVsSet && mesh.UVs.DirtyIndex)
                {
                    _renderContextImp.SetUVs(meshImp, mesh.UVs.AsReadOnlySpan);
                }

                if (mesh.Tangents != null && mesh.TangentsSet && mesh.Tangents.DirtyIndex)
                {
                    _renderContextImp.SetTangents(meshImp, mesh.Tangents.AsReadOnlySpan);
                }

                if (mesh.BiTangents != null && mesh.BiTangentsSet && mesh.BiTangents.DirtyIndex)
                {
                    _renderContextImp.SetBiTangents(meshImp, mesh.BiTangents.AsReadOnlySpan);
                }

                if (mesh.BoneWeights != null && mesh.BoneWeightsSet && mesh.BoneWeights.DirtyIndex)
                {
                    _renderContextImp.SetBoneWeights(meshImp, mesh.BoneWeights.AsReadOnlySpan);
                }

                if (mesh.BoneIndices != null && mesh.BoneIndicesSet && mesh.BoneIndices.DirtyIndex)
                {
                    _renderContextImp.SetBoneIndices(meshImp, mesh.BoneIndices.AsReadOnlySpan);
                }

                if (mesh.Colors0 != null && mesh.Colors0Set && mesh.Colors0.DirtyIndex)
                {
                    _renderContextImp.SetColors(meshImp, mesh.Colors0.AsReadOnlySpan);
                }

                if (mesh.Colors1 != null && mesh.Colors1Set && mesh.Colors1.DirtyIndex)
                {
                    _renderContextImp.SetColors1(meshImp, mesh.Colors1.AsReadOnlySpan);
                }

                if (mesh.Colors2 != null && mesh.Colors2Set && mesh.Colors2.DirtyIndex)
                {
                    _renderContextImp.SetColors2(meshImp, mesh.Colors2.AsReadOnlySpan);
                }

                if (mesh.Flags != null && mesh.FlagsSet && mesh.Flags.DirtyIndex)
                {
                    _renderContextImp.SetFlags(meshImp, mesh.Flags.AsReadOnlySpan);
                }

                // TODO: Prepared for next change with index list
                //if (!mesh.Vertices.DirtyIndices.Empty)
                //{
                //    // update all vertices
                //    // TODO: use BufferSubData() if possible, implement in interface in implement in imp
                //    _renderContextImp.SetVertices(meshImp, mesh.Vertices.AsReadOnlySpan);
                //    mesh.BoundingBox = new AABBf(mesh.Vertices.AsReadOnlySpan);
                //}
                //if (mesh.Triangles.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetTriangles(meshImp, mesh.Triangles.AsReadOnlySpan);
                //}
                //if (mesh.NormalsSet && mesh.Normals.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetNormals(meshImp, mesh.Normals.AsReadOnlySpan);
                //}
                //if (mesh.UVsSet && mesh.UVs.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetUVs(meshImp, mesh.UVs.AsReadOnlySpan);
                //}
                //if (mesh.TangentsSet && mesh.Tangents.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetTangents(meshImp, mesh.Tangents.AsReadOnlySpan);
                //}
                //if (mesh.BiTangentsSet && mesh.BiTangents.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetBiTangents(meshImp, mesh.BiTangents.AsReadOnlySpan);
                //}
                //if (mesh.BoneWeightsSet && mesh.BoneWeights.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetBoneWeights(meshImp, mesh.BoneWeights.AsReadOnlySpan);
                //}
                //if (mesh.BoneIndicesSet && mesh.BoneIndices.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetBoneIndices(meshImp, mesh.BoneIndices.AsReadOnlySpan);
                //}
                //if (mesh.Colors0Set && mesh.Colors0.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetColors(meshImp, mesh.Colors0.AsReadOnlySpan);
                //}
                //if (mesh.Colors1Set && mesh.Colors1.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetColors1(meshImp, mesh.Colors1.AsReadOnlySpan);
                //}
                //if (mesh.Colors2Set && mesh.Colors2.DirtyIndices.Empty)
                //{
                //    _renderContextImp.SetColors2(meshImp, mesh.Colors2.AsReadOnlySpan);
                //}
                mesh.ResetIndexLists();
            }
        }

        private void DisposeInstanceData(object? sender, InstanceDataChangedEventArgs instanceDataEventArgs)
        {
            if (!_identifierToInstanceDataImpDictionary.TryGetValue(instanceDataEventArgs.InstanceData.UniqueId, out IInstanceDataImp instanceDataImp))
                throw new KeyNotFoundException("InstanceData is not registered.");

            // Add the meshImp to the toBeDeleted Stack...
            _toBeDeletedInstanceDataImps.Push(instanceDataImp);

            // remove the meshImp from the dictionary, the meshImp data now only resides inside the gpu and will be cleaned up on bottom of Render(Mesh mesh)
            _ = _identifierToInstanceDataImpDictionary.Remove(instanceDataEventArgs.InstanceData.UniqueId);
        }

        private void InstanceDataChanged(object? sender, InstanceDataChangedEventArgs instanceDataEventArgs)
        {
            if (!_identifierToInstanceDataImpDictionary.TryGetValue(instanceDataEventArgs.InstanceData.UniqueId, out var instanceImp))
            {
                throw new ArgumentException("InstanceData is not registered yet. Use RegisterInstanceData first.");
            }

            var instanceData = (InstanceData)instanceDataEventArgs.InstanceData;

            switch (instanceDataEventArgs.ChangedEnum)
            {
                case InstanceDataChangedEnum.Transform:
                    _renderContextImp.SetInstanceTransform(instanceImp, instanceData.Positions, instanceData.Rotations, instanceData.Scales);
                    break;
                case InstanceDataChangedEnum.Colors:
                    _renderContextImp.SetInstanceColor(instanceImp, instanceData.Colors);
                    break;
            }
        }

        public void RegisterNewMesh(GpuMesh mesh, float3[] vertices, uint[] triangles, float2[]? uvs = null,
            float3[]? normals = null, uint[]? colors = null, uint[]? colors1 = null, uint[]? colors2 = null,
            float4[]? tangents = null, float3[]? bitangents = null, float4[]? boneIndices = null, float4[]? boneWeights = null, uint[]? flags = null)
        {
            var meshImp = _renderContextImp.CreateMeshImp();
            _renderContextImp.SetVertexArrayObject(meshImp);

            if (triangles != null)
                _renderContextImp.SetTriangles(meshImp, triangles);

            if (vertices != null)
                _renderContextImp.SetVertices(meshImp, vertices);

            if (uvs != null)
                _renderContextImp.SetUVs(meshImp, uvs);

            if (normals != null)
                _renderContextImp.SetNormals(meshImp, normals);

            if (colors != null)
                _renderContextImp.SetColors(meshImp, colors);

            if (colors1 != null)
                _renderContextImp.SetColors1(meshImp, colors1);

            if (colors2 != null)
                _renderContextImp.SetColors2(meshImp, colors2);

            if (boneIndices != null)
                _renderContextImp.SetBoneIndices(meshImp, boneIndices);

            if (boneWeights != null)
                _renderContextImp.SetBoneWeights(meshImp, boneWeights);

            if (tangents != null)
                _renderContextImp.SetTangents(meshImp, tangents);

            if (bitangents != null)
                _renderContextImp.SetBiTangents(meshImp, bitangents);

            if (flags != null)
                _renderContextImp.SetFlags(meshImp, flags);

            mesh.DisposeData += DisposeMesh;
            meshImp.MeshType = mesh.MeshType;

            _identifierToMeshImpDictionary.Add(mesh.UniqueIdentifier, (meshImp, null));
        }

        // Configure newly created MeshImp to reflect Mesh's properties on GPU (allocate buffers)
        private IMeshImp RegisterNewMesh(Mesh mesh)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.

            var meshImp = _renderContextImp.CreateMeshImp();

            _renderContextImp.SetVertexArrayObject(meshImp);

            if (mesh.TrianglesSet)
                _renderContextImp.SetTriangles(meshImp, mesh.Triangles.AsReadOnlySpan);

            if (mesh.VerticesSet)
                _renderContextImp.SetVertices(meshImp, mesh.Vertices.AsReadOnlySpan);

            if (mesh.UVsSet)
                _renderContextImp.SetUVs(meshImp, mesh.UVs.AsReadOnlySpan);

            if (mesh.NormalsSet)
                _renderContextImp.SetNormals(meshImp, mesh.Normals.AsReadOnlySpan);

            if (mesh.Colors0Set)
                _renderContextImp.SetColors(meshImp, mesh.Colors0.AsReadOnlySpan);

            if (mesh.Colors1Set)
                _renderContextImp.SetColors1(meshImp, mesh.Colors1.AsReadOnlySpan);

            if (mesh.Colors2Set)
                _renderContextImp.SetColors2(meshImp, mesh.Colors2.AsReadOnlySpan);

            if (mesh.BoneIndicesSet)
                _renderContextImp.SetBoneIndices(meshImp, mesh.BoneIndices.AsReadOnlySpan);

            if (mesh.BoneWeightsSet)
                _renderContextImp.SetBoneWeights(meshImp, mesh.BoneWeights.AsReadOnlySpan);

            if (mesh.TangentsSet)
                _renderContextImp.SetTangents(meshImp, mesh.Tangents.AsReadOnlySpan);

            if (mesh.BiTangentsSet)
                _renderContextImp.SetBiTangents(meshImp, mesh.BiTangents.AsReadOnlySpan);

            if (mesh.FlagsSet)
                _renderContextImp.SetFlags(meshImp, mesh.Flags.AsReadOnlySpan);

            //mesh.MeshChanged += MeshChanged; // <- Replace with UpdateGPU method!

            mesh.DisposeData += DisposeMesh;

            meshImp.MeshType = mesh.MeshType;

            _identifierToMeshImpDictionary.Add(mesh.UniqueIdentifier, (meshImp, mesh));

            return meshImp;

#pragma warning restore CS8602 // Dereference of a possibly null reference.

        }

        private IInstanceDataImp RegisterNewInstanceData(Mesh mesh, InstanceData instanceData)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(mesh.UniqueIdentifier, out var meshImp))
            {
                throw new ArgumentException("Mesh is not registered yet. Use RegisterMesh first.");
            }

            instanceData.DataChanged += InstanceDataChanged;
            instanceData.DisposeData += DisposeInstanceData;

            var instanceDataImp = _renderContextImp.CreateInstanceDataImp(meshImp.IMeshImp);
            instanceDataImp.Amount = instanceData.Amount;

            _identifierToInstanceDataImpDictionary.Add(instanceData.UniqueId, instanceDataImp);
            _renderContextImp.SetInstanceTransform(instanceDataImp, instanceData.Positions, instanceData.Rotations, instanceData.Scales);
            _renderContextImp.SetInstanceColor(instanceDataImp, instanceData.Colors);

            return instanceDataImp;
        }

        public IMeshImp GetImpFromMesh(Mesh m)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(m.UniqueIdentifier, out var foundMeshImp))
            {
                return RegisterNewMesh(m);
            }
            return foundMeshImp.IMeshImp;
        }

        public IMeshImp GetImpFromMesh(GpuMesh m)
        {
            if (!_identifierToMeshImpDictionary.TryGetValue(m.UniqueIdentifier, out var foundMeshImp))
            {
                throw new ArgumentException("GpuMesh not found, make sure you created it first.");
            }
            return foundMeshImp.IMeshImp;
        }

        public IInstanceDataImp GetImpFromInstanceData(Mesh m, InstanceData instanceData)
        {
            if (!_identifierToInstanceDataImpDictionary.TryGetValue(instanceData.UniqueId, out IInstanceDataImp imp))
            {
                return RegisterNewInstanceData(m, instanceData);
            }
            return imp;
        }

        /// <summary>
        /// Call this method on the main thread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            while (_toBeDeletedMeshImps.Count > 0)
            {
                var tobeDeletedMeshImp = _toBeDeletedMeshImps.Pop();
                Remove(tobeDeletedMeshImp);
            }

            while (_toBeDeletedInstanceDataImps.Count > 0)
            {
                var tobeDeletedInstanceImp = _toBeDeletedInstanceDataImps.Pop();
                Remove(tobeDeletedInstanceImp);
            }
        }
    }
}