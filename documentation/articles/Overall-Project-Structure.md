This page describes the rules after that projects within FUSEE should be structured as well as
the reasons behind these rules. 
Note that these rules were created some time after the FUSEE project started. Still not all FUSEE 
contents may already obey to these rules.

### On this page
  - [Coverage](#coverage)
  - [FUSEE Project Details](#fusee-project-details)
  - [FUSEE Project Conventions](#fusee-project-conventions)
  - [Naming Conventions](#naming-conventions)
  - [Example File Structure](#example-file-structure)

Coverage
========
The guidelines in this document cover how to handle the following items

 - Namespaces / Class names
 - File names
 - Folders
 - Project file names
 - Generated dll names
 - Handwritten js projects in terms of script names and source directories
 - Additional dll versions for different platforms (like shared implementation dlls - probably
   RenderContextImp for OpenTK/Desktop vs OpenTK/Android)


FUSEE Project Details
=====================
FUSEE's goal is to provide a software infrastructure for platform independent 3D realtime 
applications. Mainly this infrastructure consists of libraries (dlls) providing pre-defined 
functionality. In some cases FUSEE also contains deliverables like build-tools, plug-ins etc. 
(in source code). Platform independence means that most parts of the FUSEE code should be 
able to run on different computers, operating systems, or other environments (like web browsers). 
This requirement is probably the most influencing force on how the FUSEE project is 
organized and has a direct impact on the rules defined here. FUSEE uses C# as its 
main programming language, Visual Studio as its main development IDE, MONO and .NET as 
its main runtime-platforms, Xamarin as its hub for ports to Android and iOS, JSIL 
for translating .NET IL code to javascript, in order to make FUSEE run plugin-free 
in web browsers and a modified version of protobuf-net for automatic serialization. 
These different technologies all come with a set of requirements 
which have to be taken into account when designing a project strucure.

Android / iOS Requirements 
--------------------------
To enable C# code to run on Android / iOS, it must be compiled using special project 
settings. Either C# code is re-used in different projects directly aimed at each 
platform (desktop, Android, iOS) - so called "shared libraries" - or code is compiled into
so called "portable libraries" which can be used in binary form on all platforms.

FUSEE tries to handle as much as 
possible in portable libraries. Since portable libraries can only reference a very 
limited subset of the "normal" .NET platform library, a lot of platform specific 
functionality that a portable platform depends on must be injected into the portable
library. This requires hand-coded interfaces a portable library is implemented against
and then a set of platform specific implementation libraries implementing these 
interfaces. The injection then happens either at run-time or at compile-time of an
application using the portable library on a specific platform.

In some cases the platform specific to-be-injected implementations of such an 
interface only differ slightly among the different platforms. In these cases code is
re-used in shared libraries. Typically this code is larded with `#if PLATFORM_XYZ`
conditional compile paths. One example is the implementation of the platform-specific 
3D code which, on most platforms, is routed through different implementations of OpenTK.
Most of the implementations are the same, except for some minor platform-specific differences
which then can be sorted out by using `#if PLATFORM_XYZ` conditional compile paths and 
re-using the .cs file in builds for different platforms.

With this in mind, FUSEE C# library projects (.csproj generating a dll) come in three different flavors:

 - _Log-Libs_: Logic Libraries. Portable library projects containing extensive functionality 
    - Are portable and can thus be shared across platforms in binary form
    - contain lots of method implementations - the "business logic"
    - Access platform-specific functionality (file-access, hardware-access, 3d-rendering, etc.)
      through interfaces.
 - _Dec-Libs_: Declaration Libraries. Portable library projects containing interfaces
    - Imp-Libs can be implemented against these interfaces. 
    - Additionally contain simple implementations of 
        - helper functions
        - structures/classes used as parameters and return values
        - enums
    - Contain only declarative or very lightweight implementations not relying on anything else
      but the least common denominator portable .NET base (or relying on other _Dec-Libs_).
 - _Imp-Libs_: Implementation Libraries. Non-portable libraries directly referencing platform-specific 
   libraries. 
    - Contain implementations of interfaces declared in Dec-libs.
    - May be made of shared libraries which can be compiled to different platforms.
    - Code can be shared among different platforms if specific differences in shared code is marked with
      conditional compilation sections.
    - _Imp-Libs_ are never directly referenced by _Log-Libs_ or _Dec-Libs_ but are injected where needed
      e.g. by the platform-specific application (in the future maybe by some dependency injection container).


JSIL requirements
-----------------
JSIL can handle most of the code by directly translating it from the dll's IL code to
javascript, maintaining functionality of most of the standard .NET platform libraries
(mscorlib, system, ...). This holds particularly for portable libraries, so _Dec-Libs_ as well as 
_Log-Libs_ should cross-compile with JSIL in a straight-forward way.

_Imp-Libs_ are setup by implementing a set of interfaces in C# but containing mostly dummy implementations 
for the methods (implemented by throwing an Exception to make them compilable without bothering about 
providing a return value. All dummby-methods are attributed with `[JSExternal]`. In addition _Imp-Libs_ contain 
a considerable amount of JavaScript code continaing the hand-coded javascript implementation in a way 
that can be handled by a browser. 

Additionally, _Log-Libs_ sometimes contain parts that need special hand-coded implementations in
javascript. This can occur if hand-coded javascript turns out to be faster than JSIL-compiled code, 
or in the very rare cases where JSIL is not capable of translating the logic. In these cases, the 
members (mostly methods) that need hand-coded javacript implementations are decorated with the
`[JSExternal]` attribute in the C# source. These libraries can then be accompanied by a hand-coded 
.js file containing implementations for the  members declared `[JSExternal]`.

Application Projects
--------------------
In addition to FUSEE Library projects creating dlls, another family of project types exist producing 
applications for the various platforms supported by FUSEE: Desktop-Apps (exe), Web-Apps (html/js), 
Android (apk). These projects are platform dependent by nature. Still, applications determined to run
on different platforms should be setup similar to library projects by centralizing most functionality
(business logic) in a platform-independent portable library and keeping the platform dependent Application
projects as thin as possible. Ideally, a platform dependent Application contains nothing more but 
instantiations of the _Imp-Libs_ matching their platform and used by the application. Take a look at the "Simple"
example to see how a multi-platform application can be set up.    

Tools
-----
tbd.


FUSEE Project Conventions
=========================

Root Folder structure
---------------------
The FUSEE development root folder contains (at least) the following directories

 - _bin_: Contains all output. Generated DLLs, EXEs, comment-xml-files and javascript cross-compiled 
   by JSIL is written to _Bin_.
 - _src_: Contains all Source code. Any Source Code part of FUSEE should be placed in a sub-folder under
   _src_.
 - _tmp_: All temporary output generated by the build process should be placed in sub-folders below _tmp_.
 - _ext_: Contains binaries, such as the JSIL compiler, used for the FUSEE build process.
 - _hlp_: Contains help files and documentation.


### Contents of the _src_ Folder 
The _src_ folder contains individual projects (called ***root projects*** from now on).
There may be dependencies among these root projects. Actually each root project may be made up
out of several ***sub projects***, so there are cases where there is no single .csproj file
containing a sub project. 
  
Root projects within _src_ that are referenced by others typically contain very basic functionality
that can be re-used in other projects. Currently all Source Code in FUSEE is contained in one single
GitHub repository. In the future, individual root projects directly below _src_ may be put in individual
repositories (on GitHub) to make them avaliable to other projects without the need to clone the entire
FUSEE project. Root projects that can be found directly below _src_ are

 - _Math_: The Math library used throughout FUSEE.
 - _Xirkit_: A library for building graphs of objects that are interconnected by their members
   (fields and properties) with an automated updating system delivering values over the connections.
 - _Serialization_: Central repository of all container classes in entire FUSEE.
 - ...


Project Granularity
-------------------
To keep the number of projects and files low, there shouldn't be too many _Dec-Lib_ style projects. 
As a recommendation, there should be only one such project per sub-project below _src_. In addition,
any _Log-Lib_ style project should only expose zero or one single javascript file containting all the
hand-coded members (mainly methods) which are `[JSExternal]`. The number of _Imp-Lib_ style projects 
per sub-project (below _src_) is not generally restricted, though it should be kept as low as possible.
Each _Imp-Lib_ project should consist of a self-containing set of functionality which, from an application
point-of-view, can be considered being a component to be switched on or off. 

Naming conventions
==================
The decisions listed here were made to offer developers creating new FUSEE sub- or root-projects a set
of consistent rules. The goal is to keep developers from inventing new naming schemes for each project added
(or refactored) which will lead to inconsistency over time. Especially the namespace naming rule will bloat
FUSEE with a very fine granular spate of namespaces, but this drawback is outweighed by a clear and easy-to-obey
convention keeping a concise project structure even after years of development. 

Namespaces / Class names / File names
-------------------------------------
Mainly like in Java - Namespace: project folder name below _src_, starting with Fusee. _src_ is omitted! 
Each subfolder generates a new namespace. File names and class names: Exceptions to the Java rule may occur.
Public Enums, Delegates and simple structs, especially if used as parameters or return types, may be declared
within some "heavyweight" class file.  

Folders
-------
Each folder resembles a sub-namespace. Strict 1 on 1 correspondence between folder names and namespaces. If
possible, no sub-folders within projects (as this will generate namespaces). Consider flattening the project or
creating sub-projects. Keep exceptions to this rule minimal.

Declaration Libraries must be *Common*
--------------------------------------
Since the number of _Dec-Libs_ should be kept minimal (if possible, only one per sub-project below _src_), these
_Dec-Libs_ must use the special name "Fusee.<Subproject>._Common_". As the other conventions require, this name is
used as the name of the project as well as the name of the resulting dll.

Logic Libraries may be *Core*
-----------------------------
If a sub project below _src_ needs to be separated into declaration, logic and implementation parts (because the
logic accesses platform-specific functionality), it might be appropriate to centralize common logic into one single
library. The name of such a general library containing logic should be "core". It might as well be appropriate
to separate the a sub project below _src_ into more than one logic library. This might be helpful in cases where
a self-contained set of functionality is likely to be used only in some applications (and *not* in others).

Project file names
------------------
Project file names contain the complete name dot-separated starting with Fusee (and omitting _src_). Thus a
project file is exactly the same as the namespace it defines (and where the classes/types it defines reside in).

Generated dll names
-------------------
Same as project file name: The project Fusee.SomeNamespace.SomeSubNamespace.csproj will build
Fusee.SomeNamespace.SomeSubNamespace.dll (and most likely Fusee.SomeNamespace.SomeSubNamespace.xml).

Implementation variants of _Imp-Libs_ for different platforms
-------------------------------------------------------------
Generally implementation libraries should reside below some Imp directory and thus both, the project file as well as the built dll file name (and the name space) will contain
"Imp" in their respective name. Below "Imp" there might be a number of subdirectories
for the different platforms. Common platform names are

 - Desktop (for desktop platforms (Windows, MacOS, Linux)
 - Android (for Android devices)
 - iOS (for iOS devices)
 - Web (for Web builds using JSIL)
  
In certain cases more than one implementation may exist for a single platform (e.g.
OpenTK and DirectX implementation of the graphics layer for desktop platforms). 
In these cases further subdirectories (involving sub-namespaces, project names etc.)
should be introduced.

Handwritten js files
--------------------
Handwritten implementations of _Imp-Libs_ as well as additional external 
method implementations for  _Log-Libs_  obey to the already mentioned 
naming rules. In addition to that  external Javascript files that replace single members (methods) 
should bear the accompanying Dll's full name with the ".Ext.js" extension (`Fusee.XYZ.Imp.Web.js` / `Fusee.XYZ.Core.js`). 
(where XYZ stands for the namespace, e.g. "Fusee.Xirkit.Ext.js"). According to this rule only one js file 
implementing externals per library is recommended. additional javascript libraries containing implementation 
required by imp-js or ext-js files. 

Project dependencies
--------------------
Obviously, as enforced by Visual Studio, portable projects can only reference other portable projects.
As _Dec-Libs_ and _Log-Libs_ must be portable, this restriction holds for these types of libraries.
Generally, projects must not reference other projects in lower sub-directories than the referencing
project. References among siblings (at the same directory level) are allowed, as well as references 
to projects further up in the directory tree. In addition, the following rules must be obeyed:

  - _Dec-Libs_ must not reference any other project type than _Dec-Libs_. _Dec-Libs_ must *not* reference
    _Log-Libs_.
  - _Log-Libs_ may reference _Dec-Libs_ as well as other _Log-Libs_ (as long as they are at the same
    directory level or up).
  - _Imp-Libs_ must *not* be referenced directly by _Dec-Libs_ or _Log-Libs_. Obviously _Log-Lib_ code
    needs to call _Imp-Lib_ code. This must be implemented using dependency injection/interfaces.
  - _Imp-Libs_ will obviously reference _Dec-Libs_ because they implement interfaces declared in _Dec-Libs_.
  - _Imp-Libs_ must *not* reference _Log-Libs_. In special cases _Log-Lib_-like helper dlls may be implemented
    containing code common among different platform implementations. Consider using shared libraries instead.



Example File Structure
======================
At the time of writing the following structure seems a good goal to be applied to the current 
-rather historically grown- project structure. Each entry shows the directeroy level followed
by the complete namespace/project name/assembly name until that level and finally the type
of the generated assembly at that point in curly braces. If no type is specified, no project
exists at that point (only a directory/namespace). "**tbi**" stands for "to be implemented".

All items below Fusee/src:

 - Base (`Fusee.Base`) tbi - Contains base implementation used by other FUSEE subprojects   
    - Common (`Fusee.Base.Common`) {_dec_}
    - Core (`Fusee.Base.Core`) {_log_}
    - Imp (`Fusee.Base.Imp`) 
        - Desktop (`Fusee.Base.Imp.Desktop`) {_imp_}
        - Android (`Fusee.Base.Imp.Android`) {_imp_}
        - Web (`Fusee.Base.Imp.Web`) {_imp_}
 - Math (`Fusee.Math`) 
    - Core (`Fusee.Math.Core`) {_log_}
 - Engine (`Fusee.Engine`)
    - Common (`Fusee.Engine.Common`) {_dec_}
    - Core (`Fusee.Engine.Core`) {_log_}
    - Physics (`Fusee.Engine.Physics`) {_log_} tbi
    - Imp (`Fusee.Engine.Imp`)
        - Audio (`Fusee.Engine.Imp.Audio`)
            - Desktop (`Fusee.Engine.Imp.Audio.Desktop`)
            - Android (`Fusee.Engine.Imp.Audio.Android`) tbi
            - Web (`Fusee.Engine.Imp.Audio.Web`)
        - Graphics (`Fusee.Engine.Graphics`)
            - Desktop (`Fusee.Engine.Imp.Graphics.Desktop`)
            - Android (`Fusee.Engine.Imp.Graphics.Android`) tbi
            - Web (`Fusee.Engine.Imp.Graphics.Web`)
        - Network (`Fusee.Engine.Network`)
            - Desktop (`Fusee.Engine.Imp.Network.Desktop`)
            - Android (`Fusee.Engine.Imp.Network.Android`) tbi
            - Web (`Fusee.Engine.Imp.Network.Web`)
        - Input (`Fusee.Engine.Input`)
            - Desktop (`Fusee.Engine.Imp.Input.Desktop`)
            - Android (`Fusee.Engine.Imp.Input.Android`) tbi
            - Web (`Fusee.Engine.Imp.Input.Web`)
        - Physics (`Fusee.Engine.Physics`)
            - Desktop (`Fusee.Engine.Imp.Physics.Desktop`)
            - Android (`Fusee.Engine.Imp.Physics.Android`) tbi
            - Web (`Fusee.Engine.Imp.Physics.Web`) tbi
 - Serialization (`Fusee.Serialization`) 
 - Xirkit (`Fusee.Xirkit`) 
 - Jometri (`Fusee.Jometri`) 
 - CrossSL (`Fusee.CrossSL`) 
 - Uniplug (`Fusee.Uniplug`)
 