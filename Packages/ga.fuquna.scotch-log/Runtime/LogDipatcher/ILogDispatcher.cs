using System.Collections.Generic;

namespace ScotchLog
{
    public interface ILogDispatcher
    {
        void Log(LogEntry logEntry);
        public void Register(ISink sink, LogFilter logFilter);
        public void Unregister(ISink sink);
    }
}