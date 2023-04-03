using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace StatSystem.Editor
{
    public class StatDatabaseEditorWindow : EditorWindow
    {
        private StatDatabase m_StatDatabase;
        
        [MenuItem("Window/StatSystem/StatDatabase")]
        public static void ShowWindow()
        {
            StatDatabaseEditorWindow _window = GetWindow<StatDatabaseEditorWindow>();
            _window.minSize = new Vector2(800, 600);
            _window.titleContent = new GUIContent("StatDatabase");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int _instanceId, int _line)
        {
            if (EditorUtility.InstanceIDToObject(_instanceId) is StatDatabase)
            {
                ShowWindow();
                return true;
            }

            return false;
        }

        private void OnSelectionChange()
        {
            m_StatDatabase = Selection.activeObject as StatDatabase;
        }

        public void CreateGUI()
        {
            VisualElement _root = rootVisualElement;

            var _visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/@MyProject/StatSystem/Scripts/Editor/StatDatabaseEditorWindow.uxml");
            _visualTree.CloneTree(_root);

            var _styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/@MyProject/StatSystem/Scripts/Editor/StatDatabaseEditorWindow.uss");
        }
    }
}