using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core;
using StatSystem;
using UnityEngine;
using UnityEngine.Events;
using Attribute = StatSystem.Attribute;

namespace AbilitySystem
{
    /// <summary>
    /// controller를 소유하는 entity에 effect를 적용할 때 사용합니다.
    /// 적용된 persistent effect의 유효 시간이 만료되었는지도 확인하고, 만료된 경우 삭제합니다.
    /// </summary>
    [RequireComponent(typeof(StatController))]
    [RequireComponent(typeof(TagController))]
    public partial class GameplayEffectController : MonoBehaviour
    {
        /// <summary>
        /// controller를 소유한 entity에게 적용중인 persistent effect 목록입니다.
        /// </summary>
        protected List<GameplayPersistentEffect> m_ActiveEffects = new List<GameplayPersistentEffect>();

        public ReadOnlyCollection<GameplayPersistentEffect> activeEffects => m_ActiveEffects.AsReadOnly();

        /// <summary>
        /// 이 controller 컴포넌트가 시작될 때 controller를 소유한 entity에게 적용되는 effect 목록입니다.
        /// </summary>
        [SerializeField] private List<GameplayEffectDefinition> m_StartEffectDefinitions;

        /// <summary>
        /// 이 controller가 초기화되었는지에 대한 여부입니다.
        /// </summary>
        private bool m_IsInitialized;

        public bool isInitialized => m_IsInitialized;

        /// <summary>
        /// 이 controller가 초기화되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public UnityEvent initialized;

        protected StatController m_StatController;
        protected TagController m_TagController;

        public bool CanApplyAttributeModifiers(GameplayEffectDefinition _effectDefinition)
        {
            foreach (var _modifierDefinition in _effectDefinition.ModifierDefinitions)
            {
                m_StatController.stats.TryGetValue(_modifierDefinition.statName, out Stat _stat);

                if (_stat == null)
                {
                    Debug.LogWarning($"{_modifierDefinition.statName} attribute를 찾을 수 없습니다!");
                    return false;
                }

                Attribute _attribute = _stat as Attribute;

                if (_attribute == null)
                {
                    Debug.LogWarning($"{_modifierDefinition.statName}이 attribute가 아닙니다! attribute만 cost로 사용될 수 있습니다.");
                    return false;
                }

                if (_modifierDefinition.type != ModifierOperationType.Additive)
                {
                    Debug.LogWarning("cost는 additive만 지원합니다!");
                    return false;
                }

                if (_attribute.currentValue < Mathf.Abs(_modifierDefinition.formula.CalculateValue(gameObject)))
                {
                    Debug.Log($"{_effectDefinition.name} 사용에 필요한 cost를 만족하지 않습니다! ({_modifierDefinition.statName} 부족)");
                    return false;
                }
            }

            return true;
        }

        public void ApplyGameplayEffectToSelf(GameplayEffect _effectToApply)
        {
            if (_effectToApply is GameplayPersistentEffect _persistentEffect)
            {
                // persistent effect인 경우,
                // 나중에 effect의 만료 시간을 초과하거나 삭제를 원할 때 삭제될 수 있어야 하므로 목록에 추가하고 적용합니다.
                AddGameplayEffect(_persistentEffect);
            }
            else
            {
                // 아닌 경우, 일회성으로 적용되는 effect이므로
                // 목록에 저장하지 않고 적용만 합니다.
                ExecuteGameplayEffect(_effectToApply);
            }

            if (_effectToApply.definition.specialEffectDefinition != null)
            {
                // 특수 효과 재생이 필요한 경우 재생합니다.
                PlaySpecialEffect(_effectToApply);
            }
        }

        private void AddGameplayEffect(GameplayPersistentEffect _effect)
        {
            m_ActiveEffects.Add(_effect);
            AddUninhibitedEffects(_effect);

            if (_effect.definition.isPeriodic
                && _effect.definition.executePeriodicEffectOnApplication)
            {
                ExecuteGameplayEffect(_effect);
            }
        }

        private void RemoveActiveGameplayEffect(GameplayPersistentEffect _effect, bool _prematureRemoval)
        {
            m_ActiveEffects.Remove(_effect);
            RemoveUninhibitedEffects(_effect);
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

            foreach (string _tag in _effect.definition.grantedTags)
            {
                m_TagController.AddTag(_tag);
            }

            if (_effect.definition.specialPersistentEffectDefinition != null)
            {
                PlaySpecialEffect(_effect);
            }
        }

        /// <summary>
        /// effect가 제거되어 stat들에 영향을 그만 주어야 할 때 호출합니다.
        /// effect modifier에 의해 영향을 받는 각 stat들은 이 effect로부터 영향받는 modifier를 전부 제거합니다.
        /// </summary
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

            foreach (string _tag in _effect.definition.grantedTags)
            {
                m_TagController.RemoveTag(_tag);
            }

            if (_effect.definition.specialPersistentEffectDefinition != null)
            {
                StopSpecialEffect(_effect);
            }
        }

        /// <summary>
        /// persistent effect가 만료되었는지 확인하고, 만료된 경우 해당 effect를 삭제합니다.
        /// </summary>
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

                if (_activeEffect.definition.isPeriodic)
                {
                    _activeEffect.remainingPeriod = Math.Max(_activeEffect.remainingPeriod - Time.deltaTime, 0f);
                    if (Mathf.Approximately(_activeEffect.remainingPeriod, 0f))
                    {
                        ExecuteGameplayEffect(_activeEffect);
                        _activeEffect.remainingPeriod = _activeEffect.definition.period;
                    }
                }
            }

            foreach (GameplayPersistentEffect _effect in _effectsToRemove)
            {
                RemoveActiveGameplayEffect(_effect, false);
            }
        }

        private void OnStatControllerInitialized()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (GameplayEffectDefinition _effectDefinition in m_StartEffectDefinitions)
            {
                EffectTypeAttribute _attribute =
                    _effectDefinition.GetType().GetCustomAttributes(true)
                        .OfType<EffectTypeAttribute>().FirstOrDefault();

                GameplayEffect _effect = Activator.CreateInstance(
                    _attribute.type,
                    _effectDefinition, // definition
                    m_StartEffectDefinitions, // source
                    gameObject // instigator
                ) as GameplayEffect;

                ApplyGameplayEffectToSelf(_effect);
            }

            m_IsInitialized = true;
            initialized?.Invoke();
        }

        private void Awake()
        {
            m_StatController = GetComponent<StatController>();
            m_TagController = GetComponent<TagController>();
        }

        private void OnEnable()
        {
            m_StatController.initialized.AddListener(OnStatControllerInitialized);
            if (m_StatController.IsInitialized())
                OnStatControllerInitialized();
        }

        private void Update()
        {
            HandleDuration();
        }
    }
}