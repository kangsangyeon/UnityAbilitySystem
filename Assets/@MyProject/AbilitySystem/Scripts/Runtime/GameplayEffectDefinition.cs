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
        [SerializeField] private string m_Description;
        public string description => m_Description;

        /// <summary>
        /// 대상 entity에게 effect가 적용되면 적용할 stat modifier 목록입니다.
        /// </summary>
        [SerializeField] protected List<AbstractGameplayEffectStatModifierDefinition> m_ModifierDefinitions;

        public ReadOnlyCollection<AbstractGameplayEffectStatModifierDefinition> ModifierDefinitions =>
            m_ModifierDefinitions.AsReadOnly();

        /// <summary>
        /// 대상 entity에게 effect가 적용되면 적용할 상태 이상 effect 목록입니다.
        /// </summary>
        [SerializeField] private List<GameplayEffectDefinition> m_ConditionalEffects;

        public ReadOnlyCollection<GameplayEffectDefinition> conditionalEffects => m_ConditionalEffects.AsReadOnly();

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

        /// <summary>
        /// 이 effect가 적용될 때, 적용  대상에게 이미 적용중인 이 태그를 가진 effect들을 삭제합니다.
        /// </summary>
        [SerializeField] private List<string> m_RemoveEffectsWithTags;

        public ReadOnlyCollection<string> removeEffectsWithTags => m_RemoveEffectsWithTags.AsReadOnly();

        /// <summary>
        /// 이 effect가 적용되기 위해 필요한 태그 목록입니다.
        /// 적용 대상의 tag controller가 이 태그를 하나라도 소유하지 않으면 적용되지 않습니다.
        /// </summary>
        [SerializeField] private List<string> m_ApplicationMustBePresentTags;

        public ReadOnlyCollection<string> applicationMustBePresentTags => m_ApplicationMustBePresentTags.AsReadOnly();

        /// <summary>
        /// 이 effect가 적용되기 위해 가지고 있지 말아야 할 할 태그 목록입니다.
        /// 적용 대상의 tag controller가 이 태그를 하나라도 소유하고 있으면 적용되지 않습니다.
        /// </summary>
        [SerializeField] private List<string> m_ApplicationMustBeAbsentTags;

        public ReadOnlyCollection<string> applicationMustBeAbsentTags => m_ApplicationMustBeAbsentTags.AsReadOnly();
    }
}