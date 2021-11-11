using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Contains mostly static functions for diagnostic purposes.
    /// </summary>
    public static class Diagnostics
    {
        #region Fields

        private static Stopwatch _daWatch;
        private static bool _useFile;
        private static string _fileName = "Fusee.Log.txt";
        private static SeverityLevel _minLogLevelFile = SeverityLevel.None;
        private static SeverityLevel _minLogLevelConsole;
        private static SeverityLevel _minLogLevelDebug;

        private static Formater _format = (caller, lineNumber, callerFile, lvl, msg, ex, args) =>
                    {
                        ColorConsoleOutput(lvl);

                        string f;

                        if (lvl == SeverityLevel.Debug)
                        {
                            f = $"[{caller}(){(lineNumber != 0 ? ":" + lineNumber : "")}] {msg}";
                        }
                        else
                        {
                            f = $"{DateTime.Now}, [{SeverityLevelToString(lvl)}] {(!string.IsNullOrEmpty(callerFile) ? "[" + Path.GetFileName(callerFile) + "]" : "")} [{caller}(){(lineNumber != 0 ? ":" + lineNumber : "")}] {msg}";
                        }

                        f += (ex != null ? $",\nException: {ex}" : "");
                        f += (ex?.InnerException != null ? $",\nInner exception: {ex.InnerException}" : "");

                        if (args != null)
                        {
                            f += "\nArguments:\n";

                            foreach (var a in args)
                            {
                                var argMsg = a == null ? "<null>" : a.ToString();
                                f += $"{argMsg}\n";
                            }

                        }

                        return f + "\n";
                    };

        /// <summary>
        ///     The methods used for formating messages
        /// </summary>
        /// <param name="callingMethod">The calling method</param>
        /// <param name="lineNumber">The line number of the calling file</param>
        /// <param name="sourceFilePath">The calling file</param>
        /// <param name="lvl">The severity level</param>
        /// <param name="msg">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="args">Possible arguments</param>
        public delegate string Formater(string callingMethod, int lineNumber, string sourceFilePath, SeverityLevel lvl, string msg, Exception ex = null, object[] args = null);

        /// <summary>
        /// High precision timer values.
        /// </summary>
        /// <value>
        /// A double value containing consecutive real time values in milliseconds.
        /// </value>
        /// <remarks>
        /// To measure the elapsed time between two places in code get this value twice and calculate the difference.
        /// </remarks>
        public static double Timer
        {
            get
            {
                if (_daWatch == null)
                {
                    _daWatch = new Stopwatch();
                    _daWatch.Start();
                }
                return (1000.0 * ((double)_daWatch.ElapsedTicks)) / ((double)Stopwatch.Frequency);
            }
        }

        /// <summary>
        ///     The severity level at which logging is enabled
        /// </summary>
        public enum SeverityLevel
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            Verbose = -1,
            Debug = 0,
            Info,
            Warn,
            Error,
            None = 42
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        private static string SeverityLevelToString(SeverityLevel lvl)
        {
            return lvl switch
            {
                SeverityLevel.Verbose => "Verbose",
                SeverityLevel.Debug => "Debug",
                SeverityLevel.Info => "Info",
                SeverityLevel.Warn => "Warning",
                SeverityLevel.Error => "Error",
                SeverityLevel.None => "None",
                _ => "Error while parsing severity level",
            };
        }

        private static void ColorConsoleOutput(SeverityLevel lvl)
        {
            switch (lvl)
            {
                case SeverityLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case SeverityLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case SeverityLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case SeverityLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case SeverityLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }

        #endregion

        #region Members

        /// <summary>
        ///     Enable / disable text file logging, default is disabled
        /// </summary>
        /// <param name="logToTxtFile"></param>
        /// <param name="logFileName"></param>
        public static void LogToTextFile(bool logToTxtFile, string logFileName = "")
        {
#if ANDROID
            throw new NotImplementedException("File log does not work within Android, yet!");
#endif
            _useFile = logToTxtFile;
            _fileName = (logFileName == string.Empty ? "Fusee.Log.txt" : logFileName);
            if (_useFile && !File.Exists(_fileName)) File.Create(_fileName).Close();
        }

        /// <summary>
        ///     Change the min logging severity level before logging is placed within the log text file
        /// </summary>
        /// <param name="lvl"></param>
        public static void SetMinTextFileLoggingSeverityLevel(SeverityLevel lvl)
        {
#if ANDROID
            throw new NotImplementedException("File log does not work within Android, yet!");
#endif

            if (!_useFile)
                Warn("Level set without enabled text file logging. Please enable text file logging fist via LogToTextFile(true)");

            _minLogLevelFile = lvl;
        }

        /// <summary>
        ///     Change the min logging severity level before logging is written to the console
        /// </summary>
        /// <param name="lvl"></param>
        public static void SetMinConsoleLoggingSeverityLevel(SeverityLevel lvl)
        {
            _minLogLevelConsole = lvl;
        }

        /// <summary>
        ///     Change the min logging severity level before logging is written to the debug output
        /// </summary>
        /// <param name="lvl"></param>
        public static void SetMinDebugOutputLoggingSeverityLevel(SeverityLevel lvl)
        {
            _minLogLevelDebug = lvl;
        }

        /// <summary>
        ///     Update the format of the logging messages
        /// </summary>
        /// <param name="formater"></param>
        public static void SetFormat(Formater formater)
        {
            _format = formater;
        }

        private static void Writer(object o, SeverityLevel logLevel, Exception ex = null, object[] args = null, string callerName = "", int sourceLineNumber = 0, string sourceFilePath = "")
        {
            var msg = o == null ? "<null>" : o.ToString();

            if (_useFile && _minLogLevelFile <= logLevel)
                File.AppendAllText(_fileName, _format(callerName, sourceLineNumber, sourceFilePath, logLevel, msg, ex, args));

            if (_minLogLevelConsole <= logLevel && Console.Out != null)
                Console.WriteLine(_format(callerName, sourceLineNumber, sourceFilePath, logLevel, msg));

            if (_minLogLevelDebug <= logLevel || Console.Out == null) // when there is no console present (android, wasm, etc. log anything to debug output
                System.Diagnostics.Debug.WriteLine(_format(callerName, sourceLineNumber, sourceFilePath, logLevel, msg, ex, args));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a debug output message to the respective output console.
        /// </summary>
        /// <param name="o">The object to log. Will be converted to a string.</param>
        /// <param name="logLevel">The level to log, see <see cref="SeverityLevel"></see> for a list</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        [Obsolete("Please use the new logging methods (Debug, Warn, Error) instead")]
        [Conditional("DEBUG")]
        public static void Log(object o, SeverityLevel logLevel = SeverityLevel.Debug, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, logLevel, null, null, callerName, sourceLineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Log a debug event.
        ///     Per default visible within the Visual Studio debug console and the console window in debug builds.
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        [Conditional("DEBUG")]
        public static void Verbose(object o, Exception ex = null, object[] args = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, SeverityLevel.Verbose, ex, args, callerName, sourceLineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Log a debug event.
        ///     Per default visible within the Visual Studio debug console and the console window in debug builds.
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        [Conditional("DEBUG")]
        public static void Debug(object o, Exception ex = null, object[] args = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, SeverityLevel.Debug, ex, args, callerName, sourceLineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Log a info event.
        ///     Per default visible within the Visual Studio debug console and the console window in debug builds, as well as in release builds.
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        public static void Info(object o, Exception ex = null, object[] args = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, SeverityLevel.Info, ex, args, callerName, sourceLineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Log a warning event.
        ///     Per default visible within the Visual Studio debug console and the console window in debug builds.
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        public static void Warn(object o, Exception ex = null, object[] args = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, SeverityLevel.Warn, ex, args, callerName, sourceLineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Log an error event.
        ///     Per default visible within the Visual Studio debug console and the console window in debug and release builds.
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        /// <param name="sourceLineNumber">The line number, optional.</param>
        /// <param name="sourceFilePath">The file path, optional.</param>
        public static void Error(object o, Exception ex = null, object[] args = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = "")
        {
            Writer(o, SeverityLevel.Error, ex, args, callerName, sourceLineNumber, sourceFilePath);
        }

        #endregion
    }
}