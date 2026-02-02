using UnityEngine;

namespace CategorizedLogging
{
    public class LogToUnityDebugLogSetting : LoggerSettingComponentBase
    {
        [Header("LogType per LogLevel Settings")]
        public UnityLogTypeWithNone logTypeForTrace = UnityLogTypeWithNone.None;	
        public UnityLogTypeWithNone logTypeForDebug = UnityLogTypeWithNone.Log;	
        public UnityLogTypeWithNone logTypeForInformation = UnityLogTypeWithNone.Log;
        public UnityLogTypeWithNone logTypeForWarning = UnityLogTypeWithNone.Warning;	
        public UnityLogTypeWithNone logTypeForError = UnityLogTypeWithNone.Error;
        public UnityLogTypeWithNone logTypeForCritical = UnityLogTypeWithNone.Error;
 

        public LogToUnityDebugLog Logger { get; } = new();
        
        protected override ILogger GetLogger() => Logger;


        private void OnValidate()
        {
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Trace] = logTypeForTrace.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Debug] = logTypeForDebug.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Information] = logTypeForInformation.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Warning] = logTypeForWarning.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Error] = logTypeForError.ToLogType();
            Logger.LogLevelToUnityLogTypeTable[LogLevel.Critical] = logTypeForCritical.ToLogType();
        }
    }
}