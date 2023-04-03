using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NodeType : Attribute
    {
        public readonly Type type;
        public NodeType(Type _type) => this.type = _type;
    }
}