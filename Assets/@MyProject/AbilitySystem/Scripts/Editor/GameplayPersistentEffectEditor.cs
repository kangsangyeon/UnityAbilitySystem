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
            _root.Add(CreateSpecialEffectFieldsGUI());
            _root.Add(CreateDurationFieldsGUI());
            _root.Add(CreatePeriodFieldsGUI());
            _root.Add(CreateTagFieldsGUI());

            RegisterCallbacks(_root);

            return _root;
        }

        protected override VisualElement CreateSpecialEffectFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_SpecialPersistentEffectDefinition")));
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

        private VisualElement CreatePeriodFieldsGUI()
        {
            VisualElement _periodFields = new VisualElement() { name = "period" };
            _periodFields.Add(new PropertyField(serializedObject.FindProperty("m_Period")));
            _periodFields.Add(new PropertyField(serializedObject.FindProperty("m_ExecutePeriodicEffectOnApplication")));

            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_IsPeriodic")) { name = "is-periodic" });
            _root.Add(_periodFields);
            return _root;
        }

        private VisualElement CreateTagFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_GrantedTags")));
            return _root;
        }

        private void RegisterCallbacks(VisualElement _root)
        {
            GameplayPersistentEffectDefinition _definition = target as GameplayPersistentEffectDefinition;

            PropertyField _durationField = _root.Q<PropertyField>("duration-formula");
            PropertyField _isInfiniteField = _root.Q<PropertyField>("is-infinite");

            _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex;
            _isInfiniteField.RegisterValueChangeCallback(evt => { _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex; });

            VisualElement _periodFields = _root.Q("period");
            PropertyField _isPeriodicField = _root.Q<PropertyField>("is-periodic");
            _periodFields.style.display = _definition.isPeriodic ? DisplayStyle.Flex : DisplayStyle.None;
            _isPeriodicField.RegisterValueChangeCallback(evt => { _periodFields.style.display = _definition.isPeriodic ? DisplayStyle.Flex : DisplayStyle.None; });
        }
    }
}