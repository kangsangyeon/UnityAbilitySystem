using Core;
using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// effect가 대상 entity의 어떤 stat을 어떤 수치/어떤 방식으로 영향을 줄 것인지를 정의하는 scriptable object입니다.
    /// 이 scriptable object는 effect definition 내에 포함되어 저장하기 위해 사용합니다.
    /// </summary>
    public abstract class AbstractGameplayEffectStatModifierDefinition : ScriptableObject
    {
        public abstract string statName { get; }
        public abstract ModifierOperationType type { get; }

        [SerializeField] private NodeGraph m_Formula;
        public NodeGraph formula => m_Formula;
    }
}