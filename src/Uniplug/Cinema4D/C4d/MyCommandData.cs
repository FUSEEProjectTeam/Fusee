using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;

namespace C4d
{
    class MyCommandData : CommandData
    {
        public MyCommandData() : base(false)
        {
        }

        public override bool Execute(BaseDocument doc)
        {
            C4dApi.MessageDialog("ManagedPlugIn");
            C4dApi.GePrint("Console Output: ManagedPlugIn");

            BaseObject ob = doc.SearchObject("MeinObjekt");

            if (ob == null)
            {
                C4dApi.MessageDialog("Kein Objekt namens MeinObjekt gefunden");
            }
            else
            {
                for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
                {
                    int tagType = tag.GetType();
                    if (tagType == C4dApi.Texpresso)
                    {
                        C4dApi.GePrint("XPresso-Tag gefunden");
                        C4dApi.GePrint(tag.GetName());
                    }
                }
            }
            return true;
        }
    }
}
