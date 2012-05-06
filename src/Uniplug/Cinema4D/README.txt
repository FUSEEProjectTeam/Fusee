The solution contains five projects:

Swig         -  the project creating c# wrappers from the original MAXON C4D C++ API
Native       -  the native C++ Cinema4D PlugIn. Also contains the C++ part for the SWIG generated API wrappers
Bridge       -  the mixed mode (native and managed C++) dll API that connects the Managed PlugIn to the Native one.
C4D          -  the managed code master plugin that searches recursively for managed plugins with funcionality. This project also 
                contains all the managed wrapper classes around the unmanaged C4D API
YourManagedPlugin -  the managed code plugin doing the actual XPresso export.



How to build the solution
-------------------------
PREREQUISITES
1. Swig (2.0.4) must be self-built with patch 3305130 (C# csdirectorin pre/post attribute support) applied and the path to swig.exe must be part of the system path - see App. 1 below.
2. Cinema4D must be installed and the path to the installation directory must be set in the system variable C4D_DIR
3. Mono (Latest Stable Version: 2.10.2) must be installed and the path to the installation must be set in the system variable MONODIR
4. The mono installation's bin dir (e.g. C:\Programme\Mono-2.10.2\bin) must be added to the system path


Make sure you are bulding one of the [Debug|Release] [Win32|X64] Solution Configurations.

The Swig project contains no source code, only the Swig input file (C4dApi.i). This is used in the project's Custom Build Step (Projet Properties -> Custom Build Step -> Command Line). There are two prerequisites to buld the Swig project:
1. The directory Projects/Managed/C4dApi must exist (you must create it by hand, if you build for the first time) and be empty (you must empty it by hand each time you build).
2. swig.exe must be in the common search path (Control panel -> System -> Advanced -> Advanced Tab -> Environmant Variables -> Path) 

Building the Swig project yields two results:
1. The directory Projects/Managed/C4dApi will be filled with *.cs files. These files contain the C# versions of the classes and functions of the C4D Api. Only those mentioned in the C4dApi.i swig control file will be wrapped.

2. The file Projects\Native\C4DApiWrapper.cpp will be generated. This file contains all the C++ Bridge code directly called by the C# wrappers. This code mainly contains all the parameter marshalling (conversion) of the types used in the different programming environments (e.g. convert System.String in C# into C4d's String class in C++ and vice versa).


All the generated .cs files will be compiled and built under the "Managed" project. Unfortunately Visual Studio does not recognize automatically that new files have been added by building the Swig project. So before building Managed, you need to unload and reload the Manged project (right-click on "Managed" in the Solution Explorer). After unloading and reloading the Managed project will contain a folder called "C4dApi" resembling the contents of the file folder. 

Now you can build the rest of the solution the normal way: just by clicking Build -> Build Solution or hitting F6.

Make sure to repeat this process (inlcuding manually emptying the C4dApi file folder) every time you added definitions to the Swig control file (C4dApi.h).

Maually rebuild the entire solution project-by-project in the order following order: Swig, C4D, Bridge, Native, XPressoXPort. You need to rebuild whenever changing the configuration (Debug/Release/64/32).



Running / Debugging the code.
-----------------------------
After successfully building the code you can start the respective Cinema4D v12 executable (64 or 32 bit). Depending on the configuration and on your own settings, the managed part of the code runs either under the Microsoft .Net runtime virtual machine or the Mono runtime virtual machine. Mono is only available with the 32 bit version of C4D (which can run on both 32 and 64 bit Windows). MS.Net is available with the 32 bit and 64 bit versions of Cinema4D. Since Mono is available on 32 bit and 64 bit versions of Mac OS, the Managed Plugin will most likely run with 32 and 64 bit Cinema4D on the Mac, ONCE IT WAS PORTED TO THE MAC.
Please note that you cannot debug into managed code run under mono with the Visual Studio debugger. If you want to debug into managed code, you need to make sure that the MS.Net runtime is used. Placing a file named "msdotnet.cfg" into the C4D plugin directory (right next to the native.cdl or native.cdl64) will instruct the plugin to use the MS.Net runtime. The file's contents currently doesn't matter - so it may even be empty. This behavior can be overridden using the C4D_MANAGED_RUNTIME environment variable, which can be either set to "mono" or "msdotnet" resulting in using the respective managed runtime and ignoring if msdotnet.cfg is present or not.



App. 1: Why and how to build my own Swig
----------------------------------------
We are using Swig in a way where we want the wrapped C# methods to have parameters making use of the C# "ref" or "out" keywords a lot (See C# documentation). Unfortunately this leads to problems if the containing classes are used as so called "directors" (see Swig documentation). The generated C# code then provokes compiler errors. The problem is described under
http://permalink.gmane.org/gmane.comp.programming.swig/16385 and solved in a patch under
http://sourceforge.net/tracker/?func=detail&aid=3305130&group_id=1645&atid=301645.
Unless the patch becomes part of the official Swig distribution we need to manually build Swig with the patch applied. To build Swig under windows (and apply the patch) do the following:
1. Download the Swigwin distribution including a pre-built swig.exe and put it on your harddisk.
2. Copy the Source Folder to "Source.Orig"
3. Check out the contents of https://swig.svn.sourceforge.net/svnroot/swig/trunk/Source using svn over the "Source" folder of the distribution
4. Get the swigconfig.h file from Fusee/src/Misc/SwigSelfBuild/Include and copy it to Source/Include
5. Copy the parser.c and parser.h files from Source.Orig/CParse to Source/CParse
6. Get the Swig.sln and Swig.vcproj files Fusee/src/Misc/SwigSelfBuild and put them under Source
7. Apply the patch files directors-out.patch and directors-out-1.patch found in Fusee/src/Misc/SwigSelfBuild to the Swig Source folder (Right-Click on Source -> Tortoise SVN -> Apply Patch...). Tortoise SVN might ask you to rather apply it on the Swig root directory - do so). Don't forget to save all files in the Tortoise Merge window. The original download location for the two patch files is at Patch 3305130 (http://sourceforge.net/tracker/?func=detail&aid=3305130&group_id=1645&atid=301645).
8. Open Swig.sln with Visual Studio (v10) and build Swig.
9. Copy the self built Swig.exe from Homebrew/Debug to the Swig root directory.
NOTE: The above steps will become obsolete as soon as the patch made it into the official distribution. Hopefully this will happen soon.





