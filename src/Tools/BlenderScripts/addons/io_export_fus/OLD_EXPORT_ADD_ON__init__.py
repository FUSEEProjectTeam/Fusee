# THIS FILE IS HERE FOR CODE-REUSE ONLY
# It contains the old version' spaghetti code of the Blender-Export-Add-On for writing FUS-Files.
# Unless the current version (__init__.py, FusSceneWriter.py and referenced files) 
# do not contain the full functionality of the old Add-on, keep this file around
# to copy code from it, especially as the Blender Python documentation is not as
# explicit as it should.

'''
This Add-On currently supports the export of a .fus-file, containing the Mesh-Component,
the Transform-Component, Light Component (not yet supported by Fusee) and the Material-Component (Diffuse, Emissive and Specular only).
In order to make this Add-On work, you have to do the following:
1. Make sure, that Python 3 is installed (goto https://www.python.org/downloads/windows/, choose the correct installer, download and run it)
2. Install the google protobuf module (with pip installed (included in the newest python installers), simply go to the cmd and type 'pip install protobuf')
4. Put the folder of the Add-On in the Blender Add-On Folder (usually something like 'C:\Program Files\Blender Foundation\Blender\2.xx\scripts\addons')
5. Activate the Add-On in Blender (File -> User Preferences -> Add-ons -> Testing -> Import-Export:.fus format. Click 'Save User Settings' to keep it activated)
6. You have to structure the scene in such a way, that all objects that should be exported as one .fus-file, are (in)directly children of one root object
   (The best way to achieve this, is to simply parent all objects to another object)
7. Select the root object and click export (if you want to have your file saved, before exporting, simply activate 'Save File') 
8. Voilà
'''
#Register as Add-on
bl_info = {
    "name": ".fus format",
    "author": "Christoph Mueller, Moritz Roetner",
    "version": (0, 8, 0),
    "blender": (2, 8, 2),
    "location": "File > Import-Export",
    "description": "Export to the FUSEE .fus format/as a FUSEE Web application",
    "warning":"",
    "wiki_url":"",
    "support":'TESTING',
    "category": "Import-Export"
}

#import dependencies

import subprocess,os,sys
from shutil import copyfile

#set pypath
paths = os.environ['Path']
paths = paths.split(';')
for path in paths:
    if path.find('Python')!=-1:
        if path.find('Blender')==-1:
            sPath=path.split('\\')
            if sPath[-2].find('Python')!=-1:
                pypath = os.path.join(path,'Lib','site-packages')
                sys.path.append(pypath)
from .SerializeData import SerializeData, GetParents

import bpy
from bpy.props import (
        StringProperty,
        BoolProperty,
        FloatProperty
        )
from bpy_extras.io_utils import (
        ExportHelper,
        )

# Taken from https://github.com/Microsoft/PTVS/wiki/Cross-Platform-Remote-Debugging
# Now moved to https://docs.microsoft.com/en-us/visualstudio/python/debugging-cross-platform-remote
# Project repository at https://github.com/Microsoft/ptvsd
# Install latest version from pypi at https://pypi.org/project/ptvsd/
#
# Attach to PTSV Python Remote debuggee using "tcp://localhost:5678" (NO Secret!)
try:
    import ptvsd
except Exception:
    print('PTSV Debugging disabled: import ptvsd failed')
try:
#    ptvsd.enable_attach(secret=None) With ptvsd version 4 and upwards secret is no longer a named parameter
    ptvsd.enable_attach()
    print('PTSV Debugging enabled')
except Exception as e:
    print('PTSV Debugging disabled: ptvsd.enable_attach failed:')
    print(e)

