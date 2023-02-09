using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Serialization;
using Fusee.SLIRP.Core;



namespace Fusee.Tests.SLIRP.SLIRPTest
{
    internal static class Run
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Fusee Initialization Stuff");
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new IOImp();

            //Irgendwas mit Assets, keine Ahnung
            var fap = new FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) });
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                        return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                        return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)
                });

            AssetStorage.RegisterProvider(fap);

            //Erstellen der "Applikation"
            Console.WriteLine("Create SLIRPer");
            SLIRPer slirper = new SLIRPer();

            Console.WriteLine("Create PC SLIRPer");
            SLIRPer pcSlirper = new SLIRPer();
            var pcApp = new Fusee.Examples.PointCloudPotree2.Core.PointCloudPotree2();

            //Console.WriteLine("Init empty SLIRPer");
            //slirper.Init();

            Console.WriteLine("Init PC SLIRPer");
            slirper.Init(pcApp);
        }
    }
}
