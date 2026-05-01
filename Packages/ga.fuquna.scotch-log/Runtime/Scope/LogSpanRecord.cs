using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ScotchLog.Scope;

/// <summary>
/// ログスパン
/// 
/// ログスコープの実態
/// 複数スレッドから参照されること想定してclass型
/// どこからも参照されなくなったら回収して再利用したいので直接このクラスの参照を持てるのはLogSpanのみ
/// </summary>
///
public partial struct LogSpan
{
    private static readonly ConcurrentQueue<LogSpanRecord> Pool = new();
    
    
    public record LogSpanRecord
    {
        #region Static members

        private static int _lastId;
        private static readonly AsyncLocal<LogSpanRecord> CurrentScope = new();
        private static readonly LogSpanRecord RootScope = new("Root");

        public static LogSpanRecord Current
        {
            get => CurrentScope.Value　?? RootScope;
            private set => CurrentScope.Value = value;
        }

        private static int GetNextId()
        {
            return Interlocked.Increment(ref _lastId);
        }

        #endregion


        private Dictionary<string, string> _properties;


        public int Id { get; } = GetNextId();
        public string Name { get; }
        public LogSpanRecord Parent { get; }
        public DateTime StartTimeUtc { get; } = DateTime.UtcNow;
        public DateTime EndTimeUtc { get; private set; }
        public DateTime StartTime => StartTimeUtc.ToLocalTime();
        public DateTime EndTime => EndTimeUtc.ToLocalTime();
        public IReadOnlyDictionary<string, string> Properties => _properties;
        public bool IsRoot => Parent == null || Parent == this;


        public LogSpanRecord(string name = "", LogSpanRecord parent = null)
        {
            Name = name;
            Parent = parent ?? Current;
            Current = this;
        }


        public void SetProperty(string propertyName, string propertyValue)
        {
            if (EndTimeUtc != default)
            {
                throw new InvalidOperationException("Cannot set property on a closed scope.");
            }

            _properties ??= new Dictionary<string, string>();
            _properties[propertyName] = propertyValue;
        }

        public void Close()
        {
            if (EndTimeUtc != default)
            {
                throw new InvalidOperationException("Scope is already closed.");
            }

            EndTimeUtc = DateTime.UtcNow;

            if (Parent != null)
            {
                if (Current != this)
                {
                    // Scopeは本来親子関係にないものが並列に存在してもよいが現状実装がめんどくさくて例外扱い
                    // あまり無いと思うが需要が出たら対応したい
                    throw new InvalidOperationException("Current scope does not match the scope being closed.");
                }

                Current = Parent;
            }
        }
    }
}