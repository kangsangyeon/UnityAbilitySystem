﻿using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Core.Editor
{
    public class NodeGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeGraphView, UxmlTraits>
        {
        }

        private NodeGraph m_NodeGraph;

        public NodeGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            GridBackground _gridBackground = new GridBackground();
            Insert(0, _gridBackground);
            _gridBackground.StretchToParentSize();

            var _styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/@MyProject/Core/Scripts/Editor/NodeGraphEditorWindow.uss");
            styleSheets.Add(_styleSheet);
        }
    }
}