# Collect data from blender to convert to proto data

# import
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
dir_path = os.path.join(dir_path, 'proto\\')
sys.path.append(dir_path)
import Scene_pb2 as Scene

# Not possible because of pythons broken reference system
# var class = new Class()
# list.add(class)
# take object 2 and change it; result = you change everything within this list and therefore all classes because this is no new instance
class VertexWeight:
    JointIndex = 0
    Weight = 0.0

    def __init__(self, jointIdx, weight):
        self.JointIndex = jointIdx
        self.Weight = weight

# returns a serialized object, that can be saved to a file
def SerializeData(objects, isWeb, isOnlySelected, smoothing, lamps, smoothingDist, smoothingAngle):
    # init sceneContainer
    sceneContainer = Scene.SceneContainer()
    # write sceneHeader
    sceneHeader = sceneContainer.Header
    sceneHeader.Version = 0
    sceneHeader.Generator = 'Blender FUSEE Exporter AddOn'
    sceneHeader.CreatedBy = getpass.getuser()
    lt = time.localtime()
    sceneHeader.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)

    print('SERIALIZING--------')

    # differentiate between the following 2 situations:
    # serialize only selected objects
    textures = []
    if isOnlySelected == True:
        for obj in objects:
            root = GetNode(objects=obj, isWeb=isWeb,
                           isOnlySelected=isOnlySelected, smoothing=smoothing,
                           lamps=lamps, smoothingDist=smoothingDist,
                           smoothingAngle=smoothingAngle)
            sceneChildren = sceneContainer.Children.add()
            sceneChildren.payload = root.obj.SerializePartialToString()
            textures = textures + root.tex

    # serialize all top-level objects and their hierarchy
    else:
        for obj in objects:
            root = GetNode(objects=obj, isWeb=isWeb,
                           isOnlySelected=isOnlySelected, smoothing=smoothing,
                           lamps=lamps, smoothingDist=smoothingDist,
                           smoothingAngle=smoothingAngle)
            sceneChildren = sceneContainer.Children.add()
            sceneChildren.payload = root.obj.SerializePartialToString()
            textures = textures + root.tex

    # serialize SceneContainer
    sc = sceneContainer.SerializePartialToString()

    # create a namedtuple object for easier data access
    serializedData = namedtuple('serializedData', ['obj', 'tex'])
    serializedData.obj = sc
    serializedData.tex = textures
    return serializedData


# recursively get each node and its components and serialize them
def add_vertex(vi, face, i, mesh, uv_layer):
    vert = mesh.Vertices.add()
    vert.x = face.loops[vi].vert.co.x
    vert.y = face.loops[vi].vert.co.z
    vert.z = face.loops[vi].vert.co.y
    norm = mesh.Normals.add()
    # Doesn't work, see (https://developer.blender.org/T45151) meshNorm = face.loops[vi].calc_normal()
    if face.smooth:
        meshNorm = face.loops[vi].vert.normal
    else:
        meshNorm = face.normal
    norm.x = meshNorm.x
    norm.y = meshNorm.z
    norm.z = meshNorm.y
    uv = mesh.UVs.add()
    if uv_layer is not None:
        uv.x = face.loops[vi][uv_layer].uv.x
        uv.y = face.loops[vi][uv_layer].uv.y
    else:
        uv.x = 0
        uv.y = 0
    tri = mesh.Triangles.append(i)



