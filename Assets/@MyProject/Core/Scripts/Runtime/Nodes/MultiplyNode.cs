using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Core.Nodes
{
    public class MultiplyNode : IntermediateNode
    {
        [HideInInspector] public CodeFunctionNode factorA;
        [HideInInspector] public CodeFunctionNode factorB;

        public override float value => factorA.value * factorB.value;

        public override void RemoveChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) factorA = null;
            else factorB = null;
        }

        public override void AddChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) factorA = _child;
            else factorB = _child;
        }

        public override ReadOnlyCollection<CodeFunctionNode> children
        {
            get
            {
                List<CodeFunctionNode> _nodes = new List<CodeFunctionNode>();
                if (factorA != null) _nodes.Add(factorA);
                if (factorB != null) _nodes.Add(factorB);
                return _nodes.AsReadOnly();
            }
        }
    }
}