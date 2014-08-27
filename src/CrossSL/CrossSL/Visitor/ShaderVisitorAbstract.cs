using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace CrossSL
{
    internal abstract partial class ShaderVisitor
    {
        public abstract StringBuilder VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpr);
        public abstract StringBuilder VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpr);
        public abstract StringBuilder VisitAssignmentExpression(AssignmentExpression assignmentExpr);
        public abstract StringBuilder VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpr);
        public abstract StringBuilder VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpr);
        public abstract StringBuilder VisitCastExpression(CastExpression castExpr);
        public abstract StringBuilder VisitConditionalExpression(ConditionalExpression conditionalExpr);
        public abstract StringBuilder VisitDefaultValueExpression(DefaultValueExpression defaultValueExpr);
        public abstract StringBuilder VisitDirectionExpression(DirectionExpression directionExpr);
        public abstract StringBuilder VisitIdentifierExpression(IdentifierExpression identifierExpr);
        public abstract StringBuilder VisitIndexerExpression(IndexerExpression indexerExpr);
        public abstract StringBuilder VisitInvocationExpression(InvocationExpression invocationExpr);
        public abstract StringBuilder VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpr);
        public abstract StringBuilder VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpr);
        public abstract StringBuilder VisitPrimitiveExpression(PrimitiveExpression primitiveExpr);
        public abstract StringBuilder VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpr);
        public abstract StringBuilder VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpr);
        public abstract StringBuilder VisitBlockStatement(BlockStatement blockStmt);
        public abstract StringBuilder VisitExpressionStatement(ExpressionStatement exprStmt);
        public abstract StringBuilder VisitForStatement(ForStatement forStmt);
        public abstract StringBuilder VisitIfElseStatement(IfElseStatement ifElseStmt);
        public abstract StringBuilder VisitReturnStatement(ReturnStatement returnStmt);
        public abstract StringBuilder VisitWhileStatement(WhileStatement whileStmt);
        public abstract StringBuilder VisitVariableInitializer(VariableInitializer variableInitializer);
        public abstract StringBuilder VisitSimpleType(SimpleType simpleType);
        public abstract StringBuilder VisitComposedType(ComposedType composedType);
        public abstract StringBuilder VisitArraySpecifier(ArraySpecifier arraySpecifier);
        public abstract StringBuilder VisitPrimitiveType(PrimitiveType primitiveType);

        public abstract StringBuilder VisitVariableDeclarationStatement(
            VariableDeclarationStatement variableDeclarationStmt);
    }
}