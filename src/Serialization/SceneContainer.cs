using System.Collections.Generic;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// The header of a scene file.
    /// </summary>
    [ProtoContract]
    public struct SceneHeader
    {
        /// <summary>
        /// The version number of this scene file.
        /// </summary>
        [ProtoMember(1)] 
        public int Version;

        /// <summary>
        /// The generator used to create this scene file.
        /// </summary>
        [ProtoMember(2)] 
        public string Generator;

        /// <summary>
        /// The user who created this scene.
        /// </summary>
        [ProtoMember(3)] 
        public string CreatedBy;

        /// <summary>
        /// The creation date of the file.
        /// </summary>
        [ProtoMember(4)]
        public string CreationDate; 
    }

    /// <summary>
    /// The root object of a scene file.
    /// </summary>
    [ProtoContract]
    public class SceneContainer
    {
        /// <summary>
        /// The file header.
        /// </summary>
        [ProtoMember(1)]
        public SceneHeader Header;

        /// <summary>
        /// The list of child nodes. Each can contain children itself.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public List<SceneNodeContainer> Children;

    }
}
