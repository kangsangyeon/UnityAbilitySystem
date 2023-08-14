using System;
using UnityEngine;

namespace Core.Nodes
{
    public abstract class CodeFunctionNode : AbstractNode, ICloneable
    {
        public abstract float value { get; }
        public abstract float CalculateValue(GameObject _source);
        public abstract object Clone();
    }
}