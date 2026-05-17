using System.Collections;
using LevelSystem;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace StatSystem.Tests
{
    public class PlayerStatControllerTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator PlayerStatController_WhenLevelUp_GainStatPoints()
        {
            yield return null;

            PlayerStatController _controller = GameObject.FindObjectOfType<PlayerStatController>();
            ILevelable _levelable = _controller.GetComponent<ILevelable>();
            Assert.AreEqual(5, _controller.statPoints);
            Assert.AreEqual(1, _levelable.level);

            _levelable.currentExperience += _levelable.requiredExperience;
            Assert.AreEqual(10, _controller.statPoints);
            Assert.AreEqual(2, _levelable.level);
        }
    }
}