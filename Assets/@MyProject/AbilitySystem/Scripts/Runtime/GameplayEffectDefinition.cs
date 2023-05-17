using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 대상 entity의 stat에 어떤 영향들을 줄 것인지 정의하는 scriptable object입니다.
    /// effect는 stat modifier definition 목록을 가집니다.
    /// </summary>
    [EffectType(typeof(GameplayEffect))]
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "AbilitySystem/Effect/GameplayEffect", order = 0)]
    public class GameplayEffectDefinition : ScriptableObject
    {
        [SerializeField] protected List<AbstractGameplayEffectStatModifierDefinition> m_ModifierDefinitions;

        public ReadOnlyCollection<AbstractGameplayEffectStatModifierDefinition> ModifierDefinitions =>
            m_ModifierDefinitions.AsReadOnly();
    }
}