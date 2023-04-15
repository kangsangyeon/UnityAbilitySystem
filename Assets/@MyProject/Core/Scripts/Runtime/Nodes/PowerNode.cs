using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Nodes
{
    public class PowerNode : IntermediateNode
    {
        [HideInInspector] public CodeFunctionNode @base;
        [HideInInspector] public CodeFunctionNode exponent;

        public override float value => Mathf.Pow(@base.value, exponent.value);

        public override void RemoveChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) @base = null;
            else exponent = null;
        }

        public override void AddChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) @base = _child;
            else exponent = _child;
        }

        public override ReadOnlyCollection<CodeFunctionNode> children
        {
            get
            {
                List<CodeFunctionNode> _nodes = new List<CodeFunctionNode>();
                if (@base != null) _nodes.Add(@base);
                if (exponent != null) _nodes.Add(exponent);
                return _nodes.AsReadOnly();
            }
        }
    }
}