using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AbilitySystem
{
    [EffectType(typeof(GameplayEffect))]
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "AbilitySystem/Effect/GameplayEffect", order = 0)]
    public class GameplayEffectDefinition : ScriptableObject
    {
        [SerializeField] protected List<AbstractGameplayEffectStatModifierDefinition> m_ModifierDefinitions;

        public ReadOnlyCollection<AbstractGameplayEffectStatModifierDefinition> ModifierDefinitions =>
            m_ModifierDefinitions.AsReadOnly();
    }
}