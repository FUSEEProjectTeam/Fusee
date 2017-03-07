#Collect data from blender to convert to proto data

#import
import sys
import os
import getpass
from collections import namedtuple as namedtuple
import bpy
import bmesh
import time
import copy
from bpy import data as data
from mathutils import *
from math import *
dir_path = os.path.dirname(os.path.realpath(__file__))
dir_path = os.path.join(dir_path,'proto\\')
sys.path.append(dir_path)
import Scene_pb2 as Scene

#returns a serialized object, that can be saved to a file
def SerializeData(objects,isWeb, isOnlySelected,smoothing,lamps,smoothingDist,smoothingAngle):
    #init sceneContainer
    sceneContainer = Scene.SceneContainer()
    #write sceneHeader
    sceneHeader = sceneContainer.Header;
    sceneHeader.Version = 0
    sceneHeader.Generator = 'Blender FUSEE Exporter AddOn'
    sceneHeader.CreatedBy = getpass.getuser()
    lt = time.localtime()
    sceneHeader.CreationDate = str(lt.tm_year) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_wday)

    print('SERIALIZING--------')

    #differentiate between the following 2 situations:
    #serialize only selected objects
    textures = []
    if isOnlySelected==True:
        for obj in objects:
            root = GetNode(objects=obj, isWeb=isWeb,
                           isOnlySelected=isOnlySelected, smoothing=smoothing,
                           lamps=lamps,smoothingDist=smoothingDist,
                           smoothingAngle=smoothingAngle)
            sceneChildren = sceneContainer.Children.add()
            sceneChildren.payload = root.obj.SerializePartialToString()
            textures = textures + root.tex

    #serialize all top-level objects and their hierarchy
    else:
        for obj in objects:
            root = GetNode(objects=obj, isWeb=isWeb,
                           isOnlySelected=isOnlySelected, smoothing=smoothing,
                           lamps=lamps,smoothingDist=smoothingDist,
                           smoothingAngle=smoothingAngle)
            sceneChildren = sceneContainer.Children.add()
            sceneChildren.payload = root.obj.SerializePartialToString()
            textures = textures + root.tex
        
    #serialize SceneContainer
    sc = sceneContainer.SerializePartialToString()
    
    #create a namedtuple object for easier data access
    serializedData = namedtuple('serializedData',['obj','tex'])
    serializedData.obj = sc
    serializedData.tex = textures
    return serializedData

