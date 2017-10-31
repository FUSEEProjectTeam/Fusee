%FuseeRoot%bin\Debug\Tools\fusee.exe protoschema -o proto/Scene.proto
%FuseeRoot%ext\protobuf\protoc-3.4.0-win32\bin\protoc.exe -I=proto --python_out=proto proto/Scene.proto
