using JSIL.Meta;
using System.Diagnostics;

namespace Fusee.Engine
{
     /// <summary>
     /// Contains some mostly static functions for diagnostic purposes.
     /// </summary>
    public static class Diagnostics
    {
        #region Fields

        [JSIgnore]
        private static Stopwatch _daWatch;

        
        /// <summary>
        /// High precision timer values.
        /// </summary>
        /// <value>
        /// A double value containing consecutive real time values in milliseconds.
        /// </value>
        /// <remarks>
        /// To measure the elapsed time between two places in code get this value twice and calculate the difference.
        /// </remarks>
        [JSExternal]      
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

        #endregion

        #region Members

        /// <summary>
        /// Log a debug output message to the respective output console.
        /// </summary>
        /// <param name="o">The object to log. Will be converted to a string.</param>
        [JSExternal]
        public static void Log(object o)
        {
            Debug.Print(o.ToString());
        }

        #endregion
    }
}
