using System;
using UnityEngine;

namespace ScorpionLog.Scope
{
    [Serializable]
    public record LogScopeProperty
    {
        [SerializeField] private string name;
        [SerializeField] private string value;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Value
        {
            get => value;
            set => this.value = value;
        }

        public LogScopeProperty()
        {
        }
        
        public LogScopeProperty(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        
        public LogScopeProperty(LogScopeProperty other)
        {
            name = other.name;
            value = other.value;
        }
        
        public void Deconstruct(out string outName, out string outValue)
        {
            outName = Name;
            outValue = Value;
        }
        
        
        public static implicit operator LogScopeProperty((string name, string value) tuple)
        {
            return new LogScopeProperty(tuple.name, tuple.value);
        }
    }
}