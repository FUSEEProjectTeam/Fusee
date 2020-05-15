using System.Collections.Generic;

namespace Fusee.Xene
{
    /// <summary>
    /// Interface to be implemented by node types to be accessed by functionality in <see cref="Fusee.Xene"/>.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Retrieves the child nodes of this instance.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        IEnumerable<INode> EnumChildren { get; }

        /// <summary>
        /// Retrieves the components making up this node.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        IEnumerable<IComponent> EnumComponents { get; }
    }
}
