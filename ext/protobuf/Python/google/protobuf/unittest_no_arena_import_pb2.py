# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: google/protobuf/unittest_no_arena_import.proto

from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='google/protobuf/unittest_no_arena_import.proto',
  package='proto2_arena_unittest',
  syntax='proto2',
  serialized_options=None,
  serialized_pb=b'\n.google/protobuf/unittest_no_arena_import.proto\x12\x15proto2_arena_unittest\"\'\n\x1aImportNoArenaNestedMessage\x12\t\n\x01\x64\x18\x01 \x01(\x05'
)




_IMPORTNOARENANESTEDMESSAGE = _descriptor.Descriptor(
  name='ImportNoArenaNestedMessage',
  full_name='proto2_arena_unittest.ImportNoArenaNestedMessage',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='d', full_name='proto2_arena_unittest.ImportNoArenaNestedMessage.d', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=73,
  serialized_end=112,
)

DESCRIPTOR.message_types_by_name['ImportNoArenaNestedMessage'] = _IMPORTNOARENANESTEDMESSAGE
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

ImportNoArenaNestedMessage = _reflection.GeneratedProtocolMessageType('ImportNoArenaNestedMessage', (_message.Message,), {
  'DESCRIPTOR' : _IMPORTNOARENANESTEDMESSAGE,
  '__module__' : 'google.protobuf.unittest_no_arena_import_pb2'
  # @@protoc_insertion_point(class_scope:proto2_arena_unittest.ImportNoArenaNestedMessage)
  })
_sym_db.RegisterMessage(ImportNoArenaNestedMessage)


# @@protoc_insertion_point(module_scope)
