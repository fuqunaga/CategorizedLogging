using System;
using System.Collections.Concurrent;
using ScotchLog.Scope;
using UnityEngine.Pool;

namespace ScotchLog
{
    /// <summary>
    /// LogからLogDispatcherおよびSinkに渡るログデータ
    ///
    /// 別スレッドで呼ばれることを想定してstructではないが、数フレームしか有効期間がない
    /// 長期間保持する必要があるSinkは自前でデータを別の型にコピーする必要がある
    /// ディスパッチ後Dispose()が呼ばれる
    /// </summary>
    public record LogEntry : IDisposable
    {
        private static readonly ConcurrentQueue<LogEntry> Pool = new();

        private static LogEntry RentOrCreate()
        {
            return Pool.TryDequeue(out var logEntry) ? logEntry : new LogEntry();
        }
        
        public static LogEntry Rent(LogLevel logLevel, in StringWrapper message, in CallerInformation callerInfoInformation, LogScopeRecord scope = null)
        {
            var entry = RentOrCreate();
            entry.Set(logLevel, message, callerInfoInformation, scope);
            return entry;
        }

        public static void Return(LogEntry logEntry)
        {
            logEntry.Dispose();
            Pool.Enqueue(logEntry);
        }


        private DateTime _timestamp;
        private LogLevel _logLevel;
        private StringWrapper _stringWrapper;
        private CallerInformation _callerInfo;
        private LogScopeRecord _scope;


        public DateTime Timestamp
        {
            get
            {
                ThrowIfDisposed();
                return _timestamp;
            }
        }

        public LogLevel LogLevel
        {
            get
            {
                ThrowIfDisposed();
                return _logLevel;
            }
        }

        public StringWrapper StringWrapper
        {
            get
            {
                ThrowIfDisposed();
                return _stringWrapper;
            }
        }

        public CallerInformation CallerInfo
        {
            get
            {
                ThrowIfDisposed();
                return _callerInfo;
            }
        }

        public LogScopeRecord Scope
        {
            get
            {
                ThrowIfDisposed();
                return _scope;
            }
        }
        
        public string Message
        {
            get
            {
                ThrowIfDisposed();
                return StringWrapper.ToString();
            }
        }

        public bool IsDisposed { get; private set; }

        
        public void Set(LogLevel logLevel, in StringWrapper message, in CallerInformation callerInfoInformation, LogScopeRecord scope = null)
        {
            _timestamp = DateTime.Now;
            _logLevel = logLevel;
            _stringWrapper = message;
            _callerInfo = callerInfoInformation;
            _scope = scope ?? LogScopeRecord.Current;
            
            IsDisposed = false;
        }
        

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{LogLevel}] {Message}";
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            
            IsDisposed = true;
            _stringWrapper.Dispose();
            
            Return(this);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(LogEntry));
            }
        }
    }
}