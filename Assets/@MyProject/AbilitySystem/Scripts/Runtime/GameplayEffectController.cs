using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core;
using StatSystem;
using UnityEngine;
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
        public event System.Action initialized;

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

        public bool ApplyGameplayEffectToSelf(GameplayEffect _effectToApply)
        {
            // 이 effect가 면역에 의해 적용되지 말아야 하는지에 대한 여부를 판별합니다.

            bool _hasImmunity =
                m_ActiveEffects
                    .Any(e =>
                        e.isInhibited == false
                        && e.definition.grantedApplicationImmunityTags.Any(t =>
                            _effectToApply.definition.tags.Contains(t)));

            if (_hasImmunity)
            {
                Debug.Log($"{_effectToApply.definition.name}에 면역을 가지고 있어 적용되지 않습니다.");
                return false;
            }

            // 이 effect가 적용되기 위한 필요 조건을 만족하는지에 대한 여부를 판별합니다.

            if (m_TagController.SatisfiesRequirements(
                    _effectToApply.definition.applicationMustBePresentTags,
                    _effectToApply.definition.applicationMustBeAbsentTags) == false)
            {
                Debug.Log($"{_effectToApply.definition.name}을 적용하기 위한 필요 조건이 만족되지 않아 적용되지 않습니다.");
                return false;
            }

            if (_effectToApply is GameplayPersistentEffect _persistentEffectToApply)
            {
                if (m_TagController.SatisfiesRequirements(
                        _persistentEffectToApply.definition.persistMustBePresentTags,
                        _persistentEffectToApply.definition.persistMustBeAbsentTags) == false)
                {
                    Debug.Log($"지속 효과인 {_effectToApply.definition.name}이 지속되기 위한 필요 조건이 만족되지 않아 적용되지 않습니다.");
                    return false;
                }
            }

            // 이 effect가 적용됨으로써 삭제되어야 하는 effect의 목록을 조회하고 삭제합니다.

            List<GameplayPersistentEffect> _effectsToRemove =
                m_ActiveEffects
                    .Where(e => e.definition.tags.Any(t => _effectToApply.definition.removeEffectsWithTags.Contains(t)))
                    .ToList();

            _effectsToRemove.ForEach(e => RemoveActiveGameplayEffect(e, true));

            // 이 effect가 적용됨으로써 추가적으로 적용되어야 하는 effect의 목록을 읽고 적용합니다.

            foreach (GameplayEffectDefinition _additionalEffectDefinition
                     in _effectToApply.definition.additionalEffects)
            {
                EffectTypeAttribute _attribute =
                    _additionalEffectDefinition.GetType()
                        .GetCustomAttributes(true).OfType<EffectTypeAttribute>().FirstOrDefault();

                GameplayEffect _additionalEffect =
                    Activator.CreateInstance(
                        _attribute.type,
                        _additionalEffectDefinition, // definition
                        _effectToApply, // source
                        _effectToApply.instigator // instigator
                    ) as GameplayEffect;

                ApplyGameplayEffectToSelf(_additionalEffect);
            }

            bool _shouldAdd = true;
            if (_effectToApply is GameplayStackableEffect _stackableEffect)
            {
                GameplayStackableEffect _existingStackableEffect = m_ActiveEffects.Find(
                    _effect => _effect.definition == _effectToApply.definition) as GameplayStackableEffect;

                if (_existingStackableEffect != null)
                {
                    // 이 effect가 stackable effect이고,
                    // 이미 controller를 소유한 entity에게 적용중일 때 실행됩니다.

                    // 이미 적용중인 stackable effect인 경우,
                    // 기본적으로 이 effect를 controller의 effect 목록에 추가하지는 않습니다.
                    _shouldAdd = false;

                    if (_existingStackableEffect.stackCount == _existingStackableEffect.definition.stackLimitCount)
                    {
                        // 이 effect가 stackable effect이고,
                        // stack count가 limit에 닿은 상태에서 다시 한 번 stacking되려 할 때 호출됩니다.
                        // stackable effect의 definition의 overflow effect 목록을 읽고, 이를 적용합니다. 

                        foreach (GameplayEffectDefinition _effectDefinition
                                 in _existingStackableEffect.definition.overflowEffects)
                        {
                            EffectTypeAttribute _attribute =
                                _effectDefinition.GetType().GetCustomAttributes(true)
                                    .OfType<EffectTypeAttribute>().FirstOrDefault();

                            GameplayEffect _overflowEffect = Activator.CreateInstance(
                                _attribute.type,
                                _effectDefinition, // definition
                                _existingStackableEffect, // source
                                gameObject // instigator
                            ) as GameplayEffect;

                            ApplyGameplayEffectToSelf(_overflowEffect);
                        }

                        if (_existingStackableEffect.definition.clearStackOnOverflow)
                        {
                            // overflow시 stack이 clear되는 effect라면,
                            // 이 effect를 제거한 뒤 잠시 뒤의 코드에서 새로 추가합니다.
                            RemoveActiveGameplayEffect(_existingStackableEffect, true);
                            _shouldAdd = true;
                        }

                        if (_existingStackableEffect.definition.denyOverflowApplication)
                        {
                            // overflow가 일어나서는 안되는 effect가 overflow되었다면,
                            // 경고 메세지를 출력합니다.
                            Debug.Log("Deny overflow application!");
                            return false;
                        }
                    }
                }

                if (_shouldAdd == false)
                {
                    // 이 stackable effect가 이미 적용되어 있는 effect이며
                    // clear stack on overflow 속성이 false라서 새로 추가되지 않을 때 실행됩니다.

                    // 스택 카운트를 증가시킵니다.
                    _existingStackableEffect.stackCount = Math.Min(
                        _existingStackableEffect.stackCount + _stackableEffect.stackCount,
                        _existingStackableEffect.definition.stackLimitCount);

                    if (_existingStackableEffect.definition.stackDurationRefreshPolicy
                        == GameplayEffectStackingDurationPolicy.RefreshOnSuccessfulApplication)
                    {
                        // 이 stackable effect가 적용될 때마다 만료시간이 새로 초기화되어야 한다면,
                        // 만료시간을 재계산합니다.
                        _existingStackableEffect.remainingDuration =
                            _existingStackableEffect.definition.durationFormula.CalculateValue(gameObject);
                    }

                    if (_existingStackableEffect.definition.stackPeriodResetPolicy
                        == GameplayEffectStackingPeriodPolicy.ResetOnSuccessfulApplication)
                    {
                        // 이 stackable effect가 적용될 때마다 반복 주기 시간이 새로 초기화되어야 한다면,
                        // 이번 반복 주기의 끝까지 남은 시간을 초기화합니다.
                        _existingStackableEffect.remainingPeriod = _existingStackableEffect.definition.period;
                    }
                }
                else
                {
                    // 이 stackable effect가 적용되어 있지 않은 상태에서 새로이 적용되거나
                    // 또는 stack limit에 도달했으며 clear stack on overflow 속성이 true일 때 실행됩니다.
                    // effect 목록에 추가하고 적용합니다.
                    AddGameplayEffect(_stackableEffect);
                }
            }
            else if (_effectToApply is GameplayPersistentEffect _persistentEffect
                     && _shouldAdd)
            {
                // stackable effect이 아닌 persistent effect인 경우 이 쪽이 실행됩니다.
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

            return true;
        }

        private void AddGameplayEffect(GameplayPersistentEffect _effect)
        {
            m_ActiveEffects.Add(_effect);

            CheckOngoingTagRequirements(_effect);

            if (_effect.definition.isPeriodic
                && _effect.definition.executePeriodicEffectOnApplication)
            {
                if (_effect.isInhibited == false)
                    ExecuteGameplayEffect(_effect);
            }
        }

        private void RemoveActiveGameplayEffect(GameplayPersistentEffect _effect, bool _prematureRemoval)
        {
            m_ActiveEffects.Remove(_effect);

            if (_effect.isInhibited == false)
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
                        // 이 effect의 만료 시간이 다 되어 제거되어야 할 때 실행됩니다.

                        // 이 effect가 정말로 제거되어야 하는지에 대한 여부를 판별하는 값입니다.
                        // 이 effect가 stackable effect이고, expiration policy 설정으로 인해 만료 시간이 경과되어도 스택 카운트만 감소시킬 뿐 제거되지 말아야 하는 경우 이 값은 false로 설정됩니다.
                        bool _shouldRemove = true;

                        if (_activeEffect is GameplayStackableEffect _stackableEffect)
                        {
                            if (_stackableEffect.definition.stackExpirationPolicy
                                == GameplayEffectStackingExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                            {
                                // 이 effect가 stackable effect이고,
                                // 만료 정책에 의해 스택 카운트만 감소시킬 뿐 제거되지 말아야 할 때 실행됩니다.

                                --_stackableEffect.stackCount;

                                if (_stackableEffect.stackCount > 0)
                                {
                                    // 스택 카운트가 0이 되지 않았다면,
                                    // 만료 시간을 초기화할 뿐이고, effect를 제거하지 않습니다.
                                    _shouldRemove = false;
                                    _stackableEffect.remainingDuration =
                                        _stackableEffect.definition.durationFormula.CalculateValue(gameObject);
                                }
                            }
                        }

                        if (_shouldRemove)
                        {
                            _effectsToRemove.Add(_activeEffect);
                        }
                    }
                }

                if (_activeEffect.definition.isPeriodic)
                {
                    _activeEffect.remainingPeriod = Math.Max(_activeEffect.remainingPeriod - Time.deltaTime, 0f);
                    if (Mathf.Approximately(_activeEffect.remainingPeriod, 0f))
                    {
                        // 이 effect가 periodic 속성을 가지고 있고,
                        // 반복 주기의 끝에 왔을 때 실행됩니다.
                        // 다시 effect를 적용시키고, 다음 반복 주기가 시작될 때까지 남은 시간을 초기화합니다.

                        if (_activeEffect.isInhibited == false)
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

        private void CheckOngoingTagRequirements(string _tag)
        {
            m_ActiveEffects.ForEach(CheckOngoingTagRequirements);
        }

        private void CheckOngoingTagRequirements(GameplayPersistentEffect _effect)
        {
            bool _shouldBeInhibited = !m_TagController.SatisfiesRequirements(
                _effect.definition.uninhibitedMustBePresentTags,
                _effect.definition.uninhibitedMustBeAbsentTags);

            if (_effect.isInhibited != _shouldBeInhibited)
            {
                _effect.isInhibited = _shouldBeInhibited;

                if (_effect.isInhibited)
                {
                    RemoveUninhibitedEffects(_effect);
                }
                else
                {
                    if (_effect.definition.isPeriodic)
                    {
                        if (_effect.definition.periodicInhibitionPolicy ==
                            GameplayEffectPeriodInhibitionRemovedPolicy.ResetPeriod)
                        {
                            _effect.remainingPeriod = _effect.definition.period;
                        }
                        else if (_effect.definition.periodicInhibitionPolicy ==
                                 GameplayEffectPeriodInhibitionRemovedPolicy.ExecuteAndResetPeriod)
                        {
                            ExecuteGameplayEffect(_effect);
                            _effect.remainingPeriod = _effect.definition.period;
                        }
                    }

                    AddUninhibitedEffects(_effect);
                }
            }
        }

        private void CheckRemovalTagRequirements(string _tag)
        {
            m_ActiveEffects
                .Where(e => m_TagController.SatisfiesRequirements(e.definition.persistMustBePresentTags,
                    e.definition.persistMustBeAbsentTags) == false)
                .ToList()
                .ForEach(e => RemoveActiveGameplayEffect(e, true));
        }

        private void Awake()
        {
            m_StatController = GetComponent<StatController>();
            m_TagController = GetComponent<TagController>();
        }

        private void OnEnable()
        {
            m_StatController.initialized += OnStatControllerInitialized;
            if (m_StatController.IsInitialized())
                OnStatControllerInitialized();

            m_TagController.tagAdded += CheckOngoingTagRequirements;
            m_TagController.tagRemoved += CheckOngoingTagRequirements;
            m_TagController.tagAdded += CheckRemovalTagRequirements;
            m_TagController.tagRemoved += CheckRemovalTagRequirements;
        }

        private void OnDisable()
        {
            m_StatController.initialized -= OnStatControllerInitialized;

            m_TagController.tagAdded -= CheckOngoingTagRequirements;
            m_TagController.tagRemoved -= CheckOngoingTagRequirements;
            m_TagController.tagAdded -= CheckRemovalTagRequirements;
            m_TagController.tagRemoved -= CheckRemovalTagRequirements;
        }

        private void Update()
        {
            HandleDuration();
        }
    }
}