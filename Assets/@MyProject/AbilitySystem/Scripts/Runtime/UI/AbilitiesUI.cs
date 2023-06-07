using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class AbilitiesUI : MonoBehaviour
    {
        [SerializeField] private PlayerAbilityController m_Controller;

        private UIDocument m_UIDocument;
        private VisualElement m_Parent;
        private Button m_CloseButton;
        private Label m_AbilityPoints;
        private AbilityTooltipElement m_TooltipElement;

        private void Awake()
        {
            m_UIDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var _root = m_UIDocument.rootVisualElement;

            m_Parent = _root.Q("abilities__content");

            m_TooltipElement = _root.Q<AbilityTooltipElement>();

            m_CloseButton = _root.Q<Button>("abilities__close-button");
            m_CloseButton.clicked += Hide;

            foreach (var _ability in m_Controller.abilities.Values)
            {
                AbilityElement _abilityElement = new AbilityElement()
                {
                    name = _ability.definition.name
                };
                Label _level = _abilityElement.Q<Label>("ability__level");
                Label _title = _abilityElement.Q<Label>("ability__title");
                Button _incrementButton = _abilityElement.Q<Button>("ability__increment-button");
                VisualElement _icon = _abilityElement.Q("ability__icon");
                _level.text = _ability.level.ToString();
                _title.text = _ability.definition.name;
                _incrementButton.SetEnabled(
                    m_Controller.abilityPoints > 0
                    && _ability.level != _ability.definition.maxLevel);
                _incrementButton.clicked += () =>
                {
                    --m_Controller.abilityPoints;
                    ++_ability.level;
                    _level.text = _ability.level.ToString();
                    _incrementButton.SetEnabled(
                        m_Controller.abilityPoints > 0
                        && _ability.level != _ability.definition.maxLevel);
                };
                _icon.style.backgroundImage = new StyleBackground(_ability.definition.icon);
                
                _abilityElement.AddManipulator(new AbilityManipulator(_ability, m_TooltipElement));
                
                m_Parent.Add(_abilityElement);
            }

            m_AbilityPoints = _root.Q<Label>("abilities__ability-points-value");
            m_AbilityPoints.text = m_Controller.abilityPoints.ToString();

            m_Controller.abilityPointsChanged.AddListener(OnAbilityPointsChanged);
        }

        private void OnAbilityPointsChanged()
        {
            m_AbilityPoints.text = m_Controller.abilityPoints.ToString();

            for (int i = 0; i < m_Parent.childCount; ++i)
            {
                Ability _ability = m_Controller.abilities[m_Parent[i].name];
                Button _incrementButton = m_Parent[i].Q<Button>("ability__increment-button");
                _incrementButton.SetEnabled(
                    m_Controller.abilityPoints > 0
                    && _ability.level != _ability.definition.maxLevel);
            }
        }

        private void Hide()
        {
            m_UIDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            m_UIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }
}