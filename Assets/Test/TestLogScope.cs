using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ScotchLog.Scope;

namespace ScotchLog.Test.Editor
{
    public class TestLogScope
    {
        // ─── Log.BeginScope ───────────────────────────────────────────────────

        [Test]
        public void BeginScope_SetsCurrentScopeName()
        {
            var scope = Log.BeginScope("myScope");

            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("myScope"));

            scope.Dispose();
        }

        [Test]
        public void BeginScope_EmptyName_SetsCurrentScope()
        {
            var scope = Log.BeginScope();

            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo(""));

            scope.Dispose();
        }

        [Test]
        public void Dispose_RestoresPreviousScope()
        {
            var outer = Log.BeginScope("outer");
            var inner = Log.BeginScope("inner");

            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("inner"));

            inner.Dispose();

            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("outer"));

            outer.Dispose();
        }

        [Test]
        public void Dispose_RootScope_RestoredAfterDispose()
        {
            var rootBeforeTest = LogScopeRecord.Current;

            var scope = Log.BeginScope("temp");
            scope.Dispose();

            Assert.That(LogScopeRecord.Current, Is.SameAs(rootBeforeTest));
        }

        // ─── SetProperty ─────────────────────────────────────────────────────

        [Test]
        public void SetProperty_StoresPropertyInScope()
        {
            var scope = Log.BeginScope("propScope").SetProperty("key1", "val1");

            var props = LogScopeRecord.Current.Properties;
            Assert.That(props, Contains.Key("key1"));
            Assert.That(props["key1"], Is.EqualTo("val1"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_MultipleProperties_AllStored()
        {
            var scope = Log.BeginScope("multiProp")
                .SetProperty("env", "prod")
                .SetProperty("region", "ap-northeast-1");

            var props = LogScopeRecord.Current.Properties;
            Assert.That(props["env"], Is.EqualTo("prod"));
            Assert.That(props["region"], Is.EqualTo("ap-northeast-1"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_OverwritesExistingKey()
        {
            var scope = Log.BeginScope("overwrite")
                .SetProperty("key", "first")
                .SetProperty("key", "second");

            Assert.That(LogScopeRecord.Current.Properties["key"], Is.EqualTo("second"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_NonStringValue_StoredAsToString()
        {
            var scope = Log.BeginScope("intVal").SetProperty("count", 42);

            Assert.That(LogScopeRecord.Current.Properties["count"], Is.EqualTo("42"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_WithCallerArgumentExpression_UsesVariableNameAsKey()
        {
            const string requestId = "req-001";
            var scope = Log.BeginScope("callerArg").SetProperty(requestId);

            var props = LogScopeRecord.Current.Properties;
            Assert.That(props, Contains.Key("requestId"));
            Assert.That(props["requestId"], Is.EqualTo("req-001"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_WithExplicitName_StoresProvidedKey()
        {
            const string requestId = "req-override";
            var scope = Log.BeginScope("callerArgOverride")
                .SetProperty("correlationId", requestId);

            var props = LogScopeRecord.Current.Properties;
            Assert.That(props, Contains.Key("correlationId"));
            Assert.That(props["correlationId"], Is.EqualTo("req-override"));
            Assert.That(props, Does.Not.ContainKey("requestId"));

            scope.Dispose();
        }

        [Test]
        public void SetProperty_WithCallerArgumentExpression_NullValueStoredAsNull()
        {
            var optionalMessage = CreateNullMessage();
            var scope = Log.BeginScope("callerArgNull").SetProperty(optionalMessage);

            var props = LogScopeRecord.Current.Properties;
            Assert.That(props, Contains.Key("optionalMessage"));
            Assert.That(props["optionalMessage"], Is.Null);

            scope.Dispose();

            string CreateNullMessage() => null;
        }

        // ─── Log エントリにスコープが付与されること ────────────────────────────
        // LogEntry はディスパッチ後にプールに返却（Dispose）されるため、
        // Scope などのプロパティはコールバック内で取り出す必要があります。

        [Test]
        public void BeginScope_LogEntry_ReceivesScopeName()
        {
            string capturedScopeName = null;

            using (Log.Listen(LogLevel.Trace, e => capturedScopeName = e.Scope?.Name))
            using (Log.BeginScope("captureScope"))
            {
                Log.Debug("in scope");
            }

            Assert.That(capturedScopeName, Is.EqualTo("captureScope"));
        }

        [Test]
        public void BeginScope_NestedScope_LogEntry_ReceivesInnerScopeName()
        {
            string capturedScopeName = null;

            using (Log.Listen(LogLevel.Trace, e => capturedScopeName = e.Scope?.Name))
            using (Log.BeginScope("outer"))
            using (Log.BeginScope("inner"))
            {
                Log.Debug("nested");
            }

            Assert.That(capturedScopeName, Is.EqualTo("inner"));
        }

        [Test]
        public void BeginScope_Properties_AppearInLogEntryScope()
        {
            string capturedValue = null;

            using (Log.Listen(LogLevel.Trace, e => capturedValue = e.Scope?.Properties?["svcName"]))
            using (Log.BeginScope("propsScope").SetProperty("svcName", "auth"))
            {
                Log.Debug("with props");
            }

            Assert.That(capturedValue, Is.EqualTo("auth"));
        }

        [Test]
        public void BeginScope_ListenerCapturedLog_KeepsScopeAfterScopeDisposed()
        {
            LogRecord captured = default;
            var hasCaptured = false;

            try
            {
                using (Log.Listen(LogLevel.Trace, e =>
                       {
                           captured = new LogRecord(e);
                           hasCaptured = true;
                       }))
                {
                    using (Log.BeginScope("persistedScope").SetProperty("requestId", "req-keep"))
                    {
                        Log.Debug("persist test");
                    }
                }

                Assert.That(hasCaptured, Is.True);
                Assert.That(captured.Scope.Name, Is.EqualTo("persistedScope"));
                Assert.That(captured.Scope.Properties["requestId"], Is.EqualTo("req-keep"));
            }
            finally
            {
                if (hasCaptured)
                {
                    captured.Dispose();
                }
            }
        }

        // ─── Log.BeginPropertyScope ───────────────────────────────────────────

        [Test]
        public void BeginPropertyScope_StoredAsProperty()
        {
            const string requestId = "req-123";
            string capturedValue = null;

            using (Log.Listen(LogLevel.Trace, e => capturedValue = e.Scope?.Properties?["requestId"]))
            using (Log.BeginPropertyScope(requestId))
            {
                Log.Debug("prop scope test");
            }

            Assert.That(capturedValue, Is.EqualTo("req-123"));
        }

        // ─── AsyncLocal 準拠のスコープ伝播 ───────────────────────────────────

        [Test]
        public async Task BeginScope_AcrossAwait_InheritsParentScopeState()
        {
            using var scope = Log.BeginScope("asyncParent").SetProperty("requestId", "req-async");

            await Task.Delay(1).ConfigureAwait(false);

            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("asyncParent"));
            Assert.That(LogScopeRecord.Current.Properties["requestId"], Is.EqualTo("req-async"));
        }

        [Test]
        public async Task BeginScope_TaskRun_InheritsParentScopeState()
        {
            using var scope = Log.BeginScope("taskParent");
            string childScopeName = null;

            await Task.Run(() =>
            {
                childScopeName = LogScopeRecord.Current.Name;
            });

            Assert.That(childScopeName, Is.EqualTo("taskParent"));
        }

        [Test]
        public async Task BeginScope_TaskRun_ChildScopeDoesNotAffectParentScope()
        {
            using var parentScope = Log.BeginScope("parentTask");
            string childNestedScopeName = null;

            await Task.Run(() =>
            {
                using (Log.BeginScope("childTask"))
                {
                    childNestedScopeName = LogScopeRecord.Current.Name;
                }
            });

            Assert.That(childNestedScopeName, Is.EqualTo("childTask"));
            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("parentTask"));
        }

        [Test]
        public void BeginScope_NewThread_InheritsParentScope_AndChildScopeDoesNotAffectParent()
        {
            using var parentScope = Log.BeginScope("parentThread");
            string childScopeBeforeBegin = null;
            string childScopeInside = null;
            Exception threadException = null;
            var childEnteredScope = new ManualResetEventSlim(false);
            var allowChildToExitScope = new ManualResetEventSlim(false);

            var thread = new Thread(() =>
            {
                try
                {
                    childScopeBeforeBegin = LogScopeRecord.Current.Name;

                    using (Log.BeginScope("childThread"))
                    {
                        childScopeInside = LogScopeRecord.Current.Name;

                        // 子スレッドが childThread スコープ内にいることを親へ通知し、
                        // 親が確認し終わるまで scope を抜けないように待機する。
                        childEnteredScope.Set();
                        if (!allowChildToExitScope.Wait(TimeSpan.FromSeconds(5)))
                        {
                            throw new TimeoutException("Timed out while waiting for parent thread verification.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    threadException = ex;
                }
            })
            {
                IsBackground = true
            };

            thread.Start();

            Assert.That(
                childEnteredScope.Wait(TimeSpan.FromSeconds(5)),
                Is.True,
                "Child thread did not enter child scope in time.");

            // 子スレッドが childThread scope 内にいる最中でも、親スレッドの Current は変わらない。
            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("parentThread"));

            allowChildToExitScope.Set();

            Assert.That(thread.Join(TimeSpan.FromSeconds(5)), Is.True, "Worker thread join timed out.");

            if (threadException != null)
            {
                Assert.Fail($"Worker thread raised exception: {threadException}");
            }

            Assert.That(childScopeBeforeBegin, Is.EqualTo("parentThread"));
            Assert.That(childScopeInside, Is.EqualTo("childThread"));
            Assert.That(LogScopeRecord.Current.Name, Is.EqualTo("parentThread"));
        }

        // ─── LogEntry 経由での AsyncLocal 準拠スコープ伝播 ─────────────────────

        [Test]
        public async Task BeginScope_TaskRun_LogEntry_InheritsParent_AndChildDoesNotAffectParent()
        {
            using var parentScope = Log.BeginScope("parentTaskEntry");
            string childBeforeScope = null;
            string childInsideScope = null;
            string parentDuringChildScope = null;
            var childEnteredScope = new ManualResetEventSlim(false);
            var allowChildToExitScope = new ManualResetEventSlim(false);

            using (Log.Listen(LogLevel.Trace, e =>
                   {
                       switch (e.Message)
                       {
                           case "task-entry-child-before":
                               childBeforeScope = e.Scope?.Name;
                               break;
                           case "task-entry-child-inside":
                               childInsideScope = e.Scope?.Name;
                               break;
                           case "task-entry-parent-during":
                               parentDuringChildScope = e.Scope?.Name;
                               break;
                       }
                   }))
            {
                var childTask = Task.Run(() =>
                {
                    Log.Debug("task-entry-child-before");

                    using (Log.BeginScope("childTaskEntry"))
                    {
                        childEnteredScope.Set();

                        if (!allowChildToExitScope.Wait(TimeSpan.FromSeconds(5)))
                        {
                            throw new TimeoutException("Timed out while waiting for parent log emission.");
                        }

                        Log.Debug("task-entry-child-inside");
                    }
                });

                Assert.That(
                    childEnteredScope.Wait(TimeSpan.FromSeconds(5)),
                    Is.True,
                    "Task.Run child did not enter child scope in time.");

                Log.Debug("task-entry-parent-during");

                allowChildToExitScope.Set();
                await childTask;
            }

            Assert.That(childBeforeScope, Is.EqualTo("parentTaskEntry"));
            Assert.That(childInsideScope, Is.EqualTo("childTaskEntry"));
            Assert.That(parentDuringChildScope, Is.EqualTo("parentTaskEntry"));
        }

        [Test]
        public void BeginScope_NewThread_LogEntry_InheritsParent_AndChildDoesNotAffectParent()
        {
            using var parentScope = Log.BeginScope("parentThreadEntry");
            string childBeforeScope = null;
            string childInsideScope = null;
            string parentDuringChildScope = null;
            Exception threadException = null;
            var childEnteredScope = new ManualResetEventSlim(false);
            var allowChildToExitScope = new ManualResetEventSlim(false);

            using (Log.Listen(LogLevel.Trace, e =>
                   {
                       switch (e.Message)
                       {
                           case "thread-entry-child-before":
                               childBeforeScope = e.Scope?.Name;
                               break;
                           case "thread-entry-child-inside":
                               childInsideScope = e.Scope?.Name;
                               break;
                           case "thread-entry-parent-during":
                               parentDuringChildScope = e.Scope?.Name;
                               break;
                       }
                   }))
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        Log.Debug("thread-entry-child-before");

                        using (Log.BeginScope("childThreadEntry"))
                        {
                            childEnteredScope.Set();

                            if (!allowChildToExitScope.Wait(TimeSpan.FromSeconds(5)))
                            {
                                throw new TimeoutException("Timed out while waiting for parent log emission.");
                            }

                            Log.Debug("thread-entry-child-inside");
                        }
                    }
                    catch (Exception ex)
                    {
                        threadException = ex;
                    }
                })
                {
                    IsBackground = true
                };

                thread.Start();

                Assert.That(
                    childEnteredScope.Wait(TimeSpan.FromSeconds(5)),
                    Is.True,
                    "Thread child did not enter child scope in time.");

                Log.Debug("thread-entry-parent-during");

                allowChildToExitScope.Set();

                Assert.That(thread.Join(TimeSpan.FromSeconds(5)), Is.True, "Worker thread join timed out.");
            }

            if (threadException != null)
            {
                Assert.Fail($"Worker thread raised exception: {threadException}");
            }

            Assert.That(childBeforeScope, Is.EqualTo("parentThreadEntry"));
            Assert.That(childInsideScope, Is.EqualTo("childThreadEntry"));
            Assert.That(parentDuringChildScope, Is.EqualTo("parentThreadEntry"));
        }

        // ─── LogScopeRecord 直接テスト ─────────────────────────────────────────

        [Test]
        public void LogScopeRecord_Start_HasPositiveId()
        {
            var record = LogScopeRecord.Start("direct");

            Assert.That(record.Id, Is.GreaterThan(0));

            record.End();
        }

        [Test]
        public void LogScopeRecord_Start_SetsName()
        {
            var record = LogScopeRecord.Start("directName");

            Assert.That(record.Name, Is.EqualTo("directName"));

            record.End();
        }

        [Test]
        public void LogScopeRecord_Start_SetsStartTimeUtc()
        {
            var before = DateTime.UtcNow;
            var record = LogScopeRecord.Start("timeScope");
            var after = DateTime.UtcNow;

            Assert.That(record.StartTimeUtc, Is.GreaterThanOrEqualTo(before));
            Assert.That(record.StartTimeUtc, Is.LessThanOrEqualTo(after));

            record.End();
        }

        [Test]
        public void LogScopeRecord_End_SetsEndTimeUtc()
        {
            var record = LogScopeRecord.Start("endScope");

            // CreateHolder() で参照カウントを増やし、End() 後もプールに戻らないようにする。
            // （参照がなければ End() → Pool.Release() → Deactivate() → Id=-1 となり
            //   プロパティアクセスで "Scope is already closed." が投げられる）
            var holder = record.CreateHolder();

            var before = DateTime.UtcNow;
            record.End();
            var after = DateTime.UtcNow;

            // EndTimeUtc は非ゼロ、かつ End() 呼び出し前後に収まる
            Assert.That(record.EndTimeUtc, Is.GreaterThanOrEqualTo(before));
            Assert.That(record.EndTimeUtc, Is.LessThanOrEqualTo(after));

            holder.Dispose(); // 参照を解放 → ここでプールに戻る
        }

        // ─── HasEnded ─────────────────────────────────────────────────────────

        [Test]
        public void LogScopeRecord_HasEnded_IsFalseBeforeEnd()
        {
            var record = LogScopeRecord.Start("hasEndedFalse");

            Assert.That(record.HasEnded, Is.False);

            record.End();
        }

        [Test]
        public void LogScopeRecord_HasEnded_IsTrueAfterEnd()
        {
            var record = LogScopeRecord.Start("hasEndedTrue");
            var holder = record.CreateHolder();

            record.End();

            Assert.That(record.HasEnded, Is.True);

            holder.Dispose();
        }

        [Test]
        public void LogScopeRecord_End_CalledTwice_ThrowsInvalidOperationException()
        {
            var record = LogScopeRecord.Start("doubleEnd");
            record.End();

            Assert.Throws<InvalidOperationException>(() => record.End());
        }

        [Test]
        public void LogScopeRecord_SetProperty_AfterEnd_ThrowsInvalidOperationException()
        {
            var record = LogScopeRecord.Start("closedProp");
            record.End();

            Assert.Throws<InvalidOperationException>(() => record.SetProperty("k", "v"));
        }

        [Test]
        public void LogScopeRecord_IsRoot_ForCurrentWithNoActiveScope_IsTrue()
        {
            // スコープが一切ない状態で Current は RootScope
            Assert.That(LogScopeRecord.Current.IsRoot, Is.True);
        }

        [Test]
        public void LogScopeRecord_IsRoot_ForNamedScope_IsFalse()
        {
            var scope = Log.BeginScope("nonRoot");

            Assert.That(LogScopeRecord.Current.IsRoot, Is.False);

            scope.Dispose();
        }

        // ─── LogScopeRecordHolder ─────────────────────────────────────────────

        [Test]
        public void LogScopeRecordHolder_Record_ReturnsCreatingScope()
        {
            var scope = Log.BeginScope("holderScope");
            var record = LogScopeRecord.Current;
            var holder = record.CreateHolder();

            Assert.That(holder.Record, Is.SameAs(record));

            holder.Dispose();
            scope.Dispose();
        }

        [Test]
        public void LogScopeRecordHolder_Dispose_DoesNotThrow()
        {
            var scope = Log.BeginScope("holderDispose");
            var holder = LogScopeRecord.Current.CreateHolder();

            Assert.DoesNotThrow(() => holder.Dispose());

            scope.Dispose();
        }
    }
}