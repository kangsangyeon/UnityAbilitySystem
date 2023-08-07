using FishNet;
using StatSystem;
using StatSystem.FishNet;
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
        private FN_PlayerStatController m_StatController;

        public void Initialize(PlayerStatController _statController, Stat _stat)
        {
            m_StatController = _statController.GetComponent<FN_PlayerStatController>();

            BindStat(_stat);

            if (_stat is Attribute)
            {
                // attribute는 서버에서만 조작 가능합니다.
                ShowButtons(InstanceFinder.IsServer);
            }
            else if (_stat is PrimaryStat)
            {
                // primary stat은 소유자 클라이언트만 조작 가능합니다.
                ShowButtons(m_StatController.Owner.IsLocalClient);
            }
        }

        public void Uninitialize()
        {
            UnbindStat();
        }

        public void OnClickAdd(int _value)
        {
            if (InstanceFinder.IsServer)
            {
                // attribute는 서버에서만 조작 가능합니다. 서버에서 변경된 값을 클라이언트에게 전파합니다.
                if (m_Stat is Attribute _attribute)
                {
                    _attribute.ApplyModifier(new StatModifier()
                    {
                        source = this,
                        magnitude = _value,
                        type = ModifierOperationType.Additive
                    });
                }
            }

            if (m_StatController.IsOwner)
            {
                // primary stat에 stat point를 투자하는 것은 이 player의 소유자만이 할 수 있습니다.
                if (m_Stat is PrimaryStat _primaryStat)
                {
                    var _success = m_StatController.statController.TryInvest(_primaryStat, _value);
                    if (_success == false)
                    {
                        Debug.LogWarning("stat points 투자에 실패했습니다.");
                    }
                }
            }
        }

        private void BindStat(Stat _stat)
        {
            if (m_Stat == _stat)
                return;

            if (m_Stat != null)
            {
                UnbindStat();
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

        private void UnbindStat()
        {
            m_Stat.valueChanged.RemoveListener(OnStatValueChanged);
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

        private void ShowButtons(bool _show)
        {
            foreach (var _button in m_UI_Buttons)
                _button.gameObject.SetActive(_show);
        }
    }
}