import sys
import time
import getpass

sys.path.insert(0, '../../../Tools/BlenderScripts/addons/io_export_fus/proto')
import FusSerialization_pb2 as FusSer

fusFile = FusSer.FusFile()
fusFile.Header.FileVersion = 1
fusFile.Header.Generator = 'Von Hand gefuzzelt'
fusFile.Header.CreatedBy = getpass.getuser()
lt = time.localtime()
fusFile.Header.CreationDate = str(lt.tm_mday) + '-' + str(lt.tm_mon) + '-' + str(lt.tm_year)
node = fusFile.Contents.FusScene.Children.add()
node.Name = 'A Node'
print(fusFile)
print(fusFile.SerializePartialToString())


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


