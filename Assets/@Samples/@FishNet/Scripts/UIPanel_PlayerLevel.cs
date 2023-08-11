using System;
using LevelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.FishNet
{
    public class UIPanel_PlayerLevel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_UI_Txt_PlayerName;
        [SerializeField] private TextMeshProUGUI m_UI_Txt_Level;
        [SerializeField] private TextMeshProUGUI m_UI_Txt_Experience;
        [SerializeField] private Slider m_UI_Slider_Level;

        private LevelController m_LevelController;

        private Action m_OnLevelChangedAction;
        private Action m_OnCurrentExperienceChangedAction;

        public void BindPlayerLevelController(LevelController _controller)
        {
            if (m_LevelController == _controller)
                return;

            if (m_LevelController != null)
                UnbindPlayerLevelController();

            m_UI_Txt_PlayerName.text = _controller.gameObject.name;

            m_OnLevelChangedAction = () => m_UI_Txt_Level.text = _controller.level.ToString();
            m_OnLevelChangedAction.Invoke();
            _controller.levelChanged += m_OnLevelChangedAction;

            m_OnCurrentExperienceChangedAction = () =>
            {
                m_UI_Txt_Experience.text = $"{_controller.currentExperience} / {_controller.requiredExperience}";
                m_UI_Slider_Level.value = (float)_controller.currentExperience / _controller.requiredExperience;
            };
            m_OnCurrentExperienceChangedAction.Invoke();
            _controller.currentExperienceChanged += m_OnCurrentExperienceChangedAction;

            m_LevelController = _controller;
        }

        public void UnbindPlayerLevelController()
        {
            m_LevelController.levelChanged -= m_OnLevelChangedAction;
            m_OnLevelChangedAction = null;

            m_LevelController.currentExperienceChanged -= m_OnCurrentExperienceChangedAction;
            m_OnCurrentExperienceChangedAction = null;

            m_LevelController = null;
        }

        public void OnClickAddExperience(int _amount)
        {
            m_LevelController.currentExperience += _amount;
        }
    }
}