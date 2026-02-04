namespace CategorizedLogging
{
    public class MemorySinkConfig : SinkConfigMonoBehaviourBase
    {
        public int logCountMax = 1000;


        public MemorySink Sink { get; } = new();
        
        protected override ISink GetSink() => Sink;


        protected override void OnValidate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            base.OnValidate();
            Sink.LogCountMax = logCountMax;
        }
    }
}