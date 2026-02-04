using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Component to hold log settings in the scene
    /// </summary>
    public abstract class SinkConfigMonoBehaviourBase : MonoBehaviour
    {
        public SinkConfig sinkConfig;

        
        protected abstract ISink GetSink();

        
        #region Unity
        
        protected virtual void OnEnable() => Register();

        protected virtual void OnDisable() => Unregister();

        protected virtual void OnValidate()
        {
            // OnValidateはドメインリロード時に呼ばれるが、シーンロード時に別のオブジェクトとして再度呼ばれる
            // ドメインリロード時のOnValidateは無視したいので、Application.isPlayingを確認する
            if (!isActiveAndEnabled || !Application.isPlaying)
            {
                return;
            }
            
            Register();
        }
        
        #endregion


        protected virtual void Register()
        {
            if (GetSink() is not {} sink)
            {
                return;
            }

            if (Log.Logger is not { } logger)
            {
                Debug.LogWarning("[CategorizedLogging] Log.Logger is not LogDispatcher. Cannot subscribe logger.");
                return;
            }
            
            logger.Register(sink, sinkConfig.categoryLogLevels);
        }
        
        protected virtual void Unregister()
        {
            if (GetSink() is not {} sink)
            {
                return;
            }
            
            if (Log.Logger is not {} logger)
            {
                Debug.LogWarning("[CategorizedLogging] Log.Logger is not LogDispatcher. Cannot unsubscribe logger.");
                return;
            }
            
            logger.Unregister(sink);
        }
    }
}