using System;
using UnityEngine;
using UnityEngine.Events;

namespace StatSystem
{
    public class Attribute : Stat
    {
        protected int m_CurrentValue;

        public int currentValue => m_CurrentValue;
        public UnityEvent currentValueChanged;
        public UnityEvent<StatModifier> appliedModifier;

        public Attribute(StatDefinition _definition) : base(_definition)
        {
            m_CurrentValue = value;
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