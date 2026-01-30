using System;
using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Component to hold log settings in the scene
    /// </summary>
    public abstract class LoggerSubscribeSettingComponent : MonoBehaviour
    {
        public LoggerSubscribeSetting loggerSubscribeSetting;

        protected abstract ILogger GetLogger();

        private void OnEnable()
        {
            var logger = GetLogger();
            if (logger == null) return;

            if (Log.Logger is not LogDispatcher dispatcher)
            {
                Debug.LogWarning("[CategorizedLogging] Log.Logger is not LogDispatcher. Cannot subscribe logger.");
                return;
            }
            
            dispatcher.Subscribe(logger, loggerSubscribeSetting.categoryLogLevels);
        }

        private void OnDisable()
        {
            var logger = GetLogger();
            if (logger == null) return;

        }
    }
}