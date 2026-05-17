using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// effect가 대상 entity의 어떤 stat을 어떤 수치/어떤 방식으로 영향을 줄 것인지를 정의하는 scriptable object입니다.
    /// </summary>
    public class GameplayEffectStatModifierDefinition : AbstractGameplayEffectStatModifierDefinition
    {
        [SerializeField] private string m_StatName;
        public override string statName => m_StatName;

        [SerializeField] private ModifierOperationType m_Type;
        public override ModifierOperationType type => m_Type;
    }
}