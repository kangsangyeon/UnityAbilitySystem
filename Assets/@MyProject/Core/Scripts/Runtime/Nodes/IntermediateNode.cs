using System.Collections.ObjectModel;

namespace Core.Nodes
{
    public abstract class IntermediateNode : CodeFunctionNode
    {
        public abstract void RemoveChild(CodeFunctionNode _child, string _portName);
        public abstract void AddChild(CodeFunctionNode _child, string _portName);
        public abstract ReadOnlyCollection<CodeFunctionNode> children { get; }
    }
}