using System.Collections;
using NUnit.Framework;
using StatSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AbilitySystem.Tests
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

        [UnityTest]
        public IEnumerator AbilityController_WhenActivateAbility_ApplyCostEffect()
        {
            yield return null;

            AbilityController _abilityController = m_Player.GetComponent<AbilityController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            Attribute _mana = _statController.stats["Mana"] as Attribute;
            Assert.AreEqual(100, _mana.currentValue);
            _abilityController.TryActivateAbility("AbilityWithCost", m_Enemy);
            Assert.AreEqual(50, _mana.currentValue);
        }

        [UnityTest]
        public IEnumerator AbilityController_WhenCannotSatisfyAbilityCost_BlockAbilityActivation()
        {
            yield return null;

            AbilityController _abilityController = m_Player.GetComponent<AbilityController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            Attribute _mana = _statController.stats["Mana"] as Attribute;
            Assert.AreEqual(100, _mana.currentValue);
            _abilityController.TryActivateAbility("AbilityWithCost", m_Enemy);
            _abilityController.TryActivateAbility("AbilityWithCost", m_Enemy);
            Assert.AreEqual(0, _mana.currentValue);
            Assert.AreEqual(false, _abilityController.TryActivateAbility("AbilityWithCost", m_Enemy));
        }
    }
}