using System.Collections.Generic;

namespace ScorpionLog
{
    public interface ILogDispatcher
    {
        void Log(LogRecord logRecord);
        public void Register(ISink sink, LogFilter logFilter);
        public void Unregister(ISink sink);
    }
}