namespace CategorizedLogging
{
    public class InGameLogHolderSetting : LoggerSettingMonoBehaviourBase
    {
        public int logCountMax = 1000;


        public InGameLogHolder Logger { get; } = new();
        
        protected override ILogger GetLogger() => Logger;


        protected override void OnValidate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            base.OnValidate();
            Logger.LogCountMax = logCountMax;
        }
    }
}