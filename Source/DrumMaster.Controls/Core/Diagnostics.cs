using System.Diagnostics;
using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
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

        public static void DisplayRoutedEvent(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Debug.WriteLine("Routed Event Handler");
            Debug.WriteLine("====================");
            Debug.WriteLine($"Event: {e.RoutedEvent.OwnerType}.{e.RoutedEvent.Name} ({e.RoutedEvent.RoutingStrategy})");
            Debug.WriteLine($"Sender: {sender}");
            Debug.WriteLine($"Source: {e.Source}");
            Debug.WriteLine($"Orig Source: {e.OriginalSource}");
#endif
        }
    }
}
