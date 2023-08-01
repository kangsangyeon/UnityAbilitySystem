using StatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.FishNet
{
    public class UIElem_Stat : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_UI_Txt_StatName;
        [SerializeField] private TextMeshProUGUI m_UI_Txt_CurrentValue;
        [SerializeField] private Button[] m_UI_Buttons;

        private Stat m_Stat;

        public void OnClickAdd(int _value)
        {
            if (m_Stat is Attribute _attribute)
            {
                _attribute.ApplyModifier(new StatModifier()
                {
                    source = this,
                    magnitude = _value,
                    type = ModifierOperationType.Additive
                });
            }
            else if (m_Stat is PrimaryStat _primaryStat)
            {
                _primaryStat.Add(_value);
            }
            else
            {
                Debug.LogWarning("attribute와 primary stat만 조작할 수 있습니다.");
            }
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

                if (_stat is Attribute _attribute)
                {
                    m_UI_Txt_CurrentValue.text = _attribute.currentValue.ToString();
                    _attribute.currentValueChanged.AddListener(OnStatValueChanged);
                }
                else
                {
                    m_UI_Txt_CurrentValue.text = _stat.value.ToString();
                    _stat.valueChanged.AddListener(OnStatValueChanged);
                }

                bool _enableButtons = _stat is Attribute || _stat is PrimaryStat;
                foreach (var _button in m_UI_Buttons)
                    _button.gameObject.SetActive(_enableButtons);
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
            if (m_Stat is Attribute _attribute)
            {
                m_UI_Txt_CurrentValue.text = _attribute.currentValue.ToString();
            }
            else
            {
                m_UI_Txt_CurrentValue.text = m_Stat.value.ToString();
            }
        }
    }
}