def GetNode(objects, isWeb, isOnlySelected, smoothing, lamps, smoothingDist, smoothingAngle):
    obj = objects
    if obj.type == 'LAMP' and lamps:
        serializedData = namedtuple('serializedData', ['obj', 'tex'])
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
        serializedData = namedtuple('serializedData', ['obj', 'tex'])
        data = obj.data

        root = Scene.SceneNodeContainer()    

        root.Name = obj.name
        
        rootComponent1 = root.Components.add()
        rootComponent2 = root.Components.add()
        rootComponent3 = root.Components.add()            

        print('--' + root.Name)

        # init
        rootTransformComponent = Scene.SceneComponentContainer()
        rootmesh = Scene.SceneComponentContainer()

        # set current object as the active one
        bpy.context.view_layer.objects.active = obj
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

        # TRANSFORM COMPONENT
        # Neutralize the blender-specific awkward parent inverse as it is not supported by FUSEE's scenegraph
        if obj.parent is None:
            obj_mtx_clean = obj.matrix_world.copy()
        else:
            obj_mtx_clean = obj.parent.matrix_world.inverted() @ obj.matrix_world

        location, rotation, scale = obj_mtx_clean.decompose()

        # location
        transformComponent = rootTransformComponent.TransformComponent
        transformComponent.Translation.x = location.x
        transformComponent.Translation.y = location.z
        transformComponent.Translation.z = location.y

        # rotation
        rot_eul = rotation.to_euler('YXZ')
        transformComponent.Rotation.x = -rot_eul.x
        transformComponent.Rotation.y = -rot_eul.z
        transformComponent.Rotation.z = -rot_eul.y

        # scale
        # TODO: Check if it's better to apply scale to geometry (maybe based on a user preference)
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

        # convert the mesh again to a bmesh, after splitting the edges
        bm = bmesh.new()
        # bm.from_mesh(bpy.context.scene.objects.active.data)      
        damesh = prepare_mesh(bpy.context.active_object)
        bm.from_mesh(damesh)

        # <CM's Checks>
        uvActive = data.uv_layers.active
        uv_layer = None
        if uvActive is not None:
            uv_layer = bm.loops.layers.uv.active

        mesh = rootmesh.Mesh
        i = 0
        for face in bm.faces:
            '''
            print('--Face:' + str(face.index))
            if uvActive is not None:
                print('  --V0: co:' + str(face.loops[0].vert.co) + ' normal:' +  str(face.loops[0].calc_normal()) + ' uv:' + str(face.loops[0][uv_layer].uv))
                print('  --V1: co:' + str(face.loops[1].vert.co) + ' normal:' +  str(face.loops[1].calc_normal()) + ' uv:' + str(face.loops[1][uv_layer].uv))
                print('  --V2: co:' + str(face.loops[2].vert.co) + ' normal:' +  str(face.loops[2].calc_normal()) + ' uv:' + str(face.loops[2][uv_layer].uv))
            else:
                print('  --V0: co:' + str(face.loops[0].vert.co) + ' normal:' +  str(face.loops[0].calc_normal()))
                print('  --V1: co:' + str(face.loops[1].vert.co) + ' normal:' +  str(face.loops[1].calc_normal()))
                print('  --V2: co:' + str(face.loops[2].vert.co) + ' normal:' +  str(face.loops[2].calc_normal()))
            '''
            # TODO: assert that len(face.loops) == 3!
            add_vertex(0, face, i, mesh, uv_layer)
            i += 1
            add_vertex(1, face, i, mesh, uv_layer)
            i += 1
            add_vertex(2, face, i, mesh, uv_layer)
            i += 1

        # </CM's Checks

        # <Export for completely smooth shaded models.>
        # Does NOT export redundant vertices. Not in use because a mesh can have both, smooth and flat shaded faces.
        # Therefore the optimal solution would be mixture of both, the above and the following method to support the export of meshes with the minimal number of vertices.

        '''
        class AddVert:
            pass
        
        mesh = rootmesh.Mesh
        bm.from_mesh(damesh)
        uv_lay = bm.loops.layers.uv.active       

        vertList = list()

        for i, vert in enumerate(bm.verts):
            addVert = AddVert()
            addVert.thisVert = vert
            addVert.wasAdded = False
            vertList.append(addVert)            

        vertices = [None]*len(bm.verts)
        normals = [None]*len(bm.verts)
        uvs = [None]*len(bm.verts)

        for face in bm.faces:
                
            for loop in face.loops:
        
                vertIndex = loop.vert.index                       
        
                mesh.Triangles.append(vertIndex)               

                value = vertList[vertIndex]
        
                if uv_lay is not None:
                    uv = loop[uv_lay].uv
            
                if(value.wasAdded == False):

                    vertices[vertIndex] = value.thisVert.co 

                    normals[vertIndex] = loop.vert.normal                                    
                
                    if uv_lay is not None:
                        uvs[vertIndex] = uv           
            
                    value.wasAdded = True
                    vertList[vertIndex] = value 

        # fill output mesh
        for i, vert in enumerate(vertices):            
            outVert = mesh.Vertices.add()                    
            outVert.x = vert.x
            outVert.y = vert.z
            outVert.z = vert.y
    
            outNormal = mesh.Normals.add()            
            outNormal.x = normals[i].x
            outNormal.y = normals[i].z
            outNormal.z = normals[i].y           
    
            if len(uvs) != 0:
                outUv = mesh.UVs.add()
                outUv.x = uvs[i].x
                outUv.y = uvs[i].y
 
        '''
        # </Export for completely smooth shaded models>

        '''
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

        #write data to protobuf structure              
        mesh = rootmesh.Mesh
        for vert in range(0,len(vertIndexList)):
            vertexIndex = vertIndexList[vert]
            vertexCo = vertFCoList[vert]
            vertexNormal = vertNormList[vert]
            rootVert = mesh.Vertices.add()
            #VERTICES
            #the coordinate system of Blender is different to that one used by Fusee,
            #therefore the axis need to be changed:
            rootVert.x = vertexCo.x
            rootVert.y = vertexCo.z
            rootVert.z = vertexCo.y
            mesh.Triangles.append(vertexIndex)
            normal = mesh.Normals.add()
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
                uv = mesh.UVs.add()
                uv.x = uvs.x
                uv.y = uvs.y    
        '''

        # BoundingBox
        bbox = obj.bound_box
        bboxList = []
        for bboxVal in bbox:
            bboxList.append(list(bboxVal))

        # find min and max values of the bounding box and write them to the mesh
        bboxMin = min(bboxList)
        bboxMax = max(bboxList)
        # the coordinate system of Blender is different to that one used by Fusee,
        # therefore the axis need to be changed:
        mesh.BoundingBox.max.x = bboxMax[0]
        mesh.BoundingBox.max.y = bboxMax[2]
        mesh.BoundingBox.max.z = bboxMax[1]
        mesh.BoundingBox.min.x = bboxMin[0]
        mesh.BoundingBox.min.y = bboxMin[2]
        mesh.BoundingBox.min.z = bboxMin[1]

        # MATERIAL COMPONENT
        # check, if a material is set, otherwise use default material
        # also check if the material uses nodes -> cycles rendering, otherwise use default material
        textures = []
        rootMaterialComponent = None
        rootMaterialComponent, textures = GetMaterial(obj, isWeb)       

        rootComponent1.payload = rootTransformComponent.SerializePartialToString()
        rootComponent2.payload = rootMaterialComponent.SerializePartialToString()
        rootComponent3.payload = rootmesh.SerializePartialToString()


        # if obj has got children, find them recursively,
        # serialize them and write them to root as children
        # save textures
        # return root node
        if len(obj.children) == 0 or isOnlySelected == True:
            # write output
            serializedData.obj = root
            serializedData.tex = textures
            return serializedData
        else:
            # Hierarchy 2 (children of root)
            for children in obj.children:
                child = GetNode(objects=children, isWeb=isWeb,
                                isOnlySelected=False, smoothing=smoothing,
                                lamps=lamps, smoothingDist=smoothingDist,
                                smoothingAngle=smoothingAngle)
                rootChildren = root.Children.add()
                rootChildren.payload = child.obj.SerializePartialToString()
                textures = textures + child.tex
            serializedData.obj = root
            serializedData.tex = textures
            return serializedData

    elif obj.type == 'ARMATURE':
         return GetArmaturePayload(objects, isWeb, isOnlySelected, smoothing, lamps, smoothingDist, smoothingAngle)

