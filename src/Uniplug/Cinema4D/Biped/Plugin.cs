using C4d;
namespace Biped
{
    [Plugin]
    public class XXPlugin
    {
        // public const int ID_PLUGINTEMPLATE = 1000006;

        public XXPlugin()
        {
            Logger.Debug("XXPlugin()");
        }

        public bool Start()
        {
            Logger.Debug("Start()");

            /*
            string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "res", "xpressoxport.tif");

            BaseBitmap bmp = BaseBitmap.AutoBitmap(filename);
            return C4dApi.RegisterCommandPlugin(ID_PLUGINTEMPLATE, "ManagedXPressoTags", 0, bmp, "get the XPressoTags from Objects", new GetXPressoTags());        
        
            */
            return true;
        }

        public void End()
        {
        }

        public bool Message(int id)
        {
            Logger.Debug("Message("+id+")");
            return true;
        }
    }
}