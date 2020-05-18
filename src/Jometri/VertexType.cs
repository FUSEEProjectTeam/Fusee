namespace Fusee.Jometri
{
    /// <summary>
    /// To divide a polygon into y monotone pieces, the corners of this polygon must be divided into different categories.
    /// Those are start, end, split, merge, and regular vertices.
    /// </summary>
    public enum VertexType
    {
        /// <summary>
        /// A vertex is a start vertex in case his two neighbors are below him and the interior angle between the two adjacent edges is smaller than pi.
        /// </summary>
        StartVertex,

        /// <summary>
        /// A vertex is a end vertex in case his two neighbors are above him and the interior angle between the two adjacent edges is smaller than pi.
        /// </summary>
        EndVertex,

        /// <summary>
        /// A vertex is a split vertex in case his two neighbors are below him and the interior angle between the two adjacent edges is greater than pi.
        /// </summary>
        SplitVertex,

        /// <summary>
        /// A vertex is a merge vertex in case his two neighbors are above him and the interior angle between the two adjacent edges is greater than pi.
        /// </summary>
        MergeVertex,

        /// <summary>
        /// A vertex is a regular vertex in case his two neighbors and the adjacent edges do not meet one of the conditions defining the other types.
        /// </summary>
        RegularVertex
    }
}
