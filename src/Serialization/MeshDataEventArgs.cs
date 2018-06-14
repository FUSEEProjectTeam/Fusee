using System;

namespace Fusee.Serialization
{
    /// <summary>
    /// EventArgs to propagate changes of a <see cref="Mesh"/> object's life cycle and property changes.
    /// </summary>
    public class MeshDataEventArgs : EventArgs
    {
        private readonly Mesh _mesh;
        private readonly MeshChangedEnum _meshChangedEnum;

        /// <summary>
        /// The <see cref="Mesh"/> that triggered the event.
        /// </summary>
        public Mesh Mesh{
            get { return _mesh; }
        }

        /// <summary>
        /// Description enum providing details about what property of the Mesh changed.
        /// </summary>
        public MeshChangedEnum ChangedEnum
        {
            get { return _meshChangedEnum; }
        }

        /// <summary>
        /// Constructor takes a Mesh and a description which property of the mesh changed.
        /// </summary>
        /// <param name="mesh">The Mesh which property of life cycle has changed.</param>
        /// <param name="meshChangedEnum">The <see cref="MeshChangedEnum"/> describing which property of the Mesh changed.</param>
        public MeshDataEventArgs(Mesh mesh, MeshChangedEnum meshChangedEnum)
        {
            _mesh = mesh;
            _meshChangedEnum = meshChangedEnum;
        }
    }
}