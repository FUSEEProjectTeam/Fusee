namespace Fusee.Xene
{
    /// <summary>
    /// Interface to be implemented by component types to be accessed by functionality in <see cref="Xene"/>.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// If set to false, the SceneRenderer will ignore this component.
        /// </summary>0
        bool Active { get; set; }
    }
}