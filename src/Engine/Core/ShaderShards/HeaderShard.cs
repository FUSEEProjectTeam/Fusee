using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards
{
    public static class HeaderShard
    {
        public static string EsPrecision()
        {
            return "precision highp float; \n";
        }        

        public static string Version()
        {
            return "#version 300 es\n";
        }
    }
}
