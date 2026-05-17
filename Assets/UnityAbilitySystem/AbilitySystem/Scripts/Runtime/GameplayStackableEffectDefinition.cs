using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AbilitySystem
{
    public enum GameplayEffectStackingDurationPolicy
    {
        NeverRefresh,
        RefreshOnSuccessfulApplication
    }

    public enum GameplayEffectStackingPeriodPolicy
    {
        NeverRefresh,
        ResetOnSuccessfulApplication
    }

    public enum GameplayEffectStackingExpirationPolicy
    {
        NeverRefresh,
        RemoveSingleStackAndRefreshDuration
    }

    [EffectType(typeof(GameplayStackableEffect))]
    [CreateAssetMenu(
        fileName = "GameplayStackableEffectDefinition",
        menuName = "AbilitySystem/Effect/GameplayStackableEffectDefinition",
        order = 0)]
    public class GameplayStackableEffectDefinition : GameplayPersistentEffectDefinition
    {
        [SerializeField] private List<GameplayEffectDefinition> m_OverflowEffects;
        public ReadOnlyCollection<GameplayEffectDefinition> overflowEffects => m_OverflowEffects.AsReadOnly();

        [SerializeField] private bool m_DenyOverflowApplication;
        public bool denyOverflowApplication => m_DenyOverflowApplication;

        [SerializeField] private bool m_ClearStackOnOverflow;
        public bool clearStackOnOverflow => m_ClearStackOnOverflow;

        [SerializeField] private int m_StackLimitCount = 3;
        public int stackLimitCount => m_StackLimitCount;

        [SerializeField] private GameplayEffectStackingDurationPolicy m_StackDurationRefreshPolicy;
        public GameplayEffectStackingDurationPolicy stackDurationRefreshPolicy => m_StackDurationRefreshPolicy;

        [SerializeField] private GameplayEffectStackingPeriodPolicy m_StackPeriodResetPolicy;
        public GameplayEffectStackingPeriodPolicy stackPeriodResetPolicy => m_StackPeriodResetPolicy;

        [SerializeField] private GameplayEffectStackingExpirationPolicy m_StackExpirationPolicy;
        public GameplayEffectStackingExpirationPolicy stackExpirationPolicy => m_StackExpirationPolicy;
    }
}