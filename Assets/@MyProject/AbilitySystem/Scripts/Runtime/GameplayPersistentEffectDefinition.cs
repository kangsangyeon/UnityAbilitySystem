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
    [CreateAssetMenu(
        fileName = "GameplayPersistentEffect",
        menuName = "AbilitySystem/Effect/GameplayPersistentEffect",
        order = 0)]
    public class GameplayPersistentEffectDefinition : GameplayEffectDefinition
    {
        /// <summary>
        /// 무한히 지속되는 effect인지에 대한 여부입니다.
        /// 이 속성이 true라면, duration에 영향을 받지 않으며 무한히 지속됩니다.
        /// </summary>
        [SerializeField] private bool m_IsInfinite;

        public bool isInfinite => m_IsInfinite;

        /// <summary>
        /// effect의 만료 시간을 계산하는 계산식 scriptable object입니다.
        /// </summary>
        [SerializeField] protected NodeGraph m_DurationFormula;

        public NodeGraph durationFormula => m_DurationFormula;

        [SerializeField] protected bool m_IsPeriodic;
        public bool isPeriodic => m_IsPeriodic;

        [SerializeField] protected float m_Period;
        public float period => m_Period;

        [SerializeField] private bool m_ExecutePeriodicEffectOnApplication;
        public bool executePeriodicEffectOnApplication => m_ExecutePeriodicEffectOnApplication;

        /// <summary>
        /// 대상 entity에게 effect가 적용되면 부여할 tag 목록입니다.
        /// </summary>
        [Tooltip("이 태그들은 내가 적용하려는 액터에 적용됩니다.")] [SerializeField]
        protected List<string> m_GrantedTags;

        public ReadOnlyCollection<string> grantedTags => m_GrantedTags.AsReadOnly();

        /// <summary>
        /// effect가 적용될 때 생성되는 특수 효과에 대한 정보를 기록한 scriptable object입니다.
        /// </summary>
        [SerializeField] private SpecialEffectDefinition m_SpecialPersistentEffectDefinition;

        public SpecialEffectDefinition specialPersistentEffectDefinition => m_SpecialPersistentEffectDefinition;
    }
}