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
        private static readonly Action<LogEntryPersistant> DisposeLogEntry = entry => entry.Dispose();

        private ConcurrentRingBuffer<LogEntryPersistant> _logEntries = new(1000, DisposeLogEntry);

        // 別スレッドから呼ばれるので注意
        public event Action onLogEntryAddedMultiThreaded;

        public int Capacity
        {
            get => _logEntries.Capacity;
            set => _logEntries.Capacity = value;
        }

        public IEnumerable<LogEntryPersistant> LogEntries => _logEntries;

        public void Log(LogEntry logEntry)
        {
            // StringWrapperをPersistantにしてコピー
            var copiedEntry = new LogEntryPersistant(logEntry);
            _logEntries.Add(copiedEntry);

            onLogEntryAddedMultiThreaded?.Invoke();
        }

        public void Clear()
        {
            _logEntries.Clear();
        }
    }
}