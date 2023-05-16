using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using StatSystem;
using UnityEngine;
using Attribute = StatSystem.Attribute;

namespace AbilitySystem
{
    public class GameplayEffectController : MonoBehaviour
    {
        protected List<GameplayPersistentEffect> m_ActiveEffects = new List<GameplayPersistentEffect>();
        public ReadOnlyCollection<GameplayPersistentEffect> activeEffects => m_ActiveEffects.AsReadOnly();

        protected StatController m_StatController;

        public void ApplyGameplayEffectToSelf(GameplayEffect _effectToApply)
        {
            if (_effectToApply is GameplayPersistentEffect _persistentEffect)
            {
                AddGameplayEffect(_persistentEffect);
            }
            else
            {
                ExecuteGameplayEffect(_effectToApply);
            }
        }

        private void AddGameplayEffect(GameplayPersistentEffect _effect)
        {
            m_ActiveEffects.Add(_effect);
            AddUninhibitedEffects(_effect);
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

        /// <summary>
        /// effect의 modifier들에 의해 stat들에 영향을 주려할 때 호출합니다.
        /// 각 modifier는 자신이 대응하는 stat에 추가됩니다.
        /// </summary>
        private void AddUninhibitedEffects(GameplayPersistentEffect _effect)
        {
            for (int i = 0; i < _effect.modifiers.Count; ++i)
            {
                if (m_StatController.stats.TryGetValue(
                        _effect.definition.ModifierDefinitions[i].statName,
                        out Stat _stat))
                {
                    _stat.AddModifier(_effect.modifiers[i]);
                }
            }
        }

        /// <summary>
        /// effect가 제거되어 stat들에 영향을 그만 주어야 할 때 호출합니다.
        /// effect modifier에 의해 영향을 받는 각 stat들은 이 effect로부터 영향받는 modifier를 전부 제거합니다.
        /// </summary>
        private void RemoveUninhibitedEffects(GameplayPersistentEffect _effect)
        {
            for (int i = 0; i < _effect.modifiers.Count; ++i)
            {
                if (m_StatController.stats.TryGetValue(
                        _effect.definition.ModifierDefinitions[i].statName,
                        out Stat _stat))
                {
                    _stat.RemoveModifierFromSource(_effect);
                }
            }
        }

        private void RemoveActiveGameplayEffect(GameplayPersistentEffect _effect, bool _prematureRemoval)
        {
            m_ActiveEffects.Remove(_effect);
            RemoveUninhibitedEffects(_effect);
        }

        private void HandleDuration()
        {
            List<GameplayPersistentEffect> _effectsToRemove = new List<GameplayPersistentEffect>();
            foreach (GameplayPersistentEffect _activeEffect in m_ActiveEffects)
            {
                if (!_activeEffect.definition.isInfinite)
                {
                    _activeEffect.remainingDuration = Math.Max(_activeEffect.remainingDuration - Time.deltaTime, 0f);
                    if (Mathf.Approximately(_activeEffect.remainingDuration, 0f))
                    {
                        _effectsToRemove.Add(_activeEffect);
                    }
                }
            }

            foreach (GameplayPersistentEffect _effect in _effectsToRemove)
            {
                RemoveActiveGameplayEffect(_effect, false);
            }
        }

        private void Awake()
        {
            m_StatController = GetComponent<StatController>();
        }

        private void Update()
        {
            HandleDuration();
        }
    }
}