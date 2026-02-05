using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Component to hold log settings in the scene
    /// </summary>
    public abstract class SinkConfigMonoBehaviour<TSink> : SinkConfigMonoBehaviourBase<TSink>
        where TSink : ISink, new()
    {
        [SerializeField]
        private SinkFilterConfig filterConfig = new();

        public override SinkFilterConfig SinkFilterConfig => filterConfig;
    }
}