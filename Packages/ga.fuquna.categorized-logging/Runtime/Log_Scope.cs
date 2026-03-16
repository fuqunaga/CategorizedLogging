using CategorizedLogging.Scope;
using JetBrains.Annotations;

namespace CategorizedLogging
{
    /// <summary>
    /// スコープとはLogPropertyを保持する単位
    /// /// </summary>
    public static partial class Log
    {
        /// <summary>
        /// 同一スレッドにおけるスコープを開始します
        /// 主にusingとともに使用してDisposeされることを想定しています
        /// </summary>
        public static LogScope BeginScope(string name = "")
        {
            return new LogScope(name);
        }
    }
}