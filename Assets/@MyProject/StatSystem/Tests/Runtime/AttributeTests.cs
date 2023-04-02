using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace StatSystem.Tests
{
    public class AttributeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
                new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }
        
        [UnityTest]
        public IEnumerator Stat_WhenModifierApplied_DoesNotExceedCap()
        {
            yield return null;

            StatController _statController = GameObject.FindObjectOfType<StatController>();
            Attribute _health = _statController.stats["Health"] as Attribute;
            Assert.AreEqual(100, _health.currentValue);
            Assert.AreEqual(100, _health.value);
            _health.ApplyModifier(new StatModifier()
            {
                magnitude = 20,
                type = ModifierOperationType.Additive
            });
            Assert.AreEqual(100, _health.currentValue);
            Assert.AreEqual(100, _health.value);
        }
        
        [UnityTest]
        public IEnumerator Stat_WhenModifierApplied_DoesNotGoBelowZero()
        {
            yield return null;

            StatController _statController = GameObject.FindObjectOfType<StatController>();
            Attribute _health = _statController.stats["Health"] as Attribute;
            Assert.AreEqual(100, _health.currentValue);
            Assert.AreEqual(100, _health.value);
            _health.ApplyModifier(new StatModifier()
            {
                magnitude = -150,
                type = ModifierOperationType.Additive
            });
            Assert.AreEqual(0, _health.currentValue);
            Assert.AreEqual(100, _health.value);
        }
    }
}