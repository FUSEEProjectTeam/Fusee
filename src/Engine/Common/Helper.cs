﻿namespace Fusee.Engine
 {
     /// <summary>
     /// This Object is used by the OpenTK Graphics implementation in order to handle parameter passing between Fusee and OpenTK. 
     /// Do not modify this object.
     /// </summary>
     public class Helper
     {
         public static readonly string VertexAttribName = "fuVertex";
         public static readonly string ColorAttribName = "fuColor";
         public static readonly string NormalAttribName = "fuNormal";
         public static readonly string UvAttribName = "fuUV";

         public static readonly int VertexAttribLocation = 0;
         public static readonly int ColorAttribLocation = 1;
         public static readonly int NormalAttribLocation = 2;
         public static readonly int UvAttribLocation = 3;

     }
 }