﻿namespace Fusee.Engine
 {
     /// <summary>
     /// This Object is used by the OpenTK Graphics implementation in order to handle parameter passing between Fusee and OpenTK. 
     /// Do not modify this object.
     /// </summary>
     public class Helper
     {
         #region Fields

         /// <summary>
         /// The vertex attribute name.
         /// </summary>
         public static readonly string VertexAttribName = "fuVertex";
         /// <summary>
         /// The color attribute name.
         /// </summary>
         public static readonly string ColorAttribName = "fuColor";
         /// <summary>
         /// The normal attribute name.
         /// </summary>
         public static readonly string NormalAttribName = "fuNormal";
         /// <summary>
         /// The uv attribute name.
         /// </summary>
         public static readonly string UvAttribName = "fuUV";
         /// <summary>
         /// The tangent attribute name.
         /// </summary>
         public static readonly string TangentAttribName = "fuTangent";
         /// <summary>
         /// The bitangent attribute name.
         /// </summary>
         public static readonly string BitangentAttribName = "fuBitangent";


         /// <summary>
         /// The vertex attribute location index.
         /// </summary>
         public static readonly int VertexAttribLocation = 0;
         /// <summary>
         /// The color attribute location index.
         /// </summary>
         public static readonly int ColorAttribLocation = 1;
         /// <summary>
         /// The normal attribute location index.
         /// </summary>
         public static readonly int NormalAttribLocation = 2;
         /// <summary>
         /// The uv attribute location index.
         /// </summary>
         public static readonly int UvAttribLocation = 3;
         /// <summary>
         /// The tangent attribute location index.
         /// </summary>
         public static readonly int TangentAttribLocation = 4;
         /// <summary>
         /// The bitangent attribute location index.
         /// </summary>
         public static readonly int BitangentAttribLocation = 5;

         #endregion
     }
 }