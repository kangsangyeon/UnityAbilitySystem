using System;

namespace StatSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CustomAttribute : System.Attribute
    {
        public string attributeName;
        public CustomAttribute(string _attributeName) => this.attributeName = _attributeName;
    }
}