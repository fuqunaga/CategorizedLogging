using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CategorizedLogging
{
    /// <summary>
    /// アプリケーション内で管理するログ
    /// </summary>
    [Serializable]
    public class InGameLogHolder : ILogger
    {
        private readonly ConcurrentQueue<LogEntry> _logEntries = new();
        
        // 別スレッドから呼ばれるので注意
        public event Action onLogEntryAddedMultiThreaded;

        
        public int LogCountMax { get; set; } = 1000;
        
        public IEnumerable<LogEntry> LogEntries => _logEntries;


        public void Log(in LogEntry logEntry)
        {
            _logEntries.Enqueue(logEntry);
            
            // 古いログを削除
            // たぶんO(n)なのでパフォーマンスが気になったら別の方法を検討する
            while (_logEntries.Count > LogCountMax)
            {
                _logEntries.TryDequeue(out _);
            }
            
            onLogEntryAddedMultiThreaded?.Invoke();
        }
    }
}