
## New Serialization

This document describes the transition from the "old" (V1) FUSEE serialization scheme the new one.

### Conceptual Changes

V1 serialization utilizes "Container" classes and derivatives decorated for serialization with Protobuf dot Net's `[ProtoContract]` etc. attributes while at the same time using these classes directly at run-time as scene graph building blocks.



Since Protobuf dot Net version ?? protobuf does not support references within serializable properties. Allowing components within multiple nodes requires this feature. In addition at several places it was necessary to implement between scene graph building blocks used in serialization different from those used during traversal.

From V2 on, FUSEE's serialization is completely separated from scene graph building blocks in `Fusee.Engine`. As a result, `Fusee.Xene` was completely freed from all references and thus neither references `Fusee.Serialization` nor `Fusee.Engine`.

### New Files

Files added to `Engine.Common`

- SceneVisitor.cs
- SceneNode.cs
- SceneComponent.cs

V1 Serialization | V2 Serialization | `Engine.Common`
----|----|-------
 `SceneNodeContainer` | `FusNode` | `SceneNode`
 `SceneComponentContainer` | `FusComponent` | `SceneComponent`
 `TransformComponent` | `FusTransform` | `Transform`
`Mesh` | `FusMesh`| `Mesh`
`LightComponent` | `FusLight` | `Light`
`AnimationComponent` | `FusAnimation` | `Animation`
`BoneComponent` | `FusBone` | `Bone`


### TODO

- [ ] Remove MeshChangedEvent handling from V1 serialization Mesh
- [ ] Remove `Suid` from serialization
- [ ] Move animation setup (xirkit-building) from renderer to post-serialization transformation
- [ ] Move  to F.E.Common. :
  - OctantComponent
  - ProjectionComponent
  - Transformation FUS-Instanzen in Common-Instanzen
  - F.E.Core lauffähig machen




