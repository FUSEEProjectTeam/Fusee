using CommunityToolkit.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// List of indices that needs to be updated on the GPU
    /// </summary>
    public sealed class DirtyIndexList
    {
        /// <summary>
        /// Ctor as init method
        /// </summary>
        public void Init()
        {
            IndexList = new List<(int, int)>();
        }

        /// <summary>
        /// Reset the dirty index list
        /// </summary>
        public void ResetList() => Init();

        /// <summary>
        /// Adds one element to dirty index list as tuple
        /// e. g. idx: 1 => Add(1,1)
        /// </summary>
        /// <param name="idx"></param>
        public void Add(int idx) => IndexList?.Add((idx, idx));

        /// <summary>
        /// Add index range to dirty index list
        /// </summary>
        /// <param name="idx"></param>
        public void Add((int, int) idx) => IndexList?.Add(idx);

        /// <summary>
        /// The dirty index list
        /// </summary>
        public List<(int, int)>? IndexList { get; private set; }

        /// <summary>
        /// Is list empty
        /// </summary>
        public bool Empty => IndexList?.Count == 0;
    }

    /// <summary>
    /// Attribute class which contains the mesh data, as well as methods to modify the underlying data fields
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MeshAttributes<T> where T : struct
    {
        private readonly T[] _attribData;

        /* TODO
        /// <summary>
        /// List of dirty indices which needs to be updated on the GPU
        /// </summary>
        // public readonly DirtyIndexList DirtyIndices = new();
        */

        /// <summary>
        /// Are there any dirty indices which need to be updated on the GPU?
        /// </summary>
        public bool DirtyIndex { get; internal set; } = true;

        /// <summary>
        /// Get length of data
        /// </summary>
        public int Length => _attribData.Length;

        /// <summary>
        /// Generate data
        /// </summary>
        /// <param name="data"></param>
        public MeshAttributes(in ICollection<T> data)
        {
            _attribData = data.ToArray();
            //DirtyIndices.Init();
        }

        /// <summary>
        /// Quick readonly access to data
        /// Use <see cref="MeshAttributes{T}"/> indexer for read/write access
        /// </summary>
        public ReadOnlySpan<T> AsReadOnlySpan => new(_attribData);

        /// <summary>
        /// Access read/write mesh attribute data
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public T this[int idx]
        {
            get => _attribData[idx];
            set
            {
                _attribData[idx] = value;
                DirtyIndex = true;

                // TODO: Track changed indices in a fast and memory efficient way
                //if (DirtyIndices.Empty)
                //    DirtyIndices.Add(0); /// add one element to notify <see cref="MeshManager"> that values have been changed
            }
        }

        /// <summary>
        /// Returns the backing field value as array
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return _attribData;
        }

        /// <summary>
        /// Fills the array with given value beginning at index <paramref name="start"/> until <paramref name="length"/> is reached.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start">default = 0</param>
        /// <param name="length">default = -1, if -1 is given the entire length is being used</param>
        public void Fill(in T data, int start = 0, int length = -1)
        {
            Guard.IsGreaterThanOrEqualTo(start, 0);
            Guard.IsGreaterThanOrEqualTo(length, -1);
            Guard.IsNotNull(data);

            var calculatedLength = length == -1 ? (_attribData.Length + start) : length;
            Guard.IsLessThanOrEqualTo(calculatedLength, _attribData.Length);

            Array.Fill(_attribData, data, start, calculatedLength);
            DirtyIndex = true;
            //DirtyIndices.Add((0, calculatedLength - 1));
        }

        /// <summary>
        /// Sets attributes by given <paramref name="data"/>, replacing all elements beginning at given <paramref name="start"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start">default = 0</param>
        public void SetAttributeData(in T[] data, int start = 0)
        {
            Guard.IsNotNull(data);
            Guard.IsGreaterThanOrEqualTo(start, 0);

            var calculatedLength = data.Length + start;
            Guard.IsLessThanOrEqualTo(calculatedLength, _attribData.Length);

            Array.Copy(data, 0, _attribData, start, data.Length);
            DirtyIndex = true;
            //DirtyIndices.Add((start, _attribData.Length - 1));
        }
    }

    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IManagedMesh
    {
        #region RenderContext Asset Management

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs>? DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();

        /// <summary>
        /// Update all changed <see cref="MeshAttributes{T}"/> data before each frame?
        /// </summary>
        public bool UpdatePerFrame { set; get; } = true;

        /// <summary>
        /// Gather all <see cref="MeshAttributes{T}.DirtyIndex"/> values in one property.
        /// </summary>
        /// <returns><see langword="true"/> if any of the <see cref="MeshAttributes{T}.DirtyIndex"/> is true.</returns>
        public bool HasDirtyIndices
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            get
            {
                var returnVal = Vertices.DirtyIndex && Triangles.DirtyIndex;

                if (NormalsSet)
                    returnVal &= Normals.DirtyIndex;
                if (UVsSet)
                    returnVal &= UVs.DirtyIndex;
                if (TangentsSet)
                    returnVal &= Tangents.DirtyIndex;
                if (BiTangentsSet)
                    returnVal &= BiTangents.DirtyIndex;
                if (BoneIndicesSet)
                    returnVal &= BoneIndices.DirtyIndex;
                if (BoneWeightsSet)
                    returnVal &= BoneWeights.DirtyIndex;
                if (Colors0Set)
                    returnVal &= Colors0.DirtyIndex;
                if (Colors1Set)
                    returnVal &= Colors1.DirtyIndex;
                if (Colors2Set)
                    returnVal &= Colors2.DirtyIndex;
                if (FlagsSet)
                    returnVal &= Flags.DirtyIndex;

                return returnVal;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        }

        /// <summary>
        /// Reset all dirty flags
        /// </summary>
        public void ResetIndexLists()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.

            Vertices.DirtyIndex = false;
            Triangles.DirtyIndex = false;

            if (NormalsSet)
                Normals.DirtyIndex = false;
            if (UVsSet)
                UVs.DirtyIndex = false;
            if (TangentsSet)
                Tangents.DirtyIndex = false;
            if (BiTangentsSet)
                BiTangents.DirtyIndex = false;
            if (BoneIndicesSet)
                BoneIndices.DirtyIndex = false;
            if (BoneWeightsSet)
                BoneWeights.DirtyIndex = false;
            if (Colors0Set)
                Colors0.DirtyIndex = false;
            if (Colors1Set)
                Colors1.DirtyIndex = false;
            if (Colors2Set)
                Colors2.DirtyIndex = false;
            if (FlagsSet)
                Flags.DirtyIndex = false;



            // TODO: Prepared for next change with index list
            // reset index lists
            //Vertices.DirtyIndices.ResetList();
            //Triangles.DirtyIndices.ResetList();
            //Normals?.DirtyIndices.ResetList();
            //UVs?.DirtyIndices.ResetList();
            //Tangents?.DirtyIndices.ResetList();
            //BiTangents?.DirtyIndices.ResetList();
            //BoneIndices?.DirtyIndices.ResetList();
            //BoneWeights?.DirtyIndices.ResetList();
            //Colors0?.DirtyIndices.ResetList();
            //Colors1?.DirtyIndices.ResetList();
            //Colors2?.DirtyIndices.ResetList();

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        #endregion

        #region Mesh data member

        /// <summary>
        /// The triangles
        /// </summary>
        public MeshAttributes<uint>? Triangles { get; protected set; }
        /// <summary>
        /// The vertices
        /// </summary>
        public MeshAttributes<float3>? Vertices { get; protected set; }
        /// <summary>
        /// The normals
        /// </summary>
        public MeshAttributes<float3>? Normals { get; protected set; }
        /// <summary>
        /// The UV coordinates
        /// </summary>
        public MeshAttributes<float2>? UVs { get; protected set; }

        /// <summary>
        /// The bine weights
        /// </summary>
        public MeshAttributes<float4>? BoneWeights { get; internal set; } // set via SceneRenderer
        /// <summary>
        /// The bone indices
        /// </summary>
        public MeshAttributes<float4>? BoneIndices { get; internal set; } // set via SceneRenderer

        /// <summary>
        /// The tangents
        /// </summary>
        public MeshAttributes<float4>? Tangents { get; internal set; } // set via SceneRenderer
        /// <summary>
        /// The bi tangents
        /// </summary>
        public MeshAttributes<float3>? BiTangents { get; internal set; } // set via SceneRenderer

        /// <summary>
        /// The vertex color field 0
        /// </summary>
        public MeshAttributes<uint>? Colors0 { get; protected set; }
        /// <summary>
        /// The vertex color field 1
        /// </summary>
        public MeshAttributes<uint>? Colors1 { get; protected set; }
        /// <summary>
        /// The vertex color field 2
        /// </summary>
        public MeshAttributes<uint>? Colors2 { get; protected set; }

        /// <summary>
        /// The vertex flags field
        /// </summary>
        public MeshAttributes<uint>? Flags { get; protected set; }
        #endregion

        /// <summary>
        /// Protected ctor
        /// </summary>
        protected Mesh()
        {
            if (VerticesSet)
            {
                Guard.IsNotNull(Vertices);
                BoundingBox = new AABBf(Vertices.AsReadOnlySpan);
            }
        }

        /// <summary>
        /// Generates a new <see cref="Mesh"/> instance
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="vertices"></param>
        /// <param name="normals"></param>
        /// <param name="uvs"></param>
        /// <param name="boneWeights"></param>
        /// <param name="boneIndices"></param>
        /// <param name="tangents"></param>
        /// <param name="biTangents"></param>
        /// <param name="colors"></param>
        /// <param name="colors1"></param>
        /// <param name="colors2"></param>
        /// <param name="flags"></param>
        public Mesh
        (
            ICollection<uint> triangles,
            ICollection<float3> vertices,
            ICollection<float3>? normals = null,
            ICollection<float2>? uvs = null,
            ICollection<float4>? boneWeights = null,
            ICollection<float4>? boneIndices = null,
            ICollection<float4>? tangents = null,
            ICollection<float3>? biTangents = null,
            ICollection<uint>? colors = null,
            ICollection<uint>? colors1 = null,
            ICollection<uint>? colors2 = null,
            ICollection<uint>? flags = null)
        {
            Guard.IsGreaterThan(triangles.Count, 0);
            Guard.IsGreaterThan(vertices.Count, 0);

            Triangles = new MeshAttributes<uint>(triangles);
            Vertices = new MeshAttributes<float3>(vertices);

            if (normals != null)
            {
                Guard.IsEqualTo(normals.Count, vertices.Count);
                Normals = new MeshAttributes<float3>(normals);
            }

            if (uvs != null)
            {
                Guard.IsEqualTo(uvs.Count, vertices.Count);
                UVs = new MeshAttributes<float2>(uvs);
            }

            if (boneWeights != null)
            {
                Guard.IsEqualTo(boneWeights.Count, vertices.Count);
                BoneWeights = new MeshAttributes<float4>(boneWeights);
            }

            if (boneIndices != null)
            {
                Guard.IsEqualTo(boneIndices.Count, vertices.Count);
                BoneIndices = new MeshAttributes<float4>(boneIndices);
            }

            if (tangents != null)
            {
                Guard.IsEqualTo(tangents.Count, vertices.Count);
                Tangents = new MeshAttributes<float4>(tangents);
            }

            if (biTangents != null)
            {
                Guard.IsEqualTo(biTangents.Count, vertices.Count);
                BiTangents = new MeshAttributes<float3>(biTangents);
            }

            if (colors != null)
            {
                Guard.IsEqualTo(colors.Count, vertices.Count);
                Colors0 = new MeshAttributes<uint>(colors);
            }

            if (colors1 != null)
            {
                Guard.IsEqualTo(colors1.Count, vertices.Count);
                Colors0 = new MeshAttributes<uint>(colors1);
            }

            if (colors2 != null)
            {
                Guard.IsEqualTo(colors2.Count, vertices.Count);
                Colors0 = new MeshAttributes<uint>(colors2);
            }

            if (flags != null)
            {
                Guard.IsEqualTo(flags.Count, vertices.Count);
                Flags = new MeshAttributes<uint>(flags);
            }

            BoundingBox = new AABBf(Vertices.AsReadOnlySpan);
        }

        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet => Vertices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool TangentsSet => Tangents?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bi tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bi tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool BiTangentsSet => BiTangents?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool Colors0Set => Colors0?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool Colors1Set => Colors1?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool Colors2Set => Colors2?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet => Normals?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet => UVs?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => BoneWeights?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet => BoneIndices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet => Triangles?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether flags are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool FlagsSet => Flags?.Length > 0;

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox { internal set; get; }

        /// <summary>
        /// The type of mesh which is represented by this instance (e. g. triangle mesh, point, line, etc...)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        #region IDisposable Support

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
                DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~Mesh()
        {
            Dispose(false);
        }

        #endregion

    }
}