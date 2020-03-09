using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fusee.Base.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class FxParamAttribute : Attribute
    {
        ShaderCategory _usedInShader;

        public FxParamAttribute()
        {
            // Default - used in all categories
            _usedInShader = 0;
            foreach (var cat in Enum.GetValues(typeof(ShaderCategory)).Cast<ShaderCategory>())
                _usedInShader |= cat;
        }
        public FxParamAttribute(ShaderCategory usedInShards)
        {
            _usedInShader = usedInShards;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class FxShardAttribute : System.Attribute
    {
        ShaderCategory _shaderCategory;

        public FxShardAttribute(ShaderCategory category)
        {
            _shaderCategory = category;
        }
    }

    [Flags]
    public enum ShaderCategory : ushort
    {
        Vertex = 1,
        Pixel = 2,
        Geometry = 3,
    }
    
    /// <summary>
    /// An effect pass declaration contains the relevant shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public interface IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        RenderStateSet StateSet { get; set; }
        /// <summary>
        /// Vertex shader as string
        /// </summary>
        /// 
        string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        string GS { get; set; }

    }

    /// <summary>
    /// An effect pass declaration contains the vertex, pixel and geometry shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct FxPassDeclaration : IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        public RenderStateSet StateSet { get; set; }
        /// <summary>
        /// Vertex shader as string
        /// </summary>
        // ReSharper disable InconsistentNaming
        public string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        public string GS { get; set; }

        /// <summary>
        /// Pixel- or fragment shader as string
        /// </summary>
        public string PS { get; set; }
    }

    /// <summary>
    /// A "proto" effect pass declaration contains the vertex and geometry shader source code as well as a partial  Fragment Shader, that is completed in a pre-pass, depending whether we render forward or deferred.
    /// It also contains a <see cref="RenderStateSet"/> declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct FxPassDeclarationProto : IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        public RenderStateSet StateSet { get; set; }
        /// <summary>
        /// Vertex shader as string
        /// </summary>
        // ReSharper disable InconsistentNaming
        public string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        public string GS { get; set; }

        /// <summary>
        /// Partial Fragment Shader, that is completed in a pre-pass, depending whether we render forward or deferred.
        /// </summary>
        public string ProtoPS { get; set; }
    }


    public interface IFxParamDeclaration
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The Type of the parameter.
        /// </summary>
        Type ParamType { get; }

        /// <summary>
        /// Defines in which type of shader this param is used in.
        /// </summary>
        IEnumerable<ShaderCategory> UsedInShaders { get; }
    }

    /// <summary>
    /// A data type for the list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
    /// Each of this array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
    /// parameter's type.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public struct FxParamDeclaration<T> : IFxParamDeclaration
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public T Value;

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type ParamType => typeof(T);

        IEnumerable<ShaderCategory> IFxParamDeclaration.UsedInShaders => UsedInShaders;

        IEnumerable<ShaderCategory> UsedInShaders;
    }

    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect : IDisposable
    {
        /// <summary>
        /// The ShaderEffect'S uniform parameters and their values.
        /// </summary>
        public Dictionary<string, object> ParamDecl { get; protected set; }

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

        /// <summary>
        /// The default (nullary) constructor to create a shader effect.
        /// </summary>
        protected ShaderEffect()
        {
            List<FxPassDeclaration> effectPassDecl = new List<FxPassDeclaration>();
            List<IFxParamDeclaration> effectParameters = new List<IFxParamDeclaration>();
            Type t = GetType();
            foreach (var prop in t.GetProperties())
            {
                var attribs = prop.GetCustomAttributes();
                foreach (var attrib in attribs)
                {
                    switch (attrib)
                    {
                        case FxParamAttribute paramAttrib:
                            effectParameters.Add(BuildFxParamDecl(prop));
                            break;
                        case FxShardAttribute shardAttribute:
                            if (prop.GetAccessors(false).Any(x => x.IsStatic) && prop.PropertyType == typeof(string))
                                HandleShard(shardAttribute, (string)prop.GetValue(this));
                            else
                                throw new Exception($"{t.Name} ShaderEffect: Property {prop.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                            break;
                    }
                }
            }
            foreach (var field in t.GetFields())
            {
                var attribs = field.GetCustomAttributes();
                foreach (var attrib in attribs)
                {
                    switch (attrib)
                    {
                        case FxShardAttribute shardAttribute:
                            if (field.IsStatic && field.FieldType == typeof(string))
                                HandleShard(shardAttribute, (string)field.GetValue(this));
                            else
                                throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} does not contain a valid shard. Either the property is not static or it's not a string.");
                            break;
                    }
                }
            }
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

            ParamDecl = new Dictionary<string, object>();

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
                    var val = param.GetType().GetField("Value").GetValue(param);
                    ParamDecl.Add(param.Name, val);
                }
            }

            EffectEventArgs = new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.UNCHANGED);
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
            ShaderEffectChanged?.Invoke(this, new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.DISPOSE));
        }

        private void HandleShard(FxShardAttribute attrib, string shardCode)
        {
            throw new NotImplementedException();
        }

        private IFxParamDeclaration BuildFxParamDecl(PropertyInfo prop)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { prop.GetType() });
            object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());
            concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, prop.Name);
            concreteParamDecl.GetProperty("Value").SetValue(ob, prop.GetValue(this));
            return (IFxParamDeclaration)ob;
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

                    ParamDecl[name] = value;
                   
                    EffectEventArgs.Changed = ShaderEffectChangedEnum.UNIFORM_VAR_UPDATED;
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
        /// <remarks>THIS IS NOT THE ACTUAL UNIFORM VALUE</remarks>
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <returns></returns>
        public object GetFxParam(string name)
        {
            if (ParamDecl.TryGetValue(name, out var val))
            {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Returns the value of a given shader effect variable
        /// <remarks>THIS IS NOT THE ACTUAL UNIFORM VALUE</remarks>
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// /// <param name="obj">The value. Return null if no parameter was found.</param>
        /// <returns></returns>
        public void GetFxParam(string name, out object obj)
        {
            obj = default;
            if (ParamDecl.TryGetValue(name, out var val))
            {
                obj = val;
            }
        }        
    }

    internal enum ShaderEffectChangedEnum
    {
        DISPOSE = 0,
        UNIFORM_VAR_UPDATED = 1,

        //Not needed at the moment, because a ShaderEffect must declare all it's parameter declarations at creation.
        //UNIFORM_VAR_ADDED = 2,

        UNCHANGED = 3
    }
}