# BONES structure not yet valid. One needs to be the parent of the other!
def create_bone_payload(boneNode, obj, bone):
    print('found bone')
     
    # Create BoneNodeContainer structure
    # 2 components, Transform & Bone
    boneNode = Scene.SceneNodeContainer()
    
    boneNode.Name = obj.name

    boneNodeComponent1 = boneNode.Components.add()
    boneNodeComponent2 = boneNode.Components.add()

    # TRANSFORM COMPONENT
    rootTransformComp = Scene.SceneComponentContainer()

    # Neutralize the blender-specific awkward parent inverse as it is not supported by FUSEE's scenegraph
    if obj.parent is None:
        obj_mtx_clean = obj.matrix_world.copy()
    else:
        obj_mtx_clean = obj.parent.matrix_world.inverted() @ obj.matrix_world

    obj_mtx = obj_mtx_clean.inverted() @ bone.matrix

    location, rotation, scale = obj_mtx.decompose()

    # location
    transformComponent = rootTransformComp.TransformComponent
    transformComponent.Translation.x = location.x
    transformComponent.Translation.y = location.z
    transformComponent.Translation.z = location.y

    # rotation
    rot_eul = rotation.to_euler('YXZ')
    transformComponent.Rotation.x = -rot_eul.x
    transformComponent.Rotation.y = -rot_eul.z
    transformComponent.Rotation.z = -rot_eul.y

    # scale
    # TODO: Check if it's better to apply scale to geometry (maybe based on a user preference)
    transformComponent.Scale.x = scale.x
    transformComponent.Scale.y = scale.z
    transformComponent.Scale.z = scale.y
    
    # BONE COMPONENT
    rootBoneComp = Scene.SceneComponentContainer()
    boneComponent = rootBoneComp.BoneComponent
    boneComponent.Name = bone.name

    boneNodeComponent1.payload = rootTransformComp.SerializePartialToString()
    boneNodeComponent2.payload = rootBoneComp.SerializePartialToString()

    return boneNode.SerializePartialToString()

