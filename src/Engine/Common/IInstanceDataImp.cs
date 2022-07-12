
namespace Fusee.Engine.Common
{
    /// <summary>
    /// Platform abstraction for instance data implementations. These must be able to store the references to the instance buffer objects.
    /// </summary>
    public interface IInstanceDataImp
    {
        /// <summary>
        /// The number of instances that will be rendered.
        /// </summary>
        public int Amount { get; set; }
    }
}