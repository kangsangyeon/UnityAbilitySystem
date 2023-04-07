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
    }
}