using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{    
    /// <summary>
    /// One of the major building blocks for FUSEE's node-component system which is the basis for 
    /// all scene graph implementations. The other building block one is <see cref="ITreeComponent"/>.
    /// </summary>
    /// <remarks>
    /// To use all the visiting, viserating, state tracking and finding utilites implemented within fusee
    /// on any structure, make sure the structure implements ITreeNode and ITreeComponent interfaces 
    /// appropriately.
    /// </remarks>
    interface ITreeNode
    {        
        /// <summary>
        /// Retrieve the name of the node.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Enumerates the components stored within this node.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        IEnumerable<ITreeComponent> Components{ get; }

        /// <summary>
        /// Enumerates the child nodes stored within this node.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        IEnumerable<ITreeNode> Children { get; }
    }
}
