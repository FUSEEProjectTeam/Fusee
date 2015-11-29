using ProtoBuf;

namespace Fusee.Serialization
{
    // Protobuf-Sharp and Inheritance...
    // ...seem to be somewhat conflicting.
    // In C# an inherited (child-) class needs to know its parent.
    // In Protobuf every parent-class needs to be informed about all its possible inherited children classes
    // using [ProtoInclude(id, typeof(ChildClass))]. See 
    // http://www.codeproject.com/Articles/642677/Protobuf-net-the-unofficial-manual
    // Components will be the building blocks to generate user-defined data, save it in some
    // editor with the rest of standard scene data. This requires user-code to inherit from SceneComponentContainer
    // and create protobuf serialization code for inherited classes.
    // Here is a solution sketched for this scenario:
    // Instead of the ProtoInclude-Attribute call 
    // RuntimeTypeModel.Default[typeof(BaseClass)].AddSubType(id, typeof(DerivedClass));. See:
    // http://stackoverflow.com/questions/6247513/protobuf-net-inheritance
    // The required class RuntimeTypeModel ist currently NOT built with the FUSEE-Build of Protobuf. This is because
    // the "NO_RUNTIME" compiler option is SET in the build configuration. 
    // TODO: Find out if unsetting this NO_RUNTIME option will result in a Protobuf-net.dll capable of being x-compiled with JSIL.


    /// <summary>
    /// Base class for components. Each node (<see cref="SceneNodeContainer"/>) contains a list of components of various types.
    /// </summary>
    [ProtoContract]

    [ProtoInclude(100, typeof(TransformComponent))]
    [ProtoInclude(101, typeof(MeshComponent))]
    [ProtoInclude(102, typeof(MaterialComponent))]
    [ProtoInclude(103, typeof(LightComponent))]
    [ProtoInclude(104, typeof(WeightComponent))]
    [ProtoInclude(105, typeof(AnimationComponent))]
    [ProtoInclude(106, typeof(BoneComponent))]
    public class SceneComponentContainer
    {
        /// <summary>
        /// The name of this component.
        /// </summary>
        [ProtoMember(1)]
        public string Name;
    }
}