def PrintVertList(bla):
      for i in range(0, len(bla)):
            for j in range(0, len(bla[i])):
                print("VertexIdx: " + str(i) + " Joint idx: " + str(bla[i][j].JointIndex) + " and a weight of: " + str(bla[i][j].Weight))

def GetArmaturePayload(objects, isWeb, isOnlySelected, smoothing, lamps, smoothingDist, smoothingAngle):
        serializedData = namedtuple('serializedData', ['obj', 'tex'])

        print('-- found BoneAnimation')
        obj = objects
        data = obj.data

        root = Scene.SceneNodeContainer()

        # add bones
        # add one children for each bone
        if len(obj.pose.bones) > 4:
            print('-- more than 4 bones per Mesh. This is not yet supported!')
            return

        for bone in obj.pose.bones:
            rootChildren = root.Children.add()
            rootChildren.payload =  create_bone_payload(rootChildren, obj, bone)


        ##Hierarchy 1 (root = obj)
        # initialize root node with name and empty components            
        rootChildrenMeshPayloadContainer = root.Children.add()
        rootChildrenMesh = Scene.SceneNodeContainer()

        rootComponent1 = rootChildrenMesh.Components.add() # transform
        rootComponent2 = rootChildrenMesh.Components.add() # weight
        rootComponent3 = rootChildrenMesh.Components.add() # material
        rootComponent4 = rootChildrenMesh.Components.add() # mesh

        #root.Name = obj.name

        print('--' + root.Name)

        # init
        rootTransformComponent = Scene.SceneComponentContainer()
        rootmesh = Scene.SceneComponentContainer()

        # set current object as the active one
        bpy.context.view_layer.objects.active = obj

        # TRANSFORM COMPONENT
        # Neutralize the blender-specific awkward parent inverse as it is not supported by FUSEE's scenegraph
        if obj.parent is None:
            obj_mtx_clean = obj.matrix_world.copy()
        else:
            obj_mtx_clean = obj.parent.matrix_world.inverted() @ obj.matrix_world

        location, rotation, scale = obj_mtx_clean.decompose()

        # location
        transformComponent = rootTransformComponent.TransformComponent
        transformComponent.Translation.x = location.x
        transformComponent.Translation.y = location.z
        transformComponent.Translation.z = location.y

        # rotation
        rot_eul = rotation.to_euler('YXZ')
        transformComponent.Rotation.x = -rot_eul.x
        transformComponent.Rotation.y = -rot_eul.z
        transformComponent.Rotation.z = -rot_eul.y

        # scale
        # TODO: Check if it's better to apply scale to geometry (maybe based on a user preference)
        transformComponent.Scale.x = scale.x
        transformComponent.Scale.y = scale.z
        transformComponent.Scale.z = scale.y

        # ----- Collect bones and weights

        # WEIGHT COMPONENT
        rootWeightComponent = Scene.SceneComponentContainer()
        weightComponent = rootWeightComponent.WeightComponent
        # vWeight = Scene.VertexWeight

       
        # get binding matrices (init of bones)
        bindingMatricesList = []
        

        activeObj = bpy.context.active_object.children[0]

        for n in activeObj.vertex_groups:
            print("vertex_grpup name :" + str(n.name))

        obj_group_names = [g.name for g in activeObj.vertex_groups]
        obj_verts = activeObj.data.vertices

        # vertexWeightList = [[[0]] * len(obj.pose.bones) ] * len(obj_verts) # consits of: VertexWeight

        vertexWeightList = []       

        for i in range(0, len(obj_verts)):
            vertexWeightList.append([])          
            # fill the list with empty elements with current bone idx
            for j in range(0, len(obj.pose.bones)):                           
                emptyVWeight = VertexWeight(j, 0)
                #print(emptyVWeight.Weight)
                vertexWeightList[i].append(emptyVWeight)       

        # PrintVertList(vertexWeightList)

        print("vertexWeightList length:" + str(len(vertexWeightList)))       

        # TODO: change rotation order???! --> to euler(YXZ) --> matrix
        
        for i, bone in enumerate(obj.pose.bones):         

            if obj.parent is None:
                obj_mtx_clean = obj.matrix_world.copy()
            else:
                obj_mtx_clean = obj.parent.matrix_world.inverted() @ obj.matrix_world

            # change rotation order to YXZ
            boneRot_eul = bone.matrix.to_euler('YXZ')
            print(str(boneRot_eul))

            boneRot_mat_x = Matrix.Rotation(-boneRot_eul.x, 4, 'X')
            boneRot_mat_y = Matrix.Rotation(-boneRot_eul.z, 4, 'Y')
            boneRot_mat_z = Matrix.Rotation(-boneRot_eul.y, 4, 'Z')
            boneRot_mat = boneRot_mat_y @ boneRot_mat_x @ boneRot_mat_z

            print(str(boneRot_mat))

            obj_mtx = obj_mtx_clean.inverted() @ boneRot_mat            

            # convert obj_mxt to float4x4 
            tmpMat = ConvertMatrixToFloat4x4(obj_mtx)

            # add matrix to binding matrices:
            bindingMatricesList.append(tmpMat)

            if bone.name not in obj_group_names:
                continue
           
            gidx = activeObj.vertex_groups[bone.name].index
            
            bone_verts = [v for v in obj_verts if gidx in [g.group for g in v.groups]]

            for v in bone_verts:     
                for grp in v.groups:
                    if grp.group == gidx:                        
                        w = grp.weight
                        vertexWeightList[v.index][i].JointIndex = i
                        vertexWeightList[v.index][i].Weight = w
                        # print("Saving Vertex at position " + str(v.index) + " with joint idx: " + str(i) + " and a weight of: " + str(w))
                        # print("Saved: " + str(vertexWeightList[v.index][i].Weight))
                        # print("Saved: " + str(vertexWeightList[v.index][i].JointIndex))
                        
                        # print('Vertex',v.index,'has a weight of',w,'for bone',bone.name)
        print(" ")
        print(" --------------- PRINT WEIGHT FOR EACH VERTEX ----------------")
        PrintVertList(vertexWeightList)

        # append all found bindingMatrices
        weightComponent.BindingMatrices.extend(bindingMatricesList)

        #----------------------------------


        # convert the mesh again to a bmesh, after splitting the edges
        bm = bmesh.new()
        # bm.from_mesh(bpy.context.scene.objects.active.data)
        damesh = prepare_mesh(bpy.context.active_object.children[0])      
        bm.from_mesh(damesh)
        
        uvActive = obj.children[0].data.uv_layers.active
        uv_layer = None
        if uvActive is not None:
            uv_layer = bm.loops.layers.uv.active

        outputVertexWeightList = [[VertexWeight] * len(obj.pose.bones) ] * (len(bm.faces) * 3) # consits of: VertexWeight

        mesh = rootmesh.Mesh
        i = 0
        for face in bm.faces: 
            # TODO: assert that len(face.loops) == 3!
            add_vertex(0, face, i, mesh, uv_layer)
            vertIdx = face.loops[0].vert.index
            outVertWeight = vertexWeightList[vertIdx]
            outputVertexWeightList[i] = outVertWeight            
            i += 1

            add_vertex(1, face, i, mesh, uv_layer)            
            vertIdx = face.loops[1].vert.index            
            outVertWeight = vertexWeightList[vertIdx]
            outputVertexWeightList[i] = outVertWeight            
            i += 1

            add_vertex(2, face, i, mesh, uv_layer)
            vertIdx = face.loops[2].vert.index
            outVertWeight = vertexWeightList[vertIdx] 
            outputVertexWeightList[i] = outVertWeight            
            i += 1

        # PrintVertList(outputVertexWeightList)


        # iterate output, convert to protobuf
        for vWeightList in outputVertexWeightList:
            # create new vertexWeight, one vertWeight consits of 2 or more bones with their weights
            VertexWeightList = weightComponent.WeightMap.add() # per vertex
            perVertexList = []

            for vWeight in vWeightList:
                v = VertexWeightList.VertexWeights.add()
                v.JointIndex = vWeight.JointIndex
                v.Weight = vWeight.Weight
                perVertexList.append(v)

            VertexWeightList = perVertexList

        # BoundingBox
        bbox = obj.bound_box
        bboxList = []
        for bboxVal in bbox:
            bboxList.append(list(bboxVal))

        # find min and max values of the bounding box and write them to the mesh
        bboxMin = min(bboxList)
        bboxMax = max(bboxList)
        # the coordinate system of Blender is different to that one used by Fusee,
        # therefore the axes need to be changed:
        mesh.BoundingBox.max.x = bboxMax[0]
        mesh.BoundingBox.max.y = bboxMax[2]
        mesh.BoundingBox.max.z = bboxMax[1]
        mesh.BoundingBox.min.x = bboxMin[0]
        mesh.BoundingBox.min.y = bboxMin[2]
        mesh.BoundingBox.min.z = bboxMin[1]

        # MATERIAL COMPONENT
        # check, if a material is set, otherwise use default material
        # also check if the material uses nodes -> cycles rendering, otherwise use default material
        textures = []
        rootMaterialComponent, textures = GetMaterial(obj.children[0], isWeb)     

        # SCENE NODE CONTAINER
        # write rootComponents to rootNode        
        rootComponent1.payload = rootTransformComponent.SerializePartialToString()
        rootComponent2.payload = rootWeightComponent.SerializePartialToString()
        rootComponent3.payload = rootMaterialComponent.SerializePartialToString()
        rootComponent4.payload = rootmesh.SerializePartialToString()

        rootChildrenMeshPayloadContainer.payload = rootChildrenMesh.SerializePartialToString()

        # disabled for simplicity
        '''# if obj has got children, find them recursively,
        # serialize them and write them to root as children
        # save textures
        # return root node
        if len(obj.children) == 0 or isOnlySelected == True:
            # write output
            serializedData.obj = root
            serializedData.tex = textures
            return serializedData
        else:
            # Hierarchy 2 (children of root)
            for children in obj.children:
                child = GetNode(objects=children, isWeb=isWeb,
                                isOnlySelected=False, smoothing=smoothing,
                                lamps=lamps, smoothingDist=smoothingDist,
                                smoothingAngle=smoothingAngle)
                rootChildren = root.Children.add()
                rootChildren.payload = child.obj.SerializePartialToString()
                textures = textures + child.tex'''
        serializedData.obj = root
        serializedData.tex = []
        return serializedData

