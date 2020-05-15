namespace Fusee.Jometri
{
    /// <summary>
    /// <para>
    /// The types in this module implement geometry manipulation functionality.
    /// </para>
    /// <para>
    /// Geometry manipulation algorithms need to quickly access information about 
    /// a geometry's topology including queries such as "Which edges are connected to a vertex",
    /// "Retrieve all vertices for a given polygon", "Retrieve all adjacent polygons".
    /// </para>
    /// <para>
    /// The main type provided in this module is <see cref="Geometry"/> internally keeping
    /// geometry data in a data structure called DECL (doubly connected (half) edge list)
    /// allows for such queries. Additionally, a number of geometric algorithms are implemented here.
    /// </para>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}
