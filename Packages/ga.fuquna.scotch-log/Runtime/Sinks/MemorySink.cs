using System;
using System.Collections.Generic;
using Unity.Collections;

namespace ScotchLog
{
    /// <summary>
    /// アプリケーション内で管理するログ
    /// </summary>
    [Serializable]
    public class MemorySink : ISink
    {
        private static readonly Action<LogRecord> DisposeLogEntry = entry => entry.Dispose();

        private ConcurrentRingBuffer<LogRecord> _logRecords = new(1000, DisposeLogEntry);

        // 別スレッドから呼ばれるので注意
        public event Action onLogEntryAddedMultiThreaded;

        public int Capacity
        {
            get => _logRecords.Capacity;
            set => _logRecords.Capacity = value;
        }

        public IEnumerable<LogRecord> LogRecords => _logRecords;

        public void Log(LogEntry logEntry)
        {
            // StringWrapperをPersistantにしてコピー
            var copiedEntry = new LogRecord(logEntry);
            _logRecords.Add(copiedEntry);

            onLogEntryAddedMultiThreaded?.Invoke();
        }

        public void Clear()
        {
            _logRecords.Clear();
        }
    }
}