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
    public abstract class SurfaceEffectBase : Effect, IDisposable
    {
        private bool _disposed;
        internal readonly List<KeyValuePair<ShardCategory, string>> VertexShaderSrc = new();
        internal readonly List<KeyValuePair<ShardCategory, string>> GeometryShaderSrc = new();
        internal readonly List<KeyValuePair<ShardCategory, string>> FragmentShaderSrc = new();

        /// <summary>
        /// The shader shard containing the shader version.
        /// </summary>
        [FxShader(ShaderCategory.Vertex | ShaderCategory.Fragment)]
        [FxShard(ShardCategory.Header)]
        public string Version = Header.Version300Es;

        /// <summary>
        /// The shader shard containing the definition of PI.
        /// </summary>
        [FxShader(ShaderCategory.Fragment)]
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
        public SurfaceInput SurfaceInput { get; set; }
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
        public string SurfVaryingFrag = $"in {SurfaceOut.StructName} {SurfaceOut.SurfOutVaryingName};\n";

        /// <summary>
        /// Vertex shader "out" declaration of the <see cref="SurfaceOutput"/>.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string SurfVaryingVert = $"out {SurfaceOut.StructName} {SurfaceOut.SurfOutVaryingName};\n";

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
        public string VertColorIn = GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.Color);

        /// <summary>
        /// Vertex shader "out" declaration for the vertex colors.
        /// </summary>
        [FxShader(ShaderCategory.Vertex)]
        [FxShard(ShardCategory.Property)]
        public string VertColorOut = GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.Color);

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
        public SurfaceEffectBase
            (SurfaceInput surfaceInput, RenderStateSet renderStateSet = null)
        {
            EffectManagerEventArgs = new EffectManagerEventArgs(UniformChangedEnum.Unchanged);
            ParamDecl = new Dictionary<int, IFxParamDeclaration>();

            Version = Header.Version300Es;
            Pi = Header.DefinePi;
            Precision = Header.EsPrecisionHighpFloat;
            SurfVaryingFrag = $"in {SurfaceOut.StructName} {SurfaceOut.SurfOutVaryingName};\n";
            SurfVaryingVert = $"out {SurfaceOut.StructName} {SurfaceOut.SurfOutVaryingName};\n";
            UvIn = GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);
            UvOut = GLSL.CreateOut(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates);
            TBNIn = GLSL.CreateIn(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);
            TBNOut = GLSL.CreateOut(GLSL.Type.Mat3, VaryingNameDeclarations.TBN);
            
            VertIn = ShaderShards.Vertex.VertProperties.InParams(surfaceInput.TextureSetup);

            SurfaceInput = surfaceInput;
            SurfaceInput.PropertyChanged += (object sender, SurfaceEffectEventArgs args) => PropertyChangedHandler(sender, args, nameof(SurfaceInput));

            var surfInType = surfaceInput.GetType();
            var surfInName = nameof(SurfaceInput);
            HandleStruct(ShaderCategory.Fragment, surfInType);

            foreach (var structProp in surfInType.GetProperties())
            {
                var paramDcl = BuildFxParamDecl(structProp, GetType().GetProperty(surfInName));
                ParamDecl.Add(paramDcl.Hash, paramDcl);
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

            FxShaderAttribute shaderAttribute;
            FxShardAttribute shardAttribute;

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
                            ParamDecl.Add(paramDcl.Hash, paramDcl);
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
                            HandleShard(shaderAttribute.ShaderCategory, shardAttribute, (string)prop.GetValue(this));
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
                            ParamDecl.Add(paramDcl.Hash, paramDcl);
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
                            ParamDecl.Add(paramDcl.Hash, paramDcl);
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
                            var val = (string)field.GetValue(this);
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

        internal static string JoinShards(List<KeyValuePair<ShardCategory, string>> shardList, List<KeyValuePair<ShardCategory, string>> renderMathodDependentShards = null)
        {
            List<KeyValuePair<ShardCategory, string>> completeList;
            if (renderMathodDependentShards != null)
            {
                completeList = shardList.Concat(renderMathodDependentShards).ToList();
                completeList.Sort((x, y) => x.Key.CompareTo(y.Key));
            }
            else
                completeList = shardList;

            string res = string.Empty;
            foreach (var kvp in completeList)
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

            object val;
            if (parent == null)
            {
                concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, prop.Name);
                val = prop.GetValue(this);
            }
            else
            {
                concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, parent.Name + "." + prop.Name);
                val = prop.GetValue(parent.GetValue(this));
            }
            concreteParamDecl.GetField("Value").SetValue(ob, val);
            return (IFxParamDeclaration)ob;
        }

        private IFxParamDeclaration BuildFxParamDecl(FieldInfo field)
        {
            // Perform `new FxParamDeclaration<ParamType>{Name = paramName};`
            // Since we do not know ParamType at compile time we need to use reflection.
            Type concreteParamDecl = typeof(FxParamDeclaration<>).MakeGenericType(new Type[] { field.FieldType });
            //Cannot use GetConstructor(Type.EmptyTypes) with a struct!!
            //object ob = concreteParamDecl.GetConstructor(Type.EmptyTypes).Invoke(null);
            var ob = Activator.CreateInstance(concreteParamDecl);

            //Error because property ParamType has no setter.
            //concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.ParamType)).SetValue(ob, prop.GetType());

            concreteParamDecl.GetProperty(nameof(IFxParamDeclaration.Name)).SetValue(ob, field.Name);
            object val;

            val = field.GetValue(this);

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

        /// <summary>
        /// Event Handler that is called on <see cref="INotifyValueChange{T}.PropertyChanged"/>.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">The event arguments.</param>
        /// <param name="memberName">The name of the member which this event originated from.</param>
        protected void PropertyChangedHandler(object sender, SurfaceEffectEventArgs args, string memberName)
        {
            GetType().GetMethods().Where(m => m.Name == "SetFxParam").Where(item => item.GetParameters()[0].ParameterType == typeof(string)).First()
            .MakeGenericMethod(args.Type)
            .Invoke(this, new object[] { memberName + "." + args.Name, args.Value });
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            EffectChanged?.Invoke(this, new EffectManagerEventArgs(UniformChangedEnum.Dispose));

            if (disposing)
            {

            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~SurfaceEffectBase()
        {
            Dispose(false);
        }

    }
}