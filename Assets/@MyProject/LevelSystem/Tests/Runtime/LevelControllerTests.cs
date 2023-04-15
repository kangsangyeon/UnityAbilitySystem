using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace LevelSystem.Tests
{
    public class LevelControllerTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/LevelSystem/Tests/Scene/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator Stat_WhenModifierApplied_DoesNotExceedCap()
        {
            yield return null;
            LevelController _levelController = GameObject.FindObjectOfType<LevelController>();
            Assert.AreEqual(1, _levelController.level);
            Assert.AreEqual(83, _levelController.requiredExperience);
            _levelController.currentExperience += 83;
            Assert.AreEqual(2, _levelController.level);
            Assert.AreEqual(0, _levelController.currentExperience);
            Assert.AreEqual(92, _levelController.requiredExperience);
        }
    }
}