class ExportFUS(bpy.types.Operator, ExportHelper):
    #class attributes
    bl_idname = "export_scene.fus"
    bl_label = "Export FUS"
    bl_options = {'UNDO', 'PRESET'}
    filename_ext = ".fus"

    filename_ext = ".fus"
    filter_glob : StringProperty(default="*.fus", options={'HIDDEN'})
    isOnlySelected : BoolProperty(
            name="Only selected Objects",
            description="Export only selected objects",
            default=False,
            )
    isWeb : BoolProperty(
        name="Create FUSEE Web-Application",
        description="Export HTML-Viewer around the scene and run in Web Browser after export",
        default=False,
    )
    isSaveFile : BoolProperty(
        name="Save File",
        description="Save this file before exporting it",
        default=True,
    )
    isExportTex : BoolProperty(
        name="Export Textures",
        description="Export the textures used in this scene as well",
        default=True,
    )
    isLamps : BoolProperty(
        name="Export Lamps",
        description="Export lamps in the scene (not supported yet)",
        default=False,
    )

    #Smoothing is not working correctly
    isSmooth : BoolProperty(
        name="Smooth Normals",
        description="Smooth display of normals (not working correctly)",
        default=False,
    )
    smoothingDist : FloatProperty(
        name="Smoothing Distance",
        description="Maximum distance between two points",
        min=0.0,
        max=100.0,
        default=0.0,
    )
    smoothingAngle : FloatProperty(
        name="Smoothing Angle",
        description="Maximum angle between two normals",
        min=0.0,
        max=90.0,
        default=0.0,
    )

    #Operator Properties
    filepath : StringProperty(subtype='FILE_PATH')

    #get FuseeRoot environment variable
    tool_Path = 'fusee.exe'
    isRoot = None
    # path of fusee.exe
    convtool_path = tool_Path

    def draw(self, context):
        layout = self.layout
        layout.prop(self, 'isOnlySelected')
        '''layout.prop(self, 'isSmooth')
        layout.prop(self, 'smoothingDist')
        layout.prop(self, 'smoothingAngle')
        layout.prop(self, 'isLamps')'''
        layout.prop(self, 'isSaveFile')
        layout.prop(self, 'isExportTex')
        layout.prop(self, 'isWeb')
        
        


    def execute(self, context):
        #check if all paths are set
        if not self.filepath:
            raise Exception("filepath not set")
        else:
            #save current state
            if self.isSaveFile:
                if bpy.data.is_saved:
                    # save the file
                    bpy.ops.wm.save_mainfile()
                else:
                    # get the temporary path of blender, to write a temporary version of the scene
                    temp_Path = bpy.context.preferences.filepaths.temporary_directory
                    # concatenate to get the full path
                    temp_Path = os.path.join(temp_Path, 'blender_fus_export_temp.blend')
                    print('File not saved before, saving a temporary tempfile in:\n' + temp_Path)
            if self.isOnlySelected:
                obj = bpy.context.selected_objects
            else:
                obj = set()
                for objects in bpy.data.objects:
                    parent = GetParents(objects)
                    obj.add(parent)
                    try:
                        obj.remove(None)
                    except:
                        pass
           
            geoObj = False
            falseObj=[]
            #sort out unwanted objects
            #only mesh and lamp objects(if lamps==True) must be serialized
            for o in obj:
                if o.type == 'MESH':
                    geoObj = True
                elif o.type == 'ARMATURE':
                    geoObj = True
                elif o.type == 'CAMERA':
                    falseObj.append(o)
                elif o.type == 'LAMP' and self.isLamps==False:
                    falseObj.append(o)
                elif o.type == 'LAMP' and self.isLamps==True:
                    pass
                else:
                    falseObj.append(o) 
            for fObj in falseObj:
                obj.remove(fObj)
                    
            
            bpy.ops.wm.console_toggle()
            if geoObj:
                #set blender to object mode (prevents problems)
                bpy.ops.object.mode_set(mode="OBJECT")

                #WEB Viewer
                if self.isWeb:
                    #kill Server if it's already running, to prevent problems when exporting more than once per session
                    process = subprocess.run(['taskkill', '/im', 'fusee.exe', '/f'])
                    print('Server Killed: ' + str(process.returncode))
                    try:
                        serializedData = SerializeData(objects=obj, isWeb=True,
                                                       isOnlySelected=self.isOnlySelected,
                                                       smoothing=self.isSmooth, lamps=self.isLamps,
                                                       smoothingDist=self.smoothingDist,
                                                       smoothingAngle=self.smoothingAngle)
                        print('writing to file: ' + self.filepath + '----')
                        with open(self.filepath,'wb') as file:
                            file.write(serializedData.obj)
                        #format the texturepaths to be used by fusee.exe
                        dirpath = os.path.dirname(self.filepath)
                        if self.isExportTex:
                            textures = serializedData.tex
                            if len(textures)>0:
                                texturePaths = ''
                                for texture in textures:
                                    src = texture
                                    print(src)
                                    dst = os.path.join(os.path.dirname(self.filepath),os.path.basename(texture))
                                    copyfile(src,dst)
                                    texturePaths = texturePaths+'"'+dst+'",'
                                print(self.filepath)
                                runFuseeExe = (self.convtool_path + ' web "' + self.filepath + '" -o "' + dirpath + '" -l ' + texturePaths)
                            else:
                                runFuseeExe = (self.convtool_path + ' web "' + self.filepath + '" -o "' + dirpath + '"')
                        
                        print(runFuseeExe)
                        #send the command to the commandline and run it
                        p=subprocess.Popen(runFuseeExe)
                    except Exception as inst:
                        print('---- ERROR1: ' + str(inst))

                #Normal export   
                else:
                    try:
                        serializedData = SerializeData(objects=obj, isWeb=False,
                                                       isOnlySelected=self.isOnlySelected,
                                                       smoothing=self.isSmooth, lamps=self.isLamps,
                                                       smoothingDist=self.smoothingDist,
                                                       smoothingAngle=self.smoothingAngle)
                        #write .fus file
                        print('writing to file: ' + self.filepath + '----')
                        with open(self.filepath,'wb') as file:
                            file.write(serializedData.obj)
                        #copy textures to the same directory as the .fus-file
                        if self.isExportTex:
                            print('writing texture files----')
                            textures = serializedData.tex
                            for texture in textures:
                                src = texture
                                print(src)
                                dst = os.path.join(os.path.dirname(self.filepath),os.path.basename(texture))
                                copyfile(src,dst)
                    except Exception as inst:
                        print('---- ERROR2: ' + str(inst))

                print('DONE')
                return {'FINISHED'}
            else:
                print('---- ERROR: you need to export at least one "MESH" object')
                return {'FINISHED'}
            
            

def menu_func_export(self, context):
    self.layout.operator(ExportFUS.bl_idname, text="FUS (.fus)")

classes =(
    ExportFUS,
)

def register():
    from bpy.utils import register_class
    for cls in classes:
        register_class(cls)
    bpy.types.TOPBAR_MT_file_export.append(menu_func_export)

def unregister():
    from bpy.utils import unregister_class
    for cls in reversed(classes):
        unregister_class(cls)
    bpy.types.TOPBAR_MT_file_export.remove(menu_func_export)

if __name__ == "__main__":
        register()
