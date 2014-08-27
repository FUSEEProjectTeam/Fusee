using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CrossSL.Meta;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace CrossSL
{
    internal abstract class ShaderTranslator
    {
        internal ShaderDesc ShaderDesc { get; set; }

        protected ShaderMapping ShaderMapping;
        protected ShaderVisitor ShaderVisitor;

        /// <summary>
        ///     Gets the right translator for the targeted shader language.
        /// </summary>
        /// <param name="target">The targeted shader language.</param>
        /// <returns></returns>
        internal static ShaderTranslator GetTranslator(ShaderTarget target)
        {
            switch (target.Envr)
            {
                case SLEnvironment.OpenGL:
                    switch ((xSLShader.xSLTarget.GLSL) target.VersionID)
                    {
                        case xSLShader.xSLTarget.GLSL.V110:
                            return new GLSLTranslator110();

                        default:
                            return new GLSLTranslator();
                    }

                case SLEnvironment.OpenGLES:
                    return new GLSLTranslator100();

                case SLEnvironment.OpenGLMix:
                    return new GLSLTranslatorMix();
            }

            return null;
        }

        /// <summary>
        ///     Maps the return type to the targeted shader language.
        /// </summary>
        /// <param name="method">The method whose return type needs to be mapped.</param>
        /// <returns></returns>
        protected virtual StringBuilder MapReturnType(MethodDefinition method)
        {
            var retType = method.ReturnType.ToType();

            if (!ShaderMapping.Types.ContainsKey(retType))
            {
                var strAdd = (retType != typeof (Object)) ? " '" + retType.Name + "'" : String.Empty;

                var instr = method.Body.Instructions[0];
                DebugLog.Error("Method has an unsupported return type" + strAdd, instr);

                return null;
            }

            return new StringBuilder(ShaderMapping.Types[retType]);
        }

        /// <summary>
        ///     Combines the parameters of a given method into a single string.
        /// </summary>
        /// <param name="method">The method whose parameters needs to be joined.</param>
        /// <returns>The combined parameters as a <see cref="StringBuilder" />.</returns>
        protected virtual StringBuilder JoinParams(MethodDefinition method)
        {
            var result = new StringBuilder();

            foreach (var param in method.Parameters)
            {
                var paramType = param.ParameterType.ToType();

                if (!ShaderMapping.Types.ContainsKey(paramType))
                {
                    var strAdd = (paramType != typeof (Object)) ? " '" + paramType.Name + "'" : String.Empty;

                    var instr = method.Body.Instructions[0];
                    DebugLog.Error("Method has a parameter of the unsupported type" + strAdd, instr);

                    return null;
                }

                var isRef = (param.ParameterType is ByReferenceType);
                var refStr = (isRef) ? "out " : String.Empty;

                var typeMapped = ShaderMapping.Types[paramType];
                var paramName = param.Name;

                result.Append(refStr).Append(typeMapped).Space();
                result.Append(paramName).Append(", ");
            }

            if (result.Length > 0)
                result.Length -= 2;

            return result;
        }

        /// <summary>
        ///     Translates the a given method to the targeted shader language.
        /// </summary>
        /// <param name="target">The targeted shader language.</param>
        /// <param name="methodDef">The method definition.</param>
        /// <returns>A list of <see cref="FunctionDesc" />s for every translated method.</returns>
        internal virtual IEnumerable<FunctionDesc> Translate(ShaderTarget target, MethodDefinition methodDef)
        {
            var allFuncs = new List<FunctionDesc>();

            // build function signature
            var retTypeStr = MapReturnType(methodDef);
            var paramStr = JoinParams(methodDef);

            if (retTypeStr == null || paramStr == null)
                return null;

            var sig = retTypeStr.Space().Method(methodDef.Name, paramStr.ToString());

            // create DecompilerContext for given method
            ShaderVisitor.Init(methodDef);
            ShaderVisitor.Translate(ShaderMapping);

            // save information
            var result = new FunctionDesc
            {
                Definion = methodDef,
                Signature = sig,
                Body = ShaderVisitor.Result,
                Variables = ShaderVisitor.RefVariables
            };

            // translate all referenced methods
            foreach (var refMethod in ShaderVisitor.RefMethods)
                if (allFuncs.All(aMethod => aMethod.Definion != refMethod))
                    allFuncs.AddRange(Translate(target, refMethod));

            allFuncs.Add(result);
            return allFuncs;
        }

        /// <summary>
        ///     Checks member variables before translating the shader.
        /// </summary>
        protected internal virtual void PreVariableCheck()
        {
            foreach (var memberVar in ShaderDesc.Variables)
            {
                var varName = memberVar.Definition.Name;
                var varType = memberVar.DataType;

                // resolve data type of variable
                if (!ShaderMapping.Types.ContainsKey(varType))
                {
                    var strAdd = (varType != typeof (Object)) ? " type '" + varType.Name + "' " : " a type ";
                    DebugLog.Error(varType + varName + "' is of" + strAdd + "which is not supported.");
                }
            }
        }

        /// <summary>
        ///     Checks member and referenced variables after translating the shader.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        /// <param name="varDescs">The variable descs.</param>
        /// <param name="refVars">The referenced variables.</param>
        private void PostVariableCheck(SLShaderType shaderType, Collection<VariableDesc> varDescs,
            List<VariableDesc> refVars)
        {
            // check if varyings and attributes are used properly
            var varVars = varDescs.Where(var => var.Attribute == SLVariableType.xSLVaryingAttribute);

            if (shaderType == SLShaderType.FragmentShader)
            {
                foreach (var invalidVar in varVars.Where(var => !var.IsReferenced))
                    DebugLog.Error("Varying '" + invalidVar.Definition.Name + "' is used in 'FragmentShader()'" +
                                   " but was not set in 'VertexShader()'", invalidVar.Instruction);

                var attrVars = varDescs.Where(var => var.Attribute == SLVariableType.xSLAttributeAttribute).ToList();

                foreach (var invalidVar in attrVars)
                    DebugLog.Error("Attribute '" + invalidVar.Definition.Name + "' cannot be " +
                                   "used in in 'FragmentShader()'" + invalidVar.Instruction);
            }
            else
            {
                var fragFunc = ShaderDesc.Funcs[(int) SLShaderType.FragmentShader];
                var mergedVars = fragFunc.SelectMany(func => func.Variables).ToList();

                foreach (var invalidVar in varVars.Where(var => !mergedVars.Contains(var)))
                    DebugLog.Warning("Varying '" + invalidVar.Definition.Name + "' was set in 'VertexShader()'" +
                                     " but is not used in 'FragmentShader()'", invalidVar.Instruction);
            }

            // check if constants have been set
            var constVars = varDescs.Where(var => var.Attribute == SLVariableType.xSLConstAttribute).ToList();

            foreach (var constVar in refVars.Where(constVars.Contains).Where(con => con.Value != null))
                DebugLog.Error("Constant '" + constVar.Definition.Name + "' cannot be initialized " +
                               "in 'VertexShader()'", constVar.Instruction);

            foreach (var constVar in constVars.Where(var => var.Value == null))
            {
                constVar.Value = ShaderDesc.Variables.First(var => var.Definition == constVar.Definition).Value;
                if (constVar.Value != null) continue;

                DebugLog.Error("Constant '" + constVar.Definition.Name + "' was not initialized", constVar.Instruction);
            }

            // check if invalid variables are set
            var nestedTypes = typeof (xSLShader).GetNestedTypes(BindingFlags.NonPublic);

            var attrType = nestedTypes.FirstOrDefault(type => type.Name == shaderType + "Attribute");
            var mandType = nestedTypes.FirstOrDefault(type => type == typeof (xSLShader.MandatoryAttribute));

            var allProps = typeof (xSLShader).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
            var validProps = allProps.Where(prop => prop.CustomAttributes.Any(attr => attr.AttributeType == attrType));
            var validNames = validProps.Select(prop => prop.Name).ToList();

            var globalVars = refVars.Where(def => def.Definition.DeclaringType.IsType<xSLShader>()).ToList();
            var globalNames = globalVars.Select(var => var.Definition.Name).ToList();

            foreach (var memberVar in globalNames.Where(var => !validNames.Contains(var)))
            {
                var instr = globalVars.First(var => var.Definition.Name == memberVar).Instruction;
                DebugLog.Error("'" + memberVar + "' cannot be used in '" + shaderType + "()'", instr);
            }

            // check if necessary variables are set
            var mandVars = allProps.Where(prop => prop.CustomAttributes.Any(attr => attr.AttributeType == mandType));

            foreach (var mandVar in mandVars)
            {
                var mandVarName = mandVar.Name;

                if (validNames.Contains(mandVarName) && !globalNames.Contains(mandVarName))
                    DebugLog.Error("'" + mandVarName + "' has to be set in '" + shaderType + "()'");

                if (globalNames.Count(var => var == mandVarName) > 1)
                {
                    var instr = globalVars.Last(var => var.Definition.Name == mandVarName).Instruction;
                    DebugLog.Warning("'" + mandVarName + "' has been set more than" +
                                     " once in '" + shaderType + "()'", instr);
                }
            }
        }

        /// <summary>
        ///     Sets the data type precision for the given shader type.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        protected abstract StringBuilder SetPrecision(SLShaderType shaderType);

        /// <summary>
        ///     Builds the given type of shader.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        /// <returns></returns>
        internal virtual StringBuilder BuildShader(SLShaderType shaderType)
        {
            var result = new StringBuilder();

            // corresponding functions
            var functions = ShaderDesc.Funcs[(int) shaderType];

            // collect all referenced variables
            var refVars = new List<VariableDesc>();

            foreach (var func in functions.Where(func => func.Variables != null))
                refVars.AddRange(func.Variables);

            var varDescs = new Collection<VariableDesc>();
            var shaderDescType = ShaderDesc.Type.ToType();
            var memberVars = refVars.Where(var => var.Definition.DeclaringType.ToType() == shaderDescType);

            foreach (var memberVar in memberVars)
            {
                var globVar = ShaderDesc.Variables.FirstOrDefault(var => var.Definition == memberVar.Definition);

                if (globVar != null)
                {
                    var globIndex = ShaderDesc.Variables.IndexOf(globVar);

                    ShaderDesc.Variables[globIndex].IsReferenced = true;

                    memberVar.Attribute = globVar.Attribute;
                    memberVar.DataType = globVar.DataType;
                    memberVar.IsArray = globVar.IsArray;
                    memberVar.IsReferenced = true;

                    varDescs.Add(memberVar);
                }
            }

            // check variables
            PostVariableCheck(shaderType, varDescs, refVars);
            if (DebugLog.Abort) return null;

            // add precision to output
            result.Append(SetPrecision(shaderType));
            if (DebugLog.Abort) return null;

            // add variables to shader output
            foreach (var varDesc in varDescs.Distinct().OrderBy(var => var.Attribute))
            {
                var dataType = ShaderMapping.Types[varDesc.DataType];

                var varType = varDesc.Attribute.ToString().ToLower();
                varType = varType.Remove(0, 3).Remove(varType.Length - 12);

                result.Append(varType).Space().Append(dataType).Space();
                result.Append(varDesc.Definition.Name);

                if (varDesc.IsArray)
                {
                    var valStr = varDesc.Value.ToString();
                    valStr = valStr.Remove(0, valStr.IndexOf('['));
                    result.Append(valStr.Remove(valStr.IndexOf(']') + 1));
                }

                if (varDesc.Attribute == SLVariableType.xSLConstAttribute)
                    result.Assign().Append(varDesc.Value);

                result.Semicolon().NewLine();
            }

            if (result.Length > 2)
                result.Length -= 2;

            // add all functions to shader output
            foreach (var func in ShaderDesc.Funcs[(int) shaderType])
                result.NewLine(2).Append(func.Signature).NewLine().Append(func.Body);

            return result;
        }

        /// <summary>
        ///     Tests the given shaders by passing them to the GLSL/HLSL compiler.
        /// </summary>
        /// <param name="vertexShader">The vertex shader.</param>
        /// <param name="fragmentShader">The fragment shader.</param>
        internal abstract void PreCompile(StringBuilder vertexShader, StringBuilder fragmentShader);
    }
}