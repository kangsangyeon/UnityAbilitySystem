using System.Collections;
using Core;
using NUnit.Framework;
using StatSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AbilitySystem.Tests
{
    public class GameplayEffectControllerTests
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
        public IEnumerator GameplayEffectController_WhenEffectApplied_ModifyAttribute()
        {
            yield return null;
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            GameplayEffectDefinition _damageEffectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/Test_GameplayEffect_HealthModifier.asset");
            GameplayEffect _damageEffect = new GameplayEffect(_damageEffectDefinition, null, m_Enemy);
            Health _health = _statController.stats["Health"] as Health;
            Assert.AreEqual(100, _health.currentValue);
            _effectController.ApplyGameplayEffectToSelf(_damageEffect);
            Assert.AreEqual(90, _health.currentValue);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenPersistentEffectApplied_AddStatModifier()
        {
            yield return null;
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            GameplayPersistentEffectDefinition _effectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenPersistentEffectApplied_AddStatModifier/GameplayPersistentEffect.asset");
            GameplayPersistentEffect _effect = new GameplayPersistentEffect(_effectDefinition, null, m_Enemy);
            Stat _intelligence = _statController.stats["Intelligence"];
            Assert.AreEqual(1, _intelligence.value);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(4, _intelligence.value);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenPersistentEffectExpires_RemoveStatModifier()
        {
            yield return null;
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            GameplayPersistentEffectDefinition _effectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenPersistentEffectExpires_RemoveStatModifier/GameplayPersistentEffect.asset");
            GameplayPersistentEffect _effect = new GameplayPersistentEffect(_effectDefinition, null, m_Enemy);
            Stat _intelligence = _statController.stats["Intelligence"];
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(4, _intelligence.value);
            yield return new WaitForSeconds(3f);
            Assert.AreEqual(1, _intelligence.value);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenStart_AppliesStartingEffects()
        {
            yield return null;

            StatController _statController = m_Player.GetComponent<StatController>();
            Stat _dexterity = _statController.stats["Dexterity"];
            Assert.AreEqual(4, _dexterity.value);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenEffectApplied_AddGrantedTags()
        {
            yield return null;

            TagController _tagController = m_Player.GetComponent<TagController>();
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayPersistentEffectDefinition _persistentEffectDefinition = AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenEffectApplied_GrantTags/GameplayPersistentEffect.asset");
            GameplayPersistentEffect _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(true, _tagController.Contains("test"));
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenPersistentEffectExpires_RemoveGrantedTags()
        {
            yield return null;

            TagController _tagController = m_Player.GetComponent<TagController>();
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayPersistentEffectDefinition _persistentEffectDefinition = AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenEffectApplied_GrantTags/GameplayPersistentEffect.asset");
            GameplayPersistentEffect _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(true, _tagController.Contains("test"));
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(false, _tagController.Contains("test"));
        }
    }
}