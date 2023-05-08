﻿using System;
using CombatSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CombatSystem.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        private Slider m_Slider;
        private IDamageable m_Damageable;
        [SerializeField] private GameObject m_Owner;

        private void Awake()
        {
            m_Slider = GetComponent<Slider>();
            m_Damageable = m_Owner.GetComponent<IDamageable>();
        }

        private void OnEnable()
        {
            m_Damageable.initialized.AddListener(OnDamageableInitialized);
            m_Damageable.willUninitialize.AddListener(OnDamageableWillUninitialized);
            if (m_Damageable.isInitialized)
                OnDamageableInitialized();
        }

        private void OnDamageableInitialized()
        {
            m_Slider.maxValue = m_Damageable.maxHealth;
            m_Slider.value = m_Damageable.health;
            RegisterEvents();
        }

        private void OnDamageableWillUninitialized()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            m_Damageable.maxHealthChanged.AddListener(OnMaxHealthChanged);
            m_Damageable.healthChanged.AddListener(OnHealthChanged);
        }

        private void UnregisterEvents()
        {
            m_Damageable.maxHealthChanged.RemoveListener(OnMaxHealthChanged);
            m_Damageable.healthChanged.RemoveListener(OnHealthChanged);
        }

        private void OnMaxHealthChanged()
        {
            m_Slider.maxValue = m_Damageable.maxHealth;
        }

        private void OnHealthChanged()
        {
            m_Slider.value = m_Damageable.health;
        }

        private void OnValidate()
        {
            if (m_Owner != null && m_Owner.GetComponent<IDamageable>() == null)
            {
                Debug.LogWarning("Thw owner must implement the IDamageable interface!");
                m_Owner = null;
            }
        }
    }
}