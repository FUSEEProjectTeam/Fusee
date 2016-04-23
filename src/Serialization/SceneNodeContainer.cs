using System.Collections.Generic;
using System.Diagnostics;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// The building block to create hierarchies.
    /// </summary>
    [ProtoContract]
    [DebuggerDisplay("{Name,nq}, {(Components==null)? \"No\" : Components.Count.ToString(),nq} components, {(Children==null)? \"No\" : Children.Count.ToString(),nq} children")]
    public class SceneNodeContainer
    {
        /// <summary>
        /// The name.
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// The components this node is made of.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public List<SceneComponentContainer> Components;

        /// <summary>
        /// Possible children. 
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public List<SceneNodeContainer> Children;
     }
}
