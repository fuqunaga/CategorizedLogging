using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace CategorizedLogging.Editor
{
    [CustomPropertyDrawer(typeof(LoggerSetting))]
    public class LoggerSetting_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var rootContainer = new VisualElement();
            
            var categoryLogLevelsProperty = property.FindPropertyRelative(nameof(LoggerSetting.categoryLogLevels));

            Assert.IsNotNull(categoryLogLevelsProperty);
            Assert.IsTrue(categoryLogLevelsProperty.isArray);

            
            var innerUI = CreateCategoryModeUI(categoryLogLevelsProperty);
            rootContainer.Add(innerUI);

            return rootContainer;
        }
        
        
        private static VisualElement CreateCategoryModeUI(SerializedProperty categoryLogLevelsProperty)
        {
            var listView = new MultiColumnListView()
            {
                headerTitle = "Category Log Levels",
                showFoldoutHeader = true,
                showAddRemoveFooter = true,
                reorderable = true,
            };


            var categoryColumn = CreateCategoryLogLevelColumn(nameof(CategoryMinimumLogLevel.category));
            categoryColumn.stretchable = true;
            
            var logLevelColumn = CreateCategoryLogLevelColumn(nameof(CategoryMinimumLogLevel.logLevel));
            logLevelColumn.minWidth = 150;
            
            listView.columns.Add(categoryColumn);
            listView.columns.Add(logLevelColumn);

            listView.BindProperty(categoryLogLevelsProperty);
            
            return listView;
            
            Column CreateCategoryLogLevelColumn(string bindingPath)
            {
                return new Column
                {
                    title = ObjectNames.NicifyVariableName(bindingPath),
                    bindingPath = bindingPath,
                };
            }
        }

    }
}