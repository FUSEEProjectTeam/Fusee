import sys
import os
import time
import getpass

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

class FusSceneWriter:
    """Convenience Class to assemble FUSEE scene files from instances of FusNode and FusComponent (and derivatives)."""
    def __init__(self):
        super().__init__()
        self.__MaxVerts = (65000 // 3) * 3
        self.__fusFile = FusSer.FusFile()
        self.__fusFile.Header.FileVersion = 1
        self.__fusFile.Header.Generator = 'Python Fus Writer'
        self.__fusFile.Header.CreatedBy = getpass.getuser()
        lt = time.localtime()
        self.__fusFile.Header.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)
        self.__scene = self.__fusFile.Contents.FusScene
        self.__childListStack = [self.__scene.Children]
        self.__nodeStack = [None]
        self.__vertCache = {}
        self.__materialCache = {}
        self.__meshCache = {}
        # Components
        self.__curComponent = None
        self.__curMaterial = None
        self.__curMesh = None

    def CurrentNode(self):
        """Returns the current node (or None if AddChild was not yet called on the current child list)."""
        return self.__nodeStack[-1]

#### INFRASTRUCTURE ####        

    def AddComponent(self, name=None):
        """Add a (base-type) component to the current child node. Call one of the specialized Add... methods to add components of a specific type."""
        node = self.CurrentNode() 
        if node != None:
            inx = len(self.__scene.ComponentList)
            comp = self.__scene.ComponentList.add()
            if name != None:
                comp.Name = name
            node.Components.append(inx)
            return comp, inx
        else:
            raise RuntimeError('Cannot add a component to non-existing current child. Call AddChild() to add a current child.')

    def AddChild(self, name=None):
        """Adds a child node to the current child list."""
        node = self.__childListStack[-1].add()
        if name != None:
            node.Name = name
        self.__nodeStack[-1] = node

    def Push(self):
        """Pushes the child hierarchy stack down one child."""
        if self.CurrentNode() != None:
            self.__childListStack.append(self.CurrentNode().Children)
            self.__nodeStack.append(None)
        else:
            raise RuntimeError('Cannot Push without a current child. Call AddChild() to add a current child.')

    def Pop(self):
        """Performs a pop operation on the child hierarchy."""
        self.__nodeStack.pop()
        self.__childListStack.pop()

    def Print(self):
        """Print the current contents (for debugging purposes)"""
        print(self.__fusFile)

    def Serialize(self, filename):
        """Serializes the current contents to a file with the given name."""
        f = open(filename, 'wb')    
        f.write(self.__fusFile.SerializePartialToString())
        f.close()

#### TRANSFORM COMPONENT ####

    def AddTransform(self, translation, rotation, scale, name = None):
        """Adds a transform component to the current child node."""
        comp, inx = self.AddComponent(name)
        xform = comp.FusTransform
        xform.Translation.x = translation[0]
        xform.Translation.y = translation[1]
        xform.Translation.z = translation[2]
        xform.Rotation.x = rotation[0]
        xform.Rotation.y = rotation[1]
        xform.Rotation.z = rotation[2]
        xform.Scale.x = scale[0]
        xform.Scale.y = scale[1]
        xform.Scale.z = scale[2]

