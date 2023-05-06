using System.Collections;
using LevelSystem;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace StatSystem.Tests
{
    public class StatsUITests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator StatUI_WhenIncrementButtonClicked_IncrementsStatBaseValue()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            Assert.AreEqual(1, _playerStatController.stats["Strength"].value);
            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("strength");
            Button _incrementButton = _strengthElement.Q<Button>("increment-button");
            using (var e = new NavigationSubmitEvent() { target = _incrementButton })
            {
                _incrementButton.SendEvent(e);
            }

            Assert.AreEqual(2, _playerStatController.stats["Strength"].value);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenIncrementButtonClicked_DecrementsStatPoints()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            Assert.AreEqual(5, _playerStatController.statPoints);
            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("strength");
            Button _incrementButton = _strengthElement.Q<Button>("increment-button");
            using (var e = new NavigationSubmitEvent() { target = _incrementButton })
            {
                _incrementButton.SendEvent(e);
            }

            Assert.AreEqual(4, _playerStatController.statPoints);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenNoStatPoints_DisablesIncrementButtons()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            Assert.AreEqual(5, _playerStatController.statPoints);
            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("strength");
            Button _incrementButton = _strengthElement.Q<Button>("increment-button");

            int _statPoints = _playerStatController.statPoints;
            for (int i = 0; i < _statPoints; ++i)
            {
                using (var e = new NavigationSubmitEvent() { target = _incrementButton })
                {
                    _incrementButton.SendEvent(e);
                }
            }

            Assert.AreEqual(0, _playerStatController.statPoints);
            Assert.AreEqual(false, _incrementButton.enabledSelf);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenStatValueChanged_UpdatesText()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("physical-attack");
            Label _value = _strengthElement.Q<Label>("value");
            Assert.AreEqual("3", _value.text);

            _playerStatController.stats["PhysicalAttack"].AddModifier(new StatModifier()
            {
                magnitude = 5,
                type = ModifierOperationType.Additive
            });

            Assert.AreEqual("8", _value.text);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenStatCapReached_DisableIncrementButton()
        {
            yield return null;

            // 데이터베이스에서 Charisma 스탯의 cap이 1이며 base value을 1으로 설정했을 것으로 가정합니다.
            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("charisma");
            Button _button = _strengthElement.Q<Button>("increment-button");
            Assert.AreEqual(false, _button.enabledSelf);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenLevelUp_UpdatesText()
        {
            yield return null;

            LevelController _levelController = GameObject.FindObjectOfType<LevelController>();

            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("level");
            Label _label = _strengthElement.Q<Label>("value");
            Assert.AreEqual("1", _label.text);

            _levelController.currentExperience += _levelController.requiredExperience;

            Assert.AreEqual("2", _label.text);
        }

        [UnityTest]
        public IEnumerator StatUI_WhenGainExperience_UpdatesText()
        {
            yield return null;

            LevelController _levelController = GameObject.FindObjectOfType<LevelController>();

            UIDocument _uiDocument = GameObject.FindObjectOfType<UIDocument>();
            VisualElement _strengthElement = _uiDocument.rootVisualElement.Q("experience");
            Label _label = _strengthElement.Q<Label>("value");

            int _currentExperienceOrigin = _levelController.currentExperience;
            Assert.AreEqual(
                $"{_currentExperienceOrigin} / {_levelController.requiredExperience}", _label.text);

            _levelController.currentExperience += 5;

            Assert.AreEqual(
                $"{_currentExperienceOrigin + 5} / {_levelController.requiredExperience}", _label.text);
        }
    }
}