using System;
using System.Diagnostics;

namespace AKAInfluxDBdotnet.utils
{
    public class MeasureTime
    {
        public TO Measure<TO>(Func<TO> handler, Action<string> writer)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var output = handler();
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            var ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            writer("RunTime " + elapsedTime);
            writer("RunTime " + ts.TotalMilliseconds);
            return output;
        }
    }
}