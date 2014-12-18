A major concept in all 3D graphics systems is the scene graph, object hierarchy, scene tree or however a specific system calls it. The common feature of all approaches is to keep the scene data in a tree-like structure. The use of such hierarchical scenes is to group objects together with properties such as colors, position, orientation etc. are shared along the grouped objects. Certain properties, especially transformation (position, orientation, scale) are cummulated.

FUSEE's SimpleScene system can be seen as a tool box with building blocks to create various scene graph implementations. The simplest scene graph implementation is just made up of a hierarchy of scene nodes where each node may contain 0 or more children - also scene nodes. In addition each node may contain zero or more lower-level building blocks, called components. The contents a node consists of can be organized in re-usable component types.

Traversing such a structure is simply an in-order traversal of all nodes. Each visited node is traversed by traversing its components and afterwards recursively traversing its children.

Various component types may exist like geometry, transformation, material, etc. In more advanced implementations also a set of special-purpose node types may exist. Such node types might expose their user data in two ways: One way allows high-level program access to its innards using fields and properties, another way exposes the same data as lower-level components to allow backward-compatibility with already implemented use cases.

At least three categories of traversing such structures exist:

1. Classic 