using System;

namespace Fusee.Serialization
{
    public class MeshDataEventArgs : EventArgs
    {
        private readonly Mesh _mesh;
        private readonly MeshChangedEnum _meshChangedEnum;

        public Mesh Mesh{
            get { return _mesh; }
        }

        public MeshChangedEnum ChangedEnum
        {
            get { return _meshChangedEnum; }
        }

        public MeshDataEventArgs(Mesh mesh, MeshChangedEnum meshChangedEnum)
        {
            _mesh = mesh;
            _meshChangedEnum = meshChangedEnum;
        }
    }
}