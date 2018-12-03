# Blender Add-on (.fus Exporter)
_by Jonas Conrad and Patrick Foerster_
## Installation Guide
### Prerequisites
* The io_export_fus.pyproj file uses the ZIP task which is part of the MSBuild Communiy Tasks project
  * Download and install the latest version from https://github.com/loresoft/msbuildtasks
* Blender
* Fusee (with the set FuseeRoot environment variable)
* fusee.exe (if not already done, simply build the Fusee.Engine.sln)

### StepByStep Installation
1. Python
	* Download the newest Python 3 installer
	* Start the installation
	* Be sure to check the box that sais "Add Python 3.x to PATH"
	* Open the CMD-Window
	* Type `python` to verify all PATH variables have been set correctly
	* If there are no errors everything is in order otherwise, go to Step 4
	* type `quit()`

2. Protobuf
	* Reopen the CMD-Window if necessary 
	* Type `pip3 install protobuf`
	* If there are no errors close the CMD-Window
	
3. Blender 
Manual installation on production machines. For setting up a development machine allowing to edit the .py file contained in the 
project and directly use them out of Blender to enable debugging see "Development Environment for Blender Scripts" below.
	* Go to FILE->User Preferences->Add-ons
	* Click __Install from File__ and choose the .zip folder of the Add-on and finish the installation
	* click __Testing_ and activate the Addon _Import-Export: .fus format__
	* you may click __Save User Settings__ if you want to have the Add-on activated by default
		* the Add-on will be installed in the specified "Target Path" (Default: C:\Users\xx\AppData\Roaming\Blender Foundation\Blender\2.xx\scripts\addons"). 
		* you can find the correct path, by simply clicking on the little triangle next to the checkbox of the Add-on and checking the line __File:full/path/to/addon__
		
4. Set PATH variables by hand
	* go to the __System_ panel and click on _Advanced system settings__ 
	* open the __Environment Variables__ window
	* in the upper part of the window called __User variables for [UserName]__ 
	* double-click the __Path__ variable
	* click __New__ and add the following two paths pointing to where you have installed __Python__
	* these paths could look something like the following
		* C:\ .. \Python\PythonXx\
		* C:\ .. \Python\PythonXx\Scripts\
		
### How to generate new .proto files
1. Make sure a working fusee.exe exists at  %FuseeRoot%\bin\Debug\Tools\.
2. open the CMD-Window and either change to the the directory where the fusee.exe is located or simply drag the fusee.exe file into the CMD-Window.     
In both cases you have to add the following command: `protoschema -o $OUTPUT_PATH`. This will create a .proto-file in your `$OUTPUT_PATH`.     
**Note**: when your Fusee directory is not in your primary partition, you have to first change to the correct partition by simply typing it's letter, e.g. `D:` and then `cd path\to\tool`
3. rename the .proto-file to __Scene.proto__
3. download the protocol compiler from https://github.com/google/protobuf/releases, 
	* choose the correct package protoc-$VERSION-$PLATFORM.zip, download and unzip it. 
	* for installation simply follow the instructions in the README
4. NOT NECESSARY ANY MORE: "fusee protoschema" will do that: Open the .proto-file (e.g. with Notepad++) and in the first line insert `syntax = "proto2";`.
The first two lines should now look like this:    
	`syntax = "proto2";`    
	`import "bcl.proto"; // schema for protobuf-net's handling of core .NET types`

5. in the CMD-Window type `protoc -I=$SOURCE_DIR --python_out=$TARGET_DIR $FULL_PATH_TO_YOUR_PROTO_FILE`, the resulting python file will be written to your `$TARGET_DIR`, so it's best to set it to the __\proto\__ folder of the Add-on, directly.   

6. if the file is not already there, move the resulting SCENE_pb2.py file to __PATH_TO_ADDON\io_export_fus\proto\__       
You can find more information here: https://developers.google.com/protocol-buffers/docs/pythontutorial#compiling-your-protocol-buffers

### Development Environment for Blender Scripts
Typically a Blender Addon or Script is installed using the "Install From File" button in Blender's preferences. This will copy the contents of the specified file to the Appdata
directory mentioned above. When developing a Blender Script you would like to edit the file and then have Blender directly use the file in-situ because otherwise the Python 
Debugger would not recognize a copied file and could not hit breakpoints and inspect code. So for a development environment make sure to specify the BlenderScripts directory below
<FuseeRoot>/Tools in the User Preferences' File dialog box under "Scripts". This way all python code appearing under BlenderScripts/addons is directly accessed by Blender 
and thus recognized by an attached Debugger as "active" code.

### Debugging
See the [PVTS-Wiki on Remote Debugging](https://github.com/Microsoft/PTVS/wiki/Cross-Platform-Remote-Debugging) how to prepare your Blender Addon for ptvsd debugging and 
how to attach to Blender in Visual Studio's Attach Debugger dialog.


### Notes
* In order for changes in the python code of the Add-on to take effect, you have to restart Blender
* In case of an error related to python packages or imports, you may want to check, that all environment variables are set correctly (have a look at the StepByStep-Installation, Step 4)  


## Current Version (1.0.0)
### Steps Taken by the Exporter
1. Saves the file (if desired by the user) 
2. Iterates through all objects and their respective children (using a recursive function)
  1. Triangulates the mesh
  2. Applies all transformations to the object
  3. Separates all faces (makes to job easier for the script)
  4. Reads the mesh data (triangles, vertices, normals)
  5. Reads the material data (diffuse, specular, emissive) but takes only the first node he finds
  6. Serializes all data in a structured manner as defined by the .proto files
3. Writes the serialized data into the .fus file

### Web Viewer
* Web viewer
* Diffuse materials 
* Emissive materials
* UVs + textures

### .fus Exporter
* .fus exporter
* Diffuse materials 
* Specular materials
* Emissive materials
* UVs + textures

### Others
* Lamps (not supported by Fusee yet, therefore the option is currently disabled)


# Backlog

* Apply modifiers to objects such as Subdivision Surface
* Check Hierarchies and transformations
* Export smoothed normals
* Include Protobbuf-Python compiler into build to automatically generate 'Scene_pb2.py'
* Update to Python 3.6 64 bit (for Fusion) and latest Blender 64 bit (2.78b)
* Check Textures with different Settings
* Allow for higher shiniess values. Probably requires to better understand Cycles' BRDF nodes first
* Include bump map information
* Include Key frame information
* Include skeleton and weight information
* Include Lights
