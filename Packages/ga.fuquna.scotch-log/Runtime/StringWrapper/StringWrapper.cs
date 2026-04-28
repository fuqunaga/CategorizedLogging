using System;
using Unity.Collections;

namespace ScotchLog;

/// <summary>
/// Logメソッドが受け取る文字列をラップする構造体
/// </summary>
public struct StringWrapper : IDisposable
{
    public static Allocator Allocator { get; set; } = Allocator.TempJob;
    
    
    private NativeText _nativeText;
    private readonly string _string;


    private StringWrapper(NativeText nativeText)
    {
        _nativeText = nativeText;
        _string = null;
    }

    private StringWrapper(string stringValue)
    {
        _nativeText = default;
        _string = stringValue;
    }
    
    
    public override string ToString() => _string ?? _nativeText.ToString();
    
    
    public void Dispose()
    {
        _nativeText.Dispose();
    }
    
    
    public static implicit operator StringWrapper(string str)
    {
        return new StringWrapper(str);
    }
    
    public static implicit operator StringWrapper(in NativeText nativeText)
    {
        var dst = new NativeText(nativeText.Capacity, Allocator);
        dst.CopyFrom(nativeText);
        return new StringWrapper(dst);
    }
    
    public static implicit operator StringWrapper(in FixedString32Bytes str)
    {
        return new StringWrapper(new NativeText(str, Allocator));
    }

    public static implicit operator StringWrapper(in FixedString64Bytes str)
    {
        return new StringWrapper(new NativeText(str, Allocator));
    }

    public static implicit operator StringWrapper(in FixedString128Bytes str)
    {
        return new StringWrapper(new NativeText(str, Allocator));
    }

    public static implicit operator StringWrapper(in FixedString512Bytes str)
    {
        return new StringWrapper(new NativeText(str, Allocator));
    }

    public static implicit operator StringWrapper(in FixedString4096Bytes str)
    {
        return new StringWrapper(new NativeText(str, Allocator));
    }
}