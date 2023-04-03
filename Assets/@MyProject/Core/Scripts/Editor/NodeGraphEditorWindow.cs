using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Editor
{
    public class NodeGraphEditorWindow : EditorWindow
    {
        private NodeGraph m_NodeGraph;
        private NodeGraphView m_NodeGraphView;
        private VisualElement m_LeftPanel;
        
        public static void ShowWindow(NodeGraph _nodeGraph)
        {
            NodeGraphEditorWindow _window = GetWindow<NodeGraphEditorWindow>();
            _window.SelectNodeGraph(_nodeGraph);
            _window.minSize = new Vector2(800, 600);
            _window.titleContent = new GUIContent("NodeGraph");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int _instanceId, int _line)
        {
            if (EditorUtility.InstanceIDToObject(_instanceId) is NodeGraph _nodeGraph)
            {
                ShowWindow(_nodeGraph);
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/@MyProject/Core/Scripts/Editor/NodeGraphEditorWindow.uxml");
            visualTree.CloneTree(root);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/@MyProject/Core/Scripts/Editor/NodeGraphEditorWindow.uss");
            root.styleSheets.Add(styleSheet);

            m_NodeGraphView = root.Q<NodeGraphView>();
            m_LeftPanel = root.Q<VisualElement>("left-panel");
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is NodeGraph _nodeGraph)
            {
                SelectNodeGraph(_nodeGraph);
            }
        }

        private void SelectNodeGraph(NodeGraph _nodeGraph)
        {
            m_NodeGraph = _nodeGraph;
            m_NodeGraphView.PopulateView(_nodeGraph);
        }
    }
}