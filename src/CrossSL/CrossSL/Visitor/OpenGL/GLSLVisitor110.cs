using System;
using System.Linq;
using System.Text;
using Fusee.Math;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace CrossSL
{
    internal class GLSLVisitor110 : GLSLVisitor
    {
        /// <summary>
        ///     Translates a primitive type, e.g. "1f".
        /// </summary>
        /// <remarks>
        ///     GLSL 1.1 does not support type suffix.
        ///     GLSL 1.1 does not support type 'double'.
        /// </remarks>
        public override StringBuilder VisitPrimitiveExpression(PrimitiveExpression primitiveExpr)
        {
            var result = base.VisitPrimitiveExpression(primitiveExpr);

            if (primitiveExpr.Value is double)
            {
                var dInstr = GetInstructionFromStmt(primitiveExpr.GetParent<Statement>());
                DebugLog.Warning("Type 'double' is not supported in GLSL 1.1. " +
                                 "Value will be casted to type 'float'", dInstr);

                result.Replace('d', 'f');
            }

            return result.Replace("f", String.Empty);
        }

        /// <summary>
        ///     Translates an object creation, e.g. "new float4(...)".
        /// </summary>
        /// <remarks>
        ///     GLSL 1.1 does not support matrix casts.
        /// </remarks>
        public override StringBuilder VisitObjectCreateExpression(ObjectCreateExpression objCreateExpr)
        {
            if (!(objCreateExpr.Type.GetType() == typeof (SimpleType)))
                return base.VisitObjectCreateExpression(objCreateExpr);

            var simpleType = (SimpleType) objCreateExpr.Type;
            var dataType = simpleType.Annotation<TypeReference>().ToType();

            if (dataType == typeof (float3x3))
            {
                var methodRef = (MethodReference) objCreateExpr.Annotations.First();
                var methodDef = methodRef.Resolve();
                var methodParam = methodDef.Parameters.FirstOrDefault();

                if (methodParam != null && methodParam.ParameterType.IsType<float4x4>())
                {
                    var instr = GetInstructionFromStmt(objCreateExpr.GetParent<Statement>());
                    DebugLog.Warning("Matrix casting (float4x4 to float3x3) is not supported " +
                                     "in GLSL 1.1. Expression has been converted automatically", instr);

                    var argName = objCreateExpr.Arguments.First().AcceptVisitor(this);

                    var row1 = argName + "[0].xyz";
                    var row2 = argName + "[1].xyz";
                    var row3 = argName + "[2].xyz";

                    var type = objCreateExpr.Type.AcceptVisitor(this).ToString();
                    return new StringBuilder().Method(type, row1, row2, row3);
                }
            }

            return base.VisitObjectCreateExpression(objCreateExpr);
        }
    }
}