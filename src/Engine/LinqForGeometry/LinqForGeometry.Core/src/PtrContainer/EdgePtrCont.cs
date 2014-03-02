/*
    Author: Dominik Steffen
    E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
    Bachelor Thesis Summer Semester 2013
    'Computer Science in Media'
    Project: LinqForGeometry
    Professors:
    Mr. Prof. C. Müller
    Mr. Prof. W. Walter
*/

using LinqForGeometry.Core.Handles;

namespace LinqForGeometry.Core.PtrContainer
{
    /// <summary>
    /// This is a edge 'Pointer Container'.
    /// The Reference to the next 'object' are stored in here depending on the 'half-edge data strucute'
    /// Every Vertex has a reference to one of his outgoing 'Half-Edges'
    /// </summary>
    internal struct EdgePtrCont
    {
        internal HandleHalfEdge _he1;
        internal HandleHalfEdge _he2;
    }
}