#### MATERIAL COMPONENT ####
    def TryReferenceMaterial(self, name):
        """See if the material identified by its unique name (among all materials) is already present. If so, add a reference to this material to the current node"""
        inxMat = self.__materialCache.get(name, -1)
        if inxMat >= 0:
            node = self.CurrentNode() 
            if node != None:
                node.Components.append(inxMat)
                return True
            else:
                raise RuntimeError('Cannot add a referenced component to non-existing current child. Call AddChild() to add a current child.')
        else:
            return False

    def AddMaterial(self, material, name=None):
        """Adds a Material component to the current child node. If any of 'RoughnessValue', 'FresnelReflectance' or 'DiffuseFraction' keys is present, a MaterialPBR component will be added.

        :param material: A dictionary containing optional 'Albedo', 'Specular', 'Emissive', 'NormalMap' sub entries and the optional PBR settings: 'RoughnessValue', 'FresnelReflectance' and 'DiffuseFraction' 

        :Example:
        fusWriter.AddMaterial(
            {
                'Albedo':    { 'Color' : (1, 0, 0, 1), 'Texture': 'SomeTexture.png', 'Mix': 0.5 },
                'Specular':  { 'Color' : (1, 1, 1, 1), 'Texture': 'SpecMap.png', 'Mix': 1.0, 'Shininess': 2.5, 'Intensity': 0.9 },
                'Emissive':  { 'Color' : (0, 0.1, 0, 1), 'Texture': 'DiffuseMap.png', 'Mix': 0.5 },
                'NormalMap': { 'Texture': 'normalmap.png', 'Intensity': 0.9 }
                'RoughnessValue': 0.42,
            }
        )
        """
        self.BeginMaterial(name)
        albedo = material.get('Albedo', None)
        if albedo != None:
            self.AddAlbedo(albedo.get('Color', None), albedo.get('Texture', None), albedo.get('Mix', 1))
        specular = material.get('Specular', None)
        if specular != None:
            self.AddSpecular(specular.get('Color', None), specular.get('Texture', None), specular.get('Mix', 1), specular.get('Shininess', 1), specular.get('Intensity', 1))
        emissive = material.get('Emissive', None)
        if emissive != None:
            self.AddEmissive(emissive.get('Color', None), emissive.get('Texture', None), emissive.get('Mix', 1))
        normalMap = material.get('NormalMap', None)
        if normalMap != None:
            self.AddNormalMap(normalMap.get('Texture', None), normalMap.get('Intensity', 1))
        if 'RoughnessValue' in material or 'FresnelReflectance' in material or 'DiffuseFraction' in material:
            self.AddPBRMaterialSettings(material.get('RoughnessValue', 0.2), material.get('FresnelReflectance', 0.2), material.get('DiffuseFraction', 0.2))
        self.EndMaterial()

    def BeginMaterial(self, name=None):
        if self.__curComponent == None:
            self.__curComponent, inx = self.AddComponent(name)
            self.__curMaterial = self.__curComponent.FusMaterial
            self.__materialCache[name] = inx
        else:
            raise RuntimeError('Cannot begin a material component with another component not ended. Call EndXYZ() to close the currently open component.')

    def __checkMaterialOpen(self):
        if self.__curMaterial == None:
            raise RuntimeError('Cannot operate on nonexisting material component. Call BeginMaterial()/AddMaterial to add a material.')

    def AddAlbedo(self, color, texture, mix):
        self.__checkMaterialOpen()
        if color != None:
            self.__curMaterial.Albedo.Color.x = color[0]
            self.__curMaterial.Albedo.Color.y = color[1]
            self.__curMaterial.Albedo.Color.z = color[2]
            self.__curMaterial.Albedo.Color.w = color[3]
        if texture != None:
            self.__curMaterial.Albedo.Texture = texture
        self.__curMaterial.Albedo.Mix = mix

    def AddSpecular(self, color, texture, mix, shininess, intensity):
        self.__checkMaterialOpen()
        if color != None:
            self.__curMaterial.Specular.Color.x = color[0]
            self.__curMaterial.Specular.Color.y = color[1]
            self.__curMaterial.Specular.Color.z = color[2]
            self.__curMaterial.Specular.Color.w = color[3]
        if texture != None:
            self.__curMaterial.Specular.Texture = texture
        self.__curMaterial.Specular.Mix = mix
        self.__curMaterial.Specular.SpecularChannelContainer.Shininess = shininess
        self.__curMaterial.Specular.SpecularChannelContainer.Intensity = intensity

    def AddEmissive(self, color, texture, mix):
        self.__checkMaterialOpen()
        if color != None:
            self.__curMaterial.Emissive.Color.x = color[0]
            self.__curMaterial.Emissive.Color.y = color[1]
            self.__curMaterial.Emissive.Color.z = color[2]
            self.__curMaterial.Emissive.Color.w = color[3]
        if texture != None:
            self.__curMaterial.Emissive.Texture = texture
        self.__curMaterial.Emissive.Mix = mix

    def AddNormalMap(self, texture, intensity):
        self.__checkMaterialOpen()
        if texture != None:
            self.__curMaterial.NormalMap.Texture = texture
        self.__curMaterial.NormalMap.Intensity = intensity

    def AddPBRMaterialSettings(self, roughnessValue, fresnelReflectance, diffuseFraction):
        self.__checkMaterialOpen()
        self.__curMaterial.FusMaterialPBR.RoughnessValue = roughnessValue
        self.__curMaterial.FusMaterialPBR.FresnelReflectance = fresnelReflectance
        self.__curMaterial.FusMaterialPBR.DiffuseFraction = diffuseFraction

    def EndMaterial(self):
        self.__checkMaterialOpen()
        self.__curMaterial = None
        self.__curComponent = None

