using System;
using System.IO;

namespace ScorpionLog
{
    public readonly struct CallerInformation : IEquatable<CallerInformation>
    {
        public string FilePath { get; }
        public int LineNumber { get; }
        public string MemberName { get; }
        
        public string FileName => Path.GetFileName(FilePath);

        public CallerInformation(string callerFilePath, int callerLineNumber, string memberName)
        {
            FilePath = callerFilePath;
            LineNumber = callerLineNumber;
            MemberName = memberName;
        }

        public override string ToString() => $"{MemberName}@{FileName}:{LineNumber}";
        
        #region IEquatable
        
        public bool Equals(CallerInformation other)
        {
            return FilePath == other.FilePath && LineNumber == other.LineNumber && MemberName == other.MemberName;
        }

        public override bool Equals(object obj)
        {
            return obj is CallerInformation other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FilePath, LineNumber, MemberName);
        }
        
        #endregion
    }
}