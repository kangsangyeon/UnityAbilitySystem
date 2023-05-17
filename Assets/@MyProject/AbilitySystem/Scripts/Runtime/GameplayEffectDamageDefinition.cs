using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// effect를 적용 받는 대상 entity에 부여하는 데미지 modifier를 정의하는 scriptable object 클래스입니다.
    /// </summary>
    public class GameplayEffectDamageDefinition : AbstractGameplayEffectStatModifierDefinition
    {
        public override string statName => "Health";
        public override ModifierOperationType type => ModifierOperationType.Additive;

        [SerializeField] private bool m_CanCriticalHit;
        public bool canCriticalHit => m_CanCriticalHit;
    }
}