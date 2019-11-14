using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Building block to set up User Interface hierarchies.
    /// Use this as your first element in the Interface hierarchy.
    /// </summary>
    [ProtoContract]
    public class CanvasTransformComponent : SceneComponentContainer
    {
        /// <summary>
        /// Rendermode of the canvas.(Worldspace or Screenspace)
        /// </summary>
        [ProtoMember(1)]
        public CanvasRenderMode CanvasRenderMode;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        [ProtoMember(2)]
        public MinMaxRect Size;

        /// <summary>
        /// Absolute size of the Canvas. First element in the interface hierarchy.
        /// </summary>
        [ProtoMember(3)]
        public MinMaxRect ScreenSpaceSize;

        /// <summary>
        /// Scale factor for the user given offsets that define the sizes if the canvas' child elements. This becomes important when rendering in SCREEN mode.        
        /// </summary>
        [ProtoMember(4)] public float2 Scale;

        /// <summary>
        /// Defines the canvas transform component in respect of the canvas render mode.
        /// </summary>
        /// <param name="canvasRenderMode">The canvas Render mode. <see cref="CanvasRenderMode"></see> </param>
        public CanvasTransformComponent(CanvasRenderMode canvasRenderMode)
        {
            CanvasRenderMode = canvasRenderMode;
            Size = ScreenSpaceSize;
        }
    }
    ///<summary>
    ///The rendermodes available.
    ///</summary>
    [ProtoContract]
    public enum CanvasRenderMode
    {
        /// <summary>
        /// Worldspace, the canvas is rendered as part of the scene and can be positioned in 3D.
        /// </summary>
        WORLD,
        /// <summary>
        /// Screenspace, the canvas is rendered in a the top layer of the scene. 
        /// </summary>
        SCREEN
    }
}
