using System;

namespace ScotchLog
{
    /// <summary>
    /// A log sink that allows external code to listen to log messages via a callback.
    ///
    /// LogRecordはいずれIDisposableにして回収したいのでコールバックでアプリ側に公開しているのはよろしくない
    /// </summary>
    public class ListenerSink : ISink
    {
        private readonly Action<LogEntry> _callback;
        
        public ListenerSink(Action<LogEntry>　callback)
        {
            _callback = callback;
        }

        public ListenerSink(Action<string>　callback)
        {
            _callback = (record) => callback(record?.Message);
        }
        
        public void Log(LogEntry logEntry)
        {
            _callback?.Invoke(logEntry);
        }
    }
}