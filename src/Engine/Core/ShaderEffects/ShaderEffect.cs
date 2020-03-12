using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core.ShaderEffects
{
    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect : IDisposable
    {
        /// <summary>
        /// The ShaderEffect'S uniform parameters and their values.
        /// </summary>
        public Dictionary<string, IFxParamDeclaration> ParamDecl { get; protected set; }

        /// <summary>
        /// List of <see cref="RenderStateSet"/>
        /// </summary>
        public RenderStateSet[] States { get; protected set; }

        /// <summary>
        /// Vertex shaders of all passes
        /// </summary>
        public string[] VertexShaderSrc { get; protected set; }

        /// <summary>
        /// Pixel- or fragment shader of all passes
        /// </summary>
        public string[] PixelShaderSrc { get; protected set; }

        /// <summary>
        /// Geometry of all passes
        /// </summary>
        public string[] GeometryShaderSrc { get; protected set; }

        // Event ShaderEffect changes
        /// <summary>
        /// ShaderEffect event notifies observing ShaderEffectManager about property changes and the ShaderEffects's disposal.
        /// </summary>
        internal event EventHandler<ShaderEffectEventArgs> ShaderEffectChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        internal readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();

        internal ShaderEffectEventArgs EffectEventArgs;

        //TODO: clear if pass is ready
        private List<KeyValuePair<ShardCategory, string>> vertexShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        private List<KeyValuePair<ShardCategory, string>> geometryShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        private List<KeyValuePair<ShardCategory, string>> fragmentShaderSrc = new List<KeyValuePair<ShardCategory, string>>();


        /// <summary>
        /// The default (nullary) constructor to create a shader effect.
        /// Will use Reflections to generate the shader code from the Properties and fields.
        /// </summary>
        protected ShaderEffect()
        {
            EffectEventArgs = new ShaderEffectEventArgs(this, ChangedEnum.UNCHANGED);

            List<FxPassDeclaration> effectPassDecl = new List<FxPassDeclaration>();
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            //TODO: implement passes and States 
            States = new RenderStateSet[1];
            States[0] = new RenderStateSet
            {
                ZEnable = true,
                AlphaBlendEnable = true,
                SourceBlend = Blend.SourceAlpha,
                DestinationBlend = Blend.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
            };

            VertexShaderSrc = new string[1];
            PixelShaderSrc = new string[1];
            GeometryShaderSrc = new string[1];

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

            if (vertexShaderSrc.Count > 0)
                VertexShaderSrc[0] = JoinShards(vertexShaderSrc);
            if (fragmentShaderSrc.Count > 0)
                PixelShaderSrc[0] = JoinShards(fragmentShaderSrc);
            if (geometryShaderSrc.Count > 0)
                GeometryShaderSrc[0] = JoinShards(geometryShaderSrc);
        }

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectPasses">The ordered array of <see cref="FxPassDeclaration"/> items. The first item
        /// in the array is the first pass applied to rendered geometry, and so on.</param>
        /// <param name="effectParameters">A list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
        /// Each array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
        /// parameter's type.
        /// </param>
        /// <remarks>Make sure to list any parameter in any of the different passes' shaders you want to change later on in the effectParameters
        /// list. Shaders must not contain parameters with names listed in the effectParameters but declared with different types than those of 
        /// the respective default values given here.</remarks>
        public ShaderEffect(FxPassDeclaration[] effectPasses, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            if (effectPasses == null || effectPasses.Length == 0)
                throw new ArgumentNullException(nameof(effectPasses), "must not be null and must contain at least one pass");

            var nPasses = effectPasses.Length;

            States = new RenderStateSet[nPasses];

            VertexShaderSrc = new string[nPasses];
            PixelShaderSrc = new string[nPasses];
            GeometryShaderSrc = new string[nPasses];

            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            for (int i = 0; i < effectPasses.Length; i++)
            {
                States[i] = effectPasses[i].StateSet;
                VertexShaderSrc[i] = effectPasses[i].VS;
                PixelShaderSrc[i] = effectPasses[i].PS;
                GeometryShaderSrc[i] = effectPasses[i].GS;
            }

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    //var val = param.GetType().GetField("Value").GetValue(param);
                    ParamDecl.Add(param.Name, param);
                }
            }

            EffectEventArgs = new ShaderEffectEventArgs(this, ChangedEnum.UNCHANGED);
        }
        
        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetFxParam<T>(string name, T value)
        {
            if (ParamDecl != null)
            {
                if (ParamDecl.ContainsKey(name))
                {
                    if (ParamDecl[name] != null)
                        if (ParamDecl[name].Equals(value)) return;

                    //Implemented using reflections and not "(FxParamDeclaration<T>)ParamDecl[name]" because 
                    //we get a InvalidCast exception when coming from the RC (Render(Mesh)) and T is of type "object" but ParamDecl[name] "T" isn't.                    
                    ParamDecl[name].GetType().GetField("Value").SetValue(ParamDecl[name], value);

                    EffectEventArgs.Changed = ChangedEnum.UNIFORM_VAR_UPDATED;
                    EffectEventArgs.ChangedEffectVarName = name;
                    EffectEventArgs.ChangedEffectVarValue = value;

                    ShaderEffectChanged?.Invoke(this, EffectEventArgs);
                }
                else
                {
                    Diagnostics.Warn("Trying to set unknown parameter! Ignoring change....");
                }
            }
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        public T GetFxParam<T>(string name)
        {
            if (ParamDecl.TryGetValue(name, out var dcl))
            {
                return ((FxParamDeclaration<T>)dcl).Value;
            }
            return default;
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// /// <param name="obj">The value. Return null if no parameter was found.</param>
        /// <returns></returns>
        public void GetFxParam<T>(string name, out T obj)
        {
            obj = default;
            if (ParamDecl.TryGetValue(name, out var dcl))
                obj = ((FxParamDeclaration<T>)dcl).Value;
        }

        protected static IEnumerable<IFxParamDeclaration> CreateForwardLightingParamDecls(int numberOfLights)
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

        protected static IEnumerable<IFxParamDeclaration> CreateMatParamDecls()
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

        private string JoinShards(List<KeyValuePair<ShardCategory, string>> shardList)
        {
            string res = string.Empty;
            foreach (var kvp in shardList)
                res += kvp.Value;

            return res;
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire MeshChanged event.
        /// </summary>
        ~ShaderEffect()
        {
            Dispose();
        }

        /// <summary>
        /// Is called when GC of this shader effect kicks in
        /// </summary>
        public void Dispose()
        {
            ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ChangedEnum.DISPOSE));
        }
    }
}
