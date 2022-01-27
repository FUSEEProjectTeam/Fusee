#from .FusSceneWriter import FusSceneWriter
from FusSceneWriter import FusSceneWriter

import subprocess,os,sys, time
from shutil import copyfile
import traceback
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
import mathutils
from math import *

try:
    # Standard Python import
    from proto import FusSerialization_pb2 as FusSer
except Exception:
        try:
            # The hard (blender) way
            dir_path = os.path.dirname(os.path.realpath(__file__))
            dir_path = os.path.join(dir_path, 'proto')
            sys.path.append(dir_path)        
            import FusSerialization_pb2 as FusSer
        except Exception as ex:
            print('Error importing "FusSerialization_pb2.py" from "' + dir_path + '"' + '\n' + ex)


def GetPaths(filepath):
    # relative filepath -> absolute filepath
    if os.path.dirname(filepath) == '//':
        basename = os.path.basename(filepath)
        filepath = os.path.join(os.path.dirname(bpy.data.filepath), basename)        
    else:
        filepath = filepath.replace("//", "")
        basename = os.path.basename(filepath)
        filepath = os.path.join(os.path.dirname(bpy.data.filepath), filepath)        
    fullpath = filepath
    return fullpath, basename

class BlenderVisitor:
    """Traverses the Blender Scene Graph. During traversal, collects relevant data from Blender objects and invokes appropriate actions on
       a FusSceneWriter to store the data for later serialization into the .fus file format."""
    def __init__(self):
        super().__init__()
        self.__defaultMatName = 'FUSEE_Default_Material'

        # Mesh Processing flags
        self.DoApplyScale = True
        self.DoRecalcOutside = True
        self.DoApplyModifiers = True
        self.DoBakeNLATracks = False
        self.fps = bpy.context.scene.render.fps
        self.__matrixStack = [mathutils.Matrix.Identity(4)]
        self.__transformStack = [(mathutils.Vector((0, 0, 0)), mathutils.Quaternion(), mathutils.Vector((0, 0, 0)), 0.0)]
        self.__fusWriter = FusSceneWriter()
        self.__textures = []
        self.__visitors = {
            'MESH':     self.VisitMesh,
            'LIGHT':    self.VisitLight,
            'CAMERA':   self.VisitCamera,
            'ARMATURE': self.VisitArmature,
        }

    def XFormPush(self, ob):
        # Retrieve the matrix describing the relative transformation from its (already converted and thus potentially scale-applied) parent
        curMatrix = self.__matrixStack[-1].inverted() @ ob.matrix_world
        # Decompose relative position, rotation and scale from it
        curLoc, curRot, curScale = curMatrix.decompose()
        if self.DoApplyScale:
            # If scale will be applied on the mesh, only take the position and rotation part
            curMatrix = mathutils.Matrix.Translation(curLoc) @ curRot.to_matrix().to_4x4()
        # Push the absolute transformation matrix onto the stack for possible children
        self.__matrixStack.append(self.__matrixStack[-1] @ curMatrix)
        # Push the relative individual transformtion parameters (pos, rot, scale) onto the stack
        self.__transformStack.append((curLoc, curRot, curScale, 0))

    def XFormPop(self):
        self.__matrixStack.pop()
        self.__transformStack.pop()

    def XFormGetTOSMatrix(self):
        return self.__matrixStack[-1]

    def XFormGetTOSTransform(self):
        return self.__transformStack[-1]


    def TraverseList(self, oblist):
        """Traverses the given list of Blender objects. Most likely, this is the entry point for the traversal. Will also be called recursively from TraverseOb."""
        for ob in oblist:
            self.TraverseOb(ob)

    def TraverseOb(self, ob):
        """Traverse the given blender object. Call the appropriate visitor based on the object's Blender type and recursively traverse the list of children"""
        visitor = self.__visitors.get(ob.type, self.VisitUnknown)
        self.XFormPush(ob)
        visitor(ob)
        if len(ob.children) > 0:
            self.__fusWriter.Push()
            self.TraverseList(ob.children)
            self.__fusWriter.Pop()
        self.XFormPop()

    def PrintFus(self):
        """Print the current FUSEE file contents for debugging purposes"""
        self.__fusWriter.Print()

    def WriteFus(self, filepath):
        """Serialize the current FUSEE contents to the given file path"""
        self.__fusWriter.Serialize(filepath)
        for texture in self.__textures:
            src = texture
            dst = os.path.join(os.path.dirname(filepath),os.path.basename(texture))
            copyfile(src,dst)

    def __AddTransform(self):
        """Convert the current blender obj's transformation into a FUSEE Transform component"""
        location, rotation, scale, dummy = self.XFormGetTOSTransform()
        rot_eul = rotation.to_euler('YXZ')

        if self.DoApplyScale:
            scale =  mathutils.Vector((1.0, 1.0, 1.0))

        self.__fusWriter.AddTransform(
            (location.x, location.z, location.y),
            (-rot_eul.x, -rot_eul.z, -rot_eul.y),
            (scale.x, scale.z, scale.y),
        )
    # Adds a NLA-Animation if one exists
    def __AddAnimationIfPresent(self, ob):
        try:
            selected_strips = [strip for strip in ob.animation_data.nla_tracks]
            # Variables that store inserted F-Curves and Keyframes to delete them later. 
            newFC = []
            newKP = []

            for strip in selected_strips:
                for strips in strip.strips:
                    self.__fusWriter.BeginAnimation()
                    action = strips.action
                    # Create placeholder to see which/if axis is missing
                    loc = [None] * 3
                    rotE = [None] * 3
                    scl = [None] * 3      
                    for af in action.fcurves:
                        if(af.data_path == "location"):
                            loc[af.array_index] = af
                        elif(af.data_path == "rotation_euler"):
                            rotE[af.array_index] = af
                        elif(af.data_path == "scale"):
                            scl[af.array_index] = af
                    # If the data_path is still empty because no one used i.e. Scale the list will be deleted        
                    if(loc.count(None) == len(loc)):
                        loc.clear()
                    if(rotE.count(None) == len(rotE)):
                        rotE.clear()
                    if(scl.count(None) == len(scl)):
                        scl.clear()

                    # Creates new F-Curves that don't exist yet     
                    for idx in range(3):
                        if(len(loc) > 0):
                            if(loc[idx] == None):
                                locf = action.fcurves
                                loc[idx] = (locf.new("location", index=idx, action_group=str(locf[0].group.name)))
                                newFC.append(loc[idx])
                            
                        if(len(rotE) > 0):
                            if(rotE[idx] == None):
                                rotEf = action.fcurves
                                rotE[idx] = (rotEf.new("rotation_euler", index=idx, action_group=str(rotEf[0].group.name)))
                                newFC.append(rotE[idx])
                            
                        if(len(scl) > 0):
                            if(scl[idx] == None):
                                sclf = action.fcurves
                                scl[idx] = (sclf.new("scale", index=idx, action_group=str(sclf[0].group.name)))
                                newFC.append(scl[idx])
                    
                    #Starts the AnimationTrack/AddKeyframe creation
                    if(len(loc) > 0):
                        newKP = self.createKP(loc, newKP, ob)
                        self.__fusWriter.BeginAnimationTrack("Translation", FusSer.Float3, FusSer.Lerp)    
                        self.AddKeyframes(loc)
                        self.__fusWriter.EndAnimationTrack()
                    if(len(rotE) > 0):
                        newKP = self.createKP(rotE, newKP, ob)
                        self.__fusWriter.BeginAnimationTrack("Rotation", FusSer.Float3, FusSer.Lerp) 
                        self.AddKeyframes(rotE)
                        self.__fusWriter.EndAnimationTrack()  
                    if(not self.DoApplyScale):
                        if(len(scl) > 0):
                            newKP = self.createKP(scl, newKP, ob)
                            self.__fusWriter.BeginAnimationTrack("Scale", FusSer.Float3, FusSer.Lerp)   
                            self.AddKeyframes(scl)
                            self.__fusWriter.EndAnimationTrack()                 


                    self.DeleteCreatedKPFC(action, newKP, newFC)
                    self.__fusWriter.EndAnimation()
        except Exception:
            print(traceback.format_exc())
            selected_strips = []
        self.__fusWriter.EndAnimation()

    def AddKeyframes(self, _fcurve):
        # For the number of Keyframes go through the FCurves and create a tuple from them
        if(_fcurve[0].data_path == "rotation_euler"):
            for i in range(len(_fcurve[0].keyframe_points)):
                keyframe = []
                for fc in _fcurve:
                    keyframe.append(-(fc.keyframe_points[i].co[1]))
                self.__fusWriter.AddKeyframe(_fcurve[0].keyframe_points[i].co[0] / self.fps, keyframe)
        else:
            for i in range(len(_fcurve[0].keyframe_points)):
                keyframe = []
                for fc in _fcurve:
                    keyframe.append(fc.keyframe_points[i].co[1])
                self.__fusWriter.AddKeyframe(_fcurve[0].keyframe_points[i].co[0] / self.fps, keyframe)


    def DeleteCreatedKPFC(self, _action, _newKP, _newFC):
        # Variable for deletion
        kpidx = 0
        preKP = None
        preFC = None
        # Sort the FCurves by data_path and array_index
        _newKP = sorted(_newKP, key=lambda k: (k[0].data_path, k[0].array_index))

        for idx in range(len(_newKP)):
            if(_newKP[idx][0].array_index != preKP or _newKP[idx][0].data_path != preFC):
                kpidx = 0
            else:
                kpidx += 1  
            # Find keyframes on the FCurve and remove them
            _newKP[idx][0].keyframe_points.remove(_newKP[idx][0].keyframe_points[(_newKP[idx][1]) - kpidx])
            preKP = _newKP[idx][0].array_index
            preFC = _newKP[idx][0].data_path   
            
        for deleteFC in _newFC:
            # Remove created FCurves
            _action.fcurves.remove(deleteFC)

    def createKP(self, fcs, newKP, ob):
        # Find the earliest Keyframe frame
        firstFC = None
        for fc in fcs:
            if(len(fc.keyframe_points)!= 0):
                if(firstFC == None):
                    firstFC = fc.keyframe_points[0].co[0]
                elif(firstFC > fc.keyframe_points[0].co[0]):
                    firstFC = fc.keyframe_points[0].co[0]
        
        for fc in fcs:
            for cfc in fcs:
                # Create the first Keyframe if it doesn't exist or the Keyframe frame is later than the firstFC 
                if(len(cfc.keyframe_points)!= 0):
                    if(cfc.keyframe_points[0].co[0] != firstFC):
                        cfc.keyframe_points.insert(frame=firstFC, value=cfc.evaluate(firstFC))
                        newKP.append([cfc, 0])
                        
                else:
                    
                    cfc.keyframe_points.insert(frame=firstFC, value=getattr(ob, cfc.data_path)[cfc.array_index])
                    newKP.append([cfc, 0])

                # Create the remaining Keyframes    
                if(cfc != fc):       
                    for kpi in range(len(fc.keyframe_points)):
                        if(len(cfc.keyframe_points) > kpi):
                            if(cfc.keyframe_points[kpi].co[0] != fc.keyframe_points[kpi].co[0]):
                                cfc.keyframe_points.insert(frame=fc.keyframe_points[kpi].co[0], value=cfc.evaluate(fc.keyframe_points[kpi].co[0]))
                                newKP.append([cfc, kpi])
                                print(str(cfc.array_index) + " " + str(cfc.keyframe_points[kpi].co[0]) + " " + str(fc.array_index) +  " " + str(fc.keyframe_points[kpi].co[0]))
                        else:
                            cfc.keyframe_points.insert(frame=fc.keyframe_points[kpi].co[0], value=cfc.evaluate(fc.keyframe_points[kpi].co[0]))
                            newKP.append([cfc, kpi])
                    
        return newKP  
    
    def __GetProcessedBMesh(self, obj):
        """Create a modifier-applied, normal-flipped, scale-normalized, triangulated BMesh from the Blender mesh object passed. Call result.free() and del result after using the returned bmesh."""

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
        #if self.DoApplyScale:
        #    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    
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

        # Get a BMesh for scaling and triangulation
        bm = bmesh.new()
        bm.from_mesh(mesh_eval)

        # Apply Scale
        if self.DoApplyScale:
            scale = self.XFormGetTOSTransform()[2]
            bmesh.ops.scale(bm, vec=scale, verts=bm.verts)

        # Triangulate
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
        matName = materialslot.name
        if self.__fusWriter.TryReferenceMaterial(matName):
            return

        # MATERIAL COMPONENT
        nodes = materialslot.node_tree.nodes
        hasDiffuse = False
        hasSpecular = False
        hasEmissive = False
        hasMix = False
        hasBump = False
        hasBRDF = False
        hasRoughnessOnly = False

        albedoColor = None
        albedoTexture = None
        albedoMix = 1 

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

        roughnessVal = 0.3
        metallicVal = 0.0
        specularVal = 0.5
        iorVal = 1.46
        subsurfaceVal = 0.0
        subsurfaceColorVal = None

        # Iterate over nodes of the material node tree
        for node in nodes:                    
            #### DIFFUSE
            if node.type == 'BSDF_DIFFUSE' and hasDiffuse == False:
                hasDiffuse = True
                hasRoughnessOnly = True

                albedoColor = node.inputs['Color'].default_value        # Color
                roughnessVal = node.inputs['Roughness'].default_value
                # check, if material has got textures. If so, get texture filepath
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Color'].links
                
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(
                            node.inputs['Color'].links[0].from_node.image.filepath)
                        albedoTexture = basename
                        self.__textures.append(fullpath)

                linksNorm = node.inputs['Normal'].links                
                if len(linksNorm) > 0:
                    if linksNorm[0].from_node.type == 'TEX_IMAGE':
                        hasBump = True
                        fullpath, basename = GetPaths(node.inputs['Normal'].links[0].from_node.image.filepath)
                        bumpTexture = basename
                        self.__textures.append(fullpath)

            #### SPECULAR
            elif node.type == 'BSDF_GLOSSY' and hasSpecular == False:
                hasRoughnessOnly = True
                hasDiffuse = False

                albedoColor = node.inputs['Color'].default_value
                roughnessVal = node.inputs['Roughness'].default_value                
                
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(
                            node.inputs['Color'].links[0].from_node.image.filepath)
                        albedoTexture = basename
                        self.__textures.append(fullpath)

                linksNorm = node.inputs['Normal'].links
                if len(linksNorm) > 0:
                    if linksNorm[0].from_node.type == 'TEX_IMAGE':
                        hasBump = True
                        fullpath, basename = GetPaths(node.inputs['Normal'].links[0].from_node.image.filepath)
                        bumpTexture = bumpTexture
                        self.__textures.append(fullpath)

            #### EMISSIVE
            elif node.type == 'EMISSION' and hasEmissive == False:
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
                albedoMix = 1
                albedoColor = baseColor
                # check, if material has got textures. If so, get texture filepath
                links = node.inputs['Base Color'].links
                if len(links) > 0:
                    if links[0].from_node.type == 'TEX_IMAGE':
                        fullpath, basename = GetPaths(
                            node.inputs['Base Color'].links[0].from_node.image.filepath)
                        albedoTexture = basename
                        self.__textures.append(fullpath)
                
                # BSDF has no shininess-based specular calculation!
              # hasSpecular = True
              # specMix = 1
                # get material color
              # specColor = subsurfaceColor
              # specShininess = (1 - roughness) * 200 # multiply with factor 100 for tight specular light
              # specIntensity = 1.0 - (roughness + 0.2) # reduce intensity quite a bit               

                linksNorm = node.inputs['Normal'].links
                if len(linksNorm) > 0:
                    if linksNorm[0].from_node.type == 'TEX_IMAGE':
                        hasBump = True
                        fullpath, basename = GetPaths(node.inputs['Normal'].links[0].from_node.image.filepath)
                        bumpTexture = basename
                        self.__textures.append(fullpath)

                hasEmissive = True
                emissColor = node.inputs['Emission'].default_value

                hasBRDF = True
                roughnessVal = roughness
                metallicVal = metallic
                specularVal = specular
                iorVal = IOR
                subsurfaceVal = subsurface
                subsurfaceColorVal = subsurfaceColor

        if hasDiffuse:
            self.__fusWriter.BeginMaterial(matName)
            self.__fusWriter.AddAlbedo(albedoColor, albedoTexture, albedoMix)
            if hasSpecular:
                self.__fusWriter.AddSpecular(specColor, specTexture, specMix, specShininess, specIntensity)
            if hasEmissive:
                self.__fusWriter.AddEmissive(emissColor, emissTexture, emissMix)
            if hasBump:
                self.__fusWriter.AddNormalMap(bumpTexture, bumpIntensity)
            if hasBRDF:
                self.__fusWriter.AddBRDFMaterialSettings(roughnessVal, metallicVal, specularVal, iorVal, subsurfaceVal, subsurfaceColorVal)
            if hasRoughnessOnly:
                self.__fusWriter.AddRoughnessOnlyMaterialSettings(roughnessVal, False)
            self.__fusWriter.EndMaterial()
        else:
            if hasRoughnessOnly:
                self.__fusWriter.BeginMaterial(matName)
                self.__fusWriter.AddAlbedo(albedoColor, albedoTexture, albedoMix)                
                if hasBump:
                    self.__fusWriter.AddNormalMap(bumpTexture, bumpIntensity)
                self.__fusWriter.AddRoughnessOnlyMaterialSettings(roughnessVal, True)
                self.__fusWriter.EndMaterial()
            else:
                self.__AddDefaultMaterial()

    def __AddDefaultMaterial(self):
        if self.__fusWriter.TryReferenceMaterial(self.__defaultMatName):
            return
        self.__fusWriter.AddMaterial(
            {
                'Albedo':   { 'Color' : (0.7, 0.7, 0.7, 1), 'Mix': 1 },
                'Specular': { 'Color' : (1, 1, 1, 1), 'Shininess': 0.3, 'Intensity': 0.2 },
            }, self.__defaultMatName)

    def VisitMesh(self, mesh):
        self.__fusWriter.AddChild(mesh.name)
        self.__AddTransform()
        self.__AddAnimationIfPresent(mesh)
        
        if self.DoApplyScale:
            scale = self.XFormGetTOSTransform()[2]
            appliedScaleStr = '_scl' + str(scale.x) + '_' + str(scale.z) + '_' + str(scale.y)
        else:
            appliedScaleStr = ''

        materialCount = max(1, len(mesh.material_slots))

        vertsPerMat = []

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
        
        #timeCur = time.perf_counter() 
        #print(str(timeCur-timeLast) + 's to sort vertices into material bins.')
        #timeLast = timeCur
        
        bm.free()
        del bm
        # </Sort vertices into material bins>

        # <For each material: Write out vertices into FUSEE objects>
        for iMaterial in range(materialCount):

            # Figure out material name (or default) BEFORE adding the material because we might need it as part of a child node (might be added if more than one material slot is set on the object).
            matPresent = False
            if len(mesh.material_slots) > 0 and mesh.material_slots[iMaterial].name != '':
                matPresent = True
                materialName = mesh.material_slots[iMaterial].name
            else:
                materialName = self.__defaultMatName
            
            if materialCount > 1:
                self.__fusWriter.Push()
                self.__fusWriter.AddChild(mesh.name + '_' + materialName)
            
            if matPresent:
                self.__AddMaterial(mesh.material_slots[iMaterial].material)
            else:
                self.__AddDefaultMaterial()

            nVertsTotal = len(vertsPerMat[iMaterial])
            iVert = 0
            iChunk = 0

            while iVert < nVertsTotal:
                meshName = mesh.data.name + '_mat' + str(iMaterial) + '_chnk' + str(iChunk) + appliedScaleStr
                if iChunk > 0:
                    self.__fusWriter.Push()
                    self.__fusWriter.AddChild(mesh.name)

                if self.__fusWriter.TryReferenceMesh(meshName):
                    iVert = iVert + self.__fusWriter.GetReferencedMeshTriVertCount(meshName)
                else:
                    vert = vertsPerMat[iMaterial][iVert]
                    self.__fusWriter.BeginMesh(
                        vert[0],    # Vertex 
                        vert[1],    # Normal
                        vert[2],    # UV
                        name=meshName
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
                        iVert = iVert + 1
                        iVertPerChunk = iVertPerChunk + 1

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
#        self.__AddTransform(light, false)
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
        
        print('WARNING: Camera "' + camera.name + '"found but NOT exported"')
        ### Write camera node ###
#        self.__fusWriter.AddChild(camera.name)
#        self.__AddTransform(camera, false)
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