def ConvertMatrixToFloat4x4(mat):

    tmpMat = Scene.float4x4()
    row0 = mat.row[0]
    row1 = mat.row[1]
    row2 = mat.row[2]
    row3 = mat.row[3]

    tmpMat.Row0.x = row0.x
    tmpMat.Row0.y = row0.y
    tmpMat.Row0.z = row0.z
    tmpMat.Row0.w = row0.w

    tmpMat.Row1.x = row1.x
    tmpMat.Row1.y = row1.y
    tmpMat.Row1.z = row1.z
    tmpMat.Row1.w = row1.w

    tmpMat.Row2.x = row2.x
    tmpMat.Row2.y = row2.y
    tmpMat.Row2.z = row2.z
    tmpMat.Row2.w = row2.w

    tmpMat.Row3.x = row3.x
    tmpMat.Row3.y = row3.y
    tmpMat.Row3.z = row3.z
    tmpMat.Row3.w = row3.w

    return tmpMat

def GetMaterial(obj, isWeb):
        # MATERIAL COMPONENT
        # check, if a material is set, otherwise use default material
        # also check if the material uses nodes -> cycles rendering, otherwise use default material
        textures = []
        try:
            if len(obj.material_slots) > 0 and obj.material_slots[0].material.use_nodes:
                # use first material in the material_slots
                material = obj.material_slots[0]
                # init components
                rootMaterialComponent = Scene.SceneComponentContainer()

                # cycle through nodes of the material node tree
                nodes = obj.material_slots[0].material.node_tree.nodes
                isDiffuse = False
                isSpecular = False
                isEmissive = False
                isMix = False

                for node in nodes:                    
                    # find diffuse node
                    if node.type == 'BSDF_DIFFUSE' and isDiffuse == False:
                        print('----found diffuse node')
                        isDiffuse = True
                        diffuse = rootMaterialComponent.MaterialComponent.Diffuse
                        diffuse.Mix = 1
                        # get material color
                        diffCol = node.inputs['Color'].default_value
                        diffuse.Color.x = diffCol[0]
                        diffuse.Color.y = diffCol[1]
                        diffuse.Color.z = diffCol[2]
                        diffuse.Color.w = diffCol[3]
                        # check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links) > 0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                diffuse.Texture = basename
                                textures.append(fullpath)

                    # find glossy node
                    # elif node.type =='BSDF_GLOSSY' and isWeb == False and isSpecular==False:
                    elif node.type == 'BSDF_GLOSSY' and isSpecular == False:
                        print('----found glossy node')
                        isSpecular = True
                        specular = rootMaterialComponent.MaterialComponent.Specular
                        specular.Mix = 1
                        # get material color
                        specCol = node.inputs['Color'].default_value
                        specular.Color.x = specCol[0]
                        specular.Color.y = specCol[1]
                        specular.Color.z = specCol[2]
                        specular.Color.w = specCol[3]
                        # check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links) > 0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                specular.Texture = basename
                                textures.append(fullpath)

                        # get material roughness and set the specularity = 1-roughness
                        roughness = node.inputs['Roughness'].default_value
                        specular.SpecularChannelContainer.Shininess = (1 - roughness) * 200 # multipy with factor 200 for tight specular light
                        # specularIntensity = 1
                        specular.SpecularChannelContainer.Intensity = 1.0 - (roughness + 0.2) # reduce intensity quite a bit

                    # find emissive node
                    elif node.type == 'EMISSION' and isEmissive == False:
                        print('----found emissive node')
                        isEmissive = True
                        emissive = rootMaterialComponent.MaterialComponent.Emissive
                        emissive.Mix = 1
                        # get material color
                        emissiveCol = node.inputs['Color'].default_value
                        emissive.Color.x = emissiveCol[0]
                        emissive.Color.y = emissiveCol[1]
                        emissive.Color.z = emissiveCol[2]
                        emissive.Color.w = emissiveCol[3]
                        # check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Color'].links
                        if len(links) > 0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                emissive.Texture = basename
                                textures.append(fullpath)

                    # find mix node, for specular intensity
                    elif node.type == 'MIX_SHADER' and isWeb == False and isMix == False:
                        print('----found mix node')
                        isMix = True
                        # mix factor between glossy and diffuse
                        factor = node.inputs['Fac'].default_value
                        # determine, on which socket the glossy shader is connected to the Mix Shader
                        if node.inputs[1].links[0].from_node.type == 'BSDF_GLOSSY':
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = 1 - factor
                        elif node.inputs[1].links[0].from_node.type == 'BSDF_DIFFUSE':
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = factor
                        else:
                            rootMaterialComponent.MaterialComponent.Specular.SpecularChannelContainer.Intensity = 1

                    elif node.type == 'NORMAL_MAP':
                        print('----found normal map')
                        normalMap = rootMaterialComponent.MaterialComponent.Bump
                        # get bump intensity                       
                        normalMap.Intensity =  node.inputs['Strength'].default_value                         
                        # get bump texture
                        links = node.inputs['Color'].links
                        if len(links) > 0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath, basename = GetPaths(node.inputs['Color'].links[0].from_node.image.filepath)
                                normalMap.Texture = basename
                                textures.append(fullpath)

                    elif node.type == 'BSDF_PRINCIPLED':
                        print('----found bsdf principled node')
                        pbrMaterial = rootMaterialComponent.MaterialComponent.MaterialPBRComponent
                        isSpecular = True
                        isDiffuse = True
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
                        
                        diffuse = rootMaterialComponent.MaterialComponent.Diffuse
                        diffuse.Mix = 1
                        # get material color
                        diffCol = baseColor
                        diffuse.Color.x = diffCol[0]
                        diffuse.Color.y = diffCol[1]
                        diffuse.Color.z = diffCol[2]
                        diffuse.Color.w = diffCol[3]
                        # check, if material has got textures. If so, get texture filepath
                        links = node.inputs['Base Color'].links
                        if len(links) > 0:
                            if links[0].from_node.type == 'TEX_IMAGE':
                                fullpath, basename = GetPaths(
                                    node.inputs['Base Color'].links[0].from_node.image.filepath)
                                diffuse.Texture = basename
                                textures.append(fullpath)

                        isSpecular = True
                        spec = rootMaterialComponent.MaterialComponent.Specular
                        spec.Mix = 1
                        # get material color
                        specCol = subsurfaceColor
                        spec.Color.x = specCol[0]
                        spec.Color.y = specCol[1]
                        spec.Color.z = specCol[2]
                        spec.Color.w = specCol[3]
                        ## check, if material has got textures. If so, get texture filepath
                        #links = subsurfaceColor.links
                        #if len(links) > 0:
                        #    if links[0].from_node.type == 'TEX_IMAGE':
                        #        fullpath, basename = GetPaths(subsurfaceColor.links[0].from_node.image.filepath)
                        #        specular.Texture = basename
                        #        textures.append(fullpath)
                        # get material roughness and set the specularity = 1-roughness
                        spec.SpecularChannelContainer.Shininess = (1 - roughness) * 200 # multipy with factor 100 for tight specular light
                        # specularIntensity = 1
                        spec.SpecularChannelContainer.Intensity = 1.0 - (roughness + 0.2) # reduce intensity quite a bit

                        pbrMaterial.RoughnessValue = roughness
                        pbrMaterial.FresnelReflectance = specular
                        pbrMaterial.DiffuseFraction = metallic

                return rootMaterialComponent, textures
            else:
                # use default material
                rootMaterialComponent = SetDefaultMaterial(isWeb=isWeb)
                return rootMaterialComponent, textures
        except Exception as inst:
            # use default material
            print('----3' + str(inst))
            rootMaterialComponent = SetDefaultMaterial(isWeb=isWeb)
            return rootMaterialComponent, textures

