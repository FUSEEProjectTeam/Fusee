import sys
import time
import getpass

from proto import FusSerialization_pb2 as FusSer

class FusSceneWriter:
    """Convenience Class to assemble FUSEE scene files from instances of FusNode and FusComponent (and derivatives)."""
    def __init__(self):
        super().__init__()
        self.__fusFile = FusSer.FusFile()
        self.__fusFile.Header.FileVersion = 1
        self.__fusFile.Header.Generator = 'Python Fus Writer'
        self.__fusFile.Header.CreatedBy = getpass.getuser()
        lt = time.localtime()
        self.__fusFile.Header.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)
        self.__scene = self.__fusFile.Contents.FusScene
        self.__childListStack = [self.__scene.Children]
        self.__nodeStack = [None]
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
            return comp
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
        xform = self.AddComponent(name).FusTransform
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

    def AddMaterial(self, material, name=None):
        """Adds a Material component to the current child node. If any of 'RoughnessValue', 'FresnelReflectance' or 'DiffuseFraction' keys is present, a MaterialPBR component will be added.

        :param material: A dictionaray containing optional 'Diffuse', 'Specular', 'Emissive', 'Bump' sub entries and the optional PBR settings: 'RoughnessValue', 'FresnelReflectance' and 'DiffuseFraction' 

        :Example:
        fuseWriter.AddMaterial(
            {
                'Diffuse':  { 'Color' : (1, 0, 0, 1), 'Texture': 'SomeTexture.png', 'Mix': 0.5 },
                'Specular': { 'Color' : (1, 1, 1, 1), 'Texture': 'SpecMap.png', 'Mix': 1.0, 'Shininess': 2.5, 'Intensity': 0.9 },
                'Emissive': { 'Color' : (0, 0.1, 0, 1), 'Texture': 'DiffuseMap.png', 'Mix': 0.5 },
                'Bump':     { 'Texture': 'Bumpmap.png', 'Intensity': 0.9 }
                'RoughnessValue': 0.42,
            }
        )
        """
        self.BeginMaterial(name)
        diffuse = material.get('Diffuse', None)
        if diffuse != None:
            self.AddDiffuse(diffuse.get('Color', None), diffuse.get('Texture', None), diffuse.get('Mix', 1))
        specular = material.get('Specular', None)
        if specular != None:
            self.AddSpecular(specular.get('Color', None), specular.get('Texture', None), specular.get('Mix', 1), specular.get('Shininess', 1), specular.get('Intensity', 1))
        emissive = material.get('Emissive', None)
        if emissive != None:
            self.AddEmissive(emissive.get('Color', None), emissive.get('Texture', None), emissive.get('Mix', 1))
        bump = material.get('Bump', None)
        if bump != None:
            self.AddBump(bump.get('Texture', None), bump.get('Intensity', 1))
        if 'RoughnessValue' in material or 'FresnelReflectance' in material or 'DiffuseFraction' in material:
            self.AddPBRMaterialSettings(material.get('RoughnessValue', 0.2), material.get('FresnelReflectance', 0.2), material.get('DiffuseFraction', 0.2))
        self.EndMaterial()

    def BeginMaterial(self, name=None):
        if self.__curComponent == None:
            self.__curComponent = self.AddComponent(name)
            self.__curMaterial = self.__curComponent.FusMaterial
        else:
            raise RuntimeError('Cannot begin a material component with another component not ended. Call EndXYZ() to close the currently open component.')

    def __checkMaterialOpen(self):
        if self.__curMaterial == None:
            raise RuntimeError('Cannot operate on nonexisting material component. Call BeginMaterial()/AddMaterial to add a material.')

    def AddDiffuse(self, color, texture, mix):
        self.__checkMaterialOpen()
        if color != None:
            self.__curMaterial.Diffuse.Color.x = color[0]
            self.__curMaterial.Diffuse.Color.y = color[1]
            self.__curMaterial.Diffuse.Color.z = color[2]
            self.__curMaterial.Diffuse.Color.w = color[3]
        if texture != None:
            self.__curMaterial.Diffuse.Texture = texture
        self.__curMaterial.Diffuse.Mix = mix

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

    def AddBump(self, texture, intensity):
        self.__checkMaterialOpen()
        if texture != None:
            self.__curMaterial.Bump.Texture = texture
        self.__curMaterial.Bump.Intensity = intensity

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

    def AddMesh(self, mesh, name=None):
    """
    """
#       public float3[] Vertices;
#       public uint[] Colors;
#       public float3[] Normals;
#       public float2[] UVs;
#       public float4[] BoneWeights;
#       public float4[] BoneIndices;
#       public ushort[] Triangles;
#       public float4[] Tangents;
#       public float3[] BiTangents;
#       public AABBf BoundingBox;
    self.BeginMesh()
        vertices = mesh.get('Vertices', None)
        colors = mesh.get('Colors', None)
        normals = mesh.get('Normals', None)
        uvs = mesh.get('UVs', None)
        boneweights = mesh.get('BoneWeights', None)
        boneindices = mesh.get('BoneIndices', None)
        triangles = mesh.get('Triangles', None)
        tangents = mesh.get('Tangents', None)
        bitangents = mesh.get('BiTangents', None)
        boundingbox = mesh.get('BoundingBox', None)
    self.EndMesh()
    

    def __checkMeshOpen(self):
        if self.__curMesh == None:
            raise RuntimeError('Cannot operate on nonexisting mesh component. Call BeginMesh()/AddMesh() to add mesh.')

    def BeginMesh(self, name=None):
        if self.__curComponent == None:
            self.__curComponent = self.AddComponent(name)
            self.__curMesh = self.__curComponent.FusMesh
        else:
            raise RuntimeError('Cannot begin a mesh component with another component not ended. Call EndXYZ() to close the currently open component.')

    def EndMesh(self):
        self.__checkMeshOpen()
        self.__curMesh = None
        self.__curComponent = None
        
       
    
