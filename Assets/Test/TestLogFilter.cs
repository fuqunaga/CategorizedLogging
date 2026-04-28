using System.Collections.Generic;
using NUnit.Framework;
using ScotchLog.Scope;

namespace ScotchLog.Test.Editor
{
    public class TestLogFilter
    {
        private static LogFilter CreateFilter(string scopeName, LogLevel minimumLogLevel, List<LogScopeProperty> properties = null)
        {
            return new LogFilter
            {
                filterParameters = new List<LogFilterParameter>
                {
                    new() { scopeName = scopeName, minimumLogLevel = minimumLogLevel, properties = properties }
                }
            };
        }

        // -------------------------------------------------------
        // Equals
        // -------------------------------------------------------

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug };
            var b = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug };
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentScopeName_ReturnsFalse()
        {
            var a = new LogFilterParameter { scopeName = "A", minimumLogLevel = LogLevel.Debug };
            var b = new LogFilterParameter { scopeName = "B", minimumLogLevel = LogLevel.Debug };
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentLogLevel_ReturnsFalse()
        {
            var a = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug };
            var b = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Warning };
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_PropertiesSameOrderIndependent_ReturnsTrue()
        {
            var a = new LogFilterParameter
            {
                scopeName = "Test",
                minimumLogLevel = LogLevel.Debug,
                properties = new List<LogScopeProperty> { ("key1", "val1"), ("key2", "val2") }
            };
            var b = new LogFilterParameter
            {
                scopeName = "Test",
                minimumLogLevel = LogLevel.Debug,
                properties = new List<LogScopeProperty> { ("key2", "val2"), ("key1", "val1") }
            };
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_PropertiesDifferentValues_ReturnsFalse()
        {
            var a = new LogFilterParameter
            {
                scopeName = "Test",
                minimumLogLevel = LogLevel.Debug,
                properties = new List<LogScopeProperty> { ("key1", "val1") }
            };
            var b = new LogFilterParameter
            {
                scopeName = "Test",
                minimumLogLevel = LogLevel.Debug,
                properties = new List<LogScopeProperty> { ("key1", "valX") }
            };
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_BothPropertiesNull_ReturnsTrue()
        {
            var a = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug, properties = null };
            var b = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug, properties = null };
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_OnePropertiesNull_ReturnsFalse()
        {
            var a = new LogFilterParameter
            {
                scopeName = "Test",
                minimumLogLevel = LogLevel.Debug,
                properties = new List<LogScopeProperty> { ("key1", "val1") }
            };
            var b = new LogFilterParameter { scopeName = "Test", minimumLogLevel = LogLevel.Debug, properties = null };
            Assert.IsFalse(a.Equals(b));
        }

        // -------------------------------------------------------
        // LogLevel フィルタリング
        // -------------------------------------------------------

        [Test]
        public void Filter_AboveMinLevel_IsCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Debug);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("scope"))
            {
                Log.Warning("test");
            }

            Assert.IsNotNull(captured);
        }

        [Test]
        public void Filter_BelowMinLevel_IsNotCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Warning);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("scope"))
            {
                Log.Debug("test");
            }

            Assert.IsNull(captured);
        }

        // -------------------------------------------------------
        // ScopeName フィルタリング
        // -------------------------------------------------------

        [Test]
        public void Filter_Wildcard_IsCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Trace);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("AnyScopeName"))
            {
                Log.Debug("test");
            }

            Assert.IsNotNull(captured);
        }

        [Test]
        public void Filter_ExactScopeMatch_IsCaptured()
        {
            var filter = CreateFilter("MyScope", LogLevel.Trace);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("MyScope"))
            {
                Log.Debug("test");
            }

            Assert.IsNotNull(captured);
        }

        [Test]
        public void Filter_ScopeNameMismatch_IsNotCaptured()
        {
            var filter = CreateFilter("OtherScope", LogLevel.Trace);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("MyScope"))
            {
                Log.Debug("test");
            }

            Assert.IsNull(captured);
        }

        // -------------------------------------------------------
        // Properties フィルタリング
        // -------------------------------------------------------

        [Test]
        public void Filter_MatchingProperty_IsCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Trace, new List<LogScopeProperty> { ("env", "prod") });

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("scope").SetProperty("env", "prod"))
            {
                Log.Debug("test");
            }

            Assert.IsNotNull(captured);
        }

        [Test]
        public void Filter_NotMatchingProperty_IsNotCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Trace, new List<LogScopeProperty> { ("env", "prod") });

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("scope").SetProperty("env", "dev"))
            {
                Log.Debug("test");
            }

            Assert.IsNull(captured);
        }

        [Test]
        public void Filter_NullProperties_IsCaptured()
        {
            var filter = CreateFilter("*", LogLevel.Trace, null);

            LogEntry captured = null;
            using (Log.Listen(filter, e => captured = e))
            using (Log.BeginScope("scope"))
            {
                Log.Debug("test");
            }

            Assert.IsNotNull(captured);
        }
    }
}