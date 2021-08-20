using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Defines a weight map. Basically a table with a row for each vertex and a column for each bone
    /// controlling the geometry. 
    /// </summary>
    public class Weight : SceneComponent
    {
        /// <summary>
        /// The weight map. Contains as many entries as the object containing this node's geometry has vertices.
        /// </summary>       
        public List<VertexWeightList> WeightMap = new();

        /// <summary>
        /// The joint objects controlling the geometry.
        /// </summary>       
        public List<SceneNode> Joints = new();

        /// <summary>
        /// The binding matrices defining the object's untransformed state.
        /// </summary>
        public List<float4x4> BindingMatrices = new();
    }

    /// <summary>
    /// A single row of the weight table. Stored in a way respecting sparse data (only few bones control a vertex).
    /// </summary>
    public class VertexWeightList
    {
        /// <summary>
        /// List of bones controlling the vertex.
        /// </summary>        
        public List<VertexWeight> VertexWeights { get; set; }
    }


    /// <summary>
    /// A single entry of a weight table row.
    /// </summary>
    public struct VertexWeight
    {
        /// <summary>
        /// The joint index controlling the vertex.
        /// </summary>
        public int JointIndex;

        /// <summary>
        /// The weight (the influence) of the bone on the vertex's transformation.
        /// </summary>
        public float Weight;

        /// <summary>
        /// Check if two vertex weights are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is VertexWeight w && w.JointIndex.Equals(JointIndex) && w.Weight.Equals(Weight);

        /// <summary>
        /// Returns the hash code of one vertex weight
        /// </summary>        
        public override int GetHashCode() => (7 * JointIndex.GetHashCode()) + 5 * Weight.GetHashCode();

        /// <summary>
        /// Check if two vertex weights are the same
        /// </summary>       
        public static bool operator ==(VertexWeight left, VertexWeight right) => left.Equals(right);

        /// <summary>
        /// Check if two vertex weights aren't the same
        /// </summary>       
        public static bool operator !=(VertexWeight left, VertexWeight right) => !(left == right);
    }

}