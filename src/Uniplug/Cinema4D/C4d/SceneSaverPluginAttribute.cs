using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class SceneSaverPluginAttribute : PluginBaseAttribute
    {
        public bool DialogControl;

        public int Info
        {
            get
            {
                return
                    /*
                    (Hide) ?                 C4dApi.PLUGINFLAG_HIDE : 
                    (SmallNode) ?            PLUGINFLAG_SMALLNODE : 
                    (Coffee) ?               PLUGINFLAG_COFFEE : 
                    (HidePluginMenu) ?       PLUGINFLAG_HIDEPLUGINMENU : 
                    (RefreshAlways) ?        PLUGINFLAG_REFRESHALWAYS : #
                     */
                    ((DialogControl) ? C4dApi.PLUGINFLAG_SCENEFILTER_DIALOGCONTROL : 0);
            }
        }

        public string Suffix;
        public SceneSaverPluginAttribute(int id)
            : base(id)
        {
        }
        public SceneSaverPluginAttribute(int id, string name)
            : base(id, name)
        {
        }
        public SceneSaverPluginAttribute(int id, string name, string suffix)
            : base(id, name, null)
        {
            Suffix = suffix;
        }
    }
}
