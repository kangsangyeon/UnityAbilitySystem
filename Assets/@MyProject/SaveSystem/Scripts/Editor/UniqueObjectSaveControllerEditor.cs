using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SaveSystem.Editor
{
    [CustomEditor(typeof(UniqueObjectSaveController))]
    public class UniqueObjectSaveControllerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement _root = new VisualElement();
            _root.Add(CreateUniqueKeyGUI());
            return _root;
        }

        private VisualElement CreateUniqueKeyGUI()
        {
            VisualElement _root = new VisualElement();

            TextField _description = new TextField()
            {
                label = "Unique Object Key",
                bindingPath = "m_Key",
                multiline = true
            };
            _description.Bind(serializedObject);

            Button _button = new Button() { text = "Generate New Key" };
            _button.clicked += () =>
            {
                serializedObject.FindProperty("m_Key").stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            };

            _root.Add(_description);
            _root.Add(_button);
            return _root;
        }
    }
}