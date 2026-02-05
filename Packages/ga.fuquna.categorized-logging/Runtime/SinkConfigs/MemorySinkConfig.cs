namespace CategorizedLogging
{
    public class MemorySinkConfig : SinkConfigMonoBehaviour<MemorySink>
    {
        public int logCountMax = 1000;
        
        protected override void OnValidate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            base.OnValidate();
            Sink.Capacity = logCountMax;
        }
    }
}