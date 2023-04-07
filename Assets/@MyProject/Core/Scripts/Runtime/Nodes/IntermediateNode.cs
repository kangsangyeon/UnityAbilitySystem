using System.Collections.ObjectModel;

namespace Core.Nodes
{
    public abstract class IntermediateNode : CodeFunctionNode
    {
        public abstract void RemoveChild(CodeFunctionNode child, string portName);
        public abstract void AddChild(CodeFunctionNode child, string portName);
        public abstract ReadOnlyCollection<CodeFunctionNode> children { get; }
    }
}