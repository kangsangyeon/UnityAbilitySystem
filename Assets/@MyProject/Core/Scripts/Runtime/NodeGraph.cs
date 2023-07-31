using System.Collections.Generic;
using Core.Nodes;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "NodeGraph", menuName = "Core/NodeGraph", order = 0)]
    public class NodeGraph : ScriptableObject
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