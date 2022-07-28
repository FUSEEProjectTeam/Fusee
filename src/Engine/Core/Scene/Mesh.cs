using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using Microsoft.Toolkit.Diagnostics;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IManagedMesh, IDisposable
    {
        #region RenderContext Asset Management

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> MeshChanged;

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        #endregion

        #region Private mesh data member

        protected float4[] _boneWeights;
        protected float4[] _boneIndices;
        protected float4[] _tangents;

        protected float3[] _biTangents;
        protected float3[] _vertices;
        protected float3[] _normals;

        protected float2[] _uvs;

        protected ushort[] _triangles;
        protected uint[] _colors;
        protected uint[] _colors1;
        protected uint[] _colors2;

        #endregion

        #region Is Set

        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerticesSet => _vertices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsTangentsSet => _tangents?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bi tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bi tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsBiTangentsSet => _biTangents?.Length > 0;


        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsColorsSet => _colors?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsColorsSet1 => _colors1?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsColorsSet2 => _colors2?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsNormalsSet => _normals?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsUVsSet => _uvs?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsBoneWeightsSet => _boneWeights?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsBoneIndicesSet => _boneIndices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrianglesSet => _triangles?.Length > 0;

        #endregion

        #region Public Getter

        /// <summary>
        /// Returns the bone weights.
        /// </summary>
        public ReadOnlySpan<float4> BoneWeights => new(_boneWeights);
        /// <summary>
        /// Returns the bone indices.
        /// </summary>
        public ReadOnlySpan<float4> BoneIndices => new(_boneIndices);

        /// <summary>
        /// Returns the tangents.
        /// </summary>
        public ReadOnlySpan<float4> Tangents => new(_tangents);
        /// <summary>
        /// Returns the bitangents.
        /// </summary>
        public ReadOnlySpan<float3> BiTangents => new(_biTangents);

        /// <summary>
        /// Returns the vertices.
        /// </summary>
        public ReadOnlySpan<float3> Vertices => new(_vertices);
        /// <summary>
        /// Returns normals.
        /// </summary>
        public ReadOnlySpan<float3> Normals => new(_normals);

        /// <summary>
        /// Returns the UV coordinates.
        /// </summary>
        public ReadOnlySpan<float2> UVs => new(_uvs);

        /// <summary>
        /// Returns the triangles.
        /// </summary>
        public ReadOnlySpan<ushort> Triangles => new(_triangles);

        /// <summary>
        /// Returns the vertex color.
        /// </summary>
        public ReadOnlySpan<uint> Colors => new(_colors);

        /// <summary>
        /// Returns the vertex color1.
        /// </summary>
        public ReadOnlySpan<uint> Colors1 => new(_colors1);

        /// <summary>
        /// Returns the vertex color2.
        /// </summary>
        public ReadOnlySpan<uint> Colors2 => new(_colors2);

        #endregion

        #region BeginEdit() / EndEdit()

        /// <summary>
        /// <see langword="true"/> if BeginEdit() has been called
        /// <see langword="false"/> if EndEdit() has been called
        /// </summary>
        public bool IsEditActive { get; private set; }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_boneWeights"/> for edit purposes
        /// Do not forget to call <see cref="EndEditBoneWeights()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float4> BeginEditBoneWeights()
        {
            IsEditActive = true;
            return _boneWeights.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_boneIndices"/> for edit purposes
        /// Do not forget to call <see cref="EndEditBoneIndices()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float4> BeginEditBoneIndices()
        {
            IsEditActive = true;
            return _boneIndices.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_tangents"/> for edit purposes
        /// Do not forget to call <see cref="EndEditTangents()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float4> BeginEditTangents()
        {
            IsEditActive = true;
            return _tangents.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_biTangents"/> for edit purposes
        /// Do not forget to call <see cref="EndEditBiTangents()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float3> BeginEditBiTangents()
        {
            IsEditActive = true;
            return _biTangents.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_vertices"/> for edit purposes
        /// Do not forget to call <see cref="EndEditVertices()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float3> BeginEditVertices()
        {
            IsEditActive = true;
            return _vertices.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_normals"/> for edit purposes
        /// Do not forget to call <see cref="EndEditNormals()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float3> BeginEditNormals()
        {
            IsEditActive = true;
            return _normals.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_uvs"/> for edit purposes
        /// Do not forget to call <see cref="EndEditUvs()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<float2> BeginEditUvs()
        {
            IsEditActive = true;
            return _uvs.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_triangles"/> for edit purposes
        /// Do not forget to call <see cref="EndEditTriangles()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<ushort> BeginEditTriangles()
        {
            IsEditActive = true;
            return _triangles.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_colors"/> for edit purposes
        /// Do not forget to call <see cref="EndEditColors()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<uint> BeginEditColors()
        {
            IsEditActive = true;
            return _colors.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_colors1"/> for edit purposes
        /// Do not forget to call <see cref="EndEditColors1()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<uint> BeginEditColors1()
        {
            IsEditActive = true;
            return _colors1.AsSpan();
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> off the backing field <see cref="_colors2"/> for edit purposes
        /// Do not forget to call <see cref="EndEditColors2()"/>!
        /// Changes are only visible after this method has been called!
        /// </summary>
        /// <returns></returns>
        public Span<uint> BeginEditColors2()
        {
            IsEditActive = true;
            return _colors2.AsSpan();
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditBoneWeights()
        {
            IsEditActive = true;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditBoneIndices()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditTangents()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditBiTangents()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditVertices()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditNormals()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditUvs()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditTriangles()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditColors()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditColors1()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
        }

        /// <summary>
        /// Call this method after editing, calls the proper events to update visible changes
        /// </summary>
        public void EndEditColors2()
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditBoneWeights(int idx)
        {
            IsEditActive = true;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditBoneIndices(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditTangents(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditBiTangents(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditVertices(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices, idx));
        }
        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditNormals(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditUvs(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditTriangles(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditColors(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditColors1(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="idx">The index of the changed element</paramref>
        public void EndEditColors2(int idx)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2, idx));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditBoneWeights(Tuple<int, int> startEndIndices)
        {
            IsEditActive = true;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditBoneIndices(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditTangents(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditBiTangents(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditVertices(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices, startEndIndices));
        }
        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditNormals(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditUvs(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditTriangles(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditColors(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditColors1(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditColors2(Tuple<int, int> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">The start and end indices of the changed element</paramref>
        public void EndEditBoneWeights(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = true;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditBoneIndices(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditTangents(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditBiTangents(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditVertices(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices, startEndIndices));
        }
        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditNormals(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditUvs(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditTriangles(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditColors(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditColors1(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1, startEndIndices));
        }

        /// <summary>
        /// Call this method after editing a single element, calls the proper events to update visible changes
        /// </summary>
        /// <paramref name="startEndIndices">A series of start and end indices of the changed element</paramref>
        public void EndEditColors2(IEnumerable<Tuple<int, int>> startEndIndices)
        {
            IsEditActive = false;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2, startEndIndices));
        }

        #endregion

        #region Replace

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newBoneWeights"></param>
        public void ReplaceBoneWeights(float4[] newBoneWeights)
        {
            _boneWeights = newBoneWeights;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newBoneIndices"></param>
        public void ReplaceBoneIndices(float4[] newBoneIndices)
        {
            _boneIndices = newBoneIndices;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newTangents"></param>
        public void ReplaceTangents(float4[] newTangents)
        {
            _tangents = newTangents;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newBiTangents"></param>
        public void ReplaceBiTangents(float3[] newBiTangents)
        {
            _biTangents = newBiTangents;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newVertices"></param>
        public void ReplaceVertices(float3[] newVertices)
        {
            _vertices = newVertices;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newNormals"></param>
        public void ReplaceNormals(float3[] newNormals)
        {
            _normals = newNormals;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newUVs"></param>
        public void ReplaceUVs(float2[] newUVs)
        {
            _uvs = newUVs;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newTriangles"></param>
        public void ReplaceTriangles(ushort[] newTriangles)
        {
            _triangles = newTriangles;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
        }

        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newColors"></param>
        public void ReplaceColors(uint[] newColors)
        {
            _colors = newColors;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
        }


        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newColors"></param>
        public void ReplaceColors1(uint[] newColors)
        {
            _colors1 = newColors;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
        }


        /// <summary>
        /// Replaces the backing field
        /// </summary>
        /// <param name="newColors"></param>
        public void ReplaceColors2(uint[] newColors)
        {
            _colors2 = newColors;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
        }

        #endregion

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox;

        /// <summary>
        /// The type of mesh which is represented by this instance (e. g. triangle mesh, point, line, etc...)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        /// <summary>
        /// Empty ctor for inheritance
        /// </summary>
        protected Mesh()
        {

        }

        /// <summary>
        /// Generate a new <see cref="Mesh"/> instance
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="normals"></param>
        /// <param name="uvs"></param>
        /// <param name="boneWeights"></param>
        /// <param name="boneIndices"></param>
        /// <param name="tangents"></param>
        /// <param name="biTangents"></param>
        /// <param name="colors"></param>
        /// <param name="colors1"></param>
        /// <param name="colors2"></param>
        public Mesh(float3[] vertices, ushort[] triangles, float3[] normals = null, float2[] uvs = null, float4[] boneWeights = null, float4[] boneIndices = null, float4[] tangents = null, float3[] biTangents = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null)
        {
            // check that normals, uvs and colors are equal length to vertices
            if (normals != null)
                Guard.IsEqualTo(vertices.Length, normals.Length, nameof(normals));
            if (uvs != null)
                Guard.IsEqualTo(vertices.Length, uvs.Length, nameof(uvs));
            if (colors != null)
                Guard.IsEqualTo(vertices.Length, colors.Length, nameof(colors));
            if (colors1 != null)
                Guard.IsEqualTo(vertices.Length, colors1.Length, nameof(colors1));
            if (colors2 != null)
                Guard.IsEqualTo(vertices.Length, colors2.Length, nameof(colors2));

            _boneWeights = boneWeights;
            _boneIndices = boneIndices;
            _tangents = tangents;
            _biTangents = biTangents;
            _vertices = vertices;
            _normals = normals;
            _uvs = uvs;
            _triangles = triangles;
            _colors = colors;
            _colors1 = colors1;
            _colors2 = colors2;

            MeshType = PrimitiveType.Triangles;
        }

        /// <summary>
        /// Generate a new <see cref="Mesh"/> instance with empty backing files as large as the given capacity
        /// </summary>
        /// <param name="capacity">capacity of each element in the mesh</param>
        /// <param name="triangleCapacity">capacity of triangles in the mesh</param>
        public Mesh(int capacity, int triangleCapacity)
        {
            _triangles = new ushort[triangleCapacity];

            _boneWeights = new float4[capacity];
            _boneIndices = new float4[capacity];
            _tangents = new float4[capacity];
            _biTangents = new float3[capacity];
            _vertices = new float3[capacity];
            _normals = new float3[capacity];
            _uvs = new float2[capacity];
            _colors = new uint[capacity];
            _colors1 = new uint[capacity];
            _colors2 = new uint[capacity];
        }



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
                    DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));
                }

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