#recursivly get each node and its components and serialize them
def GetNode(objects, isWeb, isOnlySelected,smoothing,lamps,smoothingDist,smoothingAngle):
    obj = objects
    if obj.type == 'LAMP' and lamps:
        serializedData = namedtuple('serializedData',['obj','tex'])
        obj = bpy.data.lamps[obj.data.name]
        
        root = Scene.SceneNodeContainer()
        root.Name = obj.name
        print('--' + root.Name)
        rootComponent = root.Components.add()

        rootLightComponent = Scene.SceneComponentContainer()
        lightComponent = rootLightComponent.LightComponent
        
        if obj.type == 'SPOT':
            lightComponent.Type = Scene.LightType.Value('Spot')
        elif obj.type == 'SUN':
            lightComponent.Type = Scene.LightType.Value('Parallel')
        else:
            lightComponent.Type = Scene.LightType.Value('Point')

        try:
            lightComponent.Color.x = 1
            lightComponent.Color.y = 1
            lightComponent.Color.z = 1
            lightComponent.Intensity = 100.0
            
            for node in obj.node_tree.nodes:
                if node.type == 'EMISSION':
                    color = node.inputs['Color'].default_value
                    lightComponent.Color.x = color.x
                    lightComponent.Color.y = color.y
                    lightComponent.Color.z = color.z
                    lightComponent.Intensity = node.inputs['Strength'].default_value
        except:
            pass

        rootComponent.payload = rootLightComponent.SerializePartialToString()
        serializedData.obj = root
        serializedData.tex = []
        return serializedData

        
    elif obj.type == 'MESH':
        serializedData = namedtuple('serializedData',['obj','tex'])
        data = obj.data
        
        ##Hierarchy 1 (root = obj)
        #initialize root node with name and empty components
        root = Scene.SceneNodeContainer()
        root.Name = obj.name
        rootComponent1 = root.Components.add()
        rootComponent2 = root.Components.add()
        rootComponent3 = root.Components.add()
        print('--' + root.Name)
        
        #init
        rootTransformComponent = Scene.SceneComponentContainer()
        rootMeshComponent = Scene.SceneComponentContainer()

        #set current object as the active one
        bpy.context.scene.objects.active = obj
        '''
        #set to edit mode, in order to make all needed modifications
        bpy.ops.object.mode_set(mode='EDIT')
        #select mesh
        bpy.ops.mesh.select_all(action='SELECT')
        #triangulate mesh (doesn't matter if mesh is already triangulated)
        bpy.ops.mesh.quads_convert_to_tris()
        #set back to Object mode
        bpy.ops.object.mode_set(mode="OBJECT")
        #Apply Transforms
        #bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
        '''

        #TRANSFORM COMPONENT
        #Neutralize the blender-specific awkward parent inverse as it is not supported by FUSEE's scenegraph
        if obj.parent is None:
            obj_mtx_clean = obj.matrix_world.copy()
        else:
            obj_mtx_clean = obj.parent.matrix_world.inverted() * obj.matrix_world

        location, rotation, scale = obj_mtx_clean.decompose()

        #location
        transformComponent = rootTransformComponent.TransformComponent
        transformComponent.Translation.x = location.x
        transformComponent.Translation.y = location.z
        transformComponent.Translation.z = location.y

        #rotation
        rot_eul = rotation.to_euler()
        transformComponent.Rotation.x = rot_eul.x
        transformComponent.Rotation.y = rot_eul.z
        transformComponent.Rotation.z = rot_eul.y

        #scale 
        #TODO: Check if it's better to apply scale to geometry (maybe based on a user preference)
        transformComponent.Scale.x = scale.x
        transformComponent.Scale.y = scale.z
        transformComponent.Scale.z = scale.y

        '''
        #set current object as the active one
        bpy.ops.object.mode_set(mode='EDIT')
        bpy.ops.mesh.select_all(action='SELECT')
        #edgesplit (needed to generate one normal per vertex per triangle, easier)
        bpy.ops.mesh.edge_split()
        bpy.ops.object.mode_set(mode="OBJECT")
        '''

        #convert the mesh again to a bmesh, after splitting the edges
        bm = bmesh.new()
        # bm.from_mesh(bpy.context.scene.objects.active.data)
        bm.from_mesh(prepare_mesh(bpy.context.scene.objects.active))

        #count faces
        faces = []
        for face in bm.faces:
            faces.append(face.index)
        faceCount = len(faces)

        #get vertex-indices, -normals and -coordinates per face
        vertFCoList =[None]*(faceCount*3)
        vertIndexList = []
        vertFNormList = [None]*(faceCount*3)
        for face in bm.faces:
            for vert in face.verts:
                #continous list of vertex indices, corresponding to the faces
                vertIndex = vert.index
                vertIndexList.append(vertIndex)
                #list of vertex coordinates 
                vertFCoList[vertIndex] = vert.co
                #list of vertex normals
                vertFNormList[vertIndex] = vert.normal

        
        #copy normals, to have a list with the same structure as the original one
        vertNormList = copy.copy(vertFNormList)
        '''
        if smoothing:
            #Compare all "flat-vertex" coordinates with all "smooth-vertex" coordinates
            #If two coordinates match, replace the normal vector of the "flat-vertex"
            #with the normal vector of the "smooth-vertex"
            for fIndex in range(0,len(vertIndexList)):
                flatVertCo = vertFCoList[fIndex]
                for sIndex in range(0,len(vertSCoList)):
                    smoothVertCo = vertSCoList[sIndex]
                    if flatVertCo.angle(smoothVertCo)<0.01:
                        vertNormList[fIndex]=vertSNormList[sIndex]
        if smoothing:
            for fIndex in range(0,len(vertIndexList)):
                newNormal = vertFNormList[fIndex]
                flatVertCo = vertFCoList[fIndex]
                for other in range(fIndex,len(vertIndexList)):
                    otherVertCo = vertFCoList[other]
                    dist = flatVertCo-otherVertCo
                    angle = flatVertCo.angle(otherVertCo)
                    if dist.length<smoothingDist and angle<smoothingAngle:
                        newNormal += vertFNormList[other]

                newNormal.normalize()
                vertNormList[fIndex]=newNormal'''
                    

        #write data to protobuf structure              
        meshComponent = rootMeshComponent.MeshComponent
        for vert in range(0,len(vertIndexList)):
            vertexIndex = vertIndexList[vert]
            vertexCo = vertFCoList[vert]
            vertexNormal = vertNormList[vert]
            rootVert = meshComponent.Vertices.add()
            #VERTICES
            #the coordinate system of Blender is different to that one used by Fusee,
            #therefore the axis need to be changed:
            rootVert.x = vertexCo.x
            rootVert.y = vertexCo.z
            rootVert.z = vertexCo.y
            meshComponent.Triangles.append(vertexIndex)
            normal = meshComponent.Normals.add()
            #NORMALS
            #the coordinate system of Blender is different to that one used by Fusee,
            #therefore the axis need to be changed:
            normal.x = vertexNormal.x
            normal.y = vertexNormal.z
            normal.z = vertexNormal.y

        #get UVs and write the to protobuf structure
        uvActive = data.uv_layers.active
        if uvActive is not None:
            #BMESH
            uvList = [None]*(len(vertIndexList))
            uv_layer = bm.loops.layers.uv.active
            for face in bm.faces:
                for loop in face.loops:
                    index = loop.vert.index
                    uv = loop[uv_layer].uv
                    uvList[index] = uv     
            for uvs in uvList:
                uv = meshComponent.UVs.add()
                uv.x = uvs.x
                uv.y = uvs.y    


        #BoundingBox
        bbox = obj.bound_box
        bboxList = []
        for bboxVal in bbox:
            bboxList.append(list(bboxVal))

        #find min and max values of the bounding box and write them to the meshComponent
        bboxMin = min(bboxList)
        bboxMax = max(bboxList)
        #the coordinate system of Blender is different to that one used by Fusee,
        #therefore the axis need to be changed:
        meshComponent.BoundingBox.max.x = bboxMax[0]
        meshComponent.BoundingBox.max.y = bboxMax[2]
        meshComponent.BoundingBox.max.z = bboxMax[1]
        meshComponent.BoundingBox.min.x = bboxMin[0]
        meshComponent.BoundingBox.min.y = bboxMin[2]
        meshComponent.BoundingBox.min.z = bboxMin[1]

        #MATERIAL COMPONENT
        #check, if a material is set, otherwise use default material
        #also check if the material uses nodes -> cycles rendering, otherwise use default material
        textures = []
        try:
            if len(obj.material_slots)>0 and obj.material_slots[0].material.use_nodes:
                #use first material in the material_slots
                material =  obj.material_slots[0]
                #init components
                rootMaterialComponent = Scene.SceneComponentContainer()
                
                #cycle through nodes of the material node tree
                nodes = obj.material_slots[0].material.node_tree.nodes
                isDiffuse = False
                isSpecular = False
                isEmissive = False
                isMix = False
                    
                for node in nodes:
                    #find diffuse node
                    if node.type=='BSDF_DIFFUSE' and isDiffuse==False:
                        print('----found diffuse node')
                        isDiffuse = True
                        diffuse = rootMaterialComponent.MaterialComponent.Diffuse
                        diffuse.Mix = 1
                        #get material color
                        diffCol = node.inputs['Color'].default_value
                        diffuse.Color.x = diffCol[0]
                        diffuse.Color.y = diffCol[1]
                        diffuse.Color.z = diffCol[2]
                        #check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links)>0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath,basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                diffuse.Texture = basename
                                textures.append(fullpath)

                    #find glossy node        
                    # elif node.type =='BSDF_GLOSSY' and isWeb == False and isSpecular==False:
                    elif node.type =='BSDF_GLOSSY' and isSpecular==False:
                        print('----found glossy node')
                        isSpecular = True
                        specular = rootMaterialComponent.MaterialComponent.Specular
                        specular.Mix = 1             
                        #get material color
                        specCol = node.inputs['Color'].default_value
                        specular.Color.x = specCol[0]
                        specular.Color.y = specCol[1]
                        specular.Color.z = specCol[2]
                        #check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links)>0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath,basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                specular.Texture = basename
                                textures.append(fullpath)
                        
                        #get material roughness and set the specularity = 1-roughness
                        roughness = node.inputs['Roughness'].default_value
                        specular.SpecularChannelContainer.Shininess = 1-roughness

                    #find emissive node  
                    elif node.type =='EMISSION' and isEmissive==False:
                        print('----found emissive node')
                        isEmissive = True
                        emissive = rootMaterialComponent.MaterialComponent.Emissive
                        emissive.Mix = 1             
                        #get material color
                        emissiveCol = node.inputs['Color'].default_value
                        emissive.Color.x = emissiveCol[0]
                        emissive.Color.y = emissiveCol[1]
                        emissive.Color.z = emissiveCol[2]
                        #check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links)>0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath,basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                emissive.Texture = basename
                                textures.append(fullpath)

                    #find mix node, for specular intensity
                    elif node.type =='MIX_SHADER' and isWeb==False and isMix==False:
                        print('----found mix node')
                        isMix = True
                        #mix factor between glossy and diffuse
                        factor = node.inputs['Fac'].default_value
                        #determine, on which socket the glossy shader is connected to the Mix Shader
                        if node.inputs[1].links[0].from_node.type == 'BSDF_GLOSSY':
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = 1 -factor
                        elif node.inputs[1].links[0].from_node.type == 'BSDF_DIFFUSE':
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = factor
                        else:
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = 1
                                    
            else:
                #use default material
                rootMaterialComponent = SetDefaultMaterial(isWeb=isWeb)
        except Exception as inst:
            #use default material
            print('----3' + str(inst))
            rootMaterialComponent = SetDefaultMaterial(isWeb=isWeb)
            
            
                               
        #SCENE NODE CONTAINER
        #write rootComponents to rootNode
        rootComponent1.payload = rootTransformComponent.SerializePartialToString()
        rootComponent2.payload = rootMaterialComponent.SerializePartialToString()
        rootComponent3.payload = rootMeshComponent.SerializePartialToString()

        
        #if obj has got children, find them recursively,
        #serialize them and write them to root as children
        #save textures
        #return root node
        if len(obj.children)==0 or isOnlySelected==True:
            #write output
            serializedData.obj = root
            serializedData.tex = textures
            return serializedData
        else:
            #Hierarchy 2 (children of root)
            for children in obj.children:
                child = GetNode(objects=children, isWeb=isWeb,
                                isOnlySelected=False, smoothing=smoothing,
                                lamps=lamps,smoothingDist=smoothingDist,
                                smoothingAngle=smoothingAngle)
                rootChildren = root.Children.add()
                rootChildren.payload = child.obj.SerializePartialToString()
                textures = textures + child.tex
            serializedData.obj = root
            serializedData.tex = textures
            return serializedData         

