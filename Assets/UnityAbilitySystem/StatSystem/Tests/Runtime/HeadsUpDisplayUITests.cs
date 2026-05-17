using System.Collections;
using LevelSystem;
using NUnit.Framework;
using StatSystem.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace StatSystem.Tests
{
    public class HeadsUpDisplayUITests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator HeadsUpDisplayUI_WhenLevelUp_UpdatesText()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            ILevelable _levelable = _playerStatController.GetComponent<ILevelable>();
            HeadsUpDisplayUI _headsUpDisplayUI = GameObject.FindObjectOfType<HeadsUpDisplayUI>();
            UIDocument _uiDocument = _headsUpDisplayUI.GetComponent<UIDocument>();
            Label _level = _uiDocument.rootVisualElement.Q<Label>("level");
            Assert.AreEqual("1", _level.text);
            _levelable.currentExperience += _levelable.requiredExperience;
            Assert.AreEqual("2", _level.text);
        }

        [UnityTest]
        public IEnumerator HeadsUpDisplayUI_WhenGainExperience_UpdatesExperienceBar()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            ILevelable _levelable = _playerStatController.GetComponent<ILevelable>();
            HeadsUpDisplayUI _headsUpDisplayUI = GameObject.FindObjectOfType<HeadsUpDisplayUI>();
            UIDocument _uiDocument = _headsUpDisplayUI.GetComponent<UIDocument>();
            ProgressBar _experienceBar = _uiDocument.rootVisualElement.Q<ProgressBar>("experience");
            Assert.AreEqual(0, _experienceBar.value);
            _levelable.currentExperience += 5;
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                (float)_levelable.currentExperience / _levelable.requiredExperience * 100,
                _experienceBar.value);
        }

        [UnityTest]
        public IEnumerator HeadsUpDisplayUI_WhenLoseHealth_UpdateHealthBar()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            var _health = _playerStatController.stats["Health"] as Attribute;
            HeadsUpDisplayUI _headsUpDisplayUI = GameObject.FindObjectOfType<HeadsUpDisplayUI>();
            UIDocument _uiDocument = _headsUpDisplayUI.GetComponent<UIDocument>();
            ProgressBar _healthBar = _uiDocument.rootVisualElement.Q<ProgressBar>("health");
            Assert.AreEqual(100, _healthBar.value);
            _health.ApplyModifier(new StatModifier()
            {
                magnitude = -10,
                type = ModifierOperationType.Additive
            });
            Assert.AreEqual(90, _healthBar.value);
        }

        [UnityTest]
        public IEnumerator HeadsUpDisplayUI_WhenLoseMana_UpdateManaBar()
        {
            yield return null;

            PlayerStatController _playerStatController = GameObject.FindObjectOfType<PlayerStatController>();
            var _mana = _playerStatController.stats["Mana"] as Attribute;
            HeadsUpDisplayUI _headsUpDisplayUI = GameObject.FindObjectOfType<HeadsUpDisplayUI>();
            UIDocument _uiDocument = _headsUpDisplayUI.GetComponent<UIDocument>();
            ProgressBar _manaBar = _uiDocument.rootVisualElement.Q<ProgressBar>("mana");
            Assert.AreEqual(100, _manaBar.value);
            _mana.ApplyModifier(new StatModifier()
            {
                magnitude = -10,
                type = ModifierOperationType.Additive
            });
            Assert.AreEqual(90, _manaBar.value);
        }
    }
}