using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;
// using ProtoBuf;

namespace FuExport
{
    // Your Plugin Number for "FUSEE General Tag" is: 1036156 
    [TagPlugin(1036156, "FUSEE Step2016 Data", "FuExportLogo.tif", Visible = true)]
    public class STeP2016Data : TagBase
    {
        [TagItem(DisplayName = "Radius")]
        public int TheInt { get; set; }

        [TagItem(DisplayName = "Extra Info")]
        public string TheName { get; set; }


        public STeP2016Data() : base()
        {
            TheInt = 22;
            TheName = "HFU";
        }
    }
}
