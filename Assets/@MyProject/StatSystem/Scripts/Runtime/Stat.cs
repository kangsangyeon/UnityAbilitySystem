using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StatSystem
{
    public class Stat
    {
        protected StatDefinition m_Definition;
        protected int m_Value;
        protected List<StatModifier> m_Modifiers = new List<StatModifier>();

        public int value => m_Value;
        public virtual int baseValue => m_Definition.baseValue;
        public UnityEvent valueChanged = new UnityEvent();

        public Stat(StatDefinition _definition)
        {
            m_Definition = _definition;
        }

        public void AddModifier(StatModifier _modifier)
        {
            m_Modifiers.Add(_modifier);
            CalculateValue();
        }

        public void RemoveModifierFromSource(Object _source)
        {
            m_Modifiers.RemoveAll(m => m.source.GetInstanceID() == _source.GetInstanceID());
            CalculateValue();
        }

        protected void CalculateValue()
        {
            int _finalValue = baseValue;

            m_Modifiers.Sort((x, y) => x.type.CompareTo(y.type));

            foreach (var _modifier in m_Modifiers)
            {
                if (_modifier.type == ModifierOperationType.Additive)
                {
                    _finalValue += _modifier.magnitude;
                }
                else if (_modifier.type == ModifierOperationType.Multiplicative)
                {
                    _finalValue *= _modifier.magnitude;
                }

                if (m_Definition.cap >= 0)
                {
                    _finalValue = Mathf.Min(_finalValue, m_Definition.cap);
                }
            }

            if (m_Value != _finalValue)
            {
                m_Value = _finalValue;
                valueChanged.Invoke();
            }
        }
    }
}