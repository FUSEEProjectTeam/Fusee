using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class SimpleType
    {
        [ProtoMember(1)]
        public SceneHeaderTest Header;
    }
}


[ProtoContract]
public struct SceneHeaderTest
{
    /// <summary>
    /// Version number.
    /// </summary>
    [ProtoMember(1)]
    public int Version;
    [ProtoMember(2)]
    public string Generator;
    /// <summary>
    /// Name of the creator.
    /// </summary>
    [ProtoMember(3)]
    public string CreatedBy;
    /// <summary>
    /// Date of creation.
    /// </summary>
    [ProtoMember(4)]
    public string CreationDate;
}
