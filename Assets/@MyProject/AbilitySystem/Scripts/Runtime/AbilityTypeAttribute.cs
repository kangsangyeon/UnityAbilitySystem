using System;

namespace AbilitySystem
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AbilityTypeAttribute : System.Attribute
    {
        public readonly Type type;

        public AbilityTypeAttribute(Type _type)
        {
            this.type = _type;
        }
    }
}