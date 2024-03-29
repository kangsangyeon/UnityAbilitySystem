﻿using CombatSystem.Core;
using Core;
using StatSystem;
using UnityEngine;

namespace Game.Scripts.Runtime
{
    public class CombatableCharacter : MonoBehaviour, IDamageable
    {
        protected const string k_Health = "Health";
        protected bool m_IsInitialized;

        public int health => (m_StatController.stats[k_Health] as StatSystem.Attribute).currentValue;
        public int maxHealth => (m_StatController.stats[k_Health] as StatSystem.Attribute).value;
        public event System.Action healthChanged;
        public event System.Action maxHealthChanged;
        public bool isInitialized => m_IsInitialized;
        public event System.Action initialized;
        public event System.Action willUninitialize;
        public event System.Action defeated;
        public event System.Action<int> healed;
        public event System.Action<int, bool> damaged;

        protected StatController m_StatController;

        protected virtual void Awake()
        {
            m_StatController = GetComponent<StatController>();
        }

        protected virtual void OnEnable()
        {
            m_StatController.initialized += OnStatControllerInitialized;
            if (m_StatController.IsInitialized())
                OnStatControllerInitialized();
        }

        private void OnDisable()
        {
            m_StatController.initialized -= OnStatControllerInitialized;
            if (m_StatController.IsInitialized())
                OnStatControllerWillUninitialize();
        }

        private void OnStatControllerInitialized()
        {
            var _statControllerStat = m_StatController.stats[k_Health] as StatSystem.Attribute;
            _statControllerStat.valueChanged += OnMaxHealthChanged;
            _statControllerStat.currentValueChanged += OnHealthChanged;
            _statControllerStat.appliedModifier += OnAppliedModifier;
            m_IsInitialized = true;
            initialized?.Invoke();
        }

        private void OnStatControllerWillUninitialize()
        {
            var _statControllerStat = m_StatController.stats[k_Health] as StatSystem.Attribute;
            _statControllerStat.valueChanged -= OnMaxHealthChanged;
            _statControllerStat.currentValueChanged -= OnHealthChanged;
            _statControllerStat.appliedModifier -= OnAppliedModifier;
            m_IsInitialized = false;
            willUninitialize?.Invoke();
        }

        private void OnMaxHealthChanged()
        {
            maxHealthChanged?.Invoke();
        }

        private void OnHealthChanged()
        {
            healthChanged?.Invoke();
        }

        /// <summary>
        /// 이 캐릭터의 StatController에 Modifier가 적용되었을 때 호출되는 콜백입니다.
        /// </summary>
        private void OnAppliedModifier(StatModifier _modifier)
        {
            if (_modifier.magnitude > 0)
            {
                healed?.Invoke(_modifier.magnitude);
            }
            else
            {
                // critical hit은 HealthModifier를 사용할 때만 줄 수 있습니다.

                bool _isCriticalHit = false;
                if (_modifier is HealthModifier _healthModifier)
                {
                    _isCriticalHit = _healthModifier.isCriticalHit;
                }

                damaged?.Invoke(_modifier.magnitude, _isCriticalHit);
                if ((m_StatController.stats[k_Health] as StatSystem.Attribute).currentValue == 0)
                    defeated?.Invoke();
            }
        }

        /// <summary>
        /// 데미지를 받습니다.
        /// </summary>
        public void TakeDamage(IDamage _rawDamage)
        {
            (m_StatController.stats[k_Health] as StatSystem.Attribute).ApplyModifier(new HealthModifier()
            {
                magnitude = _rawDamage.magnitude,
                type = ModifierOperationType.Additive,
                source = _rawDamage.source,
                isCriticalHit = _rawDamage.isCriticalHit,
                instigator = _rawDamage.instigator
            });
        }

        /// <summary>
        /// 대상에게 물리 공격력만큼 데미지를 입힘니다.
        /// </summary>
        /// <param name="_source">데미지를 입히기 위해 사용한 오브젝트입니다.</param>
        /// <param name="_target">데미지를 입을 대상입니다.</param>
        public void ApplyDamage(Object _source, GameObject _target)
        {
            // 대상 게임오브젝트의 IDamageable 컴포넌트에 StatModifier rawDamage를 부여합니다.
            IDamageable _damageable = _target.GetComponent<IDamageable>();
            HealthModifier _rawDamage = new HealthModifier()
            {
                magnitude = -1 * m_StatController.stats["PhysicalAttack"].value,
                type = ModifierOperationType.Additive,
                source = _source,
                instigator = this.gameObject
            };

            // 크리티컬 공격에 성공한 경우, 공격력을 높입니다.
            if (m_StatController.stats["CriticalHitChance"].value / 100.0f >= Random.value)
            {
                _rawDamage.magnitude = Mathf.RoundToInt(_rawDamage.magnitude *
                    m_StatController.stats["CriticalHitMultiplier"].value / 100.0f);
                _rawDamage.isCriticalHit = true;
            }

            _damageable.TakeDamage(_rawDamage);
        }
    }
}