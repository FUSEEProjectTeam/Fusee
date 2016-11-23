using System;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// This excaption is thrown when the given handle of a HalfEdge Vertice or Face is not found.
    /// </summary>
    public class HandleNotFoundException : Exception
    {
        /// <summary>
        /// This excaption is thrown if the given handle of a HalfEdge Vertice or Face is not found.
        /// </summary>
        public HandleNotFoundException(string msg)
        {
        }
    }

    /// <summary>
    /// This excaption is thrown when the given HalfEdge already exists
    /// </summary>
    public class DublicatedHalfEdgeException : Exception
    {
        /// <summary>
        /// This excaption is thrown when the given HalfEdge already exists
        /// </summary>
        public DublicatedHalfEdgeException(string msg)
        {
        }
    }

}

