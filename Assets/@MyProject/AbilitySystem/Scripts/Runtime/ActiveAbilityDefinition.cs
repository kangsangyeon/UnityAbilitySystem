using UnityEngine;

namespace AbilitySystem
{
    public abstract class ActiveAbilityDefinition : AbilityDefinition
    {
        [SerializeField] private string m_AnimationName;
        public string animationName => m_AnimationName;

        [SerializeField] protected GameplayEffectDefinition m_Cost;
        public GameplayEffectDefinition cost => m_Cost;
    }
}