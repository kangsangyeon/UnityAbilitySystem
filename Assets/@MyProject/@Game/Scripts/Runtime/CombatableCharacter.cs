using CombatSystem.Core;
using StatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Runtime
{
    public class CombatableCharacter : MonoBehaviour, IDamageable
    {
        protected const string k_Health = "Health";
        protected bool m_IsInitialized;

        public int health => (m_StatController.stats[k_Health] as StatSystem.Attribute).currentValue;
        public int maxHealth => (m_StatController.stats[k_Health] as StatSystem.Attribute).value;
        public UnityEvent healthChanged { get; set; } = new UnityEvent();
        public UnityEvent maxHealthChanged { get; set; } = new UnityEvent();
        public bool isInitialized => m_IsInitialized;
        public UnityEvent initialized { get; set; } = new UnityEvent();
        public UnityEvent willUninitialize { get; set; } = new UnityEvent();
        public UnityEvent defeated { get; set; } = new UnityEvent();
        public UnityEvent<int> healed { get; set; } = new UnityEvent<int>();
        public UnityEvent<int, bool> damaged { get; set; } = new UnityEvent<int, bool>();

        protected StatController m_StatController;

        protected virtual void Awake()
        {
            m_StatController = GetComponent<StatController>();
        }

        protected virtual void OnEnable()
        {
            m_StatController.initialized.AddListener(OnStatControllerInitialized);
            if (m_StatController.IsInitialized())
                OnStatControllerInitialized();
        }

        private void OnDisable()
        {
            m_StatController.initialized.RemoveListener(OnStatControllerInitialized);
            if (m_StatController.IsInitialized())
                OnStatControllerWillUninitialize();
        }

        private void OnStatControllerInitialized()
        {
            var _statControllerStat = m_StatController.stats[k_Health] as StatSystem.Attribute;
            _statControllerStat.valueChanged.AddListener(OnMaxHealthChanged);
            _statControllerStat.currentValueChanged.AddListener(OnHealthChanged);
            _statControllerStat.appliedModifier.AddListener(OnAppliedModifier);
            m_IsInitialized = true;
            initialized?.Invoke();
        }

        private void OnStatControllerWillUninitialize()
        {
            var _statControllerStat = m_StatController.stats[k_Health] as StatSystem.Attribute;
            _statControllerStat.valueChanged.RemoveListener(OnMaxHealthChanged);
            _statControllerStat.currentValueChanged.RemoveListener(OnHealthChanged);
            _statControllerStat.appliedModifier.RemoveListener(OnAppliedModifier);
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
                var _healthModifier = _modifier as HealthModifier;
                damaged?.Invoke(_healthModifier.magnitude, _healthModifier.isCriticalHit);
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
                _rawDamage.magnitude = Mathf.RoundToInt(_rawDamage.magnitude * m_StatController.stats["CriticalHitMultiplier"].value / 100.0f);
                _rawDamage.isCriticalHit = true;
            }

            _damageable.TakeDamage(_rawDamage);
        }
    }
}