def SetDefaultMaterial(isWeb):
    print('Default Material is used')
    # set default material
    rootMaterialComponent = Scene.SceneComponentContainer()
    diffuse = rootMaterialComponent.MaterialComponent.Diffuse

    diffuse.Mix = 1
    diffuse.Color.x = 0.6
    diffuse.Color.y = 0.6
    diffuse.Color.z = 0.6
    diffuse.Color.w = 1.0

    # Webviewer had problems with specular channel, therefore it was deactivated when exporting to web
    # if isWeb == False: (indent folloing if uncommenting this)
    specular = rootMaterialComponent.MaterialComponent.Specular
    specular.Mix = 1
    specular.Color.x = 0.6
    specular.Color.y = 0.6
    specular.Color.z = 0.6
    specular.Color.w = 1.0

    specular.SpecularChannelContainer.Intensity = 0.2
    specular.SpecularChannelContainer.Shininess = 0.2
    return rootMaterialComponent


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
    # recursively search for the highest parent object
    if obj.parent == None:
        return obj
    elif obj.parent != None:
        GetParents(obj.parent)


def prepare_mesh(obj):
    # This applies all the modifiers (without altering the scene)
    # Taken from https://docs.blender.org/api/blender2.8/bpy.types.Depsgraph.html, "Evaluated ID example"
    ev_depsgraph = bpy.context.evaluated_depsgraph_get()
    object_eval = obj.evaluated_get(ev_depsgraph)
    mesh = object_eval.data

    # Triangulate for web export
    bm = bmesh.new()
    bm.from_mesh(mesh)
    bmesh.ops.triangulate(bm, faces=bm.faces)
    bm.to_mesh(mesh)
    bm.free()
    del bm

    mesh.calc_normals()
    mesh.calc_loop_triangles()

    return mesh


