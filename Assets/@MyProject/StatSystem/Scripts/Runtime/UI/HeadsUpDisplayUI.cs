using System;
using LevelSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace StatSystem.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HeadsUpDisplayUI : MonoBehaviour
    {
        [SerializeField] private PlayerStatController m_Controller;
        private UIDocument m_UIDocument;
        private ILevelable m_Levelable;
        private ProgressBar m_HealthBar;
        private ProgressBar m_ManaBar;
        private ProgressBar m_ExperienceBar;
        private Label m_Level;

        private void Awake()
        {
            m_UIDocument = GetComponent<UIDocument>();
            m_Levelable = m_Controller.GetComponent<ILevelable>();
        }

        private void OnEnable()
        {
            var _root = m_UIDocument.rootVisualElement;
            m_HealthBar = _root.Q<ProgressBar>("health");
            m_ManaBar = _root.Q<ProgressBar>("mana");
            m_ExperienceBar = _root.Q<ProgressBar>("experience");
            m_Level = _root.Q<Label>("level");
        }

        private void Start()
        {
            OnManaChanged();
            OnHealthChanged();
            OnCurrentExperienceChanged();
            OnLevelChanged();

            Attribute _mana = m_Controller.stats["Mana"] as Attribute;
            Attribute _health = m_Controller.stats["Health"] as Attribute;
            _mana.valueChanged.AddListener(OnMaxManaChanged);
            _mana.currentValueChanged.AddListener(OnManaChanged);
            _health.valueChanged.AddListener(OnMaxHealthChanged);
            _health.currentValueChanged.AddListener(OnHealthChanged);
            m_Levelable.currentExperienceChanged += OnCurrentExperienceChanged;
            m_Levelable.levelChanged += OnLevelChanged;
        }

        private void OnDestroy()
        {
            Attribute _mana = m_Controller.stats["Mana"] as Attribute;
            Attribute _health = m_Controller.stats["Health"] as Attribute;
            _mana.valueChanged.RemoveListener(OnMaxManaChanged);
            _mana.currentValueChanged.RemoveListener(OnManaChanged);
            _health.valueChanged.RemoveListener(OnMaxHealthChanged);
            _health.currentValueChanged.RemoveListener(OnHealthChanged);
            m_Levelable.currentExperienceChanged -= OnCurrentExperienceChanged;
            m_Levelable.levelChanged -= OnLevelChanged;
        }

        private void OnMaxManaChanged() => OnManaChangedInternal();
        private void OnManaChanged() => OnManaChangedInternal();

        private void OnManaChangedInternal()
        {
            Attribute _mana = m_Controller.stats["Mana"] as Attribute;
            m_ManaBar.value = (float)_mana.currentValue / _mana.value * 100.0f;
            m_ManaBar.title = $"{_mana.currentValue} / {_mana.value}";
        }

        private void OnMaxHealthChanged() => OnHealthChangedInternal();
        private void OnHealthChanged() => OnHealthChangedInternal();

        private void OnHealthChangedInternal()
        {
            Attribute _health = m_Controller.stats["Health"] as Attribute;
            m_HealthBar.value = (float)_health.currentValue / _health.value * 100.0f;
            m_HealthBar.title = $"{_health.currentValue} / {_health.value}";
        }

        private void OnCurrentExperienceChanged() => OnExperienceChangedInternal();

        private void OnLevelChanged()
        {
            OnExperienceChangedInternal();
            m_Level.text = m_Levelable.level.ToString();
        }

        private void OnExperienceChangedInternal()
        {
            m_HealthBar.value = (float)m_Levelable.currentExperience / m_Levelable.requiredExperience * 100.0f;
            m_HealthBar.title = $"{m_Levelable.currentExperience} / {m_Levelable.requiredExperience}";
        }
    }
}