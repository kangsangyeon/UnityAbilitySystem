using System;
using StatSystem;
using UnityEngine;
using Attribute = StatSystem.Attribute;

namespace AbilitySystem
{
    public class GameplayEffectController : MonoBehaviour
    {
        protected StatController m_StatController;

        public void ApplyGameplayEffectToSelf(GameplayEffect _effectToApply)
        {
            ExecuteGameplayEffect(_effectToApply);
        }

        private void ExecuteGameplayEffect(GameplayEffect _effect)
        {
            for (int i = 0; i < _effect.definition.ModifierDefinitions.Count; ++i)
            {
                if (m_StatController.stats.TryGetValue(
                        _effect.definition.ModifierDefinitions[i].statName,
                        out Stat _stat))
                {
                    if (_stat is Attribute _attribute)
                    {
                        _attribute.ApplyModifier(_effect.modifiers[i]);
                    }
                }
            }
        }

        private void Awake()
        {
            m_StatController = GetComponent<StatController>();
        }
    }
}