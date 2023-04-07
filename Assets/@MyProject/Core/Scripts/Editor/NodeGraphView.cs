using System;
using System.Collections.Generic;
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

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (m_NodeGraph.rootNode == null)
            {
                var _rootNode = ScriptableObject.CreateInstance<ResultNode>();
                _rootNode.name = _rootNode.GetType().Name;
                _rootNode.guid = GUID.Generate().ToString();

                _nodeGraph.rootNode = _rootNode;
                _nodeGraph.AddNode(m_NodeGraph.rootNode);
            }

            m_NodeGraph.nodes.ForEach(n => CreateAndAddNodeView(n));
            m_NodeGraph.nodes.ForEach(n =>
            {
                if (n is IntermediateNode _intermediateNode)
                {
                    NodeView _parentView = FindNodeView(n);
                    for (int i = 0; i < _intermediateNode.children.Count; i++)
                    {
                        NodeView _childView = FindNodeView(_intermediateNode.children[i]);
                        Edge _edge = _parentView.inputs[i].ConnectTo(_childView.output);
                        AddElement(_edge);
                    }
                }
                else if (n is ResultNode _rootNode)
                {
                    if (_rootNode.child != null)
                    {
                        NodeView _parentView = FindNodeView(n);
                        NodeView _childView = FindNodeView(_rootNode.child);
                        Edge _edge = _parentView.inputs[0].ConnectTo(_childView.output);
                        AddElement(_edge);
                    }
                }
            });
        }

        private NodeView FindNodeView(CodeFunctionNode _node)
        {
            return GetNodeByGuid(_node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange _graphViewChange)
        {
            if (_graphViewChange.elementsToRemove != null)
            {
                _graphViewChange.elementsToRemove.ForEach(e =>
                {
                    if (e is NodeView _nodeView)
                    {
                        m_NodeGraph.DeleteNode(_nodeView.node);
                    }
                    else if (e is Edge _edge)
                    {
                        NodeView _parentView = _edge.input.node as NodeView;
                        NodeView _childView = _edge.output.node as NodeView;
                        m_NodeGraph.RemoveChild(_parentView.node, _childView.node, _edge.input.portName);
                    }
                });
            }

            if (_graphViewChange.edgesToCreate != null)
            {
                _graphViewChange.edgesToCreate.ForEach(e =>
                {
                    NodeView _parentView = e.input.node as NodeView;
                    NodeView _childView = e.output.node as NodeView;
                    m_NodeGraph.AddChild(_parentView.node, _childView.node, e.input.portName);
                });
            }

            return _graphViewChange;
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

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction
                                  && endPort.node != startPort.node).ToList();
        }
    }
}