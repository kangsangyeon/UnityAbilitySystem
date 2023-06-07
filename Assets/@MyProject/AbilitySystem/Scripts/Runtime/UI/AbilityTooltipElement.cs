using Core.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.UI
{
    public class AbilityTooltipElement : TooltipElement
    {
        public new class UxmlFactory : UxmlFactory<AbilityTooltipElement, UxmlTraits>
        {
        }

        public AbilityTooltipElement()
        {
            var _visualTree = Resources.Load<VisualTreeAsset>("UI/AbilityTooltip");
            _visualTree.CloneTree(this);
            var _styleSheet = Resources.Load<StyleSheet>("UI/AbilityTooltip");
            styleSheets.Add(_styleSheet);
        }
    }
}