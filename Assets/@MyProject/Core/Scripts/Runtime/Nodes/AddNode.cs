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
    }
}