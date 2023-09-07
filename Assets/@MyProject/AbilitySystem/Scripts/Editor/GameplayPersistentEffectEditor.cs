using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.Editor
{
    [CustomEditor(typeof(GameplayPersistentEffectDefinition))]
    public class GameplayPersistentEffectEditor : GameplayEffectEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement _root = new VisualElement();

            StyleSheet _styleSheet =
                Resources.Load<StyleSheet>("Editor/GameplayEffectEditorUss");
            _root.styleSheets.Add(_styleSheet);

            _root.Add(CreateCoreFieldsGUI());
            _root.Add(CreateApplicationFieldsGUI());
            _root.Add(CreateDurationFieldsGUI());
            _root.Add(CreatePeriodFieldsGUI());
            _root.Add(CreateTagFieldsGUI());
            _root.Add(CreateSpecialEffectFieldsGUI());

            RegisterCallbacks(_root);

            return _root;
        }

        protected override VisualElement CreateSpecialEffectFieldsGUI()
        {
            VisualElement _root = base.CreateSpecialEffectFieldsGUI();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_SpecialPersistentEffectDefinition")));
            return _root;
        }

        protected VisualElement CreateDurationFieldsGUI()
        {
            VisualElement _root = new VisualElement();

            _root.Add(
                new PropertyField(serializedObject.FindProperty("m_IsInfinite")) { name = "is-infinite" });
            _root.Add(
                new PropertyField(serializedObject.FindProperty("m_DurationFormula")) { name = "duration-formula" });

            return _root;
        }

        protected VisualElement CreatePeriodFieldsGUI()
        {
            VisualElement _periodFields = new VisualElement() { name = "period" };
            _periodFields.Add(new PropertyField(serializedObject.FindProperty("m_Period")));
            _periodFields.Add(new PropertyField(serializedObject.FindProperty("m_ExecutePeriodicEffectOnApplication")));
            _periodFields.Add(new PropertyField(serializedObject.FindProperty("m_PeriodicInhibitionPolicy")));

            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_IsPeriodic")) { name = "is-periodic" });
            _root.Add(_periodFields);
            return _root;
        }

        protected override VisualElement CreateTagFieldsGUI()
        {
            VisualElement _root = base.CreateTagFieldsGUI();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_GrantedTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_GrantedApplicationImmunityTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_UninhibitedMustBePresentTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_UninhibitedMustBeAbsentTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_PersistMustBePresentTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_PersisMustBeAbsentTags")));
            return _root;
        }

        protected void RegisterCallbacks(VisualElement _root)
        {
            GameplayPersistentEffectDefinition _definition = target as GameplayPersistentEffectDefinition;

            PropertyField _durationField = _root.Q<PropertyField>("duration-formula");
            PropertyField _isInfiniteField = _root.Q<PropertyField>("is-infinite");

            _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex;
            _isInfiniteField.RegisterValueChangeCallback(evt =>
            {
                _durationField.style.display = _definition.isInfinite ? DisplayStyle.None : DisplayStyle.Flex;
            });

            VisualElement _periodFields = _root.Q("period");
            PropertyField _isPeriodicField = _root.Q<PropertyField>("is-periodic");
            _periodFields.style.display = _definition.isPeriodic ? DisplayStyle.Flex : DisplayStyle.None;
            _isPeriodicField.RegisterValueChangeCallback(evt =>
            {
                _periodFields.style.display = _definition.isPeriodic ? DisplayStyle.Flex : DisplayStyle.None;
            });
        }
    }
}