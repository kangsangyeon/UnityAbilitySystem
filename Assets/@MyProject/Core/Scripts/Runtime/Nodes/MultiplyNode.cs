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

        public override float CalculateValue(GameObject _source) =>
            factorA.CalculateValue(_source) * factorB.CalculateValue(_source);

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

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<MultiplyNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.factorA = this.factorA;
            _node.factorB = this.factorB;
            return _node;
        }

        public override void ReplaceChildNodeReference(
            Dictionary<CodeFunctionNode, CodeFunctionNode> _nodeDict)
        {
            factorA = _nodeDict[factorA];
            factorB = _nodeDict[factorB];
        }
    }
}