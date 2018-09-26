FUSEE Resource System

Mesh, Texture and Shaders/ShaderEffect are objects handled in Application code, at some point
passed to the RenderContext (if not even created by the RC), resulting in hardware resources
being allocated on GPU RAM. In the following we'll call this type of data ***Resources***

## Requirements

- Users can freely create Mesh, Texture and ShaderEffect objects with new. 

- Mesh, Texture and ShaderEffect instances can be created and can live without an existing RenderContext.

- The Render Context does NOT contain any Creation or Destruction methods for these objects.

- The Render Context DOES contain methods such as Render(Mesh mesh) and SetTexture(Texture tex)
  methods that propagate the respective objects to the rendering pipeline.
  
- To be specified: How about RenderTargetTextures, VideoTextures which somehow releate to
  texture but EITHER retrieve their contents from the RenderEngine OR have some sort of
  fast-lane pass-through for image data.
  
- Mesh, Texture and ShaderEffect Objects are scene graph agnostic, 
  meaning they CAN be used without the context of scene graph objects.
  
- Users can alter Mesh, Texture and ShaderEffect contents, e.g.
  - Change positions of vertices and other Mesh data
  - Copy (parts) of pixel data onto textures
  - Change ShaderEffect parameter values, RenderStates or even shader code.
  Any changes to 
  
- Nice To Have / To be specified in more detail / Keep in mind
  - Allow users to create their own types representing mesh geometry. The reason to allow this
    is to allow arbitrary mesh data e.g. Meshes with
    - arbitrary combinations of UVs, Normals Color.
    - more than one UV set.
    - custom data at each vertex.
    - direct vertex lists instead of index lists.
    - 32 bit indices to allow for more than 65k vertices.
    - varying color types such as uint, float3 or float4
  - Allow more sophisticated mesh change events if only a part of a data array (such as Vertices or UVs)
    was changed.

### Implementation Notes
   
- The Render Context contains Methods to "Render" or "Set" Mesh, Texture and ShaderEffects.
  Whenever an object is rendered, the Render Context creates the necessary GPU resources 
  with the help of RenderContextImplmentation methods and caches them internally using dictionaries.
  
- Mesh, Texture and ShaderEffect define events that are fired on data alteration and finalization.
  The Render Context listens to these events for each currently tracked object and applies
  the respective alteration to the underlying resource right before the next frame.
  
- In the future a sophisticated resource pool system can be applied removing unused resources from 
  GPU memory.
   
## Mesh Implementation

The following code contains a draft of the new Mesh implementation
```C#
public class Mesh
{
    public Mesh();
    public float3[] Vertices { get; set; }
    public float3[] Normals { get; set; }
    public float2[] UVs { get; set; }
    public uint[] Colors { get; set; }
    public ushort[] Triangles { get; set; }

    internal event MeshChangedDelegate Changed
}

delegate MeshChangedDelegate(object sender, MeshChangedEventArgs args);

public class MeshChangedEventArgs : EventArgs
{
    public MeshChangedType ChangedType;
}

enum MeshChangedType 
{
    Deleted,
    VerticesChanged,
    NormalsChanged,
    UVsChanged,
    ColorsChanged,
    TrianglesChanged,
}
```

## Texture implementation

Requirements:

- Create Texture from scratch, from existing image data, or probably with some convenience
  constructors/static generators (e.g. checkerboard, text label, etc. )
- Access (read / write) image data in a natural way.

Possible implementations:

1. ImageData based:
The Texture class exposes a property of type ImageData. 
Pros: minor changes to existing code
Cons: Hard to track changes through the property. Only way: Entire ImageData must be set. Partial 
      Changes must be implemented with additional information Changed.

2. Interface based
Texture and ImageData implement a common interface:

```C#
interface IImageData
{
      // Taken from the existing ImageData:
      int Width {get}
      int Height {get}
      ImagePixelFormat PixelFormat {get}
     

      // Write a block of pixels to this instance from some other IImageData
      void Blt(int xDst, int yDst, IImageData src, int xSrc=0, int ySrc=0, int width=0, int height=0)

      // Expose a set of pixel lines (enables IImageData to be used as src in other instances' Blt)
      IEnumerator<ScanLine> ScanLines(int xSrc=0, int ySrc=0, int width=0, int height=0);
}

// PixelFormat descriptor
public struct ImagePixelFormat
{
      int BytesPerPixel { get }
      
      IEnumerator<ImagePixelChannel> PixelChannel

      // Convenience stuff
      bool HasAlpha { get }
}

public struct ImagePixelChannel
{
    int  FirstBit

        long BitMask

}


// This class is a bit awkward. It tries to solve the following things:
// - IImageData instances must expose line-wise contiguous portions of internal memory where
//   requesting objects can copy pixel data from.
// - C# does not allow to simply wrap a byte[] object around existing memory without first
//   creating such an object and copy the memory.
// - We could consider returning a pointer (byte*) but this would involve other headache, including
//   unsafe code and extra-coding for JSIL.

public class ScanLine
{
   public byte[] Origin { get }  // The start of some internal array where the data comes from
   public int Offset { get }     // An Offset (in bytes) to add to the index to the first pixel of the requested line
   public int Width { get }      // The 
   public int BytesPerPixel { get }
   public ImagePixelFormat PixelFormat { get }
}

```

"IImageData" exposing a Blt method (instance
is destination). To enable IImageData for the source role,



The following code contains a draft of the new Texture implementation.
```C#
public class Texture
{
    public Texture();
    // Several convenience constructor overloads to directly create texture from
    // image data, empty textures, etc.
    public Texture( /* ... */ ); 

    public ImageData Image {get; set};
    public 
}

public struct ImageData
{
    // as specified
}

```

## Scenegraph / Serialization Notes

- If possible, the FUSEE serialization system should be kept central in the Serialization project
  which does not know about the Engine projects for good reason. Serialization objects should be 
  kept as dumb data keepers with no or only simple methods and a rather self-contained set of 
  serializable data (exception: Math datatypes such as float3, float4x4).
  
- More sophisticated objects are needed for 
