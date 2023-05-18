using UnityEngine;

namespace AbilitySystem
{
    public abstract class ActiveAbilityDefinition : AbilityDefinition
    {
        [SerializeField] private string m_AnimationName;
        public string animationName => m_AnimationName;
    }
}