#### MESH COMPONENT  ####

# public float3[] Vertices;
# public uint[] Colors;
# public float3[] Normals;
# public float2[] UVs;
# public float4[] BoneWeights;
# public float4[] BoneIndices;
# public ushort[] Triangles;
# public float4[] Tangents;
# public float3[] BiTangents;
# public AABBf BoundingBox;

    def AddVertex(self, vertex, normal=None, uv=None, tangent=None, bitangent=None):
        self.__checkMeshOpen()
        vx, vy, vz = vertex
        nx, ny, nz = normal if normal != None else (1, 1, 1)
        uvx, uvy = uv if uv != None else (1, 1)
        key = (vx, vy, vz, nx, ny, nz, uvx, uvy)
        inx = self.__vertCache.get(key, -1)
        if inx < 0:
            inx = len(self.__curMesh.Vertices)
            self.__vertCache[key] = inx
            v = self.__curMesh.Vertices.add()
            v.x = vertex[0]
            v.y = vertex[1]
            v.z = vertex[2]
            if normal != None:
                n = self.__curMesh.Normals.add()
                n.x = normal[0]
                n.y = normal[1]
                n.z = normal[2]
            if uv != None:
                u = self.__curMesh.UVs.add()
                u.x = uv[0]
                u.y = uv[1]
            if tangent != None:
                t = self.__curMesh.Tangents.add()
                t.x = tangent[0]
                t.y = tangent[1]
                t.z = tangent[2]
                t.w = tangent[3]
            if bitangent != None:
                bt = self.__curMesh.BiTangents.add()
                bt.x = bitangent[0]
                bt.y = bitangent[1]
                bt.z = bitangent[2]
            self.__AddVertexToBoundingBox(vertex)
        else:
            if not (vertex[0] == self.__curMesh.Vertices[inx].x and vertex[1] == self.__curMesh.Vertices[inx].y and vertex[2] == self.__curMesh.Vertices[inx].z):
                print('WARNING: New vertex: ' + str(vertex) + ' has same hash as existing: ' + str(self.__curMesh.Vertices[inx]))

        self.__curMesh.Triangles.append(inx)

    def __AddVertexToBoundingBox(self, vertex):
        if vertex[0] < self.__curMesh.BoundingBox.min.x: self.__curMesh.BoundingBox.min.x = vertex[0]
        if vertex[1] < self.__curMesh.BoundingBox.min.y: self.__curMesh.BoundingBox.min.y = vertex[1]
        if vertex[2] < self.__curMesh.BoundingBox.min.z: self.__curMesh.BoundingBox.min.z = vertex[2]
        if vertex[0] > self.__curMesh.BoundingBox.max.x: self.__curMesh.BoundingBox.max.x = vertex[0]
        if vertex[1] > self.__curMesh.BoundingBox.max.y: self.__curMesh.BoundingBox.max.y = vertex[1]
        if vertex[2] > self.__curMesh.BoundingBox.max.z: self.__curMesh.BoundingBox.max.z = vertex[2]

    def __checkMeshOpen(self):
        if self.__curMesh == None:
            raise RuntimeError('Cannot operate on nonexisting mesh component. Call BeginMesh()/AddMesh() to add mesh.')

    def TryReferenceMesh(self, name):
        """See if the FUSEE mesh identified by its unique name (among all meshes) is already present. If so, add a reference to this mesh to the current node"""
        inxMesh = self.__meshCache.get(name, -1)
        if inxMesh >= 0:
            node = self.CurrentNode() 
            if node != None:
                node.Components.append(inxMesh)
                return True
            else:
                raise RuntimeError('Cannot add a referenced component to non-existing current child. Call AddChild() to add a current child.')
        else:
            return False

    def GetReferencedMeshTriVertCount(self, name):
        """Return the number of entries in the triangles index list for the already present mesh identified by the name."""
        inxMesh = self.__meshCache.get(name, -1)
        if inxMesh >= 0:
            return len(self.__scene.ComponentList[inxMesh].FusMesh.Triangles)
        else:
            return -1

    def BeginMesh(self, vertex, normal=None, uv=None, tangent=None, bitangent=None, name=None):
        if self.__curComponent == None:
            self.__curComponent, inx = self.AddComponent(name)
            self.__curMesh = self.__curComponent.FusMesh
            self.__meshCache[name] = inx
            self.__curMesh.BoundingBox.min.x = vertex[0]
            self.__curMesh.BoundingBox.min.y = vertex[1]
            self.__curMesh.BoundingBox.min.z = vertex[2]
            self.__curMesh.BoundingBox.max.x = vertex[0]
            self.__curMesh.BoundingBox.max.y = vertex[1]
            self.__curMesh.BoundingBox.max.z = vertex[2]
            self.__vertCache = {}
            self.AddVertex(vertex, normal, uv, tangent, bitangent)

        else:
            raise RuntimeError('Cannot begin a mesh component with another component not ended. Call EndXYZ() to close the currently open component.')

    def MeshHasCapacity(self):
        return len(self.__curMesh.Vertices) < self.__MaxVerts or len(self.__curMesh.Triangles) % 3 != 0

    def EndMesh(self):
        self.__checkMeshOpen()
        # print('EndMesh(): ' + str(len(self.__curMesh.Triangles)/3) + ' Tris on ' + str(len(self.__curMesh.Vertices)) + ' Verts.')
        self.__curMesh = None
        self.__curComponent = None



