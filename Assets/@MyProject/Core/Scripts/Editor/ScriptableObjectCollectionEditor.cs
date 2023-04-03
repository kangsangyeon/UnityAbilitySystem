using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Editor
{
    public class ScriptableObjectCollectionEditor<T> : VisualElement
        where T : ScriptableObject
    {
        protected ScriptableObject m_Target;
        protected List<T> m_Items;
        private ListView m_ListView;
        private Button m_CreateButton;
        private List<T> m_FilteredListView;
        private InspectorElement m_Inspector;
        private ToolbarSearchField m_ToolbarSearchField;
        private PropertyField m_NameField;

        public ScriptableObjectCollectionEditor()
        {
            var _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/@MyProject/Core/Scripts/Editor/ScriptableObjectCollectionEditor.uxml");
            _visualTree.CloneTree(this);

            m_Inspector = this.Q<Inspector>();
            m_ListView = this.Q<ListView>();
            m_ToolbarSearchField = this.Q<ToolbarSearchField>();
            m_CreateButton = this.Q<Button>("create-button");
            m_NameField = this.Q<PropertyField>("name-field");
        }

        public void Initialize(ScriptableObject _target, List<T> _items)
        {
            m_Target = _target;
            m_Items = _items;
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            Func<VisualElement> _makeItem = () => new Label();
            m_ListView.makeItem = () => new Label();
            m_ListView.onSelectionChange += objects =>
            {
                T _item = objects.First() as T;
                SelectItem(_item);
            };
            Action<VisualElement, int> _bindItem = (elem, i) =>
            {
                Label _label = elem as Label;
                _label.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    evt.menu.AppendAction("Duplicate", action => DuplicateItem(m_FilteredListView[i]));
                    evt.menu.AppendAction("Remove", action => RemoveItem(m_FilteredListView[i]));
                }));
                SerializedObject _serializedObject = new SerializedObject(m_FilteredListView[i]);
                SerializedProperty _serializedProperty = _serializedObject.FindProperty("m_Name");
                _label.BindProperty(_serializedProperty);
            };
            m_ListView.bindItem = _bindItem;
            m_ListView.itemsSource = m_FilteredListView = m_Items;

            m_CreateButton.clicked += Create;

            m_ToolbarSearchField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                m_ListView.itemsSource = m_FilteredListView =
                    m_Items.Where(item => item.name.StartsWith(evt.newValue, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                m_ListView.Rebuild();
            });
        }

        private void Create()
        {
            Type[] _types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToArray();

            if (_types.Length > 1)
            {
                GenericMenu _menu = new GenericMenu();
                foreach (var _type in _types)
                {
                    _menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(_type.Name)), false,
                        () => { CreateItem(_type); });
                }
            }
            else
            {
                CreateItem(_types[0]);
            }
        }
        
        private void SelectItem(T _item)
        {
            SerializedObject _serializedObject = new SerializedObject(_item);
            m_Inspector.Bind(_serializedObject);
            m_NameField.Bind(_serializedObject);
        }

        private void CreateItem(Type _type)
        {
            T _item = ScriptableObject.CreateInstance(_type) as T;
            _item.name = "New Item";
            _item.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(_item, m_Target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            m_Items.Add(_item);
            m_ListView.Rebuild();
            SelectItem(_item);
            m_ListView.SetSelection(m_Items.Count - 1);
            EditorUtility.SetDirty(m_Target);
        }

        private void DuplicateItem(T _item)
        {
            T _duplicated = ScriptableObject.Instantiate(_item);
            _duplicated.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(_duplicated, m_Target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            m_Items.Add(_item);
            m_ListView.Rebuild();
            SelectItem(_item);
            m_ListView.SetSelection(m_Items.Count - 1);
            EditorUtility.SetDirty(m_Target);
        }
        
        private void RemoveItem(T _item)
        {
            if (EditorUtility.DisplayDialog(
                    "Delete Item",
                    $"Are you sure you want to delete {_item.name}?", "Yes",
                    "Cancel"))
            {
                ScriptableObject.DestroyImmediate(_item, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                m_Items.Add(_item);
                m_ListView.Rebuild();
                EditorUtility.SetDirty(m_Target);
            }
        }
    }
}