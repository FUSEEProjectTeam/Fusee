using CommunityToolkit.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A surface effect contains information to build a shader program. This is an abstract base class.
    /// </summary>
    public abstract class SurfaceEffectBase : Effect
    {
        internal readonly List<KeyValuePair<ShardCategory, string>> VertexShaderSrc = new();
        internal readonly List<KeyValuePair<ShardCategory, string>> GeometryShaderSrc = new();
        internal readonly List<KeyValuePair<ShardCategory, string>> FragmentShaderSrc = new();

        /// <summary>
        /// The shader shard containing the shader version.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public string Version;

        /// <summary>
        /// The shader shard containing the definition of PI.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public string Pi = Header.DefinePi;

        /// <summary>
        /// The shader shard containing the float precision.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public string Precision = Header.EsPrecisionHighpFloat;

        #region MUST HAVE fields

        //================== Surface Shard IN ==========================//
        /// <summary>
        /// User-defined input struct. Must derive from <see cref="DiffuseInput"/>.
        /// Used in the <see cref="SurfOutFragMethod"/> to modify the parameters of the chosen <see cref="SurfaceOutput"/>.
        /// </summary>
        public SurfaceEffectInput SurfaceInput { get; set; }
        //======================================================//

        //================== Surface Shard ==========================//
        /// <summary>
        /// Struct declaration in the shader code that provides the values for the position calculation
        /// of the vertex shader and lighting calculation of the fragment shader.
        /// Values of this struct can be modified by the user using <see cref="SurfOutFragMethod"/> (fragment shader).
        /// The value is filled in the constructor using the chosen lighting setup.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.SurfOutStruct)]
        public string SurfaceOutput;

        /// <summary>
        /// Fragment shader "in" declaration of the <see cref="SurfaceOutput"/>.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string SurfVaryingFrag = $"in {SurfaceEffectNameDeclarations.StructTypeName} {VaryingNameDeclarations.SurfOutVaryingName};\n";

        /// <summary>
        /// Vertex shader "out" declaration of the <see cref="SurfaceOutput"/>.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string SurfVaryingVert = $"out {SurfaceEffectNameDeclarations.StructTypeName} {VaryingNameDeclarations.SurfOutVaryingName};\n";

        /// <summary>
        /// Shader Shard Method to modify the <see cref="SurfaceOutput"/>.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.SurfOut)]
        public string SurfOutFragMethod;

        /// <summary>
        /// Shader Shard Method to modify the <see cref="SurfaceOutput"/>.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.SurfOut)]
        public string SurfOutVertMethod;
        //======================================================//
        /// <summary>
        /// Fragment shader "in" declaration for the uv coordinates.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string UvIn = GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);

        /// <summary>
        /// Vertex shader "out" declaration for the uv coordinates.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string UvOut = GLSL.CreateOut(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);

        /// <summary>
        /// Fragment shader "in" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string VertColorIn = GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Color0);

        /// <summary>
        /// Vertex shader "out" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string VertColorOut = GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Color0);

        /// <summary>
        /// Fragment shader "in" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string VertColor1In = GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Color1);

        /// <summary>
        /// Vertex shader "out" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string VertColor1Out = GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Color1);

        /// <summary>
        /// Fragment shader "in" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string VertColor2In = GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Color2);

        /// <summary>
        /// Vertex shader "out" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string VertColor2Out = GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Color2);

        /// <summary>
        /// Fragment shader "in" declaration for the TBN matrix.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Property)]
        public string TBNIn = GLSL.CreateIn(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);

        /// <summary>
        /// Vertex shader "out" declaration for the TBN matrix.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string TBNOut = GLSL.CreateOut(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);

        /// <summary>
        /// The shader shard containing "fu" variables (in and out parameters) like fuVertex, fuNormal etc.
        /// The value is filled in the constructor using the chosen lighting setup.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string VertIn;

        #endregion

        /// <summary>
        /// Creates a new Instance of type SurfaceEffect.
        /// </summary>
        /// <param name="surfaceInput"><see cref="SurfaceInput"/>. Provides the values used to modify the <see cref="SurfaceOut"/>.</param>
        /// <param name="renderStateSet">Optional. If no <see cref="RenderStateSet"/> is given a default one will be added.</param>
        public SurfaceEffectBase(SurfaceEffectInput surfaceInput, RenderStateSet? renderStateSet = null)
        {
            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
            UniformParameters = new Dictionary<int, IFxParamDeclaration>();

            if (ModuleExtensionPoint.PlatformId == FuseePlatformId.Desktop)
                Version = Header.Version460Core;
            else if (ModuleExtensionPoint.PlatformId == FuseePlatformId.Mesa)
                Version = Header.Version450Core;
            else
                Version = Header.Version300Es;
            Pi = Header.DefinePi;
            Precision = Header.EsPrecisionHighpFloat;
            SurfVaryingFrag = $"in {SurfaceEffectNameDeclarations.StructTypeName} {VaryingNameDeclarations.SurfOutVaryingName};\n";
            SurfVaryingVert = $"out {SurfaceEffectNameDeclarations.StructTypeName} {VaryingNameDeclarations.SurfOutVaryingName};\n";
            UvIn = GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);
            UvOut = GLSL.CreateOut(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);
            TBNIn = GLSL.CreateIn(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);
            TBNOut = GLSL.CreateOut(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);

            VertIn = ShaderShards.Vertex.VertProperties.InParams(surfaceInput.TextureSetup);

            SurfaceInput = surfaceInput;
            SurfaceInput.PropertyChanged += (object? sender, SurfaceEffectEventArgs args) => PropertyChangedHandler(sender, args, nameof(SurfaceInput));

            var surfInType = surfaceInput.GetType();
            var surfInName = nameof(SurfaceInput);
            HandleStruct(ShaderCategory.Fragment, surfInType);

            foreach (var structProp in surfInType.GetProperties())
            {
                var paramDcl = BuildFxParamDecl(structProp, GetType().GetProperty(surfInName));
                UniformParameters.Add(paramDcl.Hash, paramDcl);
            }

            HandleUniform(ShaderCategory.Fragment, nameof(SurfaceInput), surfInType);

            var lightingShards = SurfaceOut.GetShadingModelShards(surfaceInput.ShadingModel);
            SurfaceOutput = lightingShards.StructDecl;

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
        }

        /// <summary>
        /// Reads all Fields an Properties from this type and builds <see cref="IFxParamDeclaration"/>s and shader code snippets from them.
        /// </summary>
        protected void HandleFieldsAndProps()
        {
            Type t = GetType();

            FxShaderAttribute? shaderAttribute;
            FxShardAttribute? shardAttribute;

            var publicProps = GetPublicProperties(t);
            foreach (var prop in publicProps)
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
                    case ShardCategory.InternalUniform:
                    case ShardCategory.Uniform:
                        {
                            var paramDcl = BuildFxParamDecl(prop);
                            UniformParameters.Add(paramDcl.Hash, paramDcl);
                            HandleUniform(shaderAttribute.ShaderCategory, paramDcl.Name, paramDcl.ParamType);
                            continue;
                        }
                    case ShardCategory.Header:
                    case ShardCategory.Main:
                    case ShardCategory.SurfOutStruct:
                    case ShardCategory.Property:
                    case ShardCategory.Method:
                    case ShardCategory.SurfOut:

                        if (prop.PropertyType == typeof(string))
                            HandleShard(shaderAttribute.ShaderCategory, shardAttribute, (string?)prop.GetValue(this));
                        else
                            throw new Exception($"{t.Name} ShaderEffect: Property {prop.Name} does not contain a valid shard.");
                        continue;
                    case ShardCategory.Struct:
                        HandleStruct(shaderAttribute.ShaderCategory, prop.PropertyType);
                        continue;
                    case ShardCategory.Struct | ShardCategory.Uniform:
                        HandleStruct(shaderAttribute.ShaderCategory, prop.PropertyType);
                        foreach (var structProp in prop.PropertyType.GetProperties())
                        {
                            var paramDcl = BuildFxParamDecl(structProp, prop);
                            UniformParameters.Add(paramDcl.Hash, paramDcl);
                        }
                        HandleUniform(shaderAttribute.ShaderCategory, prop.Name, prop.PropertyType);
                        continue;
                    default:
                        throw new ArgumentException($"Unknown shard category: {shardAttribute.ShardCategory}");

                }
            }

            var allFields = GetPublicFields(t);
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
                    case ShardCategory.InternalUniform:
                        {
                            var paramDcl = BuildFxParamDecl(field);
                            UniformParameters.Add(paramDcl.Hash, paramDcl);
                            HandleUniform(shaderAttribute.ShaderCategory, paramDcl.Name, paramDcl.ParamType);
                            continue;
                        }
                    case ShardCategory.Header:
                    case ShardCategory.Main:
                    case ShardCategory.SurfOutStruct:
                    case ShardCategory.Property:
                    case ShardCategory.Method:
                    case ShardCategory.SurfOut:
                        if (field.FieldType == typeof(string))
                        {
                            var val = (string?)field.GetValue(this);
                            if (val == null || val == string.Empty)
                                continue;
                            HandleShard(shaderAttribute.ShaderCategory, shardAttribute, val);
                        }
                        else
                            throw new Exception($"{t.Name} ShaderEffect: Field {field.Name} does not contain a valid shard.");
                        continue;
                    case ShardCategory.Struct:
                        HandleStruct(shaderAttribute.ShaderCategory, field.FieldType);
                        continue;
                    default:
                        break;
                }
            }
        }

        internal static string JoinShards(List<KeyValuePair<ShardCategory, string>> shardList, List<KeyValuePair<ShardCategory, string>>? renderMethodDependentShards = null)
        {
            List<KeyValuePair<ShardCategory, string>> completeList;
            if (renderMethodDependentShards != null)
            {
                completeList = shardList.Concat(renderMethodDependentShards).ToList();
                completeList.Sort((x, y) => x.Key.CompareTo(y.Key));
            }
            else
                completeList = shardList;

            string res = string.Empty;
            foreach (var kvp in completeList)
                res += kvp.Value;

            return res;
        }

        private void HandleShard(ShaderCategory shaderCategory, FxShardAttribute shardAttrib, string? shardCode)
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

        internal void HandleUniform(ShaderCategory shaderCategory, string uniformName, Type type, ShardCategory shardCategory = ShardCategory.Property)
        {
            var uniform = "uniform ";
            switch (shaderCategory)
            {
                case ShaderCategory.Vertex:
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Fragment:
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case ShaderCategory.Geometry:
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Fragment):
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Fragment | ShaderCategory.Geometry):
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    GeometryShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    break;
                case (ShaderCategory.Vertex | ShaderCategory.Geometry | ShaderCategory.Fragment):
                    VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    VertexShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
                    FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                    GeometryShaderSrc.Add(new KeyValuePair<ShardCategory, string>(shardCategory, uniform + GLSL.DecodeType(type) + " " + uniformName + ";\n"));
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

        private IFxParamDeclaration? BuildFxParamDecl(PropertyInfo prop, PropertyInfo? parent = null)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { prop.PropertyType });
            //Cannot use GetConstructor(Type.EmptyTypes) with a struct!!
            //object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            var ob = Activator.CreateInstance(concreteParamDecl);

            //Error because property ParamType has no setter.
            //concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());

            object? val;
            if (parent == null)
            {
                concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name))?.SetValue(ob, prop.Name);
                val = prop.GetValue(this);
            }
            else
            {
                concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name))?.SetValue(ob, parent.Name + "." + prop.Name);
                val = prop.GetValue(parent.GetValue(this));
            }
            concreteParamDecl?.GetField("Value")?.SetValue(ob, val);
            return (IFxParamDeclaration?)ob;
        }

        private IFxParamDeclaration? BuildFxParamDecl(FieldInfo field)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { field.FieldType });
            //Cannot use GetConstructor(Type.EmptyTypes) with a struct!!
            //object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            var ob = Activator.CreateInstance(concreteParamDecl);

            //Error because property ParamType has no setter.
            //concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());

            concreteParamDecl?.GetProperty(nameof(IFxParamDeclaration.Name))?.SetValue(ob, field.Name);
            object? val;

            val = field.GetValue(this);

            concreteParamDecl?.GetField("Value")?.SetValue(ob, val);
            return (IFxParamDeclaration?)ob;
        }

        internal static IEnumerable<IFxParamDeclaration> CreateForwardLightingParamDecls(int numberOfLights)
        {
            for (int i = 0; i < numberOfLights; i++)
            {
                yield return new FxParamDeclaration<float3>()
                {
                    Name = UniformNameDeclarations.GetPosName(i),
                    Value = new float3(0, 0, -1.0f)
                };

                yield return new FxParamDeclaration<float4>()
                {
                    Name = UniformNameDeclarations.GetIntensitiesName(i),
                    Value = float4.Zero
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = UniformNameDeclarations.GetMaxDistName(i),
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = UniformNameDeclarations.GetStrengthName(i),
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = UniformNameDeclarations.GetOuterConeAngleName(i),
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = UniformNameDeclarations.GetInnerConeAngleName(i),
                    Value = 0.0f
                };

                yield return new FxParamDeclaration<float3>()
                {
                    Name = UniformNameDeclarations.GetDirectionName(i),
                    Value = float3.Zero
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = UniformNameDeclarations.GetTypeName(i),
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = UniformNameDeclarations.GetIsActiveName(i),
                    Value = 1
                };

                yield return new FxParamDeclaration<int>()
                {
                    Name = UniformNameDeclarations.GetIsCastingShadowsName(i),
                    Value = 0
                };

                yield return new FxParamDeclaration<float>()
                {
                    Name = UniformNameDeclarations.GetBiasName(i),
                    Value = 0f
                };
            }
        }

        /// <summary>
        /// Event Handler that is called on <see cref="INotifyValueChange{T}.PropertyChanged"/>.
        /// </summary>
        /// <param name="_">sender</param>
        /// <param name="args">The event arguments.</param>
        /// <param name="memberName">The name of the member which this event originated from.</param>
        protected void PropertyChangedHandler(object? _, SurfaceEffectEventArgs args, string memberName)
        {
            SetFxParam(memberName + "." + args.Name, args.Value);
        }

        private PropertyInfo[] GetPublicProperties(Type type)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var t = queue.Dequeue();

                if (t.BaseType == null || t.BaseType == typeof(SceneComponent)) break;
                if (considered.Contains(t.BaseType)) continue;

                considered.Add(t.BaseType);
                queue.Enqueue(t.BaseType);

                var typeProps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                propertyInfos.InsertRange(0, typeProps);
            }

            return propertyInfos.ToArray();
        }

        private FieldInfo[] GetPublicFields(Type type)
        {
            var fieldInfos = new List<FieldInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var t = queue.Dequeue();

                if (t.BaseType == null || t.BaseType == typeof(SceneComponent)) break;
                if (considered.Contains(t.BaseType)) continue;

                considered.Add(t.BaseType);
                queue.Enqueue(t.BaseType);

                var typeFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                fieldInfos.InsertRange(0, typeFields);
            }

            return fieldInfos.ToArray();
        }
    }
}