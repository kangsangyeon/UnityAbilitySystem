﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
using UnityEngine;

namespace AbilitySystem
{
    public enum GameplayEffectPeriodInhibitionRemovedPolicy
    {
        NeverReset,
        ResetPeriod,
        ExecuteAndResetPeriod
    }

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

        /// <summary>
        /// 이 effect가 대상 entity에게 일정 시간마다 반복하며 적용되어야 할 때 참으로 설정하는 플래그입니다.
        /// 일반적으로 이 속성이 참이라면 이 effect는 (일시적인 변화만을 주기 위한 용도로) attribute modifier만을 가집니다.
        /// </summary>
        [SerializeField] protected bool m_IsPeriodic;

        public bool isPeriodic => m_IsPeriodic;

        /// <summary>
        /// isPeriodic 플래그가 참일 때 사용되는 값이며, 반복 주기입니다.
        /// </summary>
        [SerializeField] protected float m_Period;

        public float period => m_Period;

        /// <summary>
        /// isPeriodic 플래그가 참일 때 사용되는 값이며,
        /// effect가 적용되기 시작한 순간에 적용을 필요로 할 때 참으로 설정하는 플래그입니다.
        /// 그렇지 않으면 매번 반복되는 주기 끝에서만 적용합니다.
        /// </summary>
        [SerializeField] private bool m_ExecutePeriodicEffectOnApplication;

        public bool executePeriodicEffectOnApplication => m_ExecutePeriodicEffectOnApplication;

        /// <summary>
        /// 대상 entity에게 effect가 적용되면 부여할 tag 목록입니다.
        /// </summary>
        [Tooltip("이 태그들은 내가 적용하려는 액터에 적용됩니다.")] [SerializeField]
        protected List<string> m_GrantedTags;

        public ReadOnlyCollection<string> grantedTags => m_GrantedTags.AsReadOnly();

        /// <summary>
        /// 대상 entity에게 effect가 적용되면 부여할 면역 tag 목록입니다.
        /// 예를 들어 대상이 "poison" 태그를 면역으로 가지고 있을 때, "poison" 태그를 가진 effect는 대상에게 적용되지 않습니다.
        /// </summary>
        [SerializeField] private List<string> m_GrantedApplicationImmunityTags;

        public ReadOnlyCollection<string> grantedApplicationImmunityTags => m_GrantedApplicationImmunityTags.AsReadOnly();

        [SerializeField] private List<string> m_UninhibitedMustBePresentTags;
        public ReadOnlyCollection<string> uninhibitedMustBePresentTags => m_UninhibitedMustBePresentTags.AsReadOnly();

        [SerializeField] private List<string> m_UninhibitedMustBeAbsentTags;
        public ReadOnlyCollection<string> uninhibitedMustBeAbsentTags => m_UninhibitedMustBeAbsentTags.AsReadOnly();

        [SerializeField] private List<string> m_PersistMustBePresentTags;
        public ReadOnlyCollection<string> persistMustBePresentTags => m_PersistMustBePresentTags.AsReadOnly();

        [SerializeField] private List<string> m_PersisMustBeAbsentTags;
        public ReadOnlyCollection<string> persistMustBeAbsentTags => m_PersisMustBeAbsentTags.AsReadOnly();

        /// <summary>
        /// effect가 적용될 때 생성되는 특수 효과에 대한 정보를 기록한 scriptable object입니다.
        /// </summary>
        [SerializeField] private SpecialEffectDefinition m_SpecialPersistentEffectDefinition;

        public SpecialEffectDefinition specialPersistentEffectDefinition => m_SpecialPersistentEffectDefinition;

        [SerializeField] private GameplayEffectPeriodInhibitionRemovedPolicy m_PeriodicInhibitionPolicy;
        public GameplayEffectPeriodInhibitionRemovedPolicy periodicInhibitionPolicy => m_PeriodicInhibitionPolicy;
    }
}