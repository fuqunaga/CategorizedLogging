using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Component to hold log settings in the scene
    /// </summary>
    public abstract class LoggerSettingMonoBehaviourBase : MonoBehaviour
    {
        public LoggerSetting loggerSetting;

        
        protected abstract ILogger GetLogger();

        
        #region Unity
        
        protected virtual void OnEnable() => Subscribe();

        protected virtual void OnDisable() => Unsubscribe();
        
        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            Unsubscribe();
            Subscribe();
        }
        
        #endregion


        protected virtual void Subscribe()
        {
            if (GetLogger() is not {} logger)
            {
                return;
            }

            if (Log.Logger is not LogDispatcher dispatcher)
            {
                Debug.LogWarning("[CategorizedLogging] Log.Logger is not LogDispatcher. Cannot subscribe logger.");
                return;
            }
            
            dispatcher.Register(logger, loggerSetting.categoryLogLevels);
        }
        
        protected virtual void Unsubscribe()
        {
            if (GetLogger() is not {} logger)
            {
                return;
            }
            
            if (Log.Logger is not LogDispatcher dispatcher)
            {
                Debug.LogWarning("[CategorizedLogging] Log.Logger is not LogDispatcher. Cannot unsubscribe logger.");
                return;
            }
            
            dispatcher.Unregister(logger);
        }
    }
}