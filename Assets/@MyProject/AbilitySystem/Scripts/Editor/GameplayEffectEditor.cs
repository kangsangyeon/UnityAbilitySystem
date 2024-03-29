﻿using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.Editor
{
    [CustomEditor(typeof(GameplayEffectDefinition))]
    public class GameplayEffectEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement _root = new VisualElement();

            StyleSheet _styleSheet =
                Resources.Load<StyleSheet>("Editor/GameplayEffectEditorUss");
            _root.styleSheets.Add(_styleSheet);

            _root.Add(CreateCoreFieldsGUI());
            _root.Add(CreateApplicationFieldsGUI());
            _root.Add(CreateGameplayEffectFieldsGUI());
            _root.Add(CreateTagFieldsGUI());
            _root.Add(CreateSpecialEffectFieldsGUI());

            return _root;
        }

        protected virtual VisualElement CreateCoreFieldsGUI()
        {
            VisualElement _root = new VisualElement();

            TextField _description = new TextField()
            {
                label = "Description",
                bindingPath = "m_Description",
                multiline = true
            };
            _description.Bind(serializedObject);
            _description.AddToClassList("description");

            _root.Add(_description);
            return _root;
        }

        protected VisualElement CreateApplicationFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            ListView _modifiers = new ListView()
            {
                bindingPath = "m_ModifierDefinitions",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderable = true,
                showFoldoutHeader = true,
                showAddRemoveFooter = true,
                headerTitle = "ModifierDefinitions"
            };
            _modifiers.Bind(serializedObject);
            Button _addButton = _modifiers.Q<Button>("unity-list-view__add-button");
            _addButton.clicked += AddButtonOnClicked;
            _root.Add(_modifiers);
            return _root;
        }

        protected virtual VisualElement CreateGameplayEffectFieldsGUI()
        {
            VisualElement _root = new VisualElement();

            ListView _listView = new ListView()
            {
                bindingPath = "m_AdditionalEffects",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderable = true,
                showFoldoutHeader = true,
                showAddRemoveFooter = true,
                headerTitle = "Additional Effects"
            };
            _listView.Bind(serializedObject);
            _root.Add(_listView);

            return _root;
        }

        protected virtual VisualElement CreateSpecialEffectFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_SpecialEffectDefinition")));
            return _root;
        }

        protected virtual VisualElement CreateTagFieldsGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(new PropertyField(serializedObject.FindProperty("m_Tags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_RemoveEffectsWithTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_ApplicationMustBePresentTags")));
            _root.Add(new PropertyField(serializedObject.FindProperty("m_ApplicationMustBeAbsentTags")));
            return _root;
        }

        private void AddButtonOnClicked()
        {
            System.Type[] _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(AbstractGameplayEffectStatModifierDefinition).IsAssignableFrom(t) && t.IsClass &&
                            !t.IsAbstract)
                .ToArray();
            if (_types.Length > 1)
            {
                GenericMenu _menu = new GenericMenu();

                foreach (Type t in _types)
                    _menu.AddItem(new GUIContent(t.Name), false, () => CreateItem(t));

                _menu.ShowAsContext();
            }
            else
            {
                CreateItem(_types[0]);
            }
        }

        private void CreateItem(Type _type)
        {
            AbstractGameplayEffectStatModifierDefinition _item =
                ScriptableObject.CreateInstance(_type) as AbstractGameplayEffectStatModifierDefinition;
            _item.name = _type.Name;
            AssetDatabase.AddObjectToAsset(_item, target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SerializedProperty _modifiers = serializedObject.FindProperty("m_ModifierDefinitions");
            _modifiers.GetArrayElementAtIndex(_modifiers.arraySize - 1).objectReferenceValue = _item;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}