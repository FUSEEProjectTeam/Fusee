﻿using System.Collections.Generic;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Defines a weight map. Basically a table with a row for each vertex and a column for each bone
    /// controlling the geometry. 
    /// </summary>
    [ProtoContract]
    public class FusWeight : FusComponent
    {
        /// <summary>
        /// The weight map. Contains as many entries as the object containing this node's geometry has vertices.
        /// </summary>
        [ProtoMember(1)]
        public List<VertexWeightList> WeightMap = new List<VertexWeightList>();

        /// <summary>
        /// The joint objects controlling the geometry.
        /// </summary>
        [ProtoMember(2)]
        public List<FusComponent> Joints = new List<FusComponent>();

        /// <summary>
        /// The binding matrices defining the object's untransformed state.
        /// </summary>
        [ProtoMember(3)]
        public List<float4x4> BindingMatrices = new List<float4x4>();
   
    }

    /// <summary>
    /// A single entry of a weight table row.
    /// </summary>
    [ProtoContract]
    public struct VertexWeight
    {
        /// <summary>
        /// The joint index controlling the vertex.
        /// </summary>
        [ProtoMember(1)]
        public int JointIndex;

        /// <summary>
        /// The weight (the influence) of the bone on the vertex's transformation.
        /// </summary>
        [ProtoMember(2)]
        public float Weight;
    }

    /// <summary>
    /// A single row of the weight table. Stored in a way respecting sparse data (only few bones control a vertex).
    /// </summary>
    [ProtoContract]
    public class VertexWeightList
    {
        /// <summary>
        /// List of bones controlling the vertex.
        /// </summary>
        [ProtoMember(1)]
        public List<VertexWeight>? VertexWeights { get; set; }
    }


    // Deprecated/unused
    /// <summary>
    /// DEPRECATED
    /// </summary>
    [ProtoContract]
    public class JointWeightColumn
    {
        /// <summary>
        /// DEPRECATED: Gets and sets the joint weights.
        /// </summary>
        /// <value>
        /// The joint weights.
        /// </value>
        [ProtoMember(1)]
        public List<double>? JointWeights { get; set; }
    }
}
