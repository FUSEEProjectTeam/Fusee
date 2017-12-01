## Cross Reference Project

This folder contains a 
[Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project to generate the FUSEE cross-reference HTML pages from the C# comments
found within the source code files of the non-platform-specific FUSEE projects.

The generated contents will be written to
%FuseeRoot%/docs/xref

To build the xref pages install the Help File Builder and Tools (v2017.5.15.0 or later) from the 
[releases page at the project's GitHub Repo](https://github.com/EWSoftware/SHFB/releases).

Open the XRef.sln (which contains XRef.shfbproj MSBuild file) in Visual Studio or open 
XRef.shfbproj in the stand-alone `Sandcastle Help File Builder GUI` executable. 

