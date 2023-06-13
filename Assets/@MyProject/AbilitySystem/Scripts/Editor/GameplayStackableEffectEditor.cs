using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AbilitySystem.Editor
{
    [CustomEditor(typeof(GameplayStackableEffectDefinition))]
    public class GameplayStackableEffectEditor : GameplayPersistentEffectEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement _root = new VisualElement();

            StyleSheet _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/@MyProject/AbilitySystem/Scripts/Editor/GameplayEffectEditor.uss");
            _root.styleSheets.Add(_styleSheet);

            _root.Add(CreateCoreFieldsGUI());
            _root.Add(CreateApplicationFieldsGUI());
            _root.Add(CreateDurationFieldsGUI());
            _root.Add(CreatePeriodFieldsGUI());
            _root.Add(CreateStackingFieldsGUI());
            _root.Add(CreateGameplayEffectFieldsGUI());
            _root.Add(CreateTagFieldsGUI());
            _root.Add(CreateSpecialEffectFieldsGUI());

            RegisterCallbacks(_root);

            return _root;
        }

        protected VisualElement CreateStackingFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_DenyOverflowApplication")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_ClearStackOnOverflow")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_StackLimitCount")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_StackDurationRefreshPolicy")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_StackPeriodResetPolicy")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_StackExpirationPolicy")));

            return _root;
        }

        protected override VisualElement CreateGameplayEffectFieldsGUI()
        {
            VisualElement _root = base.CreateGameplayEffectFieldsGUI();

            ListView _overflowGameplayEffects = new ListView
            {
                bindingPath = "m_OverflowEffects",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderable = true,
                showFoldoutHeader = true,
                headerTitle = "Overflow Gameplay Effects"
            };
            _overflowGameplayEffects.Bind(serializedObject);
            _root.Add(_overflowGameplayEffects);

            return _root;
        }
    }
}