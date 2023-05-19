﻿using System;
using CombatSystem.Core;
using Core;
using UnityEngine;
using UnityEngine.Pool;

namespace CombatSystem
{
    [RequireComponent(typeof(Collider))]
    public class CombatController : MonoBehaviour
    {
        [SerializeField] private FloatingText m_FloatingTextPrefab;
        private ObjectPool<FloatingText> m_Pool;
        private Collider m_Collider;
        private IDamageable m_Damageable;

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Damageable = GetComponent<IDamageable>();
            m_Pool = new ObjectPool<FloatingText>(OnCreate, OnGet, OnRelease);
        }

        private void OnEnable()
        {
            if (m_Collider.enabled == false)
                m_Collider.enabled = true;

            m_Damageable.initialized.AddListener(OnDamageableInitialized);
            m_Damageable.willUninitialize.AddListener(OnDamageableWillUninitialized);
            if (m_Damageable.isInitialized)
                OnDamageableInitialized();
        }

        private void OnDamageableInitialized()
        {
            m_Damageable.damaged.AddListener(DisplayDamage);
            m_Damageable.healed.AddListener(DisplayRestorationAmount);
            m_Damageable.defeated.AddListener(OnDefeated);
        }

        private void OnDamageableWillUninitialized()
        {
            m_Damageable.damaged.RemoveListener(DisplayDamage);
            m_Damageable.healed.RemoveListener(DisplayRestorationAmount);
            m_Damageable.defeated.RemoveListener(OnDefeated);
        }

        private void DisplayDamage(int _magnitude, bool _isCriticalHit)
        {
            FloatingText _damageText = m_Pool.Get();
            _damageText.Set(_magnitude.ToString(), _isCriticalHit ? Color.red : Color.white);
            _damageText.Animate();
            if (_isCriticalHit)
                _damageText.transform.localScale *= 2;
        }

        private void DisplayRestorationAmount(int _amount)
        {
            FloatingText _text = m_Pool.Get();
            _text.Set(_amount.ToString(), Color.green);
            _text.Animate();
        }

        private void OnDefeated()
        {
        }

        private FloatingText OnCreate()
        {
            FloatingText _floatingText = Instantiate(m_FloatingTextPrefab);
            _floatingText.finished.AddListener(m_Pool.Release);

            _floatingText.transform.position = transform.position + Utils.GetCenterOfCollider(m_Collider);
            _floatingText.transform.localScale = Vector3.one * 0.01f;
            return _floatingText;
        }

        private void OnGet(FloatingText _floatingText)
        {
            _floatingText.transform.position = transform.position + Utils.GetCenterOfCollider(m_Collider);
            _floatingText.transform.localScale = Vector3.one * 0.01f;
            _floatingText.gameObject.SetActive(true);
        }

        private void OnRelease(FloatingText _floatingText)
        {
            _floatingText.gameObject.SetActive(false);
        }
    }
}