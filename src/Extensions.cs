using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using client.Externals.Models;

namespace client
{
    public static class Extensions
    {
        public static TimeSpan ToTimeSpan(this PerformanceResult result)
            => new TimeSpan(0, (int)result.Hours, (int)result.Minutes, (int)result.Seconds, (int)result.Miliseconds);
        public static TimeSpan Average(this IEnumerable<TimeSpan> spans) => TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
    }

}