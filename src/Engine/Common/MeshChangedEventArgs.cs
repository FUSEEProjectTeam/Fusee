using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// EventArgs to propagate changes of a <see cref="Mesh"/> object's life cycle and property changes.
    /// </summary>
    public class MeshChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="Mesh"/> that triggered the event.
        /// </summary>
        public IManagedMesh Mesh { get; }

        /// <summary>
        /// Description enum providing details about what property of the Mesh changed.
        /// </summary>
        public MeshChangedEnum ChangedEnum { get; protected set; }

        /// <summary>
        /// Index of changed element
        /// </summary>
        public int ChangedIdx = -1;

        /// <summary>
        /// Index range of changed element
        /// </summary>
        public Tuple<int, int> ChangedRange;


        /// <summary>
        /// Ranges of index range of changed element
        /// </summary>
        public IEnumerable<Tuple<int, int>> ChangedRanges;


        /// <summary>
        /// Constructor takes a Mesh and a description which property of the mesh changed.
        /// </summary>
        /// <param name="mesh">The Mesh which property of life cycle has changed.</param>
        /// <param name="meshChangedEnum">The <see cref="MeshChangedEnum"/> describing which property of the Mesh changed.</param>
        public MeshChangedEventArgs(IManagedMesh mesh, MeshChangedEnum meshChangedEnum)
        {
            Mesh = mesh;
            ChangedEnum = meshChangedEnum;
        }

        /// <summary>
        /// Constructor takes a Mesh and a description which property of the mesh changed.
        /// </summary>
        /// <param name="mesh">The Mesh which property of life cycle has changed.</param>
        /// <param name="meshChangedEnum">The <see cref="MeshChangedEnum"/> describing which property of the Mesh changed. No <see cref="MeshChangedEnum.Disposed"/> allowed!</param>
        /// <param name="idx">Array index of changed element</param>
        public MeshChangedEventArgs(IManagedMesh mesh, MeshChangedEnum meshChangedEnum, int idx)
        {
            Guard.IsNotEqualTo((int)meshChangedEnum, (int)MeshChangedEnum.Disposed, nameof(meshChangedEnum));
            Guard.IsGreaterThanOrEqualTo(idx, 0, nameof(idx));

            Mesh = mesh;
            ChangedEnum = meshChangedEnum;
            ChangedIdx = idx;
        }

        /// <summary>
        /// Constructor takes a Mesh and a description which property of the mesh changed.
        /// </summary>
        /// <param name="mesh">The Mesh which property of life cycle has changed.</param>
        /// <param name="meshChangedEnum">The <see cref="MeshChangedEnum"/> describing which property of the Mesh changed. No <see cref="MeshChangedEnum.Disposed"/> allowed!</param>
        /// <param name="range">Array range of changed element</param>
        public MeshChangedEventArgs(IManagedMesh mesh, MeshChangedEnum meshChangedEnum, Tuple<int, int> range)
        {
            Guard.IsNotEqualTo((int)meshChangedEnum, (int)MeshChangedEnum.Disposed, nameof(meshChangedEnum));
            Guard.IsNotNull(range, nameof(range));
            Guard.IsGreaterThanOrEqualTo(range.Item1, 0, nameof(range));
            Guard.IsGreaterThanOrEqualTo(range.Item2, 0, nameof(range));
            Guard.IsGreaterThanOrEqualTo(range.Item2, range.Item1, nameof(range)); // start idx mustn't be greater than end

            Mesh = mesh;
            ChangedEnum = meshChangedEnum;
            ChangedRange = range;
        }

        /// <summary>
        /// Constructor takes a Mesh and a description which property of the mesh changed.
        /// </summary>
        /// <param name="mesh">The Mesh which property of life cycle has changed.</param>
        /// <param name="meshChangedEnum">The <see cref="MeshChangedEnum"/> describing which property of the Mesh changed. No <see cref="MeshChangedEnum.Disposed"/> allowed!</param>
        /// <param name="ranges">Array of changed ranges of changed element</param>
        public MeshChangedEventArgs(IManagedMesh mesh, MeshChangedEnum meshChangedEnum, IEnumerable<Tuple<int, int>> ranges)
        {
            Guard.IsNotEqualTo((int)meshChangedEnum, (int)MeshChangedEnum.Disposed, nameof(meshChangedEnum));
            Guard.IsNotNull(ranges, nameof(ranges));

            // check that each start, end idx is greater than 0
            // and start is not greater than end
            foreach(var range in ranges)
            {
                Guard.IsGreaterThanOrEqualTo(range.Item1, 0, nameof(range));
                Guard.IsGreaterThanOrEqualTo(range.Item2, 0, nameof(range));
                Guard.IsGreaterThanOrEqualTo(range.Item2, range.Item1, nameof(range));
            }

            Mesh = mesh;
            ChangedEnum = meshChangedEnum;
            ChangedRanges = ranges;
        }
    }
}