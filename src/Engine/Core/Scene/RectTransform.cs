using Fusee.Math.Core;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// </summary>
    public class RectTransform : SceneComponent
    {
        /// <summary>
        /// Per-cent setting where to place the anchor point in respect to 
        /// the area defined by the parent node.
        /// </summary>
        public MinMaxRect Anchors;
        /// <summary>
        /// Absolute offset values added to the anchor points.
        /// </summary>
        public MinMaxRect Offsets;
    }
}