#### CAMERA COMPONENT ####

# bool ClearColor = true;
# bool ClearDepth = true;
# int Layer;
# float4 BackgroundColor;
# ProjectionMethod ProjectionMethod;
# float Fov;
# float2 ClippingPlanes;
# float4 Viewport = new float4(0, 0, 100, 100);
# bool Active = true;
    
    def AddCamera(self, projectionmethod, fov, clippingplanes, viewport=(0, 0, 100, 100), clearcolor=True, cleardepth=True, layer=0, backgroundcolor=(1, 1, 1, 1), active=True, name=None):
        """Adds a Camera component to the current child node.

        Args:
            projectionmethod (int): - 0: Orthographic, 1: Perspective
            fov (float): The vertical (y) field of view in radians.
            clippingplanes (float2): Distance to the near and far clipping planes.
            viewport (float4): The viewport in percent [0, 100]. x: start; y: end; z: width; w: height.
            clearcolor (bool): If set to false, the color bit won't be cleared before this camera is rendered.
            cleardepth (bool): If set to false, the depth bit won't be cleared before this camera is rendered.
            layer (int): If there is more than one CameraComponent in one scene, the rendered output of the camera with a higher layer will be drawn above the output of a camera with a lower layer.
            backgroundcolor (float4): The background color for this camera's viewport.
            active (bool): A camera is active by default. Set this to false to deactivate it. 
        """
        cam = self.AddComponent(name).FusCamera
        cam.ProjectionMethod = projectionmethod
        cam.Fov = fov
        cam.ClippingPlanes.x = clippingplanes[0]
        cam.ClippingPlanes.y = clippingplanes[1]
        cam.Viewport.x = viewport[0]
        cam.Viewport.y = viewport[1]
        cam.Viewport.z = viewport[2]
        cam.Viewport.w = viewport[3]
        cam.ClearColor = clearcolor
        cam.ClearDepth = cleardepth
        cam.Layer = layer
        cam.BackgroundColor.x = backgroundcolor[0]
        cam.BackgroundColor.y = backgroundcolor[1]
        cam.BackgroundColor.z = backgroundcolor[2]
        cam.BackgroundColor.w = backgroundcolor[3]
        cam.Active = active

#### LIGHT COMPONENT ####

# public bool Active;
# public float4 Color;
# public float MaxDistance;
# public float Strength;
# public LightType Type;
# public float OuterConeAngle;
# public float InnerConeAngle;

    def AddLight(self, active, color, maxdistance, strength, type, outerconeangle, innerconeangle, name=None):
        """Adds a Light component to the current child node.
 
        :param active: bool - Represents the light status.
        :param color: float4 - Represents the color.
        :param maxdistance: float - Represents the attenuation of the light.
        :param strength: float [0..1] - Represents the strength of the light (non-physically representation of the brightness).
        :param type: int - 0: Point; 1: Parallel; 2: Spot; 3: Legacy
        :param outerconeangle: float - Represents the outer spot angle of the light.
        :param innerconeangle: float - Represents the spot inner angle of the light.
        """
        light = self.AddComponent(name).FusLight
        light.Active = active
        light.Color.x = color[0]
        light.Color.y = color[1]
        light.Color.z = color[2]
        light.Color.w = color[3]
        light.MaxDistance = maxdistance
        light.Strength = strength
        light.Type = type
        light.OuterConeAngle = outerconeangle
        light.InnerConeAngle = innerconeangle
 
