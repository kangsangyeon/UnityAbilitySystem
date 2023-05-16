using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AbilitySystem.Editor
{
    [CustomEditor(typeof(GameplayPersistentEffectDefinition))]
    public class GameplayPersistentEffectEditor : GameplayEffectEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement _root = new VisualElement();

            _root.Add(CreateApplicationFieldsGUI());
            _root.Add(CreateDurationFieldsGUI());

            RegisterCallbacks(_root);

            return _root;
        }

        private VisualElement CreateDurationFieldsGUI()
        {
            VisualElement _root = new VisualElement();

            _root.Add(
                new PropertyField(serializedObject.FindProperty("m_IsInfinite")) { name = "is-infinite" });
            _root.Add(
                new PropertyField(serializedObject.FindProperty("m_DurationFormula")) { name = "duration-formula" });

            return _root;
        }

        private void RegisterCallbacks(VisualElement _root)
        {
            GameplayPersistentEffectDefinition _definition = target as GameplayPersistentEffectDefinition;

            PropertyField _durationField = _root.Q<PropertyField>("duration-formula");
            PropertyField _isInfiniteField = _root.Q<PropertyField>("is-infinite");

            _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex;
            _isInfiniteField.RegisterValueChangeCallback(evt =>
            {
                _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex;
            });
        }
    }
}