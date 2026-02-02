using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CategorizedLogging.Samples
{
    [RequireComponent(typeof(UIDocument))]
    public class Sample : MonoBehaviour
    {
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var logEmitUI = CreateLogEmitUI();
            var inGameLogHolderUI = CreateInGameLogHolderUI();
            inGameLogHolderUI.style.marginTop = 20;
            
            root.Add(logEmitUI);
            root.Add(inGameLogHolderUI);
        }

        private VisualElement CreateLogEmitUI()
        {
            var container = CreateContainer();

            var logMessageField = new TextField("Log Message")
            {
                multiline = true,
                value = "This is a sample log message."
            };

            var logLevelDropdown = new DropdownField(
                "Log Level",
                Enum.GetNames(typeof(LogLevel)).ToList(),
                nameof(LogLevel.Information)
            );
            
            var emitButton = new Button(() =>
            {
                var message = logMessageField.value;
                var logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), logLevelDropdown.value);
                Log.EmitLog(this, logLevel, message);
            })
            {
                text = "Emit Log",
                style =
                {
                    marginTop = 10
                }
            };
            
            container.Add(logMessageField);
            container.Add(logLevelDropdown);
            container.Add(emitButton);

            return container;
        }

        
        private VisualElement CreateInGameLogHolderUI()
        {
            var container = CreateContainer();

            var label = new Label("In-Game Log Holder")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };

            var scrollView = new ScrollView()
            {
                horizontalScrollerVisibility = ScrollerVisibility.Auto
            };

            
            var logField = new TextField()
            {
                multiline = true,
                isReadOnly = true,
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                style =
                {
                    height = 500,
                }
            };
            scrollView.Add(logField);
            
            container.Add(label);
            container.Add(scrollView);

            var inGameLogHolderSetting = FindAnyObjectByType<InGameLogHolderSetting>();
            var inGameLogHolder = inGameLogHolderSetting.Logger;
            inGameLogHolder.onLogEntryAdded += UpdateLogField;

            
            return container;
 
            void UpdateLogField()
            {
                var logText = string.Join(Environment.NewLine, inGameLogHolder.LogEntries.Select(entry => entry.ToString()));
                logField.value = logText;
            }
        }

        
        private VisualElement CreateContainer()
        {
            return new VisualElement()
            {
                style =
                {
                    width = 500f,
                    backgroundColor = new StyleColor(new Color(0.7f, 0.7f, 0.7f)),
                }
            };
        }
    }
}