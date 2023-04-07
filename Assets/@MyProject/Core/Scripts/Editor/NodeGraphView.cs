using System;
using System.Linq;
using Core.Editor.Nodes;
using Core.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Core.Editor
{
    public class NodeGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeGraphView, UxmlTraits>
        {
        }

        private NodeGraph m_NodeGraph;
        public UnityAction<NodeView> nodeSelected;

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

        internal void PopulateView(NodeGraph _nodeGraph)
        {
            m_NodeGraph = _nodeGraph;

            if (m_NodeGraph.rootNode == null)
            {
                var _rootNode = ScriptableObject.CreateInstance<ResultNode>();
                _rootNode.name = _rootNode.GetType().Name;
                _rootNode.guid = GUID.Generate().ToString();

                _nodeGraph.rootNode = _rootNode;
                _nodeGraph.AddNode(m_NodeGraph.rootNode);
            }

            m_NodeGraph.nodes.ForEach(n => CreateAndAddNodeView(n));
        }

        private void CreateAndAddNodeView(CodeFunctionNode _node)
        {
            Type[] _types
                = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                    .Where(t => typeof(NodeView).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).ToArray();

            foreach (var _type in _types)
            {
                if (_type.GetCustomAttributes(typeof(NodeType), false) is NodeType[] attributes
                    && attributes.Length > 0)
                {
                    if (attributes[0].type == _node.GetType())
                    {
                        NodeView _nodeView = Activator.CreateInstance(_type) as NodeView;
                        _nodeView.node = _node;
                        _nodeView.viewDataKey = _node.guid;
                        _nodeView.style.left = _node.position.x;
                        _nodeView.style.top = _node.position.y;
                        AddNodeView(_nodeView);
                    }
                }
            }
        }

        internal void AddNodeView(NodeView _nodeView)
        {
            _nodeView.nodeSelected = nodeSelected;
            AddElement(_nodeView);
        }
    }
}