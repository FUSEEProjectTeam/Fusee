﻿using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.PotreeReader.V2
{
    public class ReadPotree2Metadata
    {
        public static PointType GetPointType(string pathToNodeFileFolder = "")
        {
            return PointType.Position_double__Color_float__Label_byte;
        }
    }
}
