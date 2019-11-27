  > ⚠️ **Pre-Release Content**  

## Overview

At the moment FUSEE supports two types of textures. Standard textures, that offer access to the pixel data and Writable Textures, that don't.  
The latter are used if we want to render to a texture or a G-Buffer, standard textures are used e. g. in ``ShaderEffects`` as albedo or normal texture.

The following class diagram breaks down the implementation:

![Class Diagram: Textures](https://raw.githubusercontent.com/wiki/FUSEEProjectTeam/Fusee/Images/Textures/textures.png)

``Textures``, ``WritableTextures`` and ``WritableCubeMaps`` are meant to be used directly in application code.
Furthermore the associated interfaces ``ITexture``, ``IWritableTexture`` and ``IWritableCubeMap`` -- here marked in green -- serve as their abstraction and allow us to use the texture classes in a cross-platform context.

## Textures

### Loading

Images can be loaded via the ``AssetStorage``:

```C#
var imgData = AssetStorage.Get<ImageData>("someTexture.png");
var tex = new Texture(imgData);
```

The first line creates a object of type ``ImageData``, which carries the payload of the image (the pixel data) and offers ``Blt`` functionality, which is also inherited by the standard ``Texture``. The second wraps it into a texture object.

Currently the engine supports the following image formats to be loaded by the ``AssetStorage``:
- jpg
- jpeg
- png
- bmp

## Writable Textures

They are not meant to be manipulated directly by the user, but can serve as render target, e. g. in multipass rendering. Therefor we do not have access to the actual pixel data here.
To create a ``WritableTexture`` the class offers a handful of static methods for special types, that make sure the texture is created correctly on the gpu. The types reflect the ones used in the deferred rendering pipeline:

- Albedo
- Position
- Normal
- Specular
- SSAO
- Depth

If we want to use a ``WritableTexture`` in an arbitrary case, ``CreateAlbedoTex`` may be the safest choice. Alternatively we can call the constructor directly, but we have to pass a set of texture options (see [Texture Options](#Texture-Options))


## Using textures in ShaderEffects

To feed a texture to a ``ShaderEffect`` and use it as a uniform variable in the associated shader code,  we can pass it a ``EffectParameterDeclaration`` to the constructor of the ``ShaderEffect``:

```C#

var effectParams = new EffectParameterDeclaration[]
{
    new EffectParameterDeclaration
    {
        Name = "DiffuseTexture",
        Value = tex
    },

    [...]
}

var effect = new ShaderEffect(effectPassDeclarations, effectParams);

```
Make sure to set the name of the ``EffectParameterDeclaration`` to the name of the uniform variable.

## Texture Options

The options of a texture define how it is created and handled on the gpu. The engine offers the following:

| Type | Name             | Function  |
| ---- |------------------| ----- |
| ``enum`` | ``ImagePixelFormat`` | Defines the color format, e. g. RGB, RGBA or Depth and, from this, the bytes per pixel. |
| ``bool`` | ``DoGenerateMipMaps``| Defines whether mip maps are created on the gpu or not. |
| ``enum`` | ``TextureWrapMode``  | Describes how the texture should be sampled when a uv coordinate is outside the range of 0 to 1.  |
| ``enum`` | ``TextureFilterMode``| Describes which texel to map the texture coordinate to. This is important, because the texture coordinates are resolution independent and described as floating point values. |