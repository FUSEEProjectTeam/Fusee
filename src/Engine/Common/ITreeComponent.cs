using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// One of the major building blocks for FUSEE's node-component system which is the basis for 
    /// all scene graph implementations. The other building block one is <see cref="ITreeNode"/>.
    /// </summary>
    interface ITreeComponent
    {
        /// <summary>
        /// Retrieve the name of the node.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
    }
}
