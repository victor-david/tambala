using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Provides some simple diagnositics.
    /// </summary>
    internal static class Diagnostics
    {
#if DEBUG
        private static readonly Stopwatch Stopwatch = new Stopwatch();
#endif
        /// <summary>
        /// Starts the timer.
        /// </summary>
        public static void StartTimer()
        {
#if DEBUG
            Stopwatch.Restart();
#endif
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public static void StopTimer()
        {
#if DEBUG
            Stopwatch.Stop();
#endif
        }

        /// <summary>
        /// Stops the timer and displays a message in the debug output
        /// with the elapsed milliseconds.
        /// </summary>
        /// <param name="message">The message</param>
        public static void StopTimer(string message)
        {
#if DEBUG
            Stopwatch.Stop();
            Debug.WriteLine($"{message} ms:{Stopwatch.ElapsedMilliseconds}");
#endif
        }
    }
}
