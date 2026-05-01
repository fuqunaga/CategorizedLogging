using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace ScotchLog;

public class ConcurrentObjectPool<T> : IObjectPool<T>
    where T : class
{
    // T はクラス型なので参照同一性（reference identity）で同値判定する。
    // value-based の GetHashCode/Equals に依存すると、Dispose 済みオブジェクトを
    // プールに戻す際に内部フィールドへのアクセスで例外が発生する可能性がある。
    private sealed class ReferenceComparer : IEqualityComparer<T>
    {
        public static readonly ReferenceComparer Instance = new();
        public bool Equals(T x, T y) => ReferenceEquals(x, y);
        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private readonly ConcurrentHashSet<T> _pool = new(ReferenceComparer.Instance);

    private readonly Func<T> _createFunc;
    private readonly Action<T> _actionOnGet;
    private readonly Action<T> _actionOnRelease;
    
    public int CountInactive => _pool.Count;
    
    
    public ConcurrentObjectPool(
        Func<T> createFunc,
        Action<T> actionOnGet = null,
        Action<T> actionOnRelease = null
    )
    {
        _createFunc = createFunc;
        _actionOnGet = actionOnGet;
        _actionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        if (!_pool.TryTake(out var value))
        {
            value = _createFunc();        
        }

        _actionOnGet?.Invoke(value);
        return value;
        
    }

    public PooledObject<T> Get(out T v)
    {
        v = Get();
        return new PooledObject<T>(v, this);
    }

    public void Release(T element)
    {
        _actionOnRelease?.Invoke(element);
        _pool.Add(element);
    }

    public void Clear()
    {
        _pool.Clear();
    }
}