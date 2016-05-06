using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;
// using ProtoBuf;

namespace FuExport
{
    // Your Plugin Number for "FUSEE General Tag" is: 1036156 
    [TagPlugin(1036156, "FUSEE General Tag", "FuExportLogo.tif", Visible = true)]
    public class GeneralTag : TagBase
    {
        [TagItem(DisplayName = "Some Int")]
        public int TheInt { get; set; }

        [TagItem(DisplayName = "Some Text")]
        public string TheName { get; set; }


        public GeneralTag() : base()
        {
            TheInt = 42;
            TheName = "Hello, Tag";
        }
    }
}
