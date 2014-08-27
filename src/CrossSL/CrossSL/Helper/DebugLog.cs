using System;
using System.IO;
using System.Text;
using Mono.Cecil.Cil;

namespace CrossSL
{
    // ReSharper disable once InconsistentNaming
    internal static class DebugLog
    {
        /// <summary>
        ///     Indicates if verbose mode is activ (i.e. if a .pdb file was found).
        /// </summary>
        /// <value>
        ///     <c>true</c> if activ; otherwise, <c>false</c>.
        /// </value>
        internal static bool Verbose { get; set; }

        /// <summary>
        ///     Indicates if warnings will be suppressed or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if warnings are disabled; otherwise, <c>false</c>.
        /// </value>
        public static bool Disabled { get; set; }

        /// <summary>
        ///     The file(name) to add to the output if no instructions are available.
        /// </summary>
        /// <value>
        ///     The default file.
        /// </value>
        internal static string DefaultFile { get; set; }

        /// <summary>
        ///     Indicates if an error was raised and the compiler has to abort.
        /// </summary>
        internal static bool Abort { get; set; }

        /// <summary>
        ///     Collects all error messages for further processing.
        /// </summary>
        internal static StringBuilder Errors { get; set; }

        /// <summary>
        ///     Resets the state of this <see cref="DebugLog" />.
        /// </summary>
        public static void Reset()
        {
            Abort = false;
            Disabled = false;

            Errors = new StringBuilder();
        }

        /// <summary>
        ///     Prints a warning to the console.
        /// </summary>
        /// <param name="msg">The warning message.</param>
        /// <param name="findSeq">The corresponding instruction.</param>
        internal static void Warning(string msg, Instruction findSeq = null)
        {
            if (Disabled) return;
            msg = "WARNING : CrossSL: " + msg;
            WriteToConsole(msg, findSeq);
        }

        /// <summary>
        ///     Prints an error to the console and sets <see cref="Abort" /> to [true].
        /// </summary>
        /// <param name="msg">The error message.</param>
        /// <param name="findSeq">The corresponding instruction.</param>
        internal static void Error(string msg, Instruction findSeq = null)
        {
            msg = "ERROR : CrossSL: " + msg;
            Errors.Append(WriteToConsole(msg, findSeq)).NewLine();

            Abort = true;
        }

        /// <summary>
        ///     Prints a message to the console.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="findSeq">The corresponding instruction.</param>
        /// <returns>The messages posted to the console.</returns>
        /// <remarks>
        ///     Some instructions have a SequencePoint which points to the specific line in
        ///     the source file. If the given line has no SequencePoint, this method will step
        ///     backwards until a SequencePoint is found (only if verbose mode is active).
        /// </remarks>
        private static string WriteToConsole(string msg, Instruction findSeq)
        {
            var result = new StringBuilder("      ");

            if (findSeq != null && Verbose)
            {
                while (findSeq.SequencePoint == null && findSeq.Previous != null)
                    findSeq = findSeq.Previous;

                if (findSeq.SequencePoint != null)
                {
                    var doc = findSeq.SequencePoint.Document.Url;
                    var line = findSeq.SequencePoint.StartLine;
                    var colmn = findSeq.SequencePoint.StartColumn;

                    result.Append(Path.GetFileName(doc) + "(" + line + "," + colmn + ") : ");
                }
            }

            if (result.Length == 6)
                result.Append(DefaultFile).Append("(1,1) : ");

            msg = result.Append(msg).Dot().ToString();
            Console.WriteLine(msg);

            return msg;
        }

        /// <summary>
        ///     Prints a usage error message to the console.
        /// </summary>
        /// <param name="msg">The message.</param>
        internal static void UsageError(string msg)
        {
            Console.WriteLine("\n\n" + msg + "\n\n");
            Console.WriteLine("----------------------------------------------------------\n");

            Console.ReadLine();
        }
    }
}