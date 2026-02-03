using System;
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
        
        protected virtual void Unregister()
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