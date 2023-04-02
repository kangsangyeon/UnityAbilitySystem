using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace StatSystem.Tests
{
    public class PrimaryStatTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator Stat_WhenAddCalled_ChangesBaseValue()
        {
            yield return null;

            StatController _statController = GameObject.FindObjectOfType<StatController>();
            PrimaryStat _strength = _statController.stats["Strength"] as PrimaryStat;
            Assert.AreEqual(1, _strength.value);
            _strength.Add(1);
            Assert.AreEqual(2, _strength.value);
        }
    }
}