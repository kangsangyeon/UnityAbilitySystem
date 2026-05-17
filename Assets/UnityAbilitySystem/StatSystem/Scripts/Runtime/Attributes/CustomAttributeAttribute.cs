using System;

namespace StatSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CustomAttributeAttribute : System.Attribute
    {
        public readonly string attributeName;
        public CustomAttributeAttribute(string _attributeName) => this.attributeName = _attributeName;
    }
}