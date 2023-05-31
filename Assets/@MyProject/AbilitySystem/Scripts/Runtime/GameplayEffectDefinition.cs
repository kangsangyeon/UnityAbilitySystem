
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
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
        /// <summary>
        /// 대상 entity에게 effect가 적용되면 적용할 stat modifier 목록입니다.
        /// </summary>
        [SerializeField] protected List<AbstractGameplayEffectStatModifierDefinition> m_ModifierDefinitions;

        public ReadOnlyCollection<AbstractGameplayEffectStatModifierDefinition> ModifierDefinitions =>
            m_ModifierDefinitions.AsReadOnly();

        /// <summary>
        /// effect가 적용될 때 생성될 특수 효과에 대한 정보를 기록한 scriptable object입니다.
        /// </summary>
        [SerializeField] private SpecialEffectDefinition m_SpecialEffectDefinition;

        public SpecialEffectDefinition specialEffectDefinition => m_SpecialEffectDefinition;

        /// <summary>
        /// effect의 태그 목록입니다.
        /// </summary>
        [SerializeField] private List<string> m_Tags;

        public ReadOnlyCollection<string> tags => m_Tags.AsReadOnly();
    }
}