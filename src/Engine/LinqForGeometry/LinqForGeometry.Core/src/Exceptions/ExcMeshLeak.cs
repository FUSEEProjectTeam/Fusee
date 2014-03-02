using System;

namespace LinqForGeometry.Core.Exceptions
{
    /// <summary>
    /// This is an exception used in the importer.
    /// </summary>
    class MeshLeakException : Exception
    {
        private const String _defMsg = "The mesh provided to the importer is leaked (Holes in structure). Please double check the mesh for integrity";

        public MeshLeakException(string msg = _defMsg)
        {
        }
    }
}
