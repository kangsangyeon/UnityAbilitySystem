using System;

namespace AbilitySystem
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EffectTypeAttribute : System.Attribute
    {
        public readonly Type type;

        public EffectTypeAttribute(Type _type)
        {
            this.type = _type;
        }
    }
}