def prepare_mesh_elaborate(obj, export_props):
    # Create a (deep (linked=False)) copy of obj.
    # Makes use of the new 2.8 override context (taken from https://blender.stackexchange.com/questions/135597/how-to-duplicate-an-object-in-2-8-via-the-python-api-without-using-bpy-ops-obje )
    bpy.ops.object.duplicate(
       {"object" : obj,
       "selected_objects" : [obj]},
       linked=False)
    # In case obj has an armature parent, bpy.context.active as well as bpy.context.object point to obj's parent AND NOT AS DOCUMENTED to the newly duplicated object.
    # As debugging shows, the first (and only) object contained in bpy.context.selected_objects points to the duplication.
    obj_copy = bpy.context.selected_objects[0]

    # Select the copy (and nothing else)
    bpy.ops.object.mode_set(mode = 'OBJECT')
    bpy.ops.object.select_all(action='DESELECT')
    bpy.context.view_layer.objects.active = obj_copy
    obj_copy.select_set(True)

    # Apply Scale
    if export_props["doApplyScale"]:
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)

    # Flip normals on the copied object's copied mesh (Important: This happens BEFORE any modifiers are applied)
    if export_props["doRecalcOutside"]:
        bpy.ops.object.mode_set(mode = 'EDIT')
        bpy.ops.mesh.select_all(action='SELECT')
        bpy.ops.mesh.normals_make_consistent(inside=False)
        bpy.ops.object.mode_set(mode = 'OBJECT')

    # Apply all the modifiers (without altering the scene)
    # Taken from https://docs.blender.org/api/blender2.8/bpy.types.Depsgraph.html, "Evaluated ID example"
    if export_props["doApplyModifiers"]:
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
    mesh_triangulated = bpy.data.meshes.new("MeshTriangulated")
    bm.to_mesh(mesh_triangulated)
    bm.free()
    del bm
        
    # Just for Debugging: link the triangulated mesh:
    # obj_triangulated = bpy.data.objects.new("ObjectTriangulated", mesh_triangulated)
    # bpy.context.collection.objects.link(obj_triangulated)

    # Delete duplicated object
    bpy.ops.object.mode_set(mode = 'OBJECT')
    bpy.ops.object.select_all(action='DESELECT')
    bpy.context.view_layer.objects.active = obj_copy
    obj_copy.select_set(True)
    bpy.ops.object.delete() 

    return mesh_triangulated    