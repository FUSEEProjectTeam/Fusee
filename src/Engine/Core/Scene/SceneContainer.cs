using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// The header of a scene.
    /// </summary>
    public struct SceneHeader
    {
        /// <summary>
        /// The generator used to create this scene.
        /// </summary>
        public string Generator;

        /// <summary>
        /// The user who created this scene.
        /// </summary>
        public string CreatedBy;

        /// <summary>
        /// The creation date of this scene.
        /// </summary>
        public string CreationDate;
    }

    /// <summary>
    /// The root object of a scene file.
    /// </summary>
    public class SceneContainer
    {
        /// <summary>
        /// The header containing meta information about the scene.
        /// </summary>
        public SceneHeader Header;

        /// <summary>
        /// The list of child nodes. Each can contain children itself.
        /// </summary>
        public List<SceneNode> Children = new List<SceneNode>();
    }
}