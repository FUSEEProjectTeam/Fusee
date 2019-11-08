using Fusee.Base.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fusee.Engine.Core.ShaderShards
{
    public static class Includes
    {
        public static string ParseIncludes(string rawShaderString)
        {           
            var fields = GetFieldValues(typeof(UniformNameDeclarations));

            var refinedShader = new List<string>();

            var allLines = rawShaderString.Split(new[] { '\r', '\n' });
            foreach (var line in allLines)
            {
                // if we have one or more includes we need to replace them                
                if (line.Contains("#include"))
                {
                    var includeLine = line.Replace(@"#include ", string.Empty);
                    includeLine = includeLine.Replace("\"", string.Empty);
                    var fileFromInclude = includeLine;
                    var foundFile = "";
                    try
                    {
                        foundFile = AssetStorage.Get<string>("Assets/Shader/" + fileFromInclude);
                        if (foundFile == null)
                            throw new FileNotFoundException(foundFile);
                    }
                    catch (FileNotFoundException e)
                    {
                        Diagnostics.Log($"[ShaderCodeBuilder.cs] Error file #include {e.FileName} not found!");
                    }
                    refinedShader.Add(foundFile);
                }
                else
                {
                    refinedShader.Add(line);
                }
            }

            // replace all names
            foreach (var field in fields)
            {
                for (var i = 0; i < refinedShader.Count; i++)
                {
                    if (refinedShader[i].Contains(field.Key))
                    {
                        refinedShader[i] = refinedShader[i].Replace(field.Key, field.Value);
                    }
                }
            }

            return String.Join("\n", refinedShader);
        }

        private static Dictionary<string, string> GetFieldValues(Type type)
        {
            return type
                      .GetProperties(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.PropertyType == typeof(string))
                      .ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
        }
    }
}
