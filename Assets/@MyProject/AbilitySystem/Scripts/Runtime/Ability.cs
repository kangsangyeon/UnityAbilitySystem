using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class Ability
    {
        protected AbilityDefinition m_Definition;
        public AbilityDefinition definition => m_Definition;

        protected AbilityController m_Controller;

        public Ability(AbilityDefinition _definition, AbilityController _controller)
        {
            m_Definition = _definition;
            m_Controller = _controller;
        }

        internal void ApplyEffects(GameObject _other)
        {
            ApplyEffectsInternal(m_Definition.gameplayEffectDefinitions, _other);
        }

        private void ApplyEffectsInternal(
            ReadOnlyCollection<GameplayEffectDefinition> _effectDefinitions,
            GameObject _other)
        {
            if (_other.TryGetComponent(out GameplayEffectController _effectController))
            {
                foreach (GameplayEffectDefinition _definition in _effectDefinitions)
                {
                    EffectTypeAttribute _attribute = _definition.GetType().CustomAttributes
                        .OfType<EffectTypeAttribute>().FirstOrDefault();

                    GameplayEffect _effect = Activator.CreateInstance(
                        _attribute.type,
                        _definition, // definition
                        this, // source
                        _other // instigator
                    ) as GameplayEffect;

                    _effectController.ApplyGameplayEffectToSelf(_effect);
                }
            }
        }
    }
}