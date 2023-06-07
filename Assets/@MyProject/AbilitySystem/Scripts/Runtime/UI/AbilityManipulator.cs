using UnityEngine.UIElements;

namespace AbilitySystem.UI
{
    public class AbilityManipulator : Manipulator
    {
        private Ability m_Ability;
        private AbilityTooltipElement m_TooltipElement;

        public AbilityManipulator(Ability _ability, AbilityTooltipElement _tooltipElement)
        {
            m_Ability = _ability;
            m_TooltipElement = _tooltipElement;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            m_TooltipElement.style.left = evt.mousePosition.x;
            m_TooltipElement.style.top = evt.mousePosition.y;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            ShowTooltip();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            HideTooltip();
        }

        private void ShowTooltip()
        {
            Label _title = m_TooltipElement.Q<Label>("ability-tooltip__title");
            _title.text = m_Ability.definition.name;

            Label _description = m_TooltipElement.Q<Label>("ability-tooltip__description");
            _description.text = m_Ability.definition.description;

            VisualElement _icon = m_TooltipElement.Q("ability-tooltip__icon");
            _icon.style.backgroundImage = new StyleBackground(m_Ability.definition.icon);

            Label _data = m_TooltipElement.Q<Label>("ability-tooltip__data");
            _data.text = m_Ability.ToString();

            m_TooltipElement.Show();
        }

        private void HideTooltip()
        {
            m_TooltipElement.Hide();
        }
    }
}