using System.Collections;
using AbilitySystem;
using NUnit.Framework;
using StatSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace MyProject.AbilitySystem.Tests.Scripts
{
    public class AbilityControllerTests
    {
        private GameObject m_PlayerPrefab;
        private GameObject m_EnemyPrefab;
        private GameObject m_Player;
        private GameObject m_Enemy;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_PlayerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/@MyProject/AbilitySystem/Tests/Prefabs/Player.prefab");

            m_EnemyPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/@MyProject/AbilitySystem/Tests/Prefabs/Enemy.prefab");
        }

        [SetUp]
        public void BeforeEachTestSetup()
        {
            m_Player = GameObject.Instantiate(m_PlayerPrefab);
            m_Enemy = GameObject.Instantiate(m_EnemyPrefab);
        }

        [UnityTest]
        public IEnumerator AbilityController_WhenStart_ApplyPassiveAbilities()
        {
            yield return null;
            StatController _statController = m_Player.GetComponent<StatController>();
            Stat _wisdom = _statController.stats["Wisdom"];
            Assert.AreEqual(4, _wisdom.value);
        }

        [UnityTest]
        public IEnumerator AbilityController_WhenActivateAbility_ApplyEffects()
        {
            yield return null;

            StatController _statController = m_Enemy.GetComponent<StatController>();
            Health _health = _statController.stats["Health"] as Health;
            Assert.AreEqual(100, _health.currentValue);

            AbilityController _abilityController = m_Player.GetComponent<AbilityController>();
            _abilityController.TryActivateAbility("SingleTargetAbility", m_Enemy);

            Assert.AreEqual(95, _health.currentValue);
        }
    }
}