using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CrossSL.Meta;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Collections.Generic;

namespace CrossSL
{
    internal static class Program
    {
        private static bool MethodExists(TypeDefinition asmType, string methodName, out MethodDefinition method)
        {
            var methodCount = asmType.Methods.Count(asmMethod => asmMethod.Name == methodName);
            method = asmType.Methods.FirstOrDefault(asmMethod => asmMethod.Name == methodName);

            if (methodCount <= 1 && method != null && method.IsVirtual) return true;

            var instr = (method != null) ? method.Body.Instructions[0] : null;
            DebugLog.Error("You did not override method '" + methodName + "' properly", instr);

            return false;
        }

        private static FieldReference GenericFieldReference(TypeDefinition gen, TypeReference inst, string name)
        {
            var genTField = gen.Fields.First(field => field.Name == name);
            var tField = new FieldReference(genTField.Name, genTField.FieldType)
            {
                DeclaringType = inst
            };
            return tField;
        }

        private static int Main(string[] args)
        {
            Console.WriteLine("---------------------- CrossSL V1.0 ----------------------");

            if (args.Length == 0 || String.IsNullOrEmpty(args[0]))
            {
                DebugLog.UsageError("Start CrossSL with a .NET assemly as argument to translate\n" +
                                    "shaders from .NET to GLSL, e.g. 'CrossSL.exe Assembly.exe'");
                return -1;
            }

            string inputPath = args[0];

            var asmName = Path.GetFileName(inputPath);
            var asmDir = Path.GetDirectoryName(inputPath);

            if (!File.Exists(inputPath))
            {
                DebugLog.UsageError("Could not find assembly '" + asmName + "' in path:\n\n" + asmDir);
                return -1;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var metaPath = Path.Combine(asmDir, "CrossSL.Meta.dll");

            if (!File.Exists(inputPath) || !File.Exists(metaPath))
            {
                DebugLog.UsageError("Found assembly '" + asmName + "' but meta assembly 'CrossSL.Meta.dll'" +
                                    "\nis missing. It needs to be in the same directory:\n\n" + asmDir);
                return -1;
            }

            Console.WriteLine("\n\nFound assembly '" + asmName + "' and meta assembly 'CrossSL.Meta.dll'.");

            // check if there is a .pdb file along with the .exe
            var debugFile = Path.ChangeExtension(inputPath, "pdb");
            var debugName = Path.GetFileName(debugFile);

            DebugLog.Verbose = File.Exists(debugFile);
            Console.WriteLine(DebugLog.Verbose
                ? "Found also .pdb file '" + debugName + "'. This allows for better debugging."
                : "Found no .pdb file '" + debugName + "'. Extended debugging has been disabled.");

            // read assembly (and symbols) with Mono.Cecil
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(asmDir);

            var readParams = new ReaderParameters {ReadSymbols = DebugLog.Verbose, AssemblyResolver = resolver};
            var asm = AssemblyDefinition.ReadAssembly(inputPath, readParams);

            // read meta assembly (without symbols) with Mono.Cecil
            var metaParams = new ReaderParameters {ReadSymbols = false};
            var metaAsm = AssemblyDefinition.ReadAssembly(metaPath, metaParams);

            // find all types with xSLShader as base type
            var shaderDescs = new List<ShaderDesc>();

            var asmTypes = asm.Modules.SelectMany(
                asmMod => asmMod.Types.Where(asmType => asmType.BaseType != null));
            var asmShaderTypes = asmTypes.Where(asmType => asmType.BaseType.IsType<xSLShader>());

            foreach (var asmType in asmShaderTypes)
            {
                DebugLog.Reset();

                if (DebugLog.Verbose)
                {
                    // load symbols from pdb file
                    var asmModule = asmType.Module;

                    using (var symbReader = new PdbReaderProvider().GetSymbolReader(asmModule, debugFile))
                        asmModule.ReadSymbols(symbReader);
                }

                var asmTypeName = asmType.Name;
                Console.WriteLine("\n\nFound a shader called '" + asmTypeName + "':");

                var shaderDesc = new ShaderDesc {Name = asmTypeName, Type = asmType};

                // check for [xSLDebug] first in case the shader should be ignored
                var debugAttr = asmType.CustomAttributes.FirstOrDefault(
                    attrType => attrType.AttributeType.IsType<xSLShader.xSLDebugAttribute>());

                if (debugAttr != null)
                {
                    shaderDesc.DebugFlags = (xSLShader.xSLDebug) debugAttr.ConstructorArguments[0].Value;

                    if ((shaderDesc.DebugFlags & xSLShader.xSLDebug.IgnoreShader) != 0)
                    {
                        Console.WriteLine("  => Found [xSLDebug] with 'IgnoreShader' flag. Shader skipped.");
                        continue;
                    }
                }

                // check for [xSLTarget] and save settings
                var targetAttr = asmType.CustomAttributes.FirstOrDefault(
                    attr => attr.AttributeType.IsType<xSLShader.xSLTargetAttribute>());

                if (targetAttr == null)
                {
                    shaderDesc.Target = new ShaderTarget {Envr = SLEnvironment.OpenGL, Version = 110, VersionID = 0};
                    DebugLog.Error("Could not find [xSLTarget]. Please specify the targeted shading language");
                }
                else
                {
                    var typeName = targetAttr.ConstructorArguments[0].Type.Name;
                    var versionID = (int) targetAttr.ConstructorArguments[0].Value;

                    var shaderTarget = new ShaderTarget {VersionID = versionID};

                    switch (typeName)
                    {
                        case "GLSL":
                            shaderTarget.Envr = SLEnvironment.OpenGL;
                            break;

                        case "GLSLES":
                            shaderTarget.Envr = SLEnvironment.OpenGLES;
                            break;

                        case "GLSLMix":
                            shaderTarget.Envr = SLEnvironment.OpenGLMix;
                            break;
                    }

                    var vStr = SLVersion.VIDs[(int) shaderTarget.Envr][versionID];

                    shaderTarget.Version = Int32.Parse(vStr);
                    shaderDesc.Target = shaderTarget;

                    if (shaderTarget.Envr == SLEnvironment.OpenGLMix)
                    {
                        typeName = "GLSL 1.10 & GLSLES";
                        vStr = "100";
                    }

                    vStr = vStr.Insert(1, ".");
                    Console.WriteLine("  => Found [xSLTarget]. Compiling shader as " + typeName + " " + vStr + ".");
                }

                var shaderTranslator = ShaderTranslator.GetTranslator(shaderDesc.Target);
                shaderTranslator.ShaderDesc = shaderDesc;

                // save debug settings
                if (debugAttr == null)
                {
                    shaderDesc.DebugFlags = xSLShader.xSLDebug.None;
                    Console.WriteLine("  => Could not find [xSLDebug]. Debugging has been disabled.");
                    DebugLog.Disabled = true;
                }
                else
                {
                    if ((shaderDesc.DebugFlags & xSLShader.xSLDebug.None) != 0)
                        Console.WriteLine("  => Found [xSLDebug] with 'None' flag. Debugging has been disabled.");
                    else
                        Console.WriteLine("  => Found [xSLDebug]. Debugging with flags: " + shaderDesc.DebugFlags);
                }

                // check for common mistakes
                Console.WriteLine("\n  1. Checking shader for obvious mistakes.");

                // check if vertex or fragment method is missing
                MethodDefinition vertexMain;
                if (!MethodExists(asmType, "VertexShader", out vertexMain)) continue;

                MethodDefinition fragmentMain;
                if (!MethodExists(asmType, "FragmentShader", out fragmentMain)) continue;

                // get their precission attributes
                var vertPrecAttr = vertexMain.CustomAttributes.FirstOrDefault(
                    attrType => attrType.AttributeType.IsType<xSLShader.xSLPrecisionAttribute>());

                var fragPrecAttr = fragmentMain.CustomAttributes.FirstOrDefault(
                    attrType => attrType.AttributeType.IsType<xSLShader.xSLPrecisionAttribute>());

                shaderDesc.Precision = new CustomAttribute[2];
                shaderDesc.Precision[(int) SLShaderType.VertexShader] = vertPrecAttr;
                shaderDesc.Precision[(int) SLShaderType.FragmentShader] = fragPrecAttr;

                // get default file for this shader in case no instructions are available
                var defaultSeq = vertexMain.Body.Instructions[0].SequencePoint;

                DebugLog.DefaultFile = defaultSeq != null ? Path.GetFileName(defaultSeq.Document.Url) : asmTypeName;

                // check if there are additional constructors for field/property initialization
                var ctorMethods = asmType.Methods.Where(asmMethod => asmMethod.IsConstructor);
                var customCtors = new Collection<MethodDefinition>();

                foreach (var ctorMethod in ctorMethods.Where(ctor => ctor.Body.CodeSize > 7))
                {
                    // see if there are field initializations (as for "constants")
                    var varInits = ctorMethod.Body.Instructions.Any(instr => instr.OpCode == OpCodes.Stfld);

                    // or property setter calls (as for "constants")
                    var funcCalls = ctorMethod.Body.Instructions.Where(instr => instr.OpCode == OpCodes.Call);
                    var propInits = funcCalls.Any(instr => ((MethodReference) instr.Operand).Resolve().IsSetter);

                    if (varInits || propInits)
                        customCtors.Add(ctorMethod);
                    else
                    {
                        var instr = ctorMethod.Body.Instructions[0];
                        DebugLog.Warning("Found a constructor with no valid content", instr);
                    }
                }

                // analyze variables used in shader
                Console.WriteLine("\n  2. Collecting information about fields and properties.");

                var variables = new Collection<VariableDesc>();
                var varTypes = Enum.GetNames(typeof (SLVariableType));

                // read and gather fields and backing fields
                foreach (var asmField in asmType.Fields)
                {
                    var varDesc = new VariableDesc {Definition = asmField};

                    var fdName = asmField.Name;
                    var fdType = asmField.FieldType.ToType();
                    var fdArray = asmField.FieldType.IsArray;
                    var attrs = asmField.CustomAttributes;

                    var isProp = fdName.Contains("<");

                    if (isProp)
                    {
                        // ReSharper disable once StringIndexOfIsCultureSpecific.1
                        fdName = fdName.Remove(0, 1).Remove(fdName.IndexOf('>') - 1);

                        var asmProp = asmType.Properties.First(prop => prop.Name == fdName);
                        varDesc.Definition = asmProp;

                        attrs = asmProp.CustomAttributes;
                        fdType = asmProp.PropertyType.ToType();
                        fdArray = asmProp.PropertyType.IsArray;
                    }

                    var attrCt = attrs.Count(attr => varTypes.Contains(attr.AttributeType.Name));

                    var validFd = (asmField.HasConstant || attrCt == 1);
                    var varType = (isProp) ? "Property '" : "Field '";

                    if (asmField.IsStatic && !asmField.HasConstant)
                        DebugLog.Error(varType + fdName + "' cannot be static");
                    else if (validFd)
                    {
                        var fdAttrName = attrs.First(attr => varTypes.Contains(attr.AttributeType.Name));
                        var fdAttr = (SLVariableType) Array.IndexOf(varTypes, fdAttrName.AttributeType.Name);

                        if (asmField.HasConstant)
                        {
                            if (fdAttr != SLVariableType.xSLConstAttribute)
                                DebugLog.Error(varType + "constant " + fdName + "' has an invalid attribute");

                            varDesc.Value = asmField.Constant;
                        }

                        varDesc.DataType = fdType;
                        varDesc.Attribute = fdAttr;
                        varDesc.IsArray = fdArray;

                        variables.Add(varDesc);
                    }
                    else
                        DebugLog.Error(varType + fdName + "' is neither a constant nor has valid attributes");
                }

                shaderDesc.Variables = variables;
                shaderTranslator.PreVariableCheck();

                // translate main, depending methods and constructors
                if (shaderDesc.Target.Envr == SLEnvironment.OpenGLMix)
                    Console.WriteLine("\n  3. Translating shader from C# to GLSL/GLSLES.");
                else
                    Console.WriteLine("\n  3. Translating shader from C# to " + shaderDesc.Target.Envr + ".");

                shaderDesc.Funcs = new IEnumerable<FunctionDesc>[2];

                var vertexFuncs = shaderTranslator.Translate(shaderDesc.Target, vertexMain);
                shaderDesc.Funcs[(int) SLShaderType.VertexShader] = vertexFuncs;

                var fragmentFuncs = shaderTranslator.Translate(shaderDesc.Target, fragmentMain);
                shaderDesc.Funcs[(int) SLShaderType.FragmentShader] = fragmentFuncs;

                // check correct use of constants
                foreach (var ctor in customCtors)
                {
                    var funcs = shaderTranslator.Translate(shaderDesc.Target, ctor);

                    var allVars = funcs.SelectMany(func => func.Variables).ToList();
                    var allGlobVars = allVars.Where(variables.Contains).ToList();
                    var illegalVars = allVars.Except(allGlobVars).ToList();

                    foreach (var illegalVar in illegalVars)
                    {
                        var name = illegalVar.Definition.Name;
                        var instr = illegalVar.Instruction;

                        DebugLog.Error("Illegal use of '" + name + "' in a constructor", instr);
                    }

                    foreach (var constVar in allGlobVars)
                    {
                        var globVar = shaderDesc.Variables.First(var => var.Equals(constVar));
                        var index = shaderDesc.Variables.IndexOf(globVar);

                        var name = constVar.Definition.Name;
                        var instr = constVar.Instruction;

                        if (globVar.Attribute != SLVariableType.xSLConstAttribute)
                            DebugLog.Error("Variable '" + name + "' is used as a constant but not marked as such'",
                                instr);
                        else if (globVar.Value != null && constVar.Value != null)
                            DebugLog.Error("Constant '" + name + "' cannot be set more than once", instr);
                        else if (constVar.Value is String)
                            DebugLog.Error("Constant '" + name + "' was initialized with an invalid value", instr);
                        else
                            shaderDesc.Variables[index].Value = constVar.Value;
                    }
                }

                // build both shaders
                var vertexResult = shaderTranslator.BuildShader(SLShaderType.VertexShader);
                var fragmentResult = shaderTranslator.BuildShader(SLShaderType.FragmentShader);
                shaderDesc = shaderTranslator.ShaderDesc;

                // see if there are unused fields/properties
                var unusedVars = shaderDesc.Variables.Where(var => !var.IsReferenced);
                unusedVars = unusedVars.Where(var => var.Attribute != SLVariableType.xSLConstAttribute);

                foreach (var unsedVar in unusedVars)
                    DebugLog.Warning("Variable '" + unsedVar.Definition.Name + "' was declared but not used");

                if (!DebugLog.Abort)
                {
                    Console.WriteLine("\n  4. Building vertex and fragment shader.");

                    // debugging: save to file first, then precompile
                    Console.WriteLine("\n  5. Applying debugging flags if any.");

                    if (!DebugLog.Abort && (xSLShader.xSLDebug.SaveToFile & shaderDesc.DebugFlags) != 0)
                    {
                        var directory = Path.GetDirectoryName(inputPath);

                        if (directory != null)
                        {
                            var combined = new StringBuilder("---- VertexShader ----").NewLine(2);
                            combined.Append(vertexResult).NewLine(3).Append("---- FragmentShader ----");
                            combined.NewLine(2).Append(fragmentResult);

                            var filePath = Path.Combine(directory, shaderDesc.Name + ".txt");
                            File.WriteAllText(filePath, combined.ToString());

                            Console.WriteLine("    => Saved shader to: '" + filePath + "'");
                        }
                    }

                    if (!DebugLog.Abort && (xSLShader.xSLDebug.PreCompile & shaderDesc.DebugFlags) != 0)
                        shaderTranslator.PreCompile(vertexResult, fragmentResult);
                }
                else
                {
                    Console.WriteLine("\n  4. Shader will not be built due to critical errors.");
                    Console.WriteLine("\n  5. Applying debugging flags if any.");
                }

                if (DebugLog.Abort)
                {
                    if ((xSLShader.xSLDebug.ThrowException & shaderDesc.DebugFlags) != 0)
                    {
                        Console.WriteLine("    => Errors will be thrown when using the shader.");
                        Console.WriteLine("\n  6. Preparing to update meta assembly for this shader.");
                    }
                }

                // save shaders or errors into the assembly
                var genShader = metaAsm.MainModule.Types.First(type => type.ToType() == typeof (xSL<>));
                var instShader = new GenericInstanceType(genShader);

                var asmTypeImport = metaAsm.MainModule.Import(asmType);
                instShader.GenericArguments.Add(asmTypeImport);

                var instrList = new List<Instruction>();

                if (!DebugLog.Abort)
                {
                    Console.WriteLine("\n  6. Preparing to update meta assembly for this shader.");

                    var vertField = GenericFieldReference(genShader, instShader, "_vertex");
                    var fragField = GenericFieldReference(genShader, instShader, "_fragment");
                    var transField = GenericFieldReference(genShader, instShader, "_translated");

                    metaAsm.MainModule.Import(vertField);
                    metaAsm.MainModule.Import(fragField);
                    metaAsm.MainModule.Import(transField);

                    instrList.Add(Instruction.Create(OpCodes.Ldstr, vertexResult.ToString()));
                    instrList.Add(Instruction.Create(OpCodes.Stsfld, vertField));

                    instrList.Add(Instruction.Create(OpCodes.Ldstr, fragmentResult.ToString()));
                    instrList.Add(Instruction.Create(OpCodes.Stsfld, fragField));

                    instrList.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                    instrList.Add(Instruction.Create(OpCodes.Stsfld, transField));
                }

                // apply debug mode ThrowException
                if (DebugLog.Abort && (xSLShader.xSLDebug.ThrowException & shaderDesc.DebugFlags) != 0)
                {
                    var errors = DebugLog.Errors.ToString().Replace("    =>", "=>");
                    var errField = GenericFieldReference(genShader, instShader, "_error");

                    instrList.Add(Instruction.Create(OpCodes.Ldstr, errors));
                    instrList.Add(Instruction.Create(OpCodes.Stsfld, errField));
                }

                shaderDesc.Instructions = instrList;

                Console.WriteLine(DebugLog.Abort
                    ? "\n  ---- Translation failed ----"
                    : "\n  ---- Translation succeeded ----");

                shaderDescs.Add(shaderDesc);
            }

            // write shaders into assembly
            var invalidCt = shaderDescs.Count(shader => !shader.Instructions.Any());

            if (invalidCt == shaderDescs.Count)
                Console.WriteLine("\n\nAssembly will not be updated as no shader was translated successfully.");
            else
            {
                Console.WriteLine("\n\nUpdating CrossSL meta assembly:");

                var asmModule = metaAsm.MainModule;
                var genShader = asmModule.Types.First(type => type.ToType() == typeof (xSL<>));

                var xSLInit = genShader.Methods.First(method => method.Name == "Init");
                var ilProc = xSLInit.Body.GetILProcessor();

                xSLInit.Body.Instructions.Clear();

                foreach (var shaderDesc in shaderDescs)
                    foreach (var instr in shaderDesc.Instructions)
                        ilProc.Append(instr);

                var ret = Instruction.Create(OpCodes.Ret);
                ilProc.Append(ret);

                try
                {
                    var writeParams = new WriterParameters {WriteSymbols = false};
                    asmModule.Write(metaPath, writeParams);

                    foreach (var shaderDesc in shaderDescs)
                        if (shaderDesc.Instructions.Count() > 2)
                            Console.WriteLine("  => Added shader '" + shaderDesc.Name + "' to assembly.");
                        else if (shaderDesc.Instructions.Any())
                            Console.WriteLine("  => [ThrowException] mode was applied for shader '" +
                                              shaderDesc.Name + "'.");

                    Console.WriteLine("\n  => Saved assembly as '" + metaPath + "'");
                }
                catch (IOException)
                {
                    Console.WriteLine("  => Cannot update assembly. File might be missing, read-only or in use.");
                }
            }

            DebugLog.UsageError("\nDone.");


            return 0;
        }
    }
}