using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace ScorpionLog.Test.Editor
{
    public class PerformanceLog
    {
        private const int Iterations = 10000;

        [Test]
        public void Performance_NoScope_NoProperty()
        {
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                Log.Debug($"message {i}");
            }

            sw.Stop();
            Debug.Log($"[NoScope/NoProp] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }

        [Test]
        public void Performance_WithScope_NoProperty()
        {
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                using (Log.BeginScope("PerfScope"))
                {
                    Log.Debug($"message {i}");
                }
            }

            sw.Stop();
            Debug.Log($"[WithScope/NoProp] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }

        [Test]
        public void Performance_WithScope_WithProperty()
        {
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                using (Log.BeginScope("PerfScope")
                           .SetProperty("env", "prod")
                           .SetProperty("iteration", i.ToString()))
                {
                    Log.Debug($"message {i}");
                }
            }

            sw.Stop();
            Debug.Log($"[WithScope/WithProp] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }

        [Test]
        public void Performance_NestedScope_WithProperty()
        {
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                using (Log.BeginScope("OuterScope").SetProperty("env", "prod"))
                using (Log.BeginScope("InnerScope").SetProperty("iteration", i.ToString()))
                {
                    Log.Debug($"message {i}");
                }
            }

            sw.Stop();
            Debug.Log($"[NestedScope/WithProp] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }

        [Test]
        public void Performance_WithFilter_Matched()
        {
            var filter = new LogFilter
            {
                filterParameters = new List<LogFilterParameter>
                {
                    new() { scopeName = "PerfScope", minimumLogLevel = LogLevel.Debug }
                }
            };

            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                using (Log.BeginScope("PerfScope"))
                {
                    Log.Debug($"message {i}");
                }
            }

            sw.Stop();
            Debug.Log($"[WithFilter/Matched] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }

        [Test]
        public void Performance_WithFilter_NotMatched()
        {
            var filter = new LogFilter
            {
                filterParameters = new List<LogFilterParameter>
                {
                    new() { scopeName = "OtherScope", minimumLogLevel = LogLevel.Debug }
                }
            };

            var sw = Stopwatch.StartNew();

            for (var i = 0; i < Iterations; i++)
            {
                using (Log.BeginScope("PerfScope"))
                {
                    Log.Debug($"message {i}");
                }
            }

            sw.Stop();
            Debug.Log($"[WithFilter/NotMatched] {Iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                      $"({sw.Elapsed.TotalMilliseconds / Iterations:F4}ms/call)");
        }
    }
}