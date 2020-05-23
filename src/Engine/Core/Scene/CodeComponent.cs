
namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Use this component to add Code to Objects in the scene graph. Will not serialize/deserialize
    /// </summary>
    public class CodeComponent : SceneComponent
    {
        /// <summary>
        /// Tells if the mouse is over the SceneNodeContainer this code component belongs to.
        /// </summary>
        public bool IsMouseOver;

        /// <summary>
        ///     A delegation for the event listeners of a <see cref="CodeComponent" />.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void InteractionHandler(CodeComponent sender);

    }
}