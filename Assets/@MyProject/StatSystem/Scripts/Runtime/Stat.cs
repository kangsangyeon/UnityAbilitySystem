using System.Collections.Generic;
using UnityEngine;

namespace StatSystem
{
    public partial class Stat
    {
        protected StatDefinition m_Definition;
        protected StatController m_Controller;
        protected int m_Value;
        protected List<StatModifier> m_Modifiers = new List<StatModifier>();

        public StatDefinition definition => m_Definition;
        public int value => m_Value;
        public virtual int baseValue => m_Definition.baseValue;
        public event System.Action valueChanged;

        public Stat(StatDefinition _definition, StatController statController)
        {
            m_Definition = _definition;
            m_Controller = statController;
        }

        /// <summary>
        /// 생성자에서 호출했을 때, definition의 formula가 초기화 되어 있지 않으므로 오류가 발생했기 때문에,
        /// 생성자 호출 시점과 값 초기화 시점 중간에 formula를 초기화하기 위해 분리한 함수입니다.
        /// StatController가 graph를 읽어 한 번에 모든 node들의 stat 인스턴스가 초기화시킵니다.
        /// 그 전까지는 formula내 어떠한 node의 value도 정상적으로 가져올 수 없기 때문에, 이보다 CalculateValue 호출을 늦춰야 합니다.
        /// </summary>
        public virtual void Initialize()
        {
            CalculateValue();
        }

        public void AddModifier(StatModifier _modifier)
        {
            m_Modifiers.Add(_modifier);
            CalculateValue();
        }

        public void RemoveModifierFromSource(object _source)
        {
            int _count = m_Modifiers.RemoveAll(m => m.source == _source);
            if (_count > 0)
                CalculateValue();
        }

        internal void CalculateValue()
        {
            int _finalValue = baseValue;

            if (m_Definition.formula != null && m_Definition.formula.rootNode != null)
            {
                _finalValue += Mathf.RoundToInt(m_Definition.formula.rootNode.CalculateValue(m_Controller.gameObject));
            }

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
                valueChanged?.Invoke();
            }
        }
    }
}