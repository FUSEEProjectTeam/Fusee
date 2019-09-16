# Contents of this directory

## protoc-3.9.1-win64

Protbuf compiler generating target-language dependent code based on .proto definition files. Used in FUSEE to generate Python serialization
code for .fus files from the .proto file created by the 
```
> fusee.exe protoschema
```

command, which generates the .proto-files from the C# protobuf-net declarations. The python serialization of .fus files is used in the FUSEE Blender Add-On.

## Python

Protobuf python code usually installed with "pip install protobuf". The code ist held here to be able to copy it to the neutered Python installation
shipped with Blender (which lacks a lot including "pip"...). This code will be held directly with the FUSEE Blender Add-On, which might be
installed without internet connection on a machine without Administrator privileges.

Additionally, this directory holds the "six.py" file also lacking in Blender Python, referenced by protobuf's "descriptor.py".

-----------------------------------

### What about protobuf-net.dll formerly found in Debug and Release directories?

Since FUSEE Release 0.8 protobuf-net.dll is referenced as a [nuget package](https://www.nuget.org/packages/protobuf-net/3.0.0-alpha.55).

### What about precompile.exe formerly found in the PrecompileTool directory?

The protobuf-net version used since FUSEE Release 0.8 (3.0.0-alpha.55) works without precompiling serialization code during build. Depending on the platform protobuf-net emits serialization code on the fly (if Reflection.Emit is supported) or falls back on a slower serialization without generated code.


