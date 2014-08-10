using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace CrossSL
{
    internal abstract partial class ShaderVisitor
    {
        private StringBuilder InvalidNode(AstNode invalidNode, string msg)
        {
            DebugLog.Error(msg, GetInstructionFromStmt(invalidNode.GetParent<Statement>()));
            return new StringBuilder();
        }

        public StringBuilder VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpr)
        {
            return InvalidNode(anonymousMethodExpr, "Anonymous method expressions are not supported");
        }

        public StringBuilder VisitUndocumentedExpression(UndocumentedExpression undocumentedExpr)
        {
            return InvalidNode(undocumentedExpr, "Non-standard language extensions are not supported");
        }

        public StringBuilder VisitAsExpression(AsExpression asExpr)
        {
            return InvalidNode(asExpr, "Type casts with keyword 'as' are not supported");
        }

        public StringBuilder VisitQueryExpression(QueryExpression queryExpr)
        {
            return InvalidNode(queryExpr, "LINQ is not supported");
        }

        public StringBuilder VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
        {
            return InvalidNode(queryContinuationClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryFromClause(QueryFromClause queryFromClause)
        {
            return InvalidNode(queryFromClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryLetClause(QueryLetClause queryLetClause)
        {
            return InvalidNode(queryLetClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryWhereClause(QueryWhereClause queryWhereClause)
        {
            return InvalidNode(queryWhereClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryJoinClause(QueryJoinClause queryJoinClause)
        {
            return InvalidNode(queryJoinClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryOrderClause(QueryOrderClause queryOrderClause)
        {
            return InvalidNode(queryOrderClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryOrdering(QueryOrdering queryOrdering)
        {
            return InvalidNode(queryOrdering, "LINQ is not supported");
        }

        public StringBuilder VisitQuerySelectClause(QuerySelectClause querySelectClause)
        {
            return InvalidNode(querySelectClause, "LINQ is not supported");
        }

        public StringBuilder VisitQueryGroupClause(QueryGroupClause queryGroupClause)
        {
            return InvalidNode(queryGroupClause, "LINQ is not supported");
        }
    }
}