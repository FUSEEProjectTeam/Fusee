# How to generate python code from the annotated C# classes in the Serialization project.


## Step 1: Create Scene.proto from the classes below Fusee.Serialization

Calling `fusee.exe protoschema` generates `Scene.proto` into this folder (`proto_src`). The protobuf-net functionality behind this makes `Scene.proto` reference the two files `bcl.proto` and `protogen.proto` in the subfolder `protobuf-net`.

There are two bad things about this: 

 1. The generation process will NOT generate those two referenced files.
 2. The subfolder containes a hyphen - not a good idea in the python code generation when the subfolder name should become a module name.

Point 1 is handled by directly copying the two files `bcl.proto` and `protogen.proto` from the original [protobuf-net repository](https://github.com/protobuf-net/protobuf-net/tree/master/src/protobuf-net.Reflection/protobuf-net).

Point 2 is handled automatically by protoc.exe but results in the hyphen converted to an underscore (`protobuf_net`), generating executable python code. 

## Step 2: Create functional python code from the three .proto files

Both, the protobuf compiler (protoc.exe) and the python protobuf runtime system (`pip google.protobuf`) are very fragile and need to be instrumented well to generate running python code. A working solution is to call protoc.exe individually 
for each of the three .proto files mentioned in step 1 (`Scene.proto`, `protobuf-net/bcl.proto` and `protobuf-net/protogen.proto`). It's vital that all three conversions are started relative to the directory containing Scene.proto. This will make protoc
generate the correct naming changes to the hyphenized subfolder `protobuf-net` to a syntactically correct python module name `protobuf_net`. The following calls will generate the result into the `proto` subdirectory:

Compile `Scene.proto` directly from its containing folder.
```
C:\Parent\proto_src>%FuseeRoot%\ext\protobuf\protoc-3.9.1-win64\bin\protoc.exe --python_out=..\proto Scene.proto
```

Compile `bcl.proto` from its containing sub-folder (protobuf-net) to the corresponding pyhtonized subfolder (protobuf_net - underscore, no hyphen)
```
C:\Parent\proto_src>%FuseeRoot%\ext\protobuf\protoc-3.9.1-win64\bin\protoc.exe --python_out=..\proto protbuf-net\bcl.proto
```

Do the same with `protogen.proto`;
```
C:\Parent\proto_src>%FuseeRoot%\ext\protobuf\protoc-3.9.1-win64\bin\protoc.exe --python_out=..\proto protbuf-net\protogen.proto
```

# Implemented in fuseeCmdLine.csproj

Both steps are implemented within fuseeCmdLine.csproj and executed during build.

