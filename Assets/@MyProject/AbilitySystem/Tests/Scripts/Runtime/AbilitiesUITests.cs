using System.Collections;
using AbilitySystem.UI;
using LevelSystem;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace AbilitySystem.Tests
{
    public class AbilitiesUITests
    {
        private PlayerAbilityController m_PlayerPrefab;
        private PlayerAbilityController m_PlayerAbilityController;
        private AbilitiesUI m_AbilitiesUIPrefab;
        private AbilitiesUI m_AbilitiesUI;
        private LevelController m_LevelController;
        private UIDocument m_UIDocument;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_PlayerPrefab = AssetDatabase.LoadAssetAtPath<PlayerAbilityController>(
                "Assets/@MyProject/AbilitySystem/Tests/Prefabs/Player.prefab");
            m_AbilitiesUIPrefab = AssetDatabase.LoadAssetAtPath<AbilitiesUI>(
                "Assets/@MyProject/AbilitySystem/Tests/Prefabs/AbilitiesUI.prefab");
        }

        [SetUp]
        public void BeforeEachTestSetup()
        {
            m_PlayerAbilityController = GameObject.Instantiate(m_PlayerPrefab);
            m_AbilitiesUI = GameObject.Instantiate(m_AbilitiesUIPrefab);
            SerializedObject _so = new SerializedObject(m_AbilitiesUI);
            _so.FindProperty("m_Controller").objectReferenceValue = m_PlayerAbilityController;
            _so.ApplyModifiedProperties();
            m_LevelController = m_PlayerAbilityController.GetComponent<LevelController>();
            m_LevelController.currentExperience += m_LevelController.requiredExperience;
            m_UIDocument = m_AbilitiesUI.GetComponent<UIDocument>();
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenIncrementButtonClicked_IncrementsAbilityLevel()
        {
            yield return null;

            Assert.AreEqual(0, m_PlayerAbilityController.abilities["Test"].level);

            VisualElement _testElement = m_UIDocument.rootVisualElement.Q("Test");
            Button _incrementButton = _testElement.Q<Button>("ability__increment-button");
            using (var _e = new NavigationSubmitEvent() { target = _incrementButton })
            {
                _incrementButton.SendEvent(_e);
            }

            Assert.AreEqual(1, m_PlayerAbilityController.abilities["Test"].level);
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenIncrementButtonClicked_DecrementsAbilityPoints()
        {
            yield return null;

            Assert.AreEqual(3, m_PlayerAbilityController.abilityPoints);

            VisualElement _testElement = m_UIDocument.rootVisualElement.Q("Test");
            Button _incrementButton = _testElement.Q<Button>("ability__increment-button");
            using (var _e = new NavigationSubmitEvent() { target = _incrementButton })
            {
                _incrementButton.SendEvent(_e);
            }

            Assert.AreEqual(2, m_PlayerAbilityController.abilityPoints);
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenNoAbilityPoints_DisablesIncrementButtons()
        {
            yield return null;

            VisualElement _testElement = m_UIDocument.rootVisualElement.Q("Test");
            Button _incrementButton = _testElement.Q<Button>("ability__increment-button");
            for (int i = 0; i < 3; ++i)
            {
                using (var _e = new NavigationSubmitEvent() { target = _incrementButton })
                {
                    _incrementButton.SendEvent(_e);
                }
            }

            Assert.AreEqual(0, m_PlayerAbilityController.abilityPoints);
            Assert.AreEqual(false, _incrementButton.enabledSelf);
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenAbilityLevelChanged_UpdatesLevelText()
        {
            yield return null;

            VisualElement _testElement = m_UIDocument.rootVisualElement.Q("Test");
            Button _incrementButton = _testElement.Q<Button>("ability__increment-button");
            Label _level = _testElement.Q<Label>("ability__level");
            Assert.AreEqual("0", _level.text);

            using (var _e = new NavigationSubmitEvent() { target = _incrementButton })
            {
                _incrementButton.SendEvent(_e);
            }

            Assert.AreEqual("1", _level.text);
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenMaxLevelReached_DisablesIncrementButton()
        {
            yield return null;

            m_LevelController.currentExperience += 5000;
            Ability _ability = m_PlayerAbilityController.abilities["Test"];

            VisualElement _testElement = m_UIDocument.rootVisualElement.Q("Test");
            Button _incrementButton = _testElement.Q<Button>("ability__increment-button");
            Assert.AreEqual(0, _ability.level);

            for (int i = 0; i < _ability.definition.maxLevel; ++i)
            {
                using (var _e = new NavigationSubmitEvent() { target = _incrementButton })
                {
                    _incrementButton.SendEvent(_e);
                }
            }

            Assert.AreEqual(_ability.definition.maxLevel, _ability.level);
            Assert.AreEqual(false, _incrementButton.enabledSelf);
        }

        [UnityTest]
        public IEnumerator AbilitiesUI_WhenLevelUp_UpdatesAbilityPointsText()
        {
            yield return null;

            Label _abilityPointsValue = m_UIDocument.rootVisualElement.Q<Label>("abilities__ability-points-value");

            Assert.AreEqual("3", _abilityPointsValue.text);
            m_LevelController.currentExperience += m_LevelController.requiredExperience;
            Assert.AreEqual("6", _abilityPointsValue.text);
        }
    }
}