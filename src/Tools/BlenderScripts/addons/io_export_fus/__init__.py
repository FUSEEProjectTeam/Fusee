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

import subprocess,os,sys, time
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
import bmesh
from mathutils import *
from math import *

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
        name="Create FUSEE Web-Application",
        description="Apply modifiers to mesh objects",
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
        timeLast = time.perf_counter()
        visitor.WriteFus(self.filepath)
        timeCur = time.perf_counter()
        print(str(timeCur-timeLast) + 's to serialize entire payload')
        timeLast = timeCur
    
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

def GetPaths(filepath):
    # relative filepath -> absolute filepath
    basename = os.path.basename(filepath)
    if os.path.dirname(filepath) == '//':
        filepath = os.path.join(os.path.dirname(bpy.data.filepath), basename)        
    else:
        filepath = filepath.replace("//", "")
        filepath = os.path.join(os.path.dirname(bpy.data.filepath), filepath)        
    fullpath = filepath
    return fullpath, basename

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

        # Mesh Processing flags
        self.DoApplyScale = True
        self.DoRecalcOutside = True
        self.DoApplyModifiers = True

        self.__fusWriter = FusSceneWriter()
        self.__level = 0
        self.__textures = []
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
        for texture in self.__textures:
            src = texture
            dst = os.path.join(os.path.dirname(self.filepath),os.path.basename(texture))
            copyfile(src,dst)

    def __AddTransform(self, obj):
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
            (1, 1, 1) if self.DoApplyScale else (scale.x, scale.z, scale.y)
        )

    def __GetProcessedBMesh(self, obj):
        """Create a modifier-applied, normal-flipped, scale-normalized, triangulated BMesh from the Blender mesh object passed. Call result.free() and del result after use on the returned bmesh."""

        # Create a (deep (linked=False)) copy of obj.
        # Makes use of the new 2.8 override context (taken from https://blender.stackexchange.com/questions/135597/how-to-duplicate-an-object-in-2-8-via-the-python-api-without-using-bpy-ops-obje )
        bpy.ops.object.duplicate(
            {
                "object" : obj,
                "selected_objects" : [obj]
            },
            linked=False
        )
        # In case obj has an armature parent, bpy.context.active as well as bpy.context.object point to obj's parent AND NOT AS DOCUMENTED to the newly duplicated object.
        # As debugging shows, the first (and only) object contained in bpy.context.selected_objects points to the duplication.
        obj_copy = bpy.context.selected_objects[0]

        # Select the copy (and nothing else)
        if bpy.context.mode != 'OBJECT' and bpy.ops.object.mode_set.poll():
            bpy.ops.object.mode_set(mode = 'OBJECT')
        bpy.ops.object.select_all(action='DESELECT')
        bpy.context.view_layer.objects.active = obj_copy
        obj_copy.select_set(True)

        # Apply Scale
        if self.DoApplyScale:
            bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    
        # Flip normals on the copied object's copied mesh (Important: This happens BEFORE any modifiers are applied)
        if self.DoRecalcOutside:
            bpy.ops.object.mode_set(mode = 'EDIT')
            bpy.ops.mesh.select_all(action='SELECT')
            bpy.ops.mesh.normals_make_consistent(inside=False)
            bpy.ops.object.mode_set(mode = 'OBJECT')

        # Apply all the modifiers (without altering the scene)
        # Taken from https://docs.blender.org/api/blender2.8/bpy.types.Depsgraph.html, "Evaluated ID example"
        if self.DoApplyModifiers:
            ev_depsgraph = bpy.context.evaluated_depsgraph_get()
            obj_eval = obj_copy.evaluated_get(ev_depsgraph)
            mesh_eval = obj_eval.data
        else:
           mesh_eval = obj_copy.data

        # Triangulate
        bm = bmesh.new()
        bm.from_mesh(mesh_eval)
        # Normal recalculation already performed BEFORE modifiers were applied. Otherwise face recalculation yields normals facing inside
        # bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
        bmesh.ops.triangulate(bm, faces=bm.faces)

        # Just for Debugging: link the triangulated mesh:
        # obj_triangulated = bpy.data.objects.new("ObjectTriangulated", mesh_triangulated)
        # bpy.context.collection.objects.link(obj_triangulated)
        
        # Delete duplicated object
        bpy.ops.object.mode_set(mode = 'OBJECT')
        bpy.ops.object.select_all(action='DESELECT')
        bpy.context.view_layer.objects.active = obj_copy
        obj_copy.select_set(True)
        bpy.ops.object.delete()         

        return bm  # Don't forget to release the returned bmesh

        # mesh_triangulated = bpy.data.meshes.new("MeshTriangulated")
        # bm.to_mesh(mesh_triangulated)
        # bm.free()
        # del bm

        # return mesh_triangulated

    #def __AddBoundingBox(self, meshobj):
    #    """Retrieves the bounding box from the given mesh object and adds it to the currently written FUSEE mesh"""
    #    bbox = meshobj.bound_box
    #    bboxList = []
    #    for bboxVal in bbox:
    #        bboxList.append(list(bboxVal))
    #
    #    # find min and max values of the bounding box and write them to the mesh
    #    bboxMin = min(bboxList)
    #    bboxMax = max(bboxList)
    #
    #    self.__fusWriter.AddBoundingBox(
    #        (bboxMin[0], bboxMin[2], bboxMin[1]),
    #        (bboxMax[0], bboxMax[2], bboxMax[1])
    #    )

    def __AddMaterial(self, materialslot):
        # MATERIAL COMPONENT
        nodes = materialslot.node_tree.nodes
        hasDiffuse = False
        hasSpecular = False
        hasEmissive = False
        hasMix = False
        hasBump = False
        hasPBR = False

        diffColor = None
        diffTexture = None
        diffMix = 1 

        specColor = None
        specTexture = None
        specMix = 1 
        specShininess = 0.2
        specIntensity = 0.2

        emissColor = None
        emissTexture = None
        emissMix = 1 

        bumpTexture = None
        bumpIntensity = 1

        pbrRoughnessValue = 0.2
        pbrFresnelReflectance = 0.2
        pbrDiffuseFraction = 0.2

        # Iterate over nodes of the material node tree
        for node in nodes:                    
            #### DIFFUSE
            if node.type == 'BSDF_DIFFUSE' and hasDiffuse == False:
                hasDiffuse = True

                diffColor = node.inputs['Color'].default_value        # Color
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                        difftexture = basename
                        self.__textures.append(fullpath)

            #### SPECULAR
            elif node.type == 'BSDF_GLOSSY' and hasSpecular == False:
                hasSpecular = True

                specColor = node.inputs['Color'].default_value
                # get material roughness and set the specularity = 1-roughness
                roughness = node.inputs['Roughness'].default_value
                specShininess = (1 - roughness) * 200       # multipy with factor 200 for tight specular light
                if not hasMix:
                    specIntensity = 1.0 - (roughness + 0.2)     # reduce intensity quite a bit
                # Check, if material has got textures. If so, get texture filepath
                links = node.inputs['Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                        specTexture = basename
                        self.__textures.append(fullpath)

            #### EMISSIVE
            elif node.type == 'EMISSION' and hasEmissive == False:
                print('----found emissive node')
                hasEmissive = True
                # get material color
                emissColor = node.inputs['Color'].default_value
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                        emissTexture = basename
                        self.__textures.append(fullpath)

            #### SPECULAR INTENSITY, found in a MIX node
            elif node.type == 'MIX_SHADER' and hasMix == False:
                hasMix = True
                # mix factor between glossy and diffuse
                factor = node.inputs['Fac'].default_value
                # determine, on which socket the glossy shader is connected to the Mix Shader
                if node.inputs[1].links[0].from_node.type == 'BSDF_GLOSSY':
                    specIntensity = 1 - factor
                elif node.inputs[1].links[0].from_node.type == 'BSDF_DIFFUSE':
                    specIntensity = factor
                else:
                    specIntensity = 1

            #### BUMP
            elif node.type == 'NORMAL_MAP':
                hasBump = True
                bumpIntensity =  node.inputs['Strength'].default_value                         
                links = node.inputs['Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                        bumpTexture = basename
                        self.__textures.append(fullpath)

            #### OR GET IT ALL FROM PRINCIPLED
            elif node.type == 'BSDF_PRINCIPLED':
                # Read all relevant information into easy-to-access variables
                baseColor = node.inputs['Base Color'].default_value
                subsurface = node.inputs['Subsurface'].default_value
                subsurfaceColor = node.inputs['Subsurface Color'].default_value
                metallic = node.inputs['Metallic'].default_value
                specular = node.inputs['Specular'].default_value
                specularTint = node.inputs['Specular Tint'].default_value
                roughness = node.inputs['Roughness'].default_value
                antistropic = node.inputs['Anisotropic'].default_value
                antistriopicRot = node.inputs['Anisotropic Rotation'].default_value
                IOR = node.inputs['IOR'].default_value
                
                hasDiffuse = True
                diffMix = 1
                diffColor = baseColor
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Base Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(
                            node.inputs['Base Color'].links[0].from_node.image.filepath)
                        diffTexture = basename
                        self.__textures.append(fullpath)

                hasSpecular = True
                specMix = 1
                # get material color
                specColor = subsurfaceColor
                specShininess = (1 - roughness) * 200 # multiply with factor 100 for tight specular light
                specIntensity = 1.0 - (roughness + 0.2) # reduce intensity quite a bit

                # TODO: Bump and Emissive from BSDF_PRINCIPLED

                hasPBR = True
                pbrRoughnessValue = roughness
                pbrFresnelReflectance = specular
                pbrDiffuseFraction = metallic

        if hasDiffuse:
            self.__fusWriter.BeginMaterial()
            self.__fusWriter.AddDiffuse(diffColor, diffTexture, diffMix)
            if hasSpecular:
                self.__fusWriter.AddSpecular(specColor, specTexture, specMix, specShininess, specIntensity)
            if hasEmissive:
                self.__fusWriter.AddEmissive(emissColor, emissTexture, emissMix)
            if hasBump:
                self.__fusWriter.AddBump(bumpTexture, bumpIntensity)
            if hasPBR:
                self.__fusWriter.AddPBRMaterialSettings(pbrRoughnessValue, pbrFresnelReflectance, pbrDiffuseFraction)
            self.__fusWriter.EndMaterial()
        else:
            self.__AddDefaultMaterial()

    def __AddDefaultMaterial(self):
        self.__fusWriter.AddMaterial(
            {
                'Diffuse':  { 'Color' : (0.7, 0.7, 0.7, 1), 'Mix': 1 },
                'Specular': { 'Color' : (1, 1, 1, 1), 'Shininess': 0.3, 'Intensity': 0.2 },
            })

    def VisitMesh(self, mesh):
        self.__fusWriter.AddChild(mesh.name)
        self.__AddTransform(mesh)

        materialCount = max(1, len(mesh.material_slots))

        vertsPerMat = []

        if materialCount == 1:
            if len(mesh.material_slots) < 1:
                print('WARNING: Object "' + mesh.name + '" has no material. Adding Default material.')
                self.__AddDefaultMaterial()
            else:
                self.__AddMaterial(mesh.material_slots[0].material)                

        # <Sort vertices into material bins>
        for iMaterial in range(materialCount):
            vertsPerMat.append([])            

        bm = self.__GetProcessedBMesh(mesh)

        uvActive = mesh.data.uv_layers.active
        uv_layer = None
        if uvActive is not None:
            uv_layer = bm.loops.layers.uv.active

        timeLast = time.perf_counter()
        
        for f in bm.faces:
            # print("Triangle with material ", f.material_index)
            for fl in f.loops:
                vertex = fl.vert.co
                normal = fl.vert.normal if f.smooth else f.normal
                uv = fl[uv_layer].uv if uv_layer is not None else mathutils.Vector((0, 0))
                vertsPerMat[f.material_index].append((
                    (vertex[0], vertex[2], vertex[1]), 
                    (normal[0], normal[2], normal[1]),
                    (uv[0], uv[1])))
        
        timeCur = time.perf_counter() 
        print(str(timeCur-timeLast) + 's to sort vertices into material bins.')
        timeLast = timeCur
        
        bm.free()
        del bm
        # </Sort vertices into material bins>

        # <For each material: Write out vertices into FUSEE objects>
        for iMaterial in range(materialCount):
            if materialCount > 1:
                self.__fusWriter.Push()
                self.__fusWriter.AddChild(mesh.name + '_' + mesh.material_slots[iMaterial].material.name)
            
            self.__AddMaterial(mesh.material_slots[iMaterial].material)

            nVertsTotal = len(vertsPerMat[iMaterial])
            iVert = 0
            iChunk = 0

            while iVert < nVertsTotal:
                if iChunk > 0:
                    self.__fusWriter.Push()
                    self.__fusWriter.AddChild(mesh.name + '_' + mesh.material_slots[iMaterial].material.name + '_' + str(iChunk))

                vert = vertsPerMat[iMaterial][iVert]
                self.__fusWriter.BeginMesh(
                    vert[0],    # Vertex 
                    vert[1],    # Normal
                    vert[2]     # UV
                )
                iVert = iVert + 1
                iVertPerChunk = 1
                while self.__fusWriter.MeshHasCapacity() and iVert < nVertsTotal:
                    vert = vertsPerMat[iMaterial][iVert]
                    self.__fusWriter.AddVertex(
                        vert[0],    # Vertex 
                        vert[1],    # Normal
                        vert[2]     # UV
                    )
                    if iVert % 10000 == 0:
                        timeCur = time.perf_counter() 
                        print(str(timeCur-timeLast) + 's to add another 10000 vertices.')
                        timeLast = timeCur

                    iVert = iVert + 1
                    iVertPerChunk = iVertPerChunk + 1

                print ('Chunk ' + mesh.name + '_' + mesh.material_slots[iMaterial].material.name + '_' + str(iChunk) + ' containing ' + str(iVertPerChunk) + ' (=3*' + str(iVertPerChunk/3) + ') verts.') 
                print ('iVert: ' + str(iVert) + ', nVertsTotal: ' + str(nVertsTotal))

                self.__fusWriter.EndMesh()
                if iChunk > 0:
                    self.__fusWriter.Pop()
 
                iChunk = iChunk + 1

            if materialCount > 1:
                self.__fusWriter.Pop()
        # </For each material: Write out vertices into FUSEE objects>


    def VisitLight(self, light):
        ### Collect light data ###
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

        print('Warning: Light "' + light.name + '"found but NOT exported"')
        ### Write light node ###
#        self.__fusWriter.AddChild(light.name)
#        self.__AddTransform(light)
#        self.__fusWriter.AddLight(
#            True, 
#            (lightData.color.r, lightData.color.g, lightData.color.b, 1),
#            lightData.distance,
#            lightData.energy / 1000.0,
#            lightType,
#            outerconeangle,
#            innerconeangle
#        )

    def VisitCamera(self, camera):
        ### Collect camera data ###
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
        
        print('Warning: Camera "' + camera.name + '"found but NOT exported"')
        ### Write camera node ###
#        self.__fusWriter.AddChild(camera.name)
#        self.__AddTransform(camera)
#        self.__fusWriter.AddCamera(
#            camType, 
#            cameraData.angle_y, 
#            (cameraData.clip_start, cameraData.clip_end),
#        )

    def VisitArmature(self, armature):
        self.__fusWriter.AddChild(armature.name)
        print('Armature: ' + armature.name)       

    def VisitUnknown(self, ob):
        print('WARNING: Type: ' + ob.type + ' of object ' + ob.name + ' not handled ')       


