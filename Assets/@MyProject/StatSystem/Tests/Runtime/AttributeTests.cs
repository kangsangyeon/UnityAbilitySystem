using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
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
        public IEnumerator Attribute_WhenModifierApplied_DoesNotExceedCap()
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
        public IEnumerator Attribute_WhenModifierApplied_DoesNotGoBelowZero()
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

        [UnityTest]
        public IEnumerator Attribute_WhenTakeDamage_DamageReducedByDefense()
        {
            yield return null;

            StatController _statController = GameObject.FindObjectOfType<StatController>();
            Health _health = _statController.stats["Health"] as Health;
            Assert.AreEqual(100, _health.currentValue);
            _health.ApplyModifier(new HealthModifier()
            {
                magnitude = -10,
                type = ModifierOperationType.Additive,
                isCriticalHit = false,
                source = ScriptableObject.CreateInstance<Ability>()
            });
            Assert.AreEqual(95, _health.currentValue);
        }

        private class Ability : ScriptableObject, ITaggable
        {
            private List<string> m_Tags = new List<string>() { "Physical" };
            public ReadOnlyCollection<string> tags => m_Tags.AsReadOnly();
        }
    }
}