def SetDefaultMaterial(isWeb):
    print('Default Material is used')
    #set default material
    rootMaterialComponent = Scene.SceneComponentContainer()
    diffuse = rootMaterialComponent.MaterialComponent.Diffuse

    diffuse.Mix = 1
    diffuse.Color.x = 0.6
    diffuse.Color.y = 0.6
    diffuse.Color.z = 0.6

    #Webviewer had problems with specular channel, therefore it was deactivated when exporting to web
    #if isWeb == False: (indent folloing if uncommenting this)
    specular = rootMaterialComponent.MaterialComponent.Specular
    specular.Mix = 1
    specular.Color.x = 0.6
    specular.Color.y = 0.6
    specular.Color.z = 0.6

    specular.SpecularChannelContainer.Intensity = 0.2
    specular.SpecularChannelContainer.Shininess = 0.2
    return rootMaterialComponent

def GetPaths(filepath):
    #relative filepath -> absolute filepath (when file is in the same folder)
    basename = os.path.basename(filepath)
    if os.path.dirname(filepath) == '//':
        filepath = os.path.join(os.path.dirname(bpy.data.filepath),basename)
    fullpath = filepath
    return fullpath,basename

def GetParents(obj):
    #recursively search for the highest parent object
    if obj.parent == None:
        return obj
    elif obj.parent != None:
        GetParents(obj.parent)

def prepare_mesh(obj):
    # This applies all the modifiers (without altering the scene)
    mesh = obj.to_mesh(bpy.context.scene, apply_modifiers=True, settings='RENDER', calc_tessface = True, calc_undeformed = False)

    # Triangulate for web export
    bm = bmesh.new()
    bm.from_mesh(mesh)
    bmesh.ops.triangulate(bm, faces=bm.faces)
    bm.to_mesh(mesh)
    bm.free()
    del bm

    mesh.calc_normals()
    mesh.calc_tessface()

    return mesh
