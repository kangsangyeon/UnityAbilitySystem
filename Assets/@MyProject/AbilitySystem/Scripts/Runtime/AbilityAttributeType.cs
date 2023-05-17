using System;

namespace AbilitySystem
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AbilityAttributeType : System.Attribute
    {
        public readonly Type type;

        public AbilityAttributeType(Type _type)
        {
            this.type = _type;
        }
    }
}