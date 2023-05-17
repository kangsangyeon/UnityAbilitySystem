using Core;
using UnityEngine;

namespace AbilitySystem
{
    [EffectType(typeof(GameplayPersistentEffect))]
    [CreateAssetMenu(fileName = "GameplayPersistentEffect", menuName = "AbilitySystem/Effect/GameplayPersistentEffect",
        order = 0)]
    public class GameplayPersistentEffectDefinition : GameplayEffectDefinition
    {
        [SerializeField] private bool m_IsInfinite;
        public bool isInfinite => m_IsInfinite;

        [SerializeField] private NodeGraph m_DurationFormula;
        public NodeGraph durationFormula => m_DurationFormula;
    }
}