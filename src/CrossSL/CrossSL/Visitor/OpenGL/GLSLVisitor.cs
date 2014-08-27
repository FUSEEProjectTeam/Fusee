using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CrossSL.Meta;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace CrossSL
{
    internal class GLSLVisitor : ShaderVisitor
    {
        /// <summary>
        ///     Translates a block statement, e.g. a method's body.
        /// </summary>
        /// <remarks>
        ///     If verbose mode is active (i.e. if a .pdb file was found) additional
        ///     line breaks between statements are considered in the output.
        /// </remarks>
        public override StringBuilder VisitBlockStatement(BlockStatement blockStmt)
        {
            var result = new StringBuilder();

            foreach (var stmt in blockStmt.Statements)
                result.Append(stmt.AcceptVisitor(this)).NewLine();

            return result;
        }

        /// <summary>
        ///     Translates a variable declaration statement, e.g. "float4 x;".
        /// </summary>
        public override StringBuilder VisitVariableDeclarationStatement(
            VariableDeclarationStatement varDeclStmt)
        {
            var result = new StringBuilder();

            var type = varDeclStmt.Type.AcceptVisitor(this);
            foreach (var varDecl in varDeclStmt.Variables)
                result.Append(type).Space().Append(varDecl.AcceptVisitor(this)).Semicolon();

            return result;
        }

        /// <summary>
        ///     Translates a while loop, e.g. "while (val) { ... }".
        /// </summary>
        public override StringBuilder VisitWhileStatement(WhileStatement whileStmt)
        {
            var result = new StringBuilder();

            var cond = whileStmt.Condition.AcceptVisitor(this);
            var stmt = whileStmt.EmbeddedStatement.AcceptVisitor(this);

            return result.Method("while ", cond.ToString()).Block(stmt);
        }

        /// <summary>
        ///     Translates a simple data type, e.g. "float4".
        /// </summary>
        public override StringBuilder VisitSimpleType(SimpleType simpleType)
        {
            var sysType = simpleType.Annotation<TypeReference>().ToType();
            var mappedType = MapDataTypeIfValid(simpleType, sysType);

            return mappedType == null
                ? new StringBuilder(simpleType.ToString())
                : new StringBuilder(mappedType);
        }

        /// <summary>
        ///     Translates a composed data type, e.g. "float4".
        /// </summary>
        /// <remarks>
        ///     Exact purpose is unknown. This has something to do with arrays.
        /// </remarks>
        public override StringBuilder VisitComposedType(ComposedType composedType)
        {
            return composedType.BaseType.AcceptVisitor(this);
        }

        /// <summary>
        ///     Translates an array specifier, e.g. "[10]".
        /// </summary>
        public override StringBuilder VisitArraySpecifier(ArraySpecifier arraySpecifier)
        {
            return new StringBuilder(arraySpecifier.Dimensions);
        }

        /// <summary>
        ///     Translates a primitive data type, e.g. "float".
        /// </summary>
        public override StringBuilder VisitPrimitiveType(PrimitiveType primitiveType)
        {
            var typeName = primitiveType.KnownTypeCode.ToString();
            var sysType = Type.GetType("System." + typeName);

            var mappedType = MapDataTypeIfValid(primitiveType, sysType);

            return mappedType == null
                ? new StringBuilder(primitiveType.Keyword)
                : new StringBuilder(mappedType);
        }

        /// <summary>
        ///     Translates a variable initializer, e.g. "x" or "x = 5".
        /// </summary>
        public override StringBuilder VisitVariableInitializer(VariableInitializer variableInit)
        {
            var result = new StringBuilder(variableInit.Name);

            if (!variableInit.Initializer.IsNull)
                result.Assign().Append(variableInit.Initializer.AcceptVisitor(this));

            return result;
        }

        /// <summary>
        ///     Translates an expression statement, e.g. "x = a + b".
        /// </summary>
        public override StringBuilder VisitExpressionStatement(ExpressionStatement exprStmt)
        {
            return exprStmt.Expression.AcceptVisitor(this).Semicolon();
        }

        /// <summary>
        ///     Translates a for loop, e.g. "for (int x = 0; x &lt; 5; x++) { ... }".
        /// </summary>
        public override StringBuilder VisitForStatement(ForStatement forStmt)
        {
            var init = JoinArgs(forStmt.Initializers);
            var cond = forStmt.Condition.AcceptVisitor(this).Semicolon();

            var iterator = JoinArgs(forStmt.Iterators);
            iterator.Length--;

            var loopSig = init.Space().Append(cond).Space().Append(iterator);
            var stmt = forStmt.EmbeddedStatement.AcceptVisitor(this);

            return new StringBuilder().Method("for ", loopSig.ToString()).Space().Block(stmt);
        }

        /// <summary>
        ///     Translates an assignment statement, e.g. "x = a + b".
        /// </summary>
        public override StringBuilder VisitAssignmentExpression(AssignmentExpression assignmentExpr)
        {
            var result = assignmentExpr.Left.AcceptVisitor(this);

            // assignment operator type mapping
            var opAssignment = new Dictionary<AssignmentOperatorType, string>
            {
                {AssignmentOperatorType.Assign, String.Empty},
                {AssignmentOperatorType.Add, "+"},
                {AssignmentOperatorType.BitwiseAnd, "&"},
                {AssignmentOperatorType.BitwiseOr, "|"},
                {AssignmentOperatorType.Divide, "/"},
                {AssignmentOperatorType.ExclusiveOr, "^"},
                {AssignmentOperatorType.Modulus, "%"},
                {AssignmentOperatorType.Multiply, "*"},
                {AssignmentOperatorType.ShiftLeft, "<<"},
                {AssignmentOperatorType.ShiftRight, ">>"},
                {AssignmentOperatorType.Subtract, "-"},
            };

            result.Assign(opAssignment[assignmentExpr.Operator]);
            var rightRes = assignmentExpr.Right.AcceptVisitor(this);

            // add value to RefVariables if this is an "constant initialization"
            if (assignmentExpr.Operator == AssignmentOperatorType.Assign)
                if (assignmentExpr.Left.IsType<MemberReferenceExpression>())
                {
                    var left = (MemberReferenceExpression) assignmentExpr.Left;
                    var memberRef = left.Annotation<IMemberDefinition>();

                    if (RefVariables.Any(var => var.Definition == memberRef))
                    {
                        var refVar = RefVariables.Last(var => var.Definition == memberRef);

                        if (assignmentExpr.Right.IsType<ObjectCreateExpression>() ||
                            assignmentExpr.Right.IsType<PrimitiveExpression>() ||
                            assignmentExpr.Right.IsType<ArrayCreateExpression>())
                            RefVariables[RefVariables.IndexOf(refVar)].Value = rightRes;
                        else
                            RefVariables[RefVariables.IndexOf(refVar)].Value = "Exception";
                    }
                }

            return result.Append(rightRes);
        }

        /// <summary>
        ///     Translates a binary operator expression, e.g. "a * b".
        /// </summary>
        public override StringBuilder VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOpExpr)
        {
            var result = new StringBuilder();

            if (binaryOpExpr.Operator == BinaryOperatorType.Modulus)
            {
                var leftOp = binaryOpExpr.Left.AcceptVisitor(this).ToString();
                var rightOp = binaryOpExpr.Right.AcceptVisitor(this).ToString();

                result.Method("mod", leftOp, rightOp);
            }
            else
            {
                result.Append(binaryOpExpr.Left.AcceptVisitor(this));

                // binary operator type mapping
                var opAssignment = new Dictionary<BinaryOperatorType, string>
                {
                    {BinaryOperatorType.Add, "+"},
                    {BinaryOperatorType.BitwiseAnd, "&"},
                    {BinaryOperatorType.BitwiseOr, "|"},
                    {BinaryOperatorType.ConditionalAnd, "&&"},
                    {BinaryOperatorType.ConditionalOr, "||"},
                    {BinaryOperatorType.Divide, "/"},
                    {BinaryOperatorType.Equality, "=="},
                    {BinaryOperatorType.ExclusiveOr, "^"},
                    {BinaryOperatorType.GreaterThan, ">"},
                    {BinaryOperatorType.GreaterThanOrEqual, ">="},
                    {BinaryOperatorType.InEquality, "!="},
                    {BinaryOperatorType.LessThan, "<"},
                    {BinaryOperatorType.LessThanOrEqual, "<="},
                    {BinaryOperatorType.Multiply, "*"},
                    {BinaryOperatorType.NullCoalescing, "??"},
                    {BinaryOperatorType.ShiftLeft, "<<"},
                    {BinaryOperatorType.ShiftRight, ">>"},
                    {BinaryOperatorType.Subtract, "-"}
                };

                result.Space().Append(opAssignment[binaryOpExpr.Operator]).Space();
                result.Append(binaryOpExpr.Right.AcceptVisitor(this));
            }

            return result;
        }

        /// <summary>
        ///     Translates an array creation expression, e.g. "new float[] { ... }".
        /// </summary>
        public override StringBuilder VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpr)
        {
            var type = arrayCreateExpr.Type.AcceptVisitor(this);
            var spec = arrayCreateExpr.AdditionalArraySpecifiers.First().AcceptVisitor(this);
            var init = arrayCreateExpr.Initializer.AcceptVisitor(this);

            if (spec.Length == 0)
            {
                var initCt = init.ToString().Count(str => str == ',') + 1;
                spec = new StringBuilder(initCt.ToString(CultureInfo.InvariantCulture));
            }

            type.Append("[").Append(spec).Append("] ");
            return new StringBuilder().Method(type.ToString(), init.ToString());
        }

        /// <summary>
        ///     Translates an array initializer, e.g. "{1, 2, 3, 4, ...}".
        /// </summary>
        public override StringBuilder VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitExpr)
        {
            return JoinArgs(arrayInitExpr.Elements);
        }

        /// <summary>
        ///     Translates a cast expression, e.g. "(float) value".
        /// </summary>
        public override StringBuilder VisitCastExpression(CastExpression castExpr)
        {
            var result = new StringBuilder();

            var castTo = castExpr.Type.AcceptVisitor(this);
            var castVal = castExpr.Expression.AcceptVisitor(this);

            return result.Method(castTo.ToString(), castVal.ToString());
        }

        /// <summary>
        ///     Translates a member reference, e.g. a field called "_value".
        /// </summary>
        public override StringBuilder VisitMemberReferenceExpression(MemberReferenceExpression memberRefExpr)
        {
            var result = new StringBuilder();

            if (!(memberRefExpr.Target is ThisReferenceExpression))
            {
                result = memberRefExpr.Target.AcceptVisitor(this);
                if (result != null && result.Length > 0) result.Dot();
            }

            var memberName = memberRefExpr.MemberName;
            if (memberRefExpr.Target is BaseReferenceExpression)
                memberName = memberName.Replace("xsl", "gl_");

            // save member reference
            var memberRef = memberRefExpr.Annotation<IMemberDefinition>();

            if (result != null && memberRef != null)
            {
                var instr = GetInstructionFromStmt(memberRefExpr.GetParent<Statement>());
                RefVariables.Add(new VariableDesc {Definition = memberRef, Instruction = instr});
            }

            return result != null ? result.Append(memberName) : new StringBuilder();
        }

        /// <summary>
        ///     Translates a base reference, i.e. "base.*".
        /// </summary>
        public override StringBuilder VisitBaseReferenceExpression(BaseReferenceExpression baseRefExpr)
        {
            return new StringBuilder();
        }

        /// <summary>
        ///     Translates an object creation, e.g. "new float4(...)".
        /// </summary>
        public override StringBuilder VisitObjectCreateExpression(ObjectCreateExpression objCreateExpr)
        {
            var type = objCreateExpr.Type.AcceptVisitor(this);
            var args = JoinArgs(objCreateExpr.Arguments);

            return type.Method(String.Empty, args.ToString());
        }

        /// <summary>
        ///     Translates a primitive type, e.g. "1f".
        /// </summary>
        public override StringBuilder VisitPrimitiveExpression(PrimitiveExpression primitiveExpr)
        {
            var result = new StringBuilder();
            var cultureInfo = CultureInfo.InvariantCulture.NumberFormat;

            if (primitiveExpr.Value is float)
            {
                var value = ((float) primitiveExpr.Value).ToString(cultureInfo);
                if (!value.Contains(".")) value += ".0";
                return result.Append(value).Append("f");
            }

            if (primitiveExpr.Value is double)
            {
                var value = ((double) primitiveExpr.Value).ToString(cultureInfo);
                if (!value.Contains(".")) value += ".0";
                return result.Append(value).Append("d");
            }

            if (primitiveExpr.Value is uint)
            {
                var value = ((uint) primitiveExpr.Value).ToString(cultureInfo);
                return result.Append(value).Append("u");
            }

            if (primitiveExpr.Value is bool)
            {
                var value = primitiveExpr.Value.ToString().ToLower();
                return result.Append(value);
            }

            return result.Append(primitiveExpr.Value);
        }

        /// <summary>
        ///     Translates a type reference, e.g. "OtherClass.".
        /// </summary>
        public override StringBuilder VisitTypeReferenceExpression(TypeReferenceExpression typeRefExpr)
        {
            var memberRef = typeRefExpr.GetParent<MemberReferenceExpression>();
            if (memberRef == null) return new StringBuilder();

            var instr = GetInstructionFromStmt(typeRefExpr.GetParent<Statement>());
            var name = memberRef.MemberName;

            DebugLog.Error("Static member '" + name + "' of class '" + typeRefExpr.Type + "' cannot be used", instr);

            return null;
        }

        /// <summary>
        ///     Translates an unary expression, e.g. "value++".
        /// </summary>
        public override StringBuilder VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOpExpr)
        {
            var result = new StringBuilder();

            var expr = unaryOpExpr.Expression.AcceptVisitor(this);

            // unary operator type mapping
            var opPreAsngmt = new Dictionary<UnaryOperatorType, string>
            {
                {UnaryOperatorType.Decrement, "--"},
                {UnaryOperatorType.Increment, "++"},
                {UnaryOperatorType.Minus, "-"},
                {UnaryOperatorType.Plus, "+"},
                {UnaryOperatorType.BitNot, "~"},
                {UnaryOperatorType.Not, "!"},
                {UnaryOperatorType.PostDecrement, "--"},
                {UnaryOperatorType.PostIncrement, "++"},
            };

            var opPostAsngmt = new Dictionary<UnaryOperatorType, string>
            {
                {UnaryOperatorType.PostDecrement, "--"},
                {UnaryOperatorType.PostIncrement, "++"},
            };

            if (opPreAsngmt.ContainsKey(unaryOpExpr.Operator))
                result.Append(opPreAsngmt[unaryOpExpr.Operator]).Append(expr);
            else if (opPostAsngmt.ContainsKey(unaryOpExpr.Operator))
                result.Append(expr).Append(opPostAsngmt[unaryOpExpr.Operator]);
            else
            {
                var dInstr = GetInstructionFromStmt(unaryOpExpr.GetParent<Statement>());
                DebugLog.Error("Unary operator '" + unaryOpExpr.Operator + "' is not supported", dInstr);
            }

            return result;
        }

        /// <summary>
        ///     Translates a conditional expression, e.g. "(x > 0) ? 1 : -1".
        /// </summary>
        public override StringBuilder VisitConditionalExpression(ConditionalExpression conditionalExpr)
        {
            var result = conditionalExpr.Condition.AcceptVisitor(this);
            var trueExpr = conditionalExpr.TrueExpression.AcceptVisitor(this);
            var falseExpr = conditionalExpr.FalseExpression.AcceptVisitor(this);

            return result.Append(" ? ").Append(trueExpr).Append(" : ").Append(falseExpr);
        }

        /// <summary>
        ///     Translates a default value expression, e.g. "float3()".
        /// </summary>
        public override StringBuilder VisitDefaultValueExpression(DefaultValueExpression defaultValueExpr)
        {
            var type = defaultValueExpr.Type.AcceptVisitor(this);
            return new StringBuilder().Method(type.ToString(), "0");
        }

        /// <summary>
        ///     Translates a direction expression, e.g. "ref value" or "out value".
        /// </summary>
        public override StringBuilder VisitDirectionExpression(DirectionExpression directionExpr)
        {
            return directionExpr.Expression.AcceptVisitor(this);
        }

        /// <summary>
        ///     Translates an identifier, e.g. a variable called "value".
        /// </summary>
        public override StringBuilder VisitIdentifierExpression(IdentifierExpression identifierExpr)
        {
            return new StringBuilder(identifierExpr.Identifier);
        }

        /// <summary>
        ///     Translates an indexer expression, e.g. "value[x]".
        /// </summary>
        public override StringBuilder VisitIndexerExpression(IndexerExpression indexerExpr)
        {
            var args = JoinArgs(indexerExpr.Arguments);
            var target = indexerExpr.Target.AcceptVisitor(this);
            return target.Append("[").Append(args).Append("]");
        }

        /// <summary>
        ///     Translates an invocation expression, e.g. "Math.Max(10, 5)".
        /// </summary>
        public override StringBuilder VisitInvocationExpression(InvocationExpression invocationExpr)
        {
            var result = new StringBuilder();

            var methodDef = invocationExpr.Annotation<MethodDefinition>() ??
                            invocationExpr.Annotation<MethodReference>().Resolve();

            var methodName = methodDef.Name;
            var declType = methodDef.DeclaringType.ToType();

            var args = JoinArgs(invocationExpr.Arguments).ToString();

            // map method if it's a mathematical method or
            // map method if it's a xSLShader class' method
            if (ShaderMapping.Types.ContainsKey(declType))
                if (ShaderMapping.Methods.ContainsKey(methodName))
                {
                    var mappedName = ShaderMapping.Methods[methodName];
                    return result.Method(mappedName, args);
                }

            // otherwise just call the method
            if (declType != typeof (xSLShader))
                RefMethods.Add(methodDef);

            return result.Method(methodDef.Name, args);
        }

        /// <summary>
        ///     Translates an if/else statement, e.g. "if (...) { ... } else { ... }".
        /// </summary>
        public override StringBuilder VisitIfElseStatement(IfElseStatement ifElseStmt)
        {
            var result = new StringBuilder();
            result.If(ifElseStmt.Condition.AcceptVisitor(this));

            var @true = (BlockStatement) ifElseStmt.TrueStatement;
            var trueStmt = @true.AcceptVisitor(this);

            if (@true.Statements.Count > 1)
                result.Block(trueStmt);
            else
                result.Intend().Append(trueStmt).Length -= 2;

            if (ifElseStmt.FalseStatement.IsNull)
                return result;

            var @false = (BlockStatement) ifElseStmt.FalseStatement;
            var falseStmt = @false.AcceptVisitor(this);

            if (@true.Statements.Count > 1)
                result.Else().Block(falseStmt);
            else
                result.Else().Intend().Append(falseStmt).Length -= 2;

            return result;
        }

        /// <summary>
        ///     Translates an return statement, e.g. "return value".
        /// </summary>
        public override StringBuilder VisitReturnStatement(ReturnStatement returnStmt)
        {
            var expr = returnStmt.Expression.AcceptVisitor(this);
            return new StringBuilder("return").Space().Append(expr).Semicolon();
        }
    }
}