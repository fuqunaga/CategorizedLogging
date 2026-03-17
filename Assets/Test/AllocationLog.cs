using System;
using NUnit.Framework;
using UnityEngine;

namespace CategorizedLogging.Test.Editor
{
    public class AllocationLog
    {
        private static double MeasureAlloc(Action action, int count = 2000)
        {
            // ウォームアップ
            action();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var startBytes = GC.GetTotalMemory(true);
            for (var i = 0; i < count; i++)
            {
                action();
            }
            var endBytes = GC.GetTotalMemory(true);

            return (double)(endBytes - startBytes) / count;
        }

        // -------------------------------------------------------
        // スコープなし・プロパティなし
        // -------------------------------------------------------

        [Test]
        public void Alloc_NoScope_NoProperty()
        {
            var bytes = MeasureAlloc(() => Log.Debug("message"));
            Debug.Log($"[NoScope/NoProp] {bytes:F2} bytes/call");
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
            Debug.Log($"[OneScope/NoProp] {bytes:F2} bytes/call");
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
            Debug.Log($"[OneScope/OneProp] {bytes:F2} bytes/call");
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
            Debug.Log($"[OneScope/ThreeProps] {bytes:F2} bytes/call");
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
            Debug.Log($"[NestedScope/NoProp] {bytes:F2} bytes/call");
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
            Debug.Log($"[NestedScope/WithProps] {bytes:F2} bytes/call");
        }

        // -------------------------------------------------------
        // Log.Debug 呼び出し回数バリエーション（スコープなし）
        // -------------------------------------------------------

        [Test]
        public void Alloc_LogDebug_1Time_NoScope()
        {
            var bytes = MeasureAlloc(() => Log.Debug("message"), count: 1);
            Debug.Log($"[Debug x1/NoScope] {bytes:F2} bytes/call");
        }

        [Test]
        public void Alloc_LogDebug_100Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 100; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x100/NoScope] {bytes / 100:F2} bytes/call (total {bytes:F2} bytes/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_1000Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 1000; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x1000/NoScope] {bytes / 1000:F2} bytes/call (total {bytes:F2} bytes/iteration)");
        }

        [Test]
        public void Alloc_LogDebug_10000Times_NoScope()
        {
            var bytes = MeasureAlloc(() =>
            {
                for (var i = 0; i < 10000; i++) Log.Debug("message");
            }, count: 100);
            Debug.Log($"[Debug x10000/NoScope] {bytes / 10000:F2} bytes/call (total {bytes:F2} bytes/iteration)");
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
            Debug.Log($"[Debug x1/WithScope] {bytes:F2} bytes/call");
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
            Debug.Log($"[Debug x100/WithScope] {bytes / 100:F2} bytes/call (total {bytes:F2} bytes/iteration)");
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
            Debug.Log($"[Debug x1000/WithScope] {bytes / 1000:F2} bytes/call (total {bytes:F2} bytes/iteration)");
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
            Debug.Log($"[Debug x10000/WithScope] {bytes / 10000:F2} bytes/call (total {bytes:F2} bytes/iteration)");
        }
    }
}