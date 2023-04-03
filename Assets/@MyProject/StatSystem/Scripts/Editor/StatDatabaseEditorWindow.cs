using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace StatSystem.Editor
{
    public class StatDatabaseEditorWindow : EditorWindow
    {
        private StatDatabase m_StatDatabase;
        private StatCollectionEditor m_Current;

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
            OnSelectionChange();
            VisualElement _root = rootVisualElement;

            var _visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/@MyProject/StatSystem/Scripts/Editor/StatDatabaseEditorWindow.uxml");
            _visualTree.CloneTree(_root);

            var _styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/@MyProject/StatSystem/Scripts/Editor/StatDatabaseEditorWindow.uss");
            _root.styleSheets.Add(_styleSheet);

            StatCollectionEditor _stats = _root.Q<StatCollectionEditor>("stats");
            _stats.Initialize(m_StatDatabase, m_StatDatabase.stats);
            Button _statsTab = _root.Q<Button>("stats-tab");
            _statsTab.clicked += () =>
            {
                m_Current.style.display = DisplayStyle.None;
                _stats.style.display = DisplayStyle.Flex;
                m_Current = _stats;
            };
            
            StatCollectionEditor _primaryStats = _root.Q<StatCollectionEditor>("primary-stats");
            _primaryStats.Initialize(m_StatDatabase, m_StatDatabase.primaryStats);
            Button _primaryStatsTab = _root.Q<Button>("primary-stats-tab");
            _primaryStatsTab.clicked += () =>
            {
                m_Current.style.display = DisplayStyle.None;
                _primaryStats.style.display = DisplayStyle.Flex;
                m_Current = _primaryStats;
            };
            
            StatCollectionEditor _attributes = _root.Q<StatCollectionEditor>("attributes");
            _attributes.Initialize(m_StatDatabase, m_StatDatabase.attributes);
            Button _attributesTab = _root.Q<Button>("attributes-tab");
            _attributesTab.clicked += () =>
            {
                m_Current.style.display = DisplayStyle.None;
                _attributes.style.display = DisplayStyle.Flex;
                m_Current = _attributes;
            };
            
            m_Current = _stats;
        }
    }
}