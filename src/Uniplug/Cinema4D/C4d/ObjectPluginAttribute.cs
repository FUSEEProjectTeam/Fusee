using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class ObjectPluginAttribute : PluginBaseAttribute
    {
        public bool Hide;
        public bool SmallNode;
        public bool Coffee;
        public bool HidePluginMenu;
        public bool RefreshAlways;
 
        public bool Modifier;
        public bool HierarchyModifier;
        public bool Generator;
        public bool Input;
        public bool ParticleModifier;
        public bool NoCacheSub;
        public bool UniqueEnumeration;
        public bool IsSpline;
        public bool CameraDependent;
        public bool UseCacheColor;
        public bool PointObject;
        public bool PolygonObject;
        public bool NoPla;
        public bool DontFreeCache;
        public bool CallAddExecution;

        public int Info
        {
            get
            {
                return
                    /*
                    (Hide) ?                 C4dApi.PLUGINFLAG_HIDE : 0 |
                    (SmallNode) ?            PLUGINFLAG_SMALLNODE : 
                    (Coffee) ?               PLUGINFLAG_COFFEE : 
                    (HidePluginMenu) ?       PLUGINFLAG_HIDEPLUGINMENU : 
                    (RefreshAlways) ?        PLUGINFLAG_REFRESHALWAYS : #
                    */
                    ((Modifier) ? C4dApi.OBJECT_MODIFIER : 0) |
                    ((HierarchyModifier) ? C4dApi.OBJECT_HIERARCHYMODIFIER : 0) |
                    ((Generator) ? C4dApi.OBJECT_GENERATOR : 0) |
                    ((Input) ? C4dApi.OBJECT_INPUT : 0) |
                    ((ParticleModifier) ? C4dApi.OBJECT_PARTICLEMODIFIER : 0) |
                    ((NoCacheSub) ? C4dApi.OBJECT_NOCACHESUB : 0) |
                    ((UniqueEnumeration) ? C4dApi.OBJECT_UNIQUEENUMERATION : 0) |
                    ((IsSpline) ? C4dApi.OBJECT_ISSPLINE : 0) |
                    ((CameraDependent) ? C4dApi.OBJECT_CAMERADEPENDENT : 0) |
                    ((UseCacheColor) ? C4dApi.OBJECT_USECACHECOLOR : 0) |
                    ((PointObject) ? C4dApi.OBJECT_POINTOBJECT : 0) |
                    ((PolygonObject) ? C4dApi.OBJECT_POLYGONOBJECT : 0) |
                    ((NoPla) ? C4dApi.OBJECT_NO_PLA : 0) |
                    ((DontFreeCache) ? C4dApi.OBJECT_DONTFREECACHE : 0) |
                    ((CallAddExecution) ? C4dApi.OBJECT_CALL_ADDEXECUTION : 0);
            }
        }

        public string Description;
        public ObjectPluginAttribute(int id) : base(id)
        {
        }
        public ObjectPluginAttribute(int id, string name) : base(id, name)
        {
        }
        public ObjectPluginAttribute(int id, string name, string iconFile)
            : base(id, name, iconFile)
        {
        }
        public ObjectPluginAttribute(int id, string name, string iconFile, string description)
            : base(id, name, iconFile)
        {
            Description = description;
        }
    }
}
