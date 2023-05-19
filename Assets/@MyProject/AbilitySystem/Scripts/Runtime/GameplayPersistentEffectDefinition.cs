using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 만료 시간을 가지는 effect scriptable object입니다.
    /// 만료 시간을 초과하면 대상 entity에 적용된 effect의 영향을 모두 제거합니다.
    /// </summary>
    [EffectType(typeof(GameplayPersistentEffect))]
    [CreateAssetMenu(fileName = "GameplayPersistentEffect", menuName = "AbilitySystem/Effect/GameplayPersistentEffect",
        order = 0)]
    public class GameplayPersistentEffectDefinition : GameplayEffectDefinition
    {
        [SerializeField] private bool m_IsInfinite;
        public bool isInfinite => m_IsInfinite;

        [SerializeField] private NodeGraph m_DurationFormula;
        public NodeGraph durationFormula => m_DurationFormula;

        [Tooltip("이 태그들은 내가 적용하려는 액터에 적용됩니다.")]
        [SerializeField] protected List<string> m_GrantedTags;
        public ReadOnlyCollection<string> grantedTags => m_GrantedTags.AsReadOnly();
    }
}