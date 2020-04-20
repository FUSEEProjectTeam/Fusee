using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A surface effect contains information to build a shader program 
    /// </summary>
    public abstract class SurfaceEffect : Effect
    {
        internal readonly List<KeyValuePair<ShardCategory, string>> VertexShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        internal readonly List<KeyValuePair<ShardCategory, string>> GeometryShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        internal readonly List<KeyValuePair<ShardCategory, string>> FragmentShaderSrc = new List<KeyValuePair<ShardCategory, string>>();

        /// <summary>
        /// The shader shard containing the shader version.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public static string Version = Header.Version300Es;

        /// <summary>
        /// The shader shard containing the float precision.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public static string Precision = Header.EsPrecisionHighpFloat;

        #region MUST HAVE for the fragment shader surface shard

        //================== Shard IN ==========================//
        public LightingSetup LightingSetup;

        //[FxShader(ShaderCategory.Fragment)]                     // => Adds shader code to the fragment shader only.
        //[FxShard(ShardCategory.Struct | ShardCategory.Uniform)] // => will crate the struct at the appropriate place in the shader.
        public ColorInput SurfaceInput { get; set; }
        //======================================================//

        //================== Shard OUT ==========================//
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public static string SurfaceOutput = string.Empty;

        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Method)]
        public static string SurfOutMethod = string.Empty;

        public static List<string> SurfOutMethodBody;
        //======================================================//
        #endregion

        /// <summary>
        /// Creates a new Instance of type SurfaceEffect.
        /// </summary>
        /// <param name="renderStateSet">Optional. If no <see cref="RenderStateSet"/> is given a default one will be added.</param>
        public SurfaceEffect(LightingSetup lightingSetup, ColorInput surfaceInput, RenderStateSet renderStateSet = null)
        {
            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            LightingSetup = lightingSetup;
            var lightingShards = ShaderSurfaceOut.GetLightingSetupShards(LightingSetup);

            SurfaceInput = surfaceInput;
            SurfaceInput.PropertyChanged += (object sender, SurfaceEffectEventArgs args) => PropertyChangedHandler(sender, args, nameof(SurfaceInput));

            var surfInType = surfaceInput.GetType();
            var surfInName = nameof(SurfaceInput);
            HandleStruct(ShaderCategory.Fragment, surfInType);
            foreach (var structProp in surfInType.GetProperties())
            {
                var paramDcl = BuildFxParamDecl(structProp, GetType().GetProperty(surfInName));
                ParamDecl.Add(paramDcl.Name, paramDcl);
            }
            HandleUniform(ShaderCategory.Fragment, nameof(SurfaceInput), surfInType);

            SurfaceOutput = lightingShards.StructDecl;

            SurfOutMethodBody = new List<string>()
            {
                $"{lightingShards.Name} OUT = {lightingShards.DefaultInstance};",
                "return OUT;"
            };

            if (renderStateSet == null)
            {
                RendererStates = new RenderStateSet
                {
                    ZEnable = true,
                    AlphaBlendEnable = true,
                    SourceBlend = Blend.SourceAlpha,
                    DestinationBlend = Blend.InverseSourceAlpha,
                    BlendOperation = BlendOperation.Add,
                };
            }

            //TODO: ParamDecls for FUSEE_Matrices -  they should not be set by ShaderEffect.FUSEE_MVP = new float4x4() because they are managed under the hood in most cases.
            foreach (var dcl in CreateMatParamDecls())
                ParamDecl.Add(dcl.Name, dcl);
        }

        protected void HandleFieldsAndProps()
        {
            Type t = GetType();

            FxShaderAttribute shaderAttribute;
            FxShardAttribute shardAttribute;

            foreach (var prop in t.GetProperties().ToList())
            {
                var attribs = prop.GetCustomAttributes().ToList();

                if (attribs.Count == 0)
                    continue;

                shaderAttribute = null;
                shardAttribute = null;

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

                if (shaderAttribute == null)
                    throw new ArgumentException("Property has no ShaderAttribute!");

                if (shardAttribute == null)
                    throw new ArgumentException("Property has no ShardAttribute!");

                switch (shardAttribute.ShardCategory)
                {
                    case ShardCategory.Uniform:
                        {
                            var paramDcl = BuildFxParamDecl(prop);
                            ParamDecl.Add(paramDcl.Name, paramDcl);
                            HandleUniform(shaderAttribute.ShaderCategory, paramDcl.Name, paramDcl.ParamType);
                            continue;
                        }
                    case ShardCategory.Header:
                    case ShardCategory.Main:
                    case ShardCategory.Property:
                    case ShardCategory.Method:
                        if (prop.GetAccessors(false).Any(x => x.IsStatic) && prop.PropertyType == typeof(string))
                            HandleShard(shaderAttribute.ShaderCategory, shardAttribute, (string)prop.GetValue(this));
                        else
                            throw new Exception($"{t.Name} ShaderEffect: Property {prop.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                        continue;
                    case ShardCategory.Struct:
                        HandleStruct(shaderAttribute.ShaderCategory, prop.PropertyType);
                        continue;
                    case ShardCategory.Struct | ShardCategory.Uniform:
                        HandleStruct(shaderAttribute.ShaderCategory, prop.PropertyType);
                        foreach (var structProp in prop.PropertyType.GetProperties())
                        {
                            var paramDcl = BuildFxParamDecl(structProp, prop);
                            ParamDecl.Add(paramDcl.Name, paramDcl);
                        }
                        HandleUniform(shaderAttribute.ShaderCategory, prop.Name, prop.PropertyType);
                        continue;
                    default:
                        break;
                }
            }

            var allFields = t.BaseType.GetFields().ToList();
            allFields.AddRange(t.GetFields());
            foreach (var field in allFields)
            {
                shaderAttribute = null;
                shardAttribute = null;

                var attribs = field.GetCustomAttributes().ToList();

                if (attribs.Count == 0)
                    continue;

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
                if (shaderAttribute == null)
                    throw new ArgumentException("Field has no ShaderAttribute!");

                if (shardAttribute == null)
                    throw new ArgumentException("Field has no ShardAttribute!");

                switch (shardAttribute.ShardCategory)
                {
                    case ShardCategory.Uniform:
                        throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} must be a Property that calls SetFxParam in the setter.");
                    case ShardCategory.Header:
                    case ShardCategory.Main:
                    case ShardCategory.Property:
                    case ShardCategory.Method:
                        if (field.IsStatic && field.FieldType == typeof(string))
                            HandleShard(shaderAttribute.ShaderCategory, shardAttribute, (string)field.GetValue(this));
                        else
                            throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                        continue;
                    case ShardCategory.Struct:
                        HandleStruct(shaderAttribute.ShaderCategory, field.FieldType);
                        continue;
                    default:
                        break;
                }

            }
        }

        internal static string JoinShards(List<KeyValuePair<ShardCategory, string>> shardList)
        {
            string res = string.Empty;
            foreach (var kvp in shardList)
                res += kvp.Value;

            return res;
        }

        private void HandleShard(ShaderCategory shaderCategory, FxShardAttribute shardAttrib, string shardCode)
        {
            switch (shaderCategory)
            {
                case ShaderCategory.Vertex:
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Fragment | ShaderCategory.Geometry):
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                default:
                    break;
            }
        }

        private void HandleUniform(ShaderCategory shaderCategory, string uniformName, Type type)
        {
            var uniform = "uniform ";
            switch (shaderCategory)
            {
                case ShaderCategory.Vertex:
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Fragment):
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Fragment | ShaderCategory.Geometry):
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;

                default:
                    break;
            }
        }

        private void HandleStruct(ShaderCategory shaderCategory, Type type)
        {
            var glslStruct = GLSL.DecodeSystemStructOrClass(type);
            switch (shaderCategory)
            {
                case ShaderCategory.Vertex:
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Fragment | ShaderCategory.Geometry):
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, glslStruct));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;

                default:
                    break;
            }
        }

        private IFxParamDeclaration BuildFxParamDecl(PropertyInfo prop, PropertyInfo parent = null)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { prop.PropertyType });
            //Cannot use GetConstructor(Type.EmptyTypes) with a struct!!
            //object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            var ob = Activator.CreateInstance(concreteParamDecl);

            //Error because property ParamType has no setter.
            //concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());

            concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, parent.Name + "." + prop.Name);
            object val;
            if (parent == null)
                val = prop.GetValue(this);
            else
                val = prop.GetValue(parent.GetValue(this));
            concreteParamDecl.GetField("Value").SetValue(ob, val);
            return (IFxParamDeclaration)ob;
        }

        internal static IEnumerable<IFxParamDeclaration> CreateForwardLightingParamDecls(int numberOfLights)
        {
            for (int i = 0; i < numberOfLights; i++)
            {
                if (!ShaderShards.Fragment.Lighting.LightPararamStringsAllLights.ContainsKey(i))
                {
                    ShaderShards.Fragment.Lighting.LightPararamStringsAllLights.Add(i, new ShaderShards.Fragment.LightParamStrings(i));
                }

                yield return new FxParamDeclaration<float3>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].PositionViewSpace,
                    Value = new float3(0, 0, -1.0f)
                };

                yield return new FxParamDeclaration<float4>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].Intensities,
                    Value = float4.Zero
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].MaxDistance,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].Strength,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].OuterAngle,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].InnerAngle,
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float3>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].Direction,
                    Value = float3.Zero
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].LightType,
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].IsActive,
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].IsCastingShadows,
                    Value = 0
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = ShaderShards.Fragment.Lighting.LightPararamStringsAllLights[i].Bias,
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

        /// <summary>
        /// Event Handler that is called on <see cref="INotifyInputChange.PropertyChanged"/>.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">The event arguments.</param>
        /// <param name="memberName">The name of the member which this event originated from.</param>
        protected void PropertyChangedHandler(object sender, SurfaceEffectEventArgs args, string memberName)
        {
            GetType().GetMethod("SetFxParam")
            .MakeGenericMethod(args.Type)
            .Invoke(this, new object[] { memberName + "." + args.Name, args.Value });
        }
    }
}

