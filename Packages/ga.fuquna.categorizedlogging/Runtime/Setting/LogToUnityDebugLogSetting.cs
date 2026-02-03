using UnityEngine;

namespace CategorizedLogging
{
    public class LogToUnityDebugLogSetting : LoggerSettingMonoBehaviourBase
    {
        [Header("LogType per LogLevel Settings")]
        public UnityLogTypeWithNone traceLogType = UnityLogTypeWithNone.None;	
        public UnityLogTypeWithNone debugLogType = UnityLogTypeWithNone.Log;	
        public UnityLogTypeWithNone informationLogType = UnityLogTypeWithNone.Log;
        public UnityLogTypeWithNone warningLogType = UnityLogTypeWithNone.Warning;	
        public UnityLogTypeWithNone errorLogType = UnityLogTypeWithNone.Error;
        public UnityLogTypeWithNone criticalLogType = UnityLogTypeWithNone.Error;

        public LogToUnityDebugLog Logger { get; } = new();
        
        protected override ILogger GetLogger() => Logger;


        protected override void OnValidate()
        {
            base.OnValidate();
            
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Trace] = traceLogType.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Debug] = debugLogType.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Information] = informationLogType.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Warning] = warningLogType.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Error] = errorLogType.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Critical] = criticalLogType.ToLogType();
        }
    }
}