using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace CrossSL
{
    internal sealed class GLSLVisitor100 : GLSLVisitor
    {
        /// <summary>
        ///     Translates a while loop, e.g. "while (val) { ... }".
        /// </summary>
        /// <remarks>
        ///     GLSLES does not support while loops.
        /// </remarks>
        public override StringBuilder VisitWhileStatement(WhileStatement whileStmt)
        {
            var instr = GetInstructionFromStmt(whileStmt.GetParent<Statement>());
            DebugLog.Error("While loops are not supported in GLSLES", instr);

            return new StringBuilder();
        }
    }
}