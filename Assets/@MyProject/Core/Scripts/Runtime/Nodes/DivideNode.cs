﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Core.Nodes
{
    public class DivideNode : IntermediateNode
    {
        [HideInInspector] public CodeFunctionNode dividend;
        [HideInInspector] public CodeFunctionNode divisor;

        public override float value => dividend.value / divisor.value;

        public override float CalculateValue(GameObject _source) =>
            dividend.CalculateValue(_source) / divisor.CalculateValue(_source);

        public override void RemoveChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) dividend = null;
            else divisor = null;
        }

        public override void AddChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) dividend = _child;
            else divisor = _child;
        }

        public override ReadOnlyCollection<CodeFunctionNode> children
        {
            get
            {
                List<CodeFunctionNode> _nodes = new List<CodeFunctionNode>();
                if (dividend != null) _nodes.Add(dividend);
                if (divisor != null) _nodes.Add(divisor);
                return _nodes.AsReadOnly();
            }
        }
    }
}