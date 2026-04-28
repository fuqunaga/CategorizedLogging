using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Profiling;

namespace ScotchLog.Test.Editor
{
    public class AllocationLog
    {
        private static double MeasureAlloc(Action action, int count = 2000, string sampleName = null,
            [CallerMemberName] string callerMemberName = "")
        {
            // ウォームアップ
            action();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var resolvedSampleName = sampleName
                ?? TestContext.CurrentContext?.Test?.Name
                ?? callerMemberName
                ?? nameof(MeasureAlloc);

            var startBytes = GC.GetTotalMemory(false);
            Profiler.BeginSample($"Alloc/{resolvedSampleName}");
            for (var i = 0; i < count; i++)
            {
                action();
            }
            Profiler.EndSample();
            var endBytes = GC.GetTotalMemory(false);

            return (double)(endBytes - startBytes) / count;
        }

        private static string FormatBytes(double bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:F2} {sizes[order]}";
        }

        // -------------------------------------------------------
        // スコープなし・プロパティなし
        // -------------------------------------------------------

        [Test]
        public void Alloc_NoScope_NoProperty()
        {
            var bytes = MeasureAlloc(() => Log.Debug("message"));
            Debug.Log($"[NoScope/NoProp] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // スコープあり（1つ）・プロパティなし
        // -------------------------------------------------------

        [Test]
        public void Alloc_OneScope_NoProperty()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope"))
                {
                    Log.Debug("message");
                }
            });
            Debug.Log($"[OneScope/NoProp] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // スコープあり（1つ）・プロパティ1つ
        // -------------------------------------------------------

        [Test]
        public void Alloc_OneScope_OneProperty()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                {
                    Log.Debug("message");
                }
            });
            Debug.Log($"[OneScope/OneProp] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // スコープあり（1つ）・プロパティ3つ
        // -------------------------------------------------------

        [Test]
        public void Alloc_OneScope_ThreeProperties()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope")
                           .SetProperty("env", "prod")
                           .SetProperty("region", "jp")
                           .SetProperty("version", "1.0"))
                {
                    Log.Debug("message");
                }
            });
            Debug.Log($"[OneScope/ThreeProps] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // ネストスコープ（2つ）・プロパティなし
        // -------------------------------------------------------

        [Test]
        public void Alloc_NestedScope_NoProperty()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Outer"))
                using (Log.BeginScope("Inner"))
                {
                    Log.Debug("message");
                }
            });
            Debug.Log($"[NestedScope/NoProp] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // ネストスコープ（2つ）・各プロパティ2つ
        // -------------------------------------------------------

        [Test]
        public void Alloc_NestedScope_WithProperties()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Outer").SetProperty("env", "prod").SetProperty("region", "jp"))
                using (Log.BeginScope("Inner").SetProperty("userId", "123").SetProperty("action", "login"))
                {
                    Log.Debug("message");
                }
            });
            Debug.Log($"[NestedScope/WithProps] {FormatBytes(bytes)}/call");
        }

        // -------------------------------------------------------
        // Log.Debug 呼び出し回数バリエーション（スコープなし）
        // -------------------------------------------------------

        [Test]
        public void Alloc_LogDebug_1Time_NoScope()
        {
            var bytes = MeasureAlloc(() => Log.Debug("message"), count: 1);
            Debug.Log($"[Debug x1/NoScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_100Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 100; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x100/NoScope] {FormatBytes(bytes / 100)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_1000Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 1000; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x1000/NoScope] {FormatBytes(bytes / 1000)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_10000Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 10000; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x10000/NoScope] {FormatBytes(bytes / 10000)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        // -------------------------------------------------------
        // Log.Debug 呼び出し回数バリエーション（スコープあり）
        // -------------------------------------------------------

        [Test]
        public void Alloc_LogDebug_1Time_WithScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                {
                    Log.Debug("message");
                }
            }, count: 1);
            Debug.Log($"[Debug x1/WithScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_100Times_WithScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                {
                    for (var i = 0; i < 100; i++) Log.Debug("message");
                }
            }, count: 100);
            Debug.Log($"[Debug x100/WithScope] {FormatBytes(bytes / 100)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_1000Times_WithScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                {
                    for (var i = 0; i < 1000; i++) Log.Debug("message");
                }
            }, count: 100);
            Debug.Log($"[Debug x1000/WithScope] {FormatBytes(bytes / 1000)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_10000Times_WithScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                {
                    for (var i = 0; i < 10000; i++) Log.Debug("message");
                }
            }, count: 100);
            Debug.Log($"[Debug x10000/WithScope] {FormatBytes(bytes / 10000)}/call (total {FormatBytes(bytes)}/iteration)");
        }

        // -------------------------------------------------------
        // 大量 float を "," 区切り文字列にして Log.Debug する
        // -------------------------------------------------------

        private static float[] CreateSequentialFloats(int count)
        {
            var values = new float[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = i * 0.1f;
            }
            return values;
        }

        private static string BuildFloatCsv(float[] values) =>
            values.Length == 0
                ? string.Empty
                : string.Join(",", values.Select(v => v.ToString("G9", CultureInfo.InvariantCulture)));

        private static double MeasureFloatCsvLogAlloc(int floatCount, int measureCount, bool withScope = false)
        {
            var values = CreateSequentialFloats(floatCount);
            var message = BuildFloatCsv(values);
            return MeasureAlloc(() =>
            {
                if (withScope)
                {
                    using (Log.BeginScope("Scope").SetProperty("env", "prod"))
                    {
                        Log.Debug(message);
                    }
                }
                else
                {
                    Log.Debug(message);
                }
            }, measureCount);
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_100_NoScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 100, measureCount: 500);
            Debug.Log($"[Debug/FloatCsv x100/NoScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_1000_NoScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 1000, measureCount: 200);
            Debug.Log($"[Debug/FloatCsv x1000/NoScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_10000_NoScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 10000, measureCount: 50);
            Debug.Log($"[Debug/FloatCsv x10000/NoScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_100_WithScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 100, measureCount: 500, withScope: true);
            Debug.Log($"[Debug/FloatCsv x100/WithScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_1000_WithScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 1000, measureCount: 200, withScope: true);
            Debug.Log($"[Debug/FloatCsv x1000/WithScope] {FormatBytes(bytes)}/call");
        }

        [Test]
        public void Alloc_LogDebug_FloatCsv_10000_WithScope()
        {
            var bytes = MeasureFloatCsvLogAlloc(floatCount: 10000, measureCount: 50, withScope: true);
            Debug.Log($"[Debug/FloatCsv x10000/WithScope] {FormatBytes(bytes)}/call");
        }
    }
}

