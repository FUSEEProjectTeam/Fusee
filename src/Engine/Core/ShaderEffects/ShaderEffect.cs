using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fusee.Base.Core;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Serialization;

namespace Fusee.Engine.Core.ShaderEffects
{
    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public abstract class ShaderEffect : IDisposable
    {
        public readonly Dictionary<string, PropertyInfo> Uniforms;

        /// <summary>
        /// List of <see cref="RenderStateSet"/>
        /// </summary>
        public RenderStateSet State { get; protected set; }

        /// <summary>
        /// Vertex shaders of all passes
        /// </summary>
        public string VertexShaderSrc { get; protected set; }

        /// <summary>
        /// Pixel- or fragment shader of all passes
        /// </summary>
        public string FragmentShaderSrc { get; protected set; }

        /// <summary>
        /// Geometry of all passes
        /// </summary>
        public string GeometryShaderSrc { get; protected set; }

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

        private readonly List<KeyValuePair<ShardCategory, string>> _vertexShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        private readonly List<KeyValuePair<ShardCategory, string>> _geometryShaderSrc = new List<KeyValuePair<ShardCategory, string>>();
        private readonly List<KeyValuePair<ShardCategory, string>> _fragmentShaderSrc = new List<KeyValuePair<ShardCategory, string>>();

        private readonly HashSet<Type> _builtStructs;

        /// <summary>
        /// The default (nullary) constructor to create a shader effect.
        /// Will use Reflections to generate the shader code from the Properties and fields.
        /// </summary>
        protected ShaderEffect()
        {
            Uniforms = new Dictionary<string, PropertyInfo>();
            _builtStructs = new HashSet<Type>();

            EffectEventArgs = new ShaderEffectEventArgs(this, ChangedEnum.UNCHANGED);

            State = RenderStateSet.Default;

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
                    if (shardAttribute.ShardCategory == ShardCategory.Struct)
                    {
                        //Build struct shard (only once)
                        if (!_builtStructs.Contains(prop.ReflectedType))
                            BuildStruct(paramAttribute, prop.ReflectedType);
                        continue;
                    }
                    else
                    {
                        Uniforms.Add(prop.Name, prop);
                        BuildUniform(paramAttribute, prop.Name, prop.ReflectedType);
                        continue;
                    }
                }
                else if (shaderAttribute != null && shaderAttribute != null)
                {
                    if (prop.GetAccessors(false).Any(x => x.IsStatic) && prop.PropertyType == typeof(string))
                        BuildShard(shaderAttribute, shardAttribute, (string)prop.GetValue(this));
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
                        BuildShard(shaderAttribute, shardAttribute, (string)field.GetValue(this));
                    else
                        throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                }
            }
            
            VertexShaderSrc = JoinShards(_vertexShaderSrc);
            FragmentShaderSrc = JoinShards(_fragmentShaderSrc);           
            GeometryShaderSrc = JoinShards(_geometryShaderSrc);
        }

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        internal void SetFxParam<T>(string name, T value)
        {
            if (Uniforms.TryGetValue(name, out PropertyInfo prop))
            {
                EffectEventArgs.Changed = ChangedEnum.UNIFORM_VAR_UPDATED;
                EffectEventArgs.ChangedEffectVarName = name;
                EffectEventArgs.ChangedEffectVarValue = value;

                ShaderEffectChanged?.Invoke(this, EffectEventArgs);

                prop.SetValue(this, value);
            }
            else
                Diagnostics.Warn("Trying to set unknown parameter! Ignoring change....");
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        internal T GetFxParam<T>(string name)
        {
            if (Uniforms.TryGetValue(name, out var prop))
                return (T)prop.GetValue(this);

            return default;
        }

        private void AddToShaderSrcCodeList(ShaderCategory shaderCategory, KeyValuePair<ShardCategory, string> kvp)
        {
            switch (shaderCategory)
            {
                case ShaderCategory.Vertex:
                    _vertexShaderSrc.Add(kvp);
                    _vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    _fragmentShaderSrc.Add(kvp);
                    _fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    _geometryShaderSrc.Add(kvp);
                    _geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Pixel:
                    _vertexShaderSrc.Add(kvp);
                    _vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    _fragmentShaderSrc.Add(kvp);
                    _fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry:
                    _vertexShaderSrc.Add(kvp);
                    _vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    _geometryShaderSrc.Add(kvp);
                    _geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry_Pixel:
                    _geometryShaderSrc.Add(kvp);
                    _geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    _fragmentShaderSrc.Add(kvp);
                    _fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Vertex_Geometry_Pixel:
                    _vertexShaderSrc.Add(kvp);
                    _vertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    _geometryShaderSrc.Add(kvp);
                    _geometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    _fragmentShaderSrc.Add(kvp);
                    _fragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                default:
                    break;
            }
        }

        private void BuildShard(FxShaderAttribute shaderAttrib, FxShardAttribute shardAttrib, string shardCode)
        {
            AddToShaderSrcCodeList(shaderAttrib.ShaderCategory, new KeyValuePair<ShardCategory, string>(shardAttrib.ShardCategory, shardCode));
        }

        private void BuildUniform(FxParamAttribute attrib, string uniformName, Type type)
        {
            var uniform = "uniform ";
            AddToShaderSrcCodeList(attrib.ShaderCategory, new KeyValuePair<ShardCategory, string>(ShardCategory.Property, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
        }

        private void BuildStruct(FxParamAttribute paramAttrib, Type type)
        {
            var srcCodeList = new List<string>
            {
                $"struct {type.Name}",
                "{"
            };

            foreach (var prop in type.GetProperties())
                srcCodeList.Add(GLSL.DecodeType(prop.ReflectedType) + " " + prop.Name);
            foreach (var field in type.GetFields())
                srcCodeList.Add(GLSL.DecodeType(field.ReflectedType) + " " + field.Name);

            srcCodeList.Add("}");

            AddToShaderSrcCodeList(paramAttrib.ShaderCategory, new KeyValuePair<ShardCategory, string>(ShardCategory.Struct, string.Join("\n", srcCodeList)));
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
