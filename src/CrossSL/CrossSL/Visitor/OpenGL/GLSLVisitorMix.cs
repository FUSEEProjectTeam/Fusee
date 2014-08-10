using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace CrossSL
{
    internal sealed class GLSLVisitorMix : GLSLVisitor110
    {
        /// <summary>
        ///     Translates a while loop, e.g. "while (val) { ... }".
        /// </summary>
        /// <remarks>
        ///     GLSLES does not support while loops.
        /// </remarks>
        public override StringBuilder VisitWhileStatement(WhileStatement whileStmt)
        {
            var instr = GetInstructionFromStmt(whileStmt);
            DebugLog.Error("While loops are not supported in GLSLES", instr);

            return new StringBuilder();
        }
    }
}