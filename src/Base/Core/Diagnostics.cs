using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

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
        private static string _fileName;
        private static SeverityLevel _minLogLevelFile;
        private static SeverityLevel _minLogLevelConsole;
        private static Formater _format;

        /// <summary>
        ///     The methods used for formating messages
        /// </summary>
        /// <param name="callingMethod">The calling method</param>
        /// <param name="lvl">The severity level</param>
        /// <param name="msg">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="args">Possible arguments</param>
        public delegate string Formater(string callingMethod, SeverityLevel lvl, string msg, Exception ex = null, object[] args = null);

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
                return (1000.0 * ((double)_daWatch.ElapsedTicks)) / ((double) Stopwatch.Frequency);
            }
        }

        public enum SeverityLevel
        {
            TRACE = 0,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL
        }

        private static string SeverityLevelToString(SeverityLevel lvl)
        {
            switch (lvl)
            {
                case SeverityLevel.TRACE:
                    return "Trace";
                case SeverityLevel.DEBUG:
                    return "Debug";
                case SeverityLevel.INFO:
                    return "Info";
                case SeverityLevel.WARN:
                    return "Warning";
                case SeverityLevel.ERROR:
                    return "Error";
                case SeverityLevel.FATAL:
                    return "Fatal";
            }

            return "error while parsing severity level";
        }

        private static void ColorConsoleOutput(SeverityLevel lvl)
        {
            switch (lvl)
            {
                case SeverityLevel.TRACE:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case SeverityLevel.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case SeverityLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case SeverityLevel.WARN:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case SeverityLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case SeverityLevel.FATAL:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }
        }

        #endregion
        
        static Diagnostics()
        {
            _useFile = true;
            _fileName = "Fusee.Log.txt";
            _minLogLevelFile = SeverityLevel.ERROR;
            _minLogLevelConsole = SeverityLevel.TRACE;

            _format = (caller, lvl, msg, ex, args) =>
            {
                ColorConsoleOutput(lvl);

                var f = $"{DateTime.Now}, [{SeverityLevelToString(lvl)}] [{caller}()] {msg}";
                f += (ex != null ? $",\nException: {ex}" : "");
                if(args != null)
                {
                    f += "\nArguments:\n";

                    foreach (var a in args)
                        f += $"{a}\n";
                }

                return f + "\n";
            };
        }

        #region Members

        /// <summary>
        ///     Enable / disable text file logging
        /// </summary>
        /// <param name="logToTxtFile"></param>
        /// <param name="logFileName"></param>
        public static void LogToTextFile(bool logToTxtFile, string logFileName = "")
        {
            _useFile = logToTxtFile;
            _fileName = (logFileName == string.Empty ? "Fusee.Log.txt" : logFileName);
        }

        /// <summary>
        ///     Change the min logging severity level before logging is placed within the log txt file
        /// </summary>
        /// <param name="lvl"></param>
        public static void SetMinTextFileLoggingSeverityLevel(SeverityLevel lvl)
        {
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
        ///     Update the format of the logging messages
        /// </summary>
        /// <param name="formater"></param>
        public static void SetFormat(Formater formater)
        {
            _format = formater;
        }

        /// <summary>
        /// Log a debug output message to the respective output console.
        /// </summary>
        /// <param name="o">The object to log. Will be converted to a string.</param>
        /// <param name="logLevel">The level to log, see <see cref="SeverityLevel"></see> for a list</param>
        /// <param name="callerName">The calling method</param>
        [Obsolete("Please use the new logging methods (Trace, Debug, ...) instead")]
        public static void Log(object o, SeverityLevel logLevel = SeverityLevel.DEBUG, [CallerMemberName] string callerName = "")
        {
            o += $"\n[{callerName}()] INFO: Diagnostics.Log is deprecated, please use the new logging methods (Trace, Debug, ...)\n";

            if (_useFile && _minLogLevelFile <= logLevel)
                File.AppendAllText(_fileName, _format(callerName, logLevel, o.ToString()));
            
            if (_minLogLevelConsole <= logLevel)
                Console.WriteLine(_format(callerName, logLevel, o.ToString()));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a trace event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="callerName">The calling method</param>
        public static void Trace(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.TRACE)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.TRACE, message));

            if (_minLogLevelConsole <= SeverityLevel.TRACE)
                Console.WriteLine(_format(callerName, SeverityLevel.TRACE, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a debug event
        /// </summary>
        /// <param name="message">The message</param>    
        /// <param name="callerName">The calling method</param>
        public static void Debug(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.DEBUG)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.DEBUG, message));

            if (_minLogLevelConsole <= SeverityLevel.DEBUG)
                Console.WriteLine(_format(callerName, SeverityLevel.DEBUG, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a debug event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="callerName">The calling method</param>
        public static void Debug(string message, Exception ex, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.DEBUG)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.DEBUG, message, ex));

            if (_minLogLevelConsole <= SeverityLevel.DEBUG)
                Console.WriteLine(_format(callerName, SeverityLevel.DEBUG, message, ex));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a debug event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        public static void Debug(string message, Exception ex, object[] args, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.DEBUG)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.DEBUG, message, ex, args));

            if (_minLogLevelConsole <= SeverityLevel.DEBUG)
                Console.WriteLine(_format(callerName, SeverityLevel.DEBUG, message, ex, args));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log an info event
        /// </summary>
        /// <param name="message">The message</param>     
        /// <param name="callerName">The calling method</param>
        public static void Info(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.INFO)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.INFO, message));

            if (_minLogLevelConsole <= SeverityLevel.INFO)
                Console.WriteLine(_format(callerName, SeverityLevel.INFO, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log an info event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="callerName">The calling method</param>
        public static void Info(string message, Exception ex, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.INFO)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.INFO, message, ex));

            if (_minLogLevelConsole <= SeverityLevel.INFO)
                Console.WriteLine(_format(callerName, SeverityLevel.INFO, message, ex));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log an info event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        public static void Info(string message, Exception ex, object[] args, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.INFO)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.INFO, message, ex, args));

            if (_minLogLevelConsole <= SeverityLevel.INFO)
                Console.WriteLine(_format(callerName, SeverityLevel.INFO, message, ex, args));

            Console.ResetColor();
        }


        /// <summary>
        ///     Log a warning event
        /// </summary>
        /// <param name="message">The message</param>    
        /// <param name="callerName">The calling method</param>
        public static void Warn(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.WARN)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.WARN, message));

            if (_minLogLevelConsole <= SeverityLevel.WARN)
                Console.WriteLine(_format(callerName, SeverityLevel.WARN, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a warning event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="callerName">The calling method</param>
        public static void Warn(string message, Exception ex, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.WARN)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.WARN, message, ex));

            if (_minLogLevelConsole <= SeverityLevel.WARN)
                Console.WriteLine(_format(callerName, SeverityLevel.WARN, message, ex));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a warning event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        public static void Warn(string message, Exception ex, object[] args, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.WARN)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.WARN, message, ex, args));

            if (_minLogLevelConsole <= SeverityLevel.WARN)
                Console.WriteLine(_format(callerName, SeverityLevel.WARN, message, ex, args));

            Console.ResetColor();
        }



        /// <summary>
        ///     Log an error event
        /// </summary>
        /// <param name="message">The message</param> 
        /// <param name="callerName">The calling method</param>
        public static void Error(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.ERROR)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.ERROR, message));

            if (_minLogLevelConsole <= SeverityLevel.ERROR)
                Console.WriteLine(_format(callerName, SeverityLevel.ERROR, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log an error event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="callerName">The calling method</param>
        public static void Error(string message, Exception ex, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.ERROR)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.ERROR, message, ex));

            if (_minLogLevelConsole <= SeverityLevel.ERROR)
                Console.WriteLine(_format(callerName, SeverityLevel.ERROR, message, ex));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log an error event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        public static void Error(string message, Exception ex, object[] args, [CallerMemberName] string callerName = "")
        {
            StackTrace stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            if (_useFile && _minLogLevelFile <= SeverityLevel.ERROR)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.ERROR, message, ex, args));

            if (_minLogLevelConsole <= SeverityLevel.ERROR)
                Console.WriteLine(_format(callerName, SeverityLevel.ERROR, message, ex, args));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a fatal event
        /// </summary>
        /// <param name="message">The message</param>  
        /// <param name="callerName">The calling method</param>
        public static void Fatal(string message, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.FATAL)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.FATAL, message));

            if (_minLogLevelConsole <= SeverityLevel.FATAL)
                Console.WriteLine(_format(callerName, SeverityLevel.FATAL, message));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a fatal event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception</param>
        /// <param name="callerName">The calling method</param>
        public static void Fatal(string message, Exception ex, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.FATAL)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.FATAL, message, ex));

            if (_minLogLevelConsole <= SeverityLevel.FATAL)
                Console.WriteLine(_format(callerName, SeverityLevel.FATAL, message, ex));

            Console.ResetColor();
        }

        /// <summary>
        ///     Log a fatal event
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">A possible exception, optional</param>
        /// <param name="args">Possible arguments, optional</param>
        /// <param name="callerName">The calling method</param>
        public static void Fatal(string message, Exception ex, object[] args, [CallerMemberName] string callerName = "")
        {
            if (_useFile && _minLogLevelFile <= SeverityLevel.FATAL)
                File.AppendAllText(_fileName, _format(callerName, SeverityLevel.FATAL, message, ex, args));

            if (_minLogLevelConsole <= SeverityLevel.FATAL)
                Console.WriteLine(_format(callerName, SeverityLevel.FATAL, message, ex, args));

            Console.ResetColor();
        }


        #endregion
    }
}
