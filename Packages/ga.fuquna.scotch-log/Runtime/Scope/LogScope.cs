using System;
using System.Runtime.CompilerServices;

namespace ScotchLog.Scope;

/// <summary>
/// ログスコープ
/// アプリケーション向けのLogSpanのインターフェース
/// スコープとはLogScopePropertyを保持する単位
/// </summary>
public readonly struct LogScope : IDisposable
{
    private readonly LogSpanRecord _record;

        
    public LogScope() : this("")
    {
    }

    public LogScope(string name, LogSpanRecord parent = null)
    {
        _record = new LogSpanRecord(name, parent);
    }
        
    public LogScope SetProperty<T>(T propertyValue, [CallerArgumentExpression("propertyValue")] string propertyName = "")
        => SetProperty(propertyName, propertyValue);
        
    public LogScope SetProperty<T>(string propertyName, T propertyValue)
        => SetProperty(propertyName, propertyValue?.ToString());
        
    public LogScope SetProperty(string propertyName, string propertyValue)
    {
        _record.SetProperty(propertyName, propertyValue);
        return this;
    }

    public void Dispose()
    {
        _record.Close();
    }
}