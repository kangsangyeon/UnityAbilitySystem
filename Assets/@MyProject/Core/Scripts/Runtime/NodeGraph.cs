using System;
using System.Collections.Generic;
using System.Linq;
using Core.Nodes;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "NodeGraph", menuName = "Core/NodeGraph", order = 0)]
    public class NodeGraph : ScriptableObject, ICloneable
    {
        public CodeFunctionNode rootNode;
        public List<CodeFunctionNode> nodes = new List<CodeFunctionNode>();

        public float CalculateValue(GameObject _source) => rootNode.CalculateValue(_source);

        public List<T> FindNodesOfType<T>()
            where T : CodeFunctionNode
        {
            List<T> _outNodes = new List<T>();
            nodes.ForEach(n =>
            {
                if (n is T _nodeOfType)
                    _outNodes.Add(_nodeOfType);
            });

            return _outNodes;
        }

        public object Clone()
        {
            Dictionary<CodeFunctionNode, CodeFunctionNode> _cloneNodeMap =
                new Dictionary<CodeFunctionNode, CodeFunctionNode>();

            nodes.ForEach(n =>
            {
                // 원본 그래프의 노드를 복제합니다.
                var _clone = n.Clone() as CodeFunctionNode;
                _cloneNodeMap.Add(n, _clone);
            });

            foreach (var n in _cloneNodeMap.Values)
            {
                // 복제된 노드들이 가지는 노드에 대한 레퍼런스를
                // 복제된 노드에 대한 레퍼런스로 수정합니다.
                if (n is IntermediateNode _intermediateNode)
                {
                    _intermediateNode.ReplaceChildNodeReference(_cloneNodeMap);
                }
            }

            // 그래프를 복제합니다.
            var _cloneNodeGraph = ScriptableObject.CreateInstance<NodeGraph>();
            _cloneNodeGraph.rootNode = _cloneNodeMap[rootNode];
            _cloneNodeGraph.nodes = _cloneNodeMap.Values.ToList();
            return _cloneNodeGraph;
        }

#if UNITY_EDITOR

        public void AddNode(CodeFunctionNode _node)
        {
            nodes.Add(_node);
            AssetDatabase.AddObjectToAsset(_node, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void DeleteNode(CodeFunctionNode _node)
        {
            nodes.Remove(_node);
            AssetDatabase.RemoveObjectFromAsset(_node);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(CodeFunctionNode _parent, CodeFunctionNode _child, string _portName)
        {
            if (_parent is IntermediateNode _intermediateNode)
            {
                _intermediateNode.AddChild(_child, _portName);
                EditorUtility.SetDirty(_intermediateNode);
            }
            else if (_parent is ResultNode _resultNode)
            {
                _resultNode.child = _child;
                EditorUtility.SetDirty(_resultNode);
            }
        }

        public void RemoveChild(CodeFunctionNode _parent, CodeFunctionNode _child, string _portName)
        {
            if (_parent is IntermediateNode _intermediateNode)
            {
                _intermediateNode.RemoveChild(_child, _portName);
                EditorUtility.SetDirty(_intermediateNode);
            }
            else if (_parent is ResultNode _resultNode)
            {
                _resultNode.child = null;
                EditorUtility.SetDirty(_resultNode);
            }
        }

#endif
    }
}