FUSEE Xene
==========

Scene Graphs
------------

A major concept in all 3D graphics systems is the scene graph, object hierarchy, scene tree or however a specific system calls it. The common feature of all approaches is to keep the scene data in a tree-like structure. The use of such hierarchical scenes is to group objects together with properties such as colors, position, orientation etc. being shared among the grouped objects. Certain properties, especially transformation (position, orientation, scale) are not only shared among siblings but additionally cummulated from parent to child (e.g. a childs position is relative to its parent's position).

FUSEE's Xene system can be seen as a tool box with building blocks to create various scene graph implementations. The simplest scene graph implementation is just made up of a hierarchy of scene nodes where each node may contain 0 or more children - also scene nodes. In addition each node may contain zero or more lower-level building blocks, called components. The contents a node consists of can be organized in re-usable component types.


Traversing a scene graph
------------------------
Traversing such a structure is simply an in-order traversal of all nodes. Each visited node is traversed by traversing its components and afterwards recursively traversing its children.

Various component types may exist like geometry, transformation, material, etc. In more advanced implementations also a set of special-purpose node types may exist. Such node types might expose their user data in two ways: One way allows high-level program access to its innards using fields and properties, another way exposes the same data as lower-level components to allow backward-compatibility with already implemented use cases.

### Categories of scene graph traversal
At least three categories of traversing such structures exist:

1. Find operations - the easiest form of traversal. No state needs to be tracked. A simple predicate is checked on each item (node or component). If the predicate is matched by the element, the element is added to the seach result list.  
2. Classical Visitor Pattern - All nodes and components are traversed in-order and -depending on their respective type- individual actions are performed. The result of a visitor operation is a set of side effects. Very often while traversing some state needs to be maintained keeping track of the overall traversal state. When side effects (such as rendering) are triggered during traversal, information from the state can be accessed.  
3. Transformation Operations - Can be seen as something in-between the two previous categories. Visitor patterns for different use cases often share two common circumstances:
	- The way how State is maintained (typically in stacks of individual   	values keeping track of information retrieved from tree elements).
	- The generated Side-Effect (rather the result) is a list of data items resulting from data from individual nodes and cummulated state data.  

Introducing the Viserator Pattern
---------------------------------
Fusee Xene contains building blocks and pre-fabricated functionality for all three scene graph traversal categories. The third use-case is handled by Xene with a class called ***Viserator***. 

A Viserator is a scene visitor which returns an enumerator of a user defined type. The Viserator class serves as a base class
for use-cases where traversing a node-component-graph should yield a list (enumerator) of results.

Very often you want to traverse a node-component-graph while maintaining a traversal state keeping track of
inividual values and their changes while traversing. At certain points during the traversal a result arises that
should be promoted to the outside of the traversal. Typically the result is derived from the state at a certain
time during traversal and some additional information of the tree object currently visited.


To implement your own Viserator you should consider which state information the Viserator must keep track of.
Either you assemble your own State type by deriving from VisitorState or choose to use one of 
the standard state types like StandardState. Then you need to derive your own class from Viserator{TItem,TState} with the TState replaced by your choice of State and TItem replaced by the type of the items you want your Viserator to yield
during the traversal.


The word Viserator is a combination of a visitor and and enumerator. Look up "to viscerate" in a dictionary and
judge for yourself if a Viserator's operation resembles disembowelling the innards of a tree structure.
