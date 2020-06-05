'''
Exports the current blender scene to the FUSEE file format (.fus)
'''
#Register as Add-on
bl_info = {
    "name": ".fus format",
    "author": "The FUSEE Project Team",
    "version": (0, 8, 0),
    "blender": (2, 80, 0),
    "location": "File > Import-Export",
    "description": "Export to the FUSEE .fus format/as a FUSEE Web application",
    "warning":"",
    "wiki_url":"",
    "support":'TESTING',
    "category": "Import-Export"
}

#import dependencies
import bpy
from bpy.props import (
        StringProperty,
        BoolProperty,
        FloatProperty
        )
from bpy_extras.io_utils import (
        ExportHelper,
        )

import subprocess,os,sys, time
from shutil import copyfile

# Add some necessary extras to blender's neutered python

# Add anything from any existing full-fledged Python installation
# because we need google.protobuf, PTVSD and other stuff!!!
paths = os.environ['Path']
paths = paths.split(';')
for path in paths:
    if path.find('Python')!=-1:
        if path.find('Blender')==-1:
            sPath=path.split('\\')
            if sPath[-2].find('Python')!=-1:
                pypath = os.path.join(path,'Lib','site-packages')
                sys.path.append(pypath)

# Add the current path of THIS file
this_path = os.path.dirname(os.path.realpath(__file__))
sys.path.append(this_path)

from .BlenderVisitor import BlenderVisitor

# Taken from https://github.com/Microsoft/PTVS/wiki/Cross-Platform-Remote-Debugging
# Now moved to https://docs.microsoft.com/en-us/visualstudio/python/debugging-cross-platform-remote
# Project repository at https://github.com/Microsoft/ptvsd
# Install latest version from pypi at https://pypi.org/project/ptvsd/
#
# From Visual Studio 2019 or Visuals Studio Code: Attach to PTSV Python Remote debuggee using "tcp://localhost:5678" (NO Secret!)
try:
    import ptvsd
except Exception:
    print('PTSV Debugging disabled: import ptvsd failed')

try:
    # accoording to https://code.visualstudio.com/docs/python/debugging#_troubleshooting 
    ptvsd.debug_this_thread()
except Exception:
    print('PTSV Debugging disabled: ptvsd.debug_this_thread() failed')

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
 
    #Operator Properties
    filepath : StringProperty(subtype='FILE_PATH')

    doApplyScale: BoolProperty(
        name="Apply Scale",
        description="Apply object scale tranformations on meshes and reset scale settings to 1.0",
        default=True,
    )

    doRecalcOutside: BoolProperty(
        name="Recalculate Outside",
        description="Try to find out outside of each mesh and make normals face outwards",
        default=True,
    )

    doApplyModifiers : BoolProperty(
        name="Apply Modifiers",
        description="Apply modifiers such as 'Subdivision Surface' or 'Edge Split' to mesh objects",
        default=True,
    )

    #get FuseeRoot environment variable
    tool_Path = 'fusee.exe'
    isRoot = None
    # path of fusee.exe
    convtool_path = tool_Path

    def draw(self, context):
        layout = self.layout
        # layout.prop(self, 'isWeb')
        layout.prop(self,'doApplyScale')
        layout.prop(self,'doRecalcOutside')
        layout.prop(self,'doApplyModifiers')
        
    def execute(self, context):
        #check if all paths are set
        if not self.filepath:
            raise Exception("file path not set")
        
        roots = set()
        for obj in bpy.data.objects:
            parent = GetParents(obj)
            roots.add(parent)
            try:
                roots.remove(None)
            except:
                pass
        
        #set blender to object mode and deselect everything (prevents problems)
        if bpy.context.mode != 'OBJECT' and bpy.ops.object.mode_set.poll():
            bpy.ops.object.mode_set(mode="OBJECT")
        bpy.ops.object.select_all(action='DESELECT')

        visitor = BlenderVisitor()

        visitor.DoApplyModifiers = self.doApplyModifiers
        visitor.DoApplyScale = self.doApplyScale
        visitor.DoRecalcOutside = self.doRecalcOutside
 
        visitor.TraverseList(roots)
        # visitor.PrintFus()
        visitor.WriteFus(self.filepath)
    
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


#### UTILITY METHODS ####

def GetParents(obj):
    """Recursively search for the highest parent object"""
    if obj.parent == None:
        return obj
    elif obj.parent != None:
        GetParents(obj.parent)

