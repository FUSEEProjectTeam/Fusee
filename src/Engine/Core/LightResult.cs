using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This struct saves a light and all its parameters, as found by a Visitor.
    /// </summary>
    internal class LightResult
    {
        /// <summary>
        /// The light component as present (1 to n times) in the scene graph.
        /// </summary>
        public Light Light { get; private set; }

        public bool ReRenderShadowMap = true;

        /// <summary>
        /// It should be possible for one instance of type LightComponent to be used multiple times in the scene graph.
        /// Therefore the LightComponent itself has no position information - it gets set while traversing the scene graph.
        /// </summary>
        public float3 WorldSpacePos
        {
            get { return _worldSpacePos; }
            set
            {
                if (value == _worldSpacePos) return;
                _worldSpacePos = value;
                ReRenderShadowMap = true;
            }
        }
        private float3 _worldSpacePos;

        /// <summary>
        /// The rotation matrix. Determines the direction of the light, also set while traversing the scene graph.
        /// </summary>
        public float4x4 Rotation
        {
            get { return _rotation; }
            set
            {
                if (value == _rotation) return;
                _rotation = value;
                ReRenderShadowMap = true;
            }
        }
        private float4x4 _rotation;

        /// <summary>
        /// The session unique identifier of tis LightResult.
        /// </summary>
        public Suid Id;

        /// <summary>
        /// Creates a new instance of type LightResult.
        /// </summary>
        /// <param name="light">The LightComponent.</param>
        public LightResult(Light light)
        {
            Light = light;
            WorldSpacePos = float3.Zero;
            Rotation = float4x4.Identity;
            Id = Suid.GenerateSuid();
        }

        /// <summary>
        /// Override for the Equals method.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>       
        public override bool Equals(object obj)
        {
            var lc = (LightResult)obj;
            return Id.Equals(lc.Id);
        }

        /// <summary>
        /// Override of the == operator.
        /// </summary>
        /// <param name="thisLc">The first LightResult that will be compared with a second one.</param>
        /// <param name="otherLc">The second LightResult that will be compared with the first one.</param>        
        public static bool operator ==(LightResult thisLc, LightResult otherLc)
        {
            return otherLc.Id.Equals(thisLc.Id);
        }

        /// <summary>
        /// Override of the != operator.
        /// </summary>
        /// <param name="thisLc">The first LightResult that will be compared with a second one.</param>
        /// <param name="otherLc">The second LightResult that will be compared with the first one.</param>
        public static bool operator !=(LightResult thisLc, LightResult otherLc)
        {
            return !otherLc.Id.Equals(thisLc.Id);
        }

        /// <summary>
        /// Override of the GetHashCode method.
        /// Returns the session unique identifier as hash code.
        /// </summary>  
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}