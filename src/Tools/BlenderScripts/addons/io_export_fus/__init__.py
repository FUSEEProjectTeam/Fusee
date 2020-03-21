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
# from .SerializeData import SerializeData, GetParents
from .FusSceneWriter import FusSceneWriter

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
    createWebApp : BoolProperty(
        name="Create FUSEE Web-Application",
        description="Export HTML-Viewer around the scene and run in Web Browser after export",
        default=False,
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
        # layout.prop(self, 'isWeb')
        
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
        
        #set blender to object mode (prevents problems)
        bpy.ops.object.mode_set(mode="OBJECT")

        visitor = BlenderVisitor()
        visitor.TraverseList(roots)
        visitor.PrintFus()
        visitor.WriteFus(self.filepath)
    
        print('DONE')
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

##### BLENDER VISITOR ####

class BlenderVisitor:
    """Traverses the Blender Scene Graph. During traversal, collects relevant data from Blender objects and invokes appropriate actions on
       a FusSceneWriter to store the data for later serialization into the .fus file format."""
    def __init__(self):
        super().__init__()
        self.__fusWriter = FusSceneWriter()
        self.__level = 0
        self.__visitors = {
            'MESH':     self.VisitMesh,
            'LIGHT':    self.VisitLight,
            'CAMERA':   self.VisitCamera,
            'ARMATURE': self.VisitArmature,
        }

    def TraverseList(self, oblist):
        """Traverses the given list of Blender objects. Most likely, this is the entry point for the traversal. Will also be called recursively from TraverseOb."""
        for ob in oblist:
            self.TraverseOb(ob)

    def TraverseOb(self, ob):
        """Traverse the given blender object. Call the appropriate visitor based on the object's Blender type and recursively traverse the list of children"""
        visitor = self.__visitors.get(ob.type, self.VisitUnknown)
        visitor(ob)
        if len(ob.children) > 0:
            self.__fusWriter.Push()
            self.TraverseList(ob.children)
            self.__fusWriter.Pop()

    def PrintFus(self):
        """Print the current FUSEE file contents for debugging purposes"""
        self.__fusWriter.Print()

    def WriteFus(self, filepath):
        """Serialize the current FUSEE contents to the given file path"""
        self.__fusWriter.Serialize(filepath)

    def __HandleTransform(self, obj):
        """Convert the given blender obj's transformation into a FUSEE Transform component"""
        # Neutralize the blender-specific awkward parent inverse as it is not supported by FUSEE's scene graph
        if obj.parent is None:
            obj_mtx_clean = obj.matrix_world.copy()
        else:
            obj_mtx_clean = obj.parent.matrix_world.inverted() @ obj.matrix_world

        location, rotation, scale = obj_mtx_clean.decompose()
        rot_eul = rotation.to_euler('YXZ')

        self.__fusWriter.AddTransform(
            (location.x, location.z, location.y),
            (-rot_eul.x, -rot_eul.z, -rot_eul.y),
            (scale.x, scale.z, scale.y)
        )

    def VisitMesh(self, mesh):
        self.__fusWriter.AddChild(mesh.name)
        self.__HandleTransform(mesh)
        print('Mesh: ' + mesh.name)       

    def VisitLight(self, light):
        self.__fusWriter.AddChild(light.name)
        self.__HandleTransform(light)

        # lightData = bpy.data.lights[light.data.name]
        lightData = light.data

        lightType = 0
        outerconeangle = 1.6
        innerconeangle = 1
        if lightData.type == 'SPOT':
            lightType = 2 # FusScene.LightType.Value('Spot')
            outerconeangle = lightData.spot_size
            innerconeangle = outerconeangle * (1.0 - lightData.spot_blend)
        elif lightData.type == 'SUN':
            lightType = 1 # FusScene.LightType.Value('Parallel')
        else:
            lightType = 0 # FusScene.LightType.Value('Point')

        self.__fusWriter.AddLight(
            True, 
            (lightData.color.r, lightData.color.g, lightData.color.b, 1),
            lightData.distance,
            lightData.energy / 1000.0,
            lightType,
            outerconeangle,
            innerconeangle
        )

    def VisitCamera(self, camera):
        self.__fusWriter.AddChild(camera.name)
        self.__HandleTransform(camera)

        # cameraData = bpy.data.cameras[camera.data.name]
        cameraData = camera.data

        camType = 0 # Perspective
        if cameraData.type == 'PERSP':
            camType = 0
        elif cameraData.type == "ORTHO":
            camType = 1
            print('WARNING: Orthographic projection type on Camera object "' + camera.name + '" is not handled correctly (yet) by FUSEE')
        else:
            print('WARNING: Unknown projection type "' + cameraData.type + '" on Camera object "' + camera.name + '"')
        self.__fusWriter.AddCamera(
            camType, 
            cameraData.angle_y, 
            (cameraData.clip_start, cameraData.clip_end),
        )

    def VisitArmature(self, armature):
        self.__fusWriter.AddChild(armature.name)
        print('Armature: ' + armature.name)       

    def VisitUnknown(self, ob):
        print('WARNING: Type: ' + ob.type + ' of object ' + ob.name + ' not handled ')       


