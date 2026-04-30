using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ScotchLog.Test.Editor
{
    public class TestLog_Listen
    {
        // ─── LogLevel overload + Action<string> ───────────────────────────

        [Test]
        public void Listen_LogLevel_StringCallback_ReceivesMessage()
        {
            var received = new List<string>();
            using (Log.Listen(LogLevel.Trace, (string msg) => received.Add(msg)))
            {
                Log.Information("hello");
            }

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.EqualTo("hello"));
        }

        // ─── LogLevel overload + Action<LogRecord> ────────────────────────

        [Test]
        public void Listen_LogLevel_RecordCallback_ReceivesRecord()
        {
            var received = new List<(LogLevel logLevel, string message)>();
            using (Log.Listen(LogLevel.Trace, r => received.Add((r.LogLevel, r.Message))))
            {
                Log.Warning("warn");
            }

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0].logLevel, Is.EqualTo(LogLevel.Warning));
            Assert.That(received[0].message, Is.EqualTo("warn"));
        }

        // ─── LogFilter overload + Action<string> ─────────────────────────

        [Test]
        public void Listen_LogFilter_StringCallback_FiltersCorrectly()
        {
            var received = new List<string>();
            var filter = LogFilter.Create(LogLevel.Warning);
            using (Log.Listen(filter, (string msg) => received.Add(msg)))
            {
                Log.Debug("debug");      // filtered out
                Log.Warning("warn");
                Log.Error("error");
            }

            Assert.That(received, Has.Count.EqualTo(2));
        }

        // ─── LogFilter overload + Action<LogRecord> ───────────────────────

        [Test]
        public void Listen_LogFilter_RecordCallback_FiltersCorrectly()
        {
            var received = new List<LogLevel>();
            var filter = LogFilter.Create(LogLevel.Error);
            using (Log.Listen(filter, r => received.Add(r.LogLevel)))
            {
                Log.Information("info"); // filtered out
                Log.Error("err");
            }

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.EqualTo(LogLevel.Error));
        }

        // ─── Dispose でリッスンが停止する ─────────────────────────────────

        [Test]
        public void Listen_AfterDispose_NoLongerReceives()
        {
            var received = new List<string>();
            using (Log.Listen(LogLevel.Trace, received.Add))
            {
                Log.Information("inside");
            }
            Log.Information("outside");

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.EqualTo("inside"));
        }

        // ─── async/await で別スレッドになっても受信できる ─────────────────

        [Test]
        public async Task Listen_AcrossAwait_StillReceivesRecord()
        {
            var received = new List<string>();
            using (Log.Listen(LogLevel.Trace, r => received.Add(r.Message)))
            {
                // ConfigureAwait(false) で継続が別スレッドになる可能性あり
                await Task.Yield();
                Log.Information("after yield");

                await Task.Run(() => Log.Debug("from Task.Run"));
            }

            Assert.That(received, Has.Count.EqualTo(2));
            Assert.That(received[0], Is.EqualTo("after yield"));
            Assert.That(received[1], Is.EqualTo("from Task.Run"));
        }

        [Test]
        public async Task Listen_AsyncStringCallback_AcrossAwait_StillReceivesMessage()
        {
            var received = new List<string>();
            using (Log.Listen(LogLevel.Trace, (string msg) => received.Add(msg)))
            {
                await Task.Delay(1).ConfigureAwait(false); // 別スレッド継続
                Log.Warning("after delay");
            }

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.EqualTo("after delay"));
        }

        [Test]
        public async Task Listen_TaskRun_ReceivesLog()
        {
            var received = new List<string>();
            using (Log.Listen(LogLevel.Trace, (r) => received.Add(r.Message)))
            {
                await Task.Run(() =>
                {
                    Log.Error("from thread pool");
                });
            }

            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.EqualTo("from thread pool"));
        }
    }
}