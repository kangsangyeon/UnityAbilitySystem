﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace StatSystem
{
    public class Attribute : Stat
    {
        protected int m_CurrentValue;

        public int currentValue => m_CurrentValue;
        public UnityEvent currentValueChanged = new UnityEvent();
        public UnityEvent<StatModifier> appliedModifier = new UnityEvent<StatModifier>();

        public Attribute(StatDefinition _definition) : base(_definition)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_CurrentValue = m_Value;
        }

        public virtual void ApplyModifier(StatModifier _modifier)
        {
            int _newValue = m_CurrentValue;

            switch (_modifier.type)
            {
                case ModifierOperationType.Additive:
                    _newValue += _modifier.magnitude;
                    break;
                case ModifierOperationType.Multiplicative:
                    _newValue *= _modifier.magnitude;
                    break;
                case ModifierOperationType.Override:
                    _newValue = _modifier.magnitude;
                    break;
            }

            // Attribute의 값의 범위는 [0-m_Value] 입니다.
            // m_Value는 Attribute ScriptableObject 애셋에서 정의한 base value 입니다.
            _newValue = Mathf.Clamp(_newValue, 0, m_Value);

            if (_newValue != m_CurrentValue)
            {
                m_CurrentValue = _newValue;
                currentValueChanged.Invoke();
            }

            appliedModifier.Invoke(_modifier);
        }
    }
}