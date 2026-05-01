using System;
using JetBrains.Annotations;

namespace ScotchLog.Scope;

/// <summary>
/// LogSpanRecordの情報を外だしするための構造体
/// LogSpanRecordはどこからも参照されなくなったら回収したいのでこのstructからのみ参照できるようにする
/// </summary>
public  readonly partial struct LogSpan : IDisposable
{
    public static LogSpan Current;
    
    // public static LogSpan Create(string name, [CanBeNull] in LogSpan? parent = null)
    // {
    //     // return new LogSpan(new LogSpanRecord(name, parent?._record));
    // }
    
    
    private readonly LogSpanRecord _record;
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}