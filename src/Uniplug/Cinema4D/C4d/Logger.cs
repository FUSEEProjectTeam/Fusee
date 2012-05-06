using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    public class Logger
    {
        public enum LogLevel
        {
            All,
            Debug,
            Warn,
            Error,
            Off,
        }

        public static LogLevel Loglevel { get; set; }
        private static void Log(LogLevel level, string msg)
        {
            if (level >= Loglevel)
            {
                C4dApi.GePrint("MP: " + msg);
            }
        }
        public static void Debug(string msg)
        {
            Log(LogLevel.Debug, msg);
        }
        public static void Warn(string msg)
        {
            Log(LogLevel.Warn, msg);
        }
        public static void Error(string msg)
        {
            Log(LogLevel.Error, "ERROR - " + msg);
        }
        public static void Info(string msg)
        {
            Log(LogLevel.All, msg);
        }
    }
}
