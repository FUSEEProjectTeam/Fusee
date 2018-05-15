using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class NineSliceComponent : SceneComponentContainer
    {
        /// <summary>
        /// The left border of the NineSliceComponent
        /// </summary>
        [ProtoMember(1)]
        public float L;

        /// <summary>
        /// The right border of the NineSliceComponent
        /// </summary>
        [ProtoMember(2)]
        public float R;

        /// <summary>
        /// The top border of the NineSliceComponent
        /// </summary>
        [ProtoMember(3)]
        public float T;

        /// <summary>
        /// The bottom border of the NineSliceComponent
        /// </summary>
        [ProtoMember(4)]
        public float B;

        /// <summary>
        /// Creates a new instance of the NineSliceComponent.
        /// </summary>
        /// <param name="l">The left border.</param>
        /// <param name="r">The right border.</param>
        /// <param name="x">The top border.</param>
        /// <param name="b">The bottom border.</param>
        public NineSliceComponent(float l, float r, float x, float b)
        {
            L = l;
            R = r;
            T = x;
            B = b;
        }
    }
}
