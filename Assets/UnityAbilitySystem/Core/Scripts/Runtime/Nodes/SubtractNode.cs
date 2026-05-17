using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Core.Nodes
{
    public class SubtractNode : IntermediateNode
    {
        [HideInInspector] public CodeFunctionNode minuend;
        [HideInInspector] public CodeFunctionNode subtrahend;

        public override float value => minuend.value - subtrahend.value;

        public override float CalculateValue(GameObject _source) =>
            minuend.CalculateValue(_source) - subtrahend.CalculateValue(_source);

        public override void RemoveChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) minuend = null;
            else subtrahend = null;
        }

        public override void AddChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) minuend = _child;
            else subtrahend = _child;
        }

        public override ReadOnlyCollection<CodeFunctionNode> children
        {
            get
            {
                List<CodeFunctionNode> _nodes = new List<CodeFunctionNode>();
                if (minuend != null) _nodes.Add(minuend);
                if (subtrahend != null) _nodes.Add(subtrahend);
                return _nodes.AsReadOnly();
            }
        }

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<SubtractNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.minuend = this.minuend;
            _node.subtrahend = this.subtrahend;
            return _node;
        }

        public override void ReplaceChildNodeReference(
            Dictionary<CodeFunctionNode, CodeFunctionNode> _nodeDict)
        {
            minuend = _nodeDict[minuend];
            subtrahend = _nodeDict[subtrahend];
        }
    }
}