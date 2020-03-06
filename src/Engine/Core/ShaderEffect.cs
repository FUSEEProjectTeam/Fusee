using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// An effect pass declaration contains the relevant shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public interface IEffectPassDeclarationBase
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
    public struct EffectPassDeclaration : IEffectPassDeclarationBase
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
    public struct EffectPassDeclarationProto : IEffectPassDeclarationBase
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

    /// <summary>
    /// A data type for the list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
    /// Each of this array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
    /// parameter's type.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public struct EffectParameterDeclaration
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Value
        /// </summary>
        public object Value;
    }

    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    /// TODO (mr): Move to Fusee.Engine.Common
    public class ShaderEffect : SceneComponent, IDisposable
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
        protected ShaderEffect() { }

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectPasses">The ordered array of <see cref="EffectPassDeclaration"/> items. The first item
        /// in the array is the first pass applied to rendered geometry, and so on.</param>
        /// <param name="effectParameters">A list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
        /// Each array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
        /// parameter's type.
        /// </param>
        /// <remarks>Make sure to list any parameter in any of the different passes' shaders you want to change later on in the effectParameters
        /// list. Shaders must not contain parameters with names listed in the effectParameters but declared with different types than those of 
        /// the respective default values given here.</remarks>
        public ShaderEffect(EffectPassDeclaration[] effectPasses, IEnumerable<EffectParameterDeclaration> effectParameters)
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
                    ParamDecl.Add(param.Name, param.Value);
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

        /// <summary>
        /// Set effect parameter
        /// </summary>
        /// <param name="name">Name of the uniform variable</param>
        /// <param name="value">Value of the uniform variable</param>
        public void SetEffectParam(string name, object value)
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
        public object GetEffectParam(string name)
        {
            object pa;
            if (ParamDecl.TryGetValue(name, out pa))
            {
                return pa;
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
        public void GetEffectParam(string name, out object obj)
        {
            obj = null;
            if (ParamDecl.TryGetValue(name, out object pa))
            {
                obj = pa;
            }

        }

        // This property returns the number of elements
        // in the inner dictionary.
        /// <summary>
        /// Size of <see cref="ParamDecl"/>
        /// Needed for <see cref="DynamicObject"/>
        /// </summary>
        public int Count => ParamDecl.Count;
    }

    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffectProtoPixel : ShaderEffect
    {
        /// <summary>
        /// The effect props are the basis on which we can decide what kind of shards this effect supports.
        /// </summary>
        public ShaderEffectProps EffectProps { get; set; }

        private readonly EffectPassDeclarationProto[] _effectPasses;

        /// <summary>
        /// Creates a new instance of type ShaderEffectProtoPixel.
        /// </summary>
        /// <param name="effectPasses"></param>
        /// <param name="effectParameters"></param>
        public ShaderEffectProtoPixel(EffectPassDeclarationProto[] effectPasses, IEnumerable<EffectParameterDeclaration> effectParameters)
        {
            if (effectPasses == null || effectPasses.Length == 0)
                throw new ArgumentNullException(nameof(effectPasses), "must not be null and must contain at least one pass");

            var nPasses = effectPasses.Length;

            States = new RenderStateSet[nPasses];

            VertexShaderSrc = new string[nPasses];
            PixelShaderSrc = new string[nPasses];
            GeometryShaderSrc = new string[nPasses];

            ParamDecl = new Dictionary<string, object>();

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    ParamDecl.Add(param.Name, param.Value);
                }
            }

            _effectPasses = effectPasses;

            for (int i = 0; i < nPasses; i++)
            {
                States[i] = effectPasses[i].StateSet;
                VertexShaderSrc[i] = effectPasses[i].VS;
                GeometryShaderSrc[i] = effectPasses[i].GS;
                //PixelShaderSrc is not set here because it gets built in a pre-pass, depending on whether we render deferred or forward.
            }

            EffectEventArgs = new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.UNCHANGED);
        }

        /// <summary>
        ///Called by the SceneVisitor in the pre-pass to create the correct fragment shader, whether we render forward or deferred.
        /// </summary>
        public void CreateFragmentShader(bool doRenderForward)
        {
            if (doRenderForward)
            {
                for (int i = 0; i < _effectPasses.Length; i++)
                {
                    var pxBody = new List<string>()
                    {
                        LightingShard.LightStructDeclaration,
                        FragPropertiesShard.FixedNumberLightArray,
                        FragPropertiesShard.ColorOut(),
                        LightingShard.AssembleLightingMethods(EffectProps),
                        FragMainShard.ForwardLighting(EffectProps)
                    };

                    PixelShaderSrc[i] = _effectPasses[i].ProtoPS + string.Join("\n", pxBody);
                }
            }
            else
            {
                for (int i = 0; i < _effectPasses.Length; i++)
                {
                    var pxBody = new List<string>()
                    {
                        FragPropertiesShard.GBufferOut(),
                        FragMainShard.RenderToGBuffer(EffectProps)
                    };
                    PixelShaderSrc[i] = _effectPasses[i].ProtoPS + string.Join("\n", pxBody);

                    States[i].AlphaBlendEnable = false;
                    States[i].ZEnable = true;
                }
            }
        }
    }

    internal class ShaderEffectEventArgs : EventArgs
    {
        internal ShaderEffect Effect { get; }
        internal ShaderEffectChangedEnum Changed { get; set; }
        internal EffectParam EffectParameter { get; }
        internal string ChangedEffectVarName { get; set; }
        internal object ChangedEffectVarValue { get; set; }

        internal ShaderEffectEventArgs(ShaderEffect effect, ShaderEffectChangedEnum changed, string changedName = null, object changedValue = null)
        {
            Effect = effect;
            Changed = changed;

            if (changedName == null || changedValue == null) return;

            ChangedEffectVarName = changedName;
            ChangedEffectVarValue = changedValue;
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
