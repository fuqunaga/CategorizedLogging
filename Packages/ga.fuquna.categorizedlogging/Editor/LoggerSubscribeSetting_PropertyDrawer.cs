using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace CategorizedLogging.Editor
{
    [CustomPropertyDrawer(typeof(LoggerSubscribeSetting))]
    public class LoggerSubscribeSetting_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var rootContainer = new VisualElement();
            RebuildUI(rootContainer, property);
            
            return rootContainer;
        }

        private static void RebuildUI(VisualElement rootContainer, SerializedProperty property, bool forceCategoryMode = false)
        {
            rootContainer.Clear();
            
            var categoryLogLevelsProperty = property.FindPropertyRelative(nameof(LoggerSubscribeSetting.categoryLogLevels));

            Assert.IsNotNull(categoryLogLevelsProperty);
            Assert.IsTrue(categoryLogLevelsProperty.isArray);


            if (!forceCategoryMode)
            {
                var isAnyCategoryLogLevels = categoryLogLevelsProperty.arraySize == 1
                                             && categoryLogLevelsProperty.GetArrayElementAtIndex(0)
                                                 ?.FindPropertyRelative(nameof(CategoryMinimumLogLevel.category))
                                                 ?.stringValue == "*";

                if (isAnyCategoryLogLevels)
                {
                    rootContainer.Add(
                        CreateAnyCategoryUI(categoryLogLevelsProperty, RebuildUIAction)
                    );
                    return;
                }
            }



            var listView = new MultiColumnListView()
            {
                headerTitle = "Category Log Levels",
                showFoldoutHeader = true,
                showAddRemoveFooter = true,
                reorderable = true,
            };

            listView.columns.Add(CreateCategoryLogLevelColumn(nameof(CategoryMinimumLogLevel.category), 200f));
            listView.columns.Add(CreateCategoryLogLevelColumn(nameof(CategoryMinimumLogLevel.logLevel), 150f));

            listView.BindProperty(categoryLogLevelsProperty);
            
            rootContainer.Add(listView);
            return;
            
            Column CreateCategoryLogLevelColumn(string bindingPath, float width)
            {
                return new Column
                {
                    title = ObjectNames.NicifyVariableName(bindingPath),
                    bindingPath = bindingPath,
                    width = width
                };
            }

            void RebuildUIAction(bool categoryMode)
            {
                RebuildUI(rootContainer, property, categoryMode);
            }
        }

        private static VisualElement CreateAnyCategoryUI(SerializedProperty categoryLogLevelsProperty, Action<bool> rebuildUIAction)
        {
            var row = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row 
                },
            };

            var firstItemLogLevelProperty = categoryLogLevelsProperty.GetArrayElementAtIndex(0)
                .FindPropertyRelative(nameof(CategoryMinimumLogLevel.logLevel));
            var logLevelField = new PropertyField(firstItemLogLevelProperty, ObjectNames.NicifyVariableName(nameof(CategoryMinimumLogLevel.logLevel)))
            {
                style =
                {
                    flexGrow = 1
                }
            };


            var addCategoryButton = new Button(() =>
            {
                rebuildUIAction(true);
            })
            {
                text = "Category Mode",
                style =
                {
                    width = 100,
                }
            };
            
            
            row.Add(logLevelField);
            row.Add(addCategoryButton);
            return row;
        }
    }
}