import sys
import time
import getpass

sys.path.insert(0, '../../../Tools/BlenderScripts/addons/io_export_fus')
import FusSceneWriter

writer = FusSceneWriter.FusSceneWriter()
writer.AddChild('Root')
writer.AddTransform( (1, 2, 3), [4, 5, 6], [7, 8, 9] )
writer.Push()
writer.AddChild('Child')
writer.AddChild('Child No2')
writer.AddMaterial(
    {
        'Diffuse':  { 'Color' : (1, 0, 0, 1), 'Mix': 0.5 },
        'Specular': { 'Color' : (1, 1, 1, 1), 'Shininess': 2.5, 'Intensity': 0.9 },
        'RoughnessValue': 0.42,
        'FresnelReflectance': 0.43,
        'DiffuseFraction': 0.44
    })

writer.Pop()
writer.AddChild("Second Root")
writer.AddMaterial(
    {
        'Diffuse':  { 'Color' : (1, 0, 0, 1), 'Texture': 'SomeTexture.png', 'Mix': 0.5 },
        'Specular': { 'Color' : (1, 1, 1, 1), 'Texture': 'SpecMap.png', 'Mix': 1.0, 'Shininess': 2.5, 'Intensity': 0.9 },
        'Emissive': { 'Color' : (0, 0.1, 0, 1), 'Texture': 'DiffuseMap.png', 'Mix': 0.5 },
        'Bump':     { 'Texture': 'Bumpmap.png', 'Intensity': 0.9 }
    })



writer.Print()
writer.Serialize('Test.fus')

#sys.path.insert(0, '../../../Tools/BlenderScripts/addons/io_export_fus/proto')
#import FusSerialization_pb2 as FusSerdidir
#
#fusFile = FusSer.FusFile()
#fusFile.Header.FileVersion = 1
#fusFile.Header.Generator = 'Von Hand gefuzzelt'
#fusFile.Header.CreatedBy = getpass.getuser()
#lt = time.localtime()
#fusFile.Header.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)
#node = fusFile.Contents.FusScene.Children.add()
#node.Name = 'A Node'
#print(fusFile)
#print(fusFile.SerializePartialToString())


# sc = Scene.SceneContainer()
#sc.Header.Version = "0.8"
#sc.Header.Generator = 'Von Hand gefuzzelt'
#sc.Header.CreatedBy = getpass.getuser()
#lt = time.localtime()
#sc.Header.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)

#node = sc.Children.add()
#node.Name = '🚗 🚗 🚗'


#print(sc)
#print(sc.SerializePartialToString())


