using System.Collections.Generic;
using System.Collections.ObjectModel;
using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    public class GameplayEffect
    {
        protected GameplayEffectDefinition m_Definition;
        public GameplayEffectDefinition definition => m_Definition;

        private object m_Source;
        public object source => m_Source;

        private GameObject m_Instigator;
        public GameObject instigator => m_Instigator;

        private List<StatModifier> m_Modifiers = new List<StatModifier>();
        public ReadOnlyCollection<StatModifier> modifiers => m_Modifiers.AsReadOnly();

        public GameplayEffect(GameplayEffectDefinition _definition, object _source, GameObject _instigator)
        {
            m_Definition = _definition;
            m_Source = _source;
            m_Instigator = _instigator;

            StatController _statController = _instigator.GetComponent<StatController>();

            foreach (var _modifierDefinition in definition.ModifierDefinitions)
            {
                StatModifier _statModifier;

                if (_modifierDefinition is GameplayEffectDamageDefinition _damageDefinition)
                {
                    HealthModifier _healthModifier = new HealthModifier()
                    {
                        magnitude = Mathf.RoundToInt(_modifierDefinition.formula.CalculateValue(_instigator)),
                        isCriticalHit = false
                    };

                    if (_damageDefinition.canCriticalHit)
                    {
                        if (_statController.stats["CriticalHitChance"].value / 100f >= Random.value)
                        {
                            _healthModifier.magnitude = Mathf.RoundToInt(
                                _healthModifier.magnitude
                                * _statController.stats["CriticalHitMultiplier"].value
                                / 100f);
                            _healthModifier.isCriticalHit = true;
                        }
                    }

                    _statModifier = _healthModifier;
                }
                else
                {
                    _statModifier = new StatModifier()
                    {
                        magnitude = Mathf.RoundToInt(_modifierDefinition.formula.CalculateValue(_instigator))
                    };
                }

                _statModifier.source = this;
                _statModifier.type = _modifierDefinition.type;
                m_Modifiers.Add(_statModifier);
            }
        }
    }
}