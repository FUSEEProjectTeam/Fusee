using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A surface effect contains information to build a shader program 
    /// </summary>
    public class SurfaceEffect : Effect
    {
       
        public readonly List<KeyValuePair<ShardCategory, string>> vertexShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        public readonly List<KeyValuePair<ShardCategory, string>> geometryShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        public readonly List<KeyValuePair<ShardCategory, string>> fragmentShaderSrc = new List<KeyValuePair<ShardCategory, string>>();

        public SurfaceEffect()
        {
            EffectEventArgs = new EffectEventArgs(this, ChangedEnum.UNCHANGED);

            List<FxPassDeclaration> effectPassDecl = new List<FxPassDeclaration>();
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            //TODO: implement passes and States 
            RendererStates = new RenderStateSet();
            RendererStates = new RenderStateSet
            {
                ZEnable = true,
                AlphaBlendEnable = true,
                SourceBlend = Blend.SourceAlpha,
                DestinationBlend = Blend.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
            };           

            //TODO: Difference forward/deferred. Lights as properties? Would be difficult because each Light property must be one property in the ShaderEffect.
            foreach (var dcl in CreateForwardLightingParamDecls(ShaderShards.Fragment.LightingShard.NumberOfLightsForward))
                ParamDecl.Add(dcl.Name, dcl);

            //TODO: ParamDecls for FUSEE_Matrices -  they should not be set by ShaderEffect.FUSEE_MVP = new float4x4() because they are managed under the hood in most cases.
            foreach (var dcl in CreateMatParamDecls())
                ParamDecl.Add(dcl.Name, dcl);

            Type t = GetType();

            FxParamAttribute paramAttribute;
            FxShaderAttribute shaderAttribute;
            FxShardAttribute shardAttribute;

            foreach (var prop in t.GetProperties())
            {
                var attribs = prop.GetCustomAttributes();

                paramAttribute = null;
                shaderAttribute = null;
                shardAttribute = null;

                foreach (var attrib in attribs)
                {
                    switch (attrib)
                    {
                        case FxParamAttribute paramAttrib:
                            paramAttribute = paramAttrib;
                            break;
                        case FxShaderAttribute shaderAttrib:
                            shaderAttribute = shaderAttrib;
                            break;
                        case FxShardAttribute shardAttrib:
                            shardAttribute = shardAttrib;
                            break;
                    }

                }
                if (paramAttribute != null)
                {
                    var paramDcl = BuildFxParamDecl(prop);
                    ParamDecl.Add(paramDcl.Name, paramDcl);
                    HandleUniform(paramAttribute, paramDcl.Name, paramDcl.ParamType);
                    continue;
                }
                else if (shaderAttribute != null && shaderAttribute != null)
                {
                    if (prop.GetAccessors(false).Any(x => x.IsStatic) && prop.PropertyType == typeof(string))
                        HandleShard(shaderAttribute, shardAttribute, (string)prop.GetValue(this));
                    else
                        throw new Exception($"{t.Name} ShaderEffect: Property {prop.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                }
            }
            foreach (var field in t.GetFields())
            {
                paramAttribute = null;
                shaderAttribute = null;
                shardAttribute = null;

                var attribs = field.GetCustomAttributes();
                foreach (var attrib in attribs)
                {
                    switch (attrib)
                    {
                        case FxShaderAttribute shaderAttrib:
                            shaderAttribute = shaderAttrib;
                            break;
                        case FxShardAttribute shardAttrib:
                            shardAttribute = shardAttrib;
                            break;
                    }
                }

                if (shaderAttribute != null && shardAttribute != null)
                {
                    if (field.IsStatic && field.FieldType == typeof(string))
                        HandleShard(shaderAttribute, shardAttribute, (string)field.GetValue(this));
                    else
                        throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                }
            }
        }        

        private void HandleShard(FxShaderAttribute shaderAttrib, FxShardAttribute shardAttrib, string shardCode)
        {
            switch (shaderAttrib.ShaderCategory)
            {
                case ShaderCategory.Vertex:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Pixel:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry_Pixel:
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry_Pixel:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                default:
                    break;
            }
        }

        private void HandleUniform(FxParamAttribute attrib, string uniformName, Type type)
        {
            var uniform = "uniform ";
            switch (attrib.ShaderCategory)
            {
                case ShaderCategory.Vertex:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Pixel:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry_Pixel:
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry_Pixel:
                    vertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    fragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    geometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;

                default:
                    break;
            }
        }

        private IFxParamDeclaration BuildFxParamDecl(PropertyInfo prop)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { prop.PropertyType }); //ok!!
            //Cannot use GetConstructor(Type.EmptyTypes) with a struct!!
            //object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            var ob = Activator.CreateInstance(concreteParamDecl);

            //Error because property ParamType has no setter.
            //concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());

            concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, prop.Name);
            var val = prop.GetValue(this);
            concreteParamDecl.GetField("Value").SetValue(ob, val);
            return (IFxParamDeclaration)ob;
        }

        public static string JoinShards(List<KeyValuePair<ShardCategory, string>> shardList)
        {
            string res = string.Empty;
            foreach (var kvp in shardList)
                res += kvp.Value;

            return res;
        }

        private static IEnumerable<IFxParamDeclaration> CreateForwardLightingParamDecls(int numberOfLights)
        {
            for (int i = 0; i < numberOfLights; i++)
            {
                if (!ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights.ContainsKey(i))
                {
                    ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights.Add(i, new ShaderShards.Fragment.LightParamStrings(i));
                }

                yield return new FxParamDeclaration<float3>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].PositionViewSpace,
                    Value = new float3(0, 0, -1.0f)
                };

                yield return new FxParamDeclaration<float4>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].Intensities,
                    Value = float4.Zero
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].MaxDistance,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].Strength,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].OuterAngle,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].InnerAngle,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float3>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].Direction,
                    Value = float3.Zero
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].LightType,
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].IsActive,
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].IsCastingShadows,
                    Value = 0
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.LightingShard.LightPararamStringsAllLights[i].Bias,
                    Value = 0f
                };
            }
        }

        private static IEnumerable<IFxParamDeclaration> CreateMatParamDecls()
        {
            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.Model,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ModelView,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ModelViewProjection,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ITModelView,
                Value = float4x4.Identity
            };

            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.IModelView,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ITView,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.View,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.Projection,
                Value = float4x4.Identity
            };
            yield return new FxParamDeclaration<float4x4[]>
            {
                Name = UniformNameDeclarations.BonesArray,
                Value = new[] { float4x4.Identity }
            };
        }

    }
}

