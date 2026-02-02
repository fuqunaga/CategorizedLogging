namespace CategorizedLogging
{
    public class InGameLogHolderSetting : LoggerSettingComponentBase
    {
        public int logCountMax = 1000;


        public InGameLogHolder Logger { get; } = new();
        
        protected override ILogger GetLogger() => Logger;


        private void OnValidate()
        {
            Logger.LogCountMax = logCountMax;
        }
    }
}