using System;
using NUnit.Framework;

namespace ScotchLog.Test.Editor
{
    public class TestLog_Output
    {
        [Test]
        public void LogTrace_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Trace("trace message");
            }

            Assert.AreEqual(LogLevel.Trace, capturedLevel);
            Assert.AreEqual("trace message", capturedMessage);
        }

        [Test]
        public void LogDebug_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Debug("debug message");
            }

            Assert.AreEqual(LogLevel.Debug, capturedLevel);
            Assert.AreEqual("debug message", capturedMessage);
        }

        [Test]
        public void LogInformation_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Information("information message");
            }

            Assert.AreEqual(LogLevel.Information, capturedLevel);
            Assert.AreEqual("information message", capturedMessage);
        }

        [Test]
        public void LogWarning_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Warning("warning message");
            }

            Assert.AreEqual(LogLevel.Warning, capturedLevel);
            Assert.AreEqual("warning message", capturedMessage);
        }

        [Test]
        public void LogError_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Error("error message");
            }

            Assert.AreEqual(LogLevel.Error, capturedLevel);
            Assert.AreEqual("error message", capturedMessage);
        }

        [Test]
        public void LogFatal_EmitsCorrectLevel()
        {
            LogLevel capturedLevel = LogLevel.None;
            string capturedMessage = null;
            
            using (Log.Listen(LogLevel.Trace, e => 
            {
                capturedLevel = e.LogLevel;
                capturedMessage = e.Message;
            }))
            {
                Log.Fatal("fatal message");
            }

            Assert.AreEqual(LogLevel.Fatal, capturedLevel);
            Assert.AreEqual("fatal message", capturedMessage);
        }

        [Test]
        public void Listen_BelowMinLevel_DoesNotCapture()
        {
            LogEntry captured = null;
            using (Log.Listen(LogLevel.Warning, e => captured = e))
            {
                Log.Debug("should not be captured");
            }

            Assert.IsNull(captured);
        }

        [Test]
        public void LogRecord_HasTimestamp()
        {
            DateTime capturedTimestamp = default;
            using (Log.Listen(LogLevel.Trace, e => capturedTimestamp = e.Timestamp))
            {
                Log.Debug("timestamp test");
            }

            Assert.AreNotEqual(default(DateTime), capturedTimestamp);
        }
    }
}
