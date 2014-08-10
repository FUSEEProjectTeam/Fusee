using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.Ast.Transforms;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CrossSL
{
    internal partial class ShaderVisitor : IAstVisitor<StringBuilder>
    {
        protected MethodDefinition CurrentMethod;
        protected ShaderMapping ShaderMapping;

        protected AstNode TopNode;

        protected internal StringBuilder Result { get; protected set; }

        protected internal Collection<MethodDefinition> RefMethods { get; protected set; }
        protected internal Collection<VariableDesc> RefVariables { get; protected set; }

        internal void Init(MethodDefinition methodDef)
        {
            CurrentMethod = methodDef;

            var type = methodDef.DeclaringType;
            var module = methodDef.DeclaringType.Module;

            var decContext = new DecompilerContext(module)
            {
                CurrentType = type,
                CurrentMethod = methodDef
            };

            // create AST and run optimization operations
            TopNode = AstMethodBodyBuilder.CreateMethodBody(methodDef, decContext);

            // replaces every "!(x == 5)" by "(x != 5)"
            var transform1 = (IAstTransform) new PushNegation();
            transform1.Run(TopNode);

            // replaces (unnecessary) while loops by for loops
            var transform2 = (IAstTransform) new PatternStatementTransform(decContext);
            transform2.Run(TopNode);

            // replaces every "x = Plus(x, y)" by "x += y", etc.
            var transform3 = (IAstTransform) new ReplaceMethodCallsWithOperators(decContext);
            transform3.Run(TopNode);

            // replaces every "var x; x = 5;" by "var x = 5;"
            var transform4 = (IAstTransform) new DeclareVariables(decContext);
            transform4.Run(TopNode);
        }

        internal void Translate(ShaderMapping shaderMapping)
        {
            ShaderMapping = shaderMapping;

            RefMethods = new Collection<MethodDefinition>();
            RefVariables = new Collection<VariableDesc>();

            Result = new StringBuilder().Block(TopNode.AcceptVisitor(this));
        }

        protected T GetAnnotations<T>(Statement stmt) where T : class
        {
            var typeRoleMap = new Dictionary<Type, int>
            {
                {typeof (ExpressionStatement), 0},
                {typeof (VariableDeclarationStatement), 1},
                {typeof (IfElseStatement), 2},
                {typeof (WhileStatement), 2},
                {typeof (ForStatement), 2}
            };

            if (!typeRoleMap.ContainsKey(stmt.GetType()))
                throw new ArgumentException("Statement type " + stmt.GetType() + " not supported.");

            switch (typeRoleMap[stmt.GetType()])
            {
                case 0:
                    return stmt.GetChildByRole(Roles.Expression).Annotation<T>();
                case 1:
                    return stmt.GetChildByRole(Roles.Variable).Annotation<T>();
                case 2:
                    return stmt.GetChildByRole(Roles.Condition).Annotation<T>();
                default:
                    throw new ArgumentException("Statement type " + stmt.GetType() + " not supported.");
            }
        }

        protected Instruction GetInstructionFromStmt(Statement stmt)
        {
            if (stmt == null) return null;

            var ilRange = GetAnnotations<List<ILRange>>(stmt).First();
            var instructions = CurrentMethod.Body.Instructions;
            return instructions.First(il => il.Offset == ilRange.From);
        }

        protected StringBuilder JoinArgs(ICollection<Expression> args)
        {
            var accArgs = args.Select(arg => arg.AcceptVisitor(this).ToString());
            return new StringBuilder(String.Join(", ", accArgs));
        }

        protected StringBuilder JoinArgs(ICollection<Statement> args)
        {
            var accArgs = args.Select(arg => arg.AcceptVisitor(this).ToString());
            return new StringBuilder(String.Join(", ", accArgs));
        }

        protected string MapDataTypeIfValid(AstType node, Type type)
        {
            if (type != null && ShaderMapping.Types.ContainsKey(type))
                return ShaderMapping.Types[type];

            var instr = GetInstructionFromStmt(node.GetParent<Statement>());
            DebugLog.Error("Type '" + type + "' is not supported", instr);

            return null;
        }
    }
}