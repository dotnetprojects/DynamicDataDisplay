using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
    public class PerformanceCounter
    {
        private static Dictionary<string, TimeSpan> performanceCounters = new Dictionary<string, TimeSpan>();
        private static Dictionary<string, DateTime> stopwachesStarts = new Dictionary<string, DateTime>();

        public static void startStopwatch(string section) {
                stopwachesStarts[section] = DateTime.Now;
            }
        public static void stopStopwatch(string section) {
            DateTime startTime = stopwachesStarts[section];
            TimeSpan diff = DateTime.Now - startTime;
            if (!performanceCounters.ContainsKey(section)) performanceCounters[section] = new TimeSpan(0);
            performanceCounters[section] += diff;
        }

        public static string GetString()
        {
            TimeSpan overall = new TimeSpan();
            foreach (KeyValuePair<string, TimeSpan> kvp in performanceCounters) overall += kvp.Value;

            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string,TimeSpan> kvp in performanceCounters) {
            strBuilder.AppendLine(kvp.Key+":\t\t"+kvp.Value +"("+((double)kvp.Value.Ticks/(double)overall.Ticks*100.0)+"%)");
            }
            return strBuilder.ToString();
        }
    }
}
