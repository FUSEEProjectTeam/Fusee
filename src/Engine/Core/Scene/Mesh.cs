using CommunityToolkit.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection.Emit;

namespace Fusee.Engine.Core.Scene
{
    public sealed class DirtyIndexList
    {
        public void Init() {
            IndexList = new List<(int, int)>();
        }

        public void ResetList() => Init();

        public void Add(int idx) => IndexList.Append((idx, idx));

        public void Add((int, int) idx) => IndexList.Append(idx);

        public IEnumerable<(int, int)> IndexList { get; private set; }

        public bool Empty => IndexList.Count() == 0;
    }

    public sealed class MeshAttributes<T> where T : struct
    {
        private readonly T[] _attribData;

        public readonly DirtyIndexList DirtyIndices = new();

        public int Length => _attribData.Length;

        public MeshAttributes(in IEnumerable<T> data)
        {
            _attribData = data.ToArray();
            DirtyIndices.Init();
        }

        /// <summary>
        /// Quick readonly access to data
        /// Use <see cref="MeshAttributes{T}"/> indexer for read/write access
        /// </summary>
        public ReadOnlySpan<T> ReadOnlyData => new(_attribData);

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
                // TODO: Track changed indices in a fast and memory efficient way
                if (DirtyIndices.Empty)
                    DirtyIndices.Add(0); /// add one element to notify <see cref="MeshManager"> that values have been changed
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
            Guard.IsLessThan(calculatedLength, _attribData.Length);

            Array.Fill(_attribData, data, start, calculatedLength);
            DirtyIndices.Add((0, calculatedLength - 1));
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
            DirtyIndices.Add((start, _attribData.Length - 1));
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
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        public bool UpdatePerFrame { set; get; } = true;

        public void UpdateGPU()
        {


            if(!Triangles.DirtyIndices.Empty)
            {

            }

            // reset index lists
            /*
            Pos._dirtyIndexList.Init();
            Label._dirtyIndexList.Init();
            Instance._dirtyIndexList.Init();
            GPSTime._dirtyIndexList.Init();
            Color._dirtyIndexList.Init();
            */
        }

        #endregion

        #region Mesh data member

        public  MeshAttributes<uint> Triangles { get; protected set; }
        public  MeshAttributes<float3> Vertices { get; protected set; }
        public  MeshAttributes<float3>? Normals { get; protected set; }
        public  MeshAttributes<float2>? UVs { get; protected set; }

        public  MeshAttributes<float4>? BoneWeights { get; internal set; } // set via SceneRenderer
        public  MeshAttributes<float4>? BoneIndices { get; internal set; } // set via SceneRenderer

        public  MeshAttributes<float4>? Tangents { get; internal set; } // set via SceneRenderer
        public  MeshAttributes<float3>? BiTangents { get; internal set; } // set via SceneRenderer

        public  MeshAttributes<uint>? Colors { get; protected set; }
        public  MeshAttributes<uint>? Colors1 { get; protected set; }
        public  MeshAttributes<uint>? Colors2 { get; protected set; }

        #endregion

        protected Mesh() { }

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
        public Mesh
        (
            IEnumerable<uint> triangles,
            IEnumerable<float3> vertices,
            IEnumerable<float3>? normals = null,
            IEnumerable<float2>? uvs = null,
            IEnumerable<float4>? boneWeights = null,
            IEnumerable<float4>? boneIndices = null,
            IEnumerable<float4>? tangents = null,
            IEnumerable<float3>? biTangents = null,
            IEnumerable<uint>? colors = null,
            IEnumerable<uint>? colors1 = null,
            IEnumerable<uint>? colors2 = null)
        {
            Guard.IsGreaterThan(triangles.Count(), 0);
            Guard.IsGreaterThan(vertices.Count(), 0);

            Triangles = new MeshAttributes<uint>(triangles);
            Vertices = new MeshAttributes<float3>(vertices);

            if (normals != null)
            {
                Guard.IsEqualTo(normals.Count(), vertices.Count());
                Normals = new MeshAttributes<float3>(normals);
            }

            if (uvs != null)
            {
                Guard.IsEqualTo(uvs.Count(), vertices.Count());
                UVs = new MeshAttributes<float2>(uvs);
            }

            if (boneWeights != null)
            {
                Guard.IsEqualTo(boneWeights.Count(), vertices.Count());
                BoneWeights = new MeshAttributes<float4>(boneWeights);
            }

            if (boneIndices != null)
            {
                Guard.IsEqualTo(boneIndices.Count(), vertices.Count());
                BoneIndices = new MeshAttributes<float4>(boneIndices);
            }

            if (tangents != null)
            {
                Guard.IsEqualTo(tangents.Count(), vertices.Count());
                Tangents = new MeshAttributes<float4>(tangents);
            }

            if (biTangents != null)
            {
                Guard.IsEqualTo(biTangents.Count(), vertices.Count());
                BiTangents = new MeshAttributes<float3>(biTangents);
            }

            if (colors != null)
            {
                Guard.IsEqualTo(colors.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors);
            }

            if (colors1 != null)
            {
                Guard.IsEqualTo(colors1.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors1);
            }

            if (colors2 != null)
            {
                Guard.IsEqualTo(colors2.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors2);
            }
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
        public bool ColorsSet => Colors?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 => Colors1?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 => Colors2?.Length > 0;

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