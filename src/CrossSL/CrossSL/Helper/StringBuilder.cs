using System;
using System.Text;

namespace CrossSL
{
    // ReSharper disable once InconsistentNaming
    internal static partial class ExtensionMethods
    {
        internal static StringBuilder Space(this StringBuilder value)
        {
            return value.Append(" ");
        }

        internal static StringBuilder Dot(this StringBuilder value)
        {
            return value.Append(".");
        }

        internal static StringBuilder Semicolon(this StringBuilder value)
        {
            return value.Append(";");
        }

        internal static StringBuilder Assign(this StringBuilder value, string op = "")
        {
            return value.Append(" " + op + "= ");
        }

        internal static StringBuilder NewLine(this StringBuilder value, int lines = 1)
        {
            for (var i = 0; i < lines; i++)
                value.Append(Environment.NewLine);

            return value;
        }

        internal static StringBuilder Intend(this StringBuilder value, int level = 1)
        {
            for (var i = 0; i < level; i++)
                value.Append("\t");

            return value;
        }

        internal static StringBuilder Block(this StringBuilder value, StringBuilder content)
        {
            content.Replace(Environment.NewLine, Environment.NewLine + "\t").Length--;
            return value.Append("{").NewLine().Intend().Append(content).Append("}");
        }

        internal static StringBuilder Method(this StringBuilder value, string name, params string[] args)
        {
            return value.Append(name).Append("(" + String.Join(", ", args) + ")");
        }

        internal static StringBuilder If(this StringBuilder value, StringBuilder cond)
        {
            return value.Method("if ", cond.ToString()).NewLine();
        }

        internal static StringBuilder Else(this StringBuilder value)
        {
            return value.NewLine().Append("else").NewLine();
        }
    }
}