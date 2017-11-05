# Contents of this directory

This directory contains the necessary dependencies for FUSEE's .fus file serialization functionality, based on Google Protocol Buffers and
its .NET adaption: protobuf-net.

## protobuf-net.dll 

Needed by C# projects defining classes that are meant to be serialized. Contains the protobuf-net C# attributes such as [ProtoContract].
This DLL results from a Fork of the original protobuf-net project maintained by the FuseeProjectTeam.

## PrecompileTool

.NET Precompiler generating a .NET serialization DLL with Serialization code for all protobuf-net attributed classes. 
The contents of this folder results from a Fork of the original protobuf-net project maintained by the FuseeProjectTeam.

## protoc-3.4.0-win32

Protbuf compiler generating target-language dependent code based on .proto definition files. Used in FUSEE to generate Python serialization
code for .fus files from the .proto file created by "fusee protoschema", which generates the .proto-files from the C# protobuf-net declarations.

## Python

Protobuf python code usually installed with "pip install protobuf". The code ist held here to be able to copy it to the neutered Python installation
shipped with Blender (which lacks a lot including "pip"...). This code will be held directly with the FUSEE Blender Add-On, which might be
installed without internet connection on a machine without Administrator privileges.

Additionally, this directory holds the "six.py" file also lacking in Blender Python, referenced by protobuf's "descriptor.py".

