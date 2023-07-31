using StatSystem;
using TMPro;
using UnityEngine;

namespace Samples.FishNet
{
    public class UIElem_Stat : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_UI_Txt_StatName;
        [SerializeField] private TextMeshProUGUI m_UI_Txt_CurrentValue;

        private Stat m_Stat;

        public void OnClickAdd(int _value)
        {
            m_Stat.AddModifier(new StatModifier()
            {
                source = this,
                magnitude = _value,
                type = ModifierOperationType.Additive
            });
        }

        public void BindStat(Stat _stat)
        {
            if (m_Stat == _stat)
                return;

            if (m_Stat != null)
            {
                UnbindStat(m_Stat);
            }

            if (_stat != null)
            {
                m_UI_Txt_StatName.text = _stat.definition.name;
                m_UI_Txt_CurrentValue.text =
                    (_stat is Attribute _attribute)
                        ? _attribute.currentValue.ToString()
                        : _stat.value.ToString();
                _stat.valueChanged.AddListener(OnStatValueChanged);
            }

            m_Stat = _stat;
        }

        public void UnbindStat()
        {
            UnbindStat(m_Stat);
        }

        private void UnbindStat(Stat _stat)
        {
            _stat.valueChanged.RemoveListener(OnStatValueChanged);
        }

        private void OnStatValueChanged()
        {
            m_UI_Txt_CurrentValue.text = m_Stat.value.ToString();
        }
    }
}