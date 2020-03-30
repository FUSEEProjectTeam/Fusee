using Fusee.Xene;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Base type for all Components used as building blocks within <see cref="Fusee.Engine"/>.
    /// </summary>
    /// <seealso cref="Fusee.Xene.IComponent" />
    public class SceneComponent : IComponent
    {
        /// <summary>
        /// The components name. Might serve as identifier in find traversals.
        /// </summary>
        /// <seealso cref="SceneExtensions.FindComponents(SceneNode, System.Predicate{SceneComponent})"/>
        public string Name;
    }
}