using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Core.Nodes
{
    public class AddNode : IntermediateNode
    {
        [HideInInspector] public CodeFunctionNode appendA;
        [HideInInspector] public CodeFunctionNode appendB;

        public override float value => appendA.value + appendB.value;

        public override float CalculateValue(GameObject _source) =>
            appendA.CalculateValue(_source) + appendB.CalculateValue(_source);

        public override void RemoveChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) appendA = null;
            else appendB = null;
        }

        public override void AddChild(CodeFunctionNode _child, string _portName)
        {
            if (_portName.Equals("A")) appendA = _child;
            else appendB = _child;
        }

        public override ReadOnlyCollection<CodeFunctionNode> children
        {
            get
            {
                List<CodeFunctionNode> _nodes = new List<CodeFunctionNode>();
                if (appendA != null) _nodes.Add(appendA);
                if (appendB != null) _nodes.Add(appendB);
                return _nodes.AsReadOnly();
            }
        }

        public override object Clone()
        {
            var _addNode = ScriptableObject.CreateInstance<AddNode>();
            _addNode.guid = this.guid;
            _addNode.position = this.position;
            _addNode.appendA = this.appendA;
            _addNode.appendB = this.appendB;
            return _addNode;
        }

        public override void ReplaceChildNodeReference(Dictionary<CodeFunctionNode, CodeFunctionNode> _nodeDict)
        {
            appendA = _nodeDict[appendA];
            appendB = _nodeDict[appendB];
        }
    }
}