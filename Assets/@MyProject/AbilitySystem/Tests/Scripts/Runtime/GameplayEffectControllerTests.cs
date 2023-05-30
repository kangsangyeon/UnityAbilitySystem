using System.Collections;
using System.Linq;
using Core;
using JetBrains.Annotations;
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
            GameplayPersistentEffectDefinition _persistentEffectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenEffectApplied_GrantTags/GameplayPersistentEffect.asset");
            GameplayPersistentEffect
                _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(true, _tagController.Contains("test"));
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenPersistentEffectExpires_RemoveGrantedTags()
        {
            yield return null;

            TagController _tagController = m_Player.GetComponent<TagController>();
            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayPersistentEffectDefinition _persistentEffectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenEffectApplied_GrantTags/GameplayPersistentEffect.asset");
            GameplayPersistentEffect
                _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(true, _tagController.Contains("test"));
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(false, _tagController.Contains("test"));
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenPeriodReached_ExecuteGameplayEffect()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            Health _health = _statController.stats["Health"] as Health;
            GameplayPersistentEffectDefinition _persistentEffectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenPeriodReached_ExecuteGameplayEffect/GameplayPersistentEffect.asset");
            GameplayPersistentEffect
                _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(100, _health.currentValue);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(95, _health.currentValue);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenApplied_ExecutePeriodicGameplayEffect()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            Health _health = _statController.stats["Health"] as Health;
            GameplayPersistentEffectDefinition _persistentEffectDefinition =
                AssetDatabase.LoadAssetAtPath<GameplayPersistentEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenApplied_ExecutePeriodicGameplayEffect/GameplayPersistentEffect.asset");
            GameplayPersistentEffect
                _effect = new GameplayPersistentEffect(_persistentEffectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_effect);
            Assert.AreEqual(95, _health.currentValue);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenOverflow_AppliesOverflowEffects()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            StatController _statController = m_Player.GetComponent<StatController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenOverflow_AppliesOverflowEffects/GameplayStackableEffectDefinition.asset");
            Health _health = _statController.stats["Health"] as Health;
            Assert.AreEqual(100, _health.currentValue);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            Assert.AreEqual(95, _health.currentValue);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            Assert.AreEqual(90, _health.currentValue);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenOverflow_ClearsStack()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();

            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenOverflow_ClearsStack/GameplayStackableEffectDefinition.asset");
            GameplayStackableEffect _stackableEffect = new GameplayStackableEffect(_effectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_stackableEffect);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            GameplayStackableEffect _secondStackableEffect =
                _effectController.activeEffects.FirstOrDefault(_effect => _effect.definition == _effectDefinition)
                    as GameplayStackableEffect;
            Assert.AreNotEqual(_stackableEffect, _secondStackableEffect);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenOverflow_DoNotApplyEffect()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenOverflow_DenyOverflow/GameplayStackableEffectDefinition.asset");
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            Assert.AreEqual(
                false,
                _effectController.ApplyGameplayEffectToSelf(
                    new GameplayStackableEffect(_effectDefinition, null, m_Player))
            );
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenOverflow_ResetDuration()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenOverflow_ResetDuration/GameplayStackableEffectDefinition.asset");
            GameplayStackableEffect _stackableEffect = new GameplayStackableEffect(_effectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_stackableEffect);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingDuration, 10f, 0.1f);
            yield return new WaitForSeconds(1f);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingDuration, 9f, 0.1f);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingDuration, 10f, 0.1f);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenOverflow_ResetPeriod()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenOverflow_ResetPeriod/GameplayStackableEffectDefinition.asset");
            GameplayStackableEffect _stackableEffect = new GameplayStackableEffect(_effectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_stackableEffect);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingPeriod, 3f, 0.1f);
            yield return new WaitForSeconds(1f);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingPeriod, 2f, 0.1f);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(_stackableEffect.remainingPeriod, 3f, 0.1f);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenApplyStack_IncreaseStackCount()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenApplyStack_IncreaseStackCount/GameplayStackableEffectDefinition.asset");
            GameplayStackableEffect _stackableEffect = new GameplayStackableEffect(_effectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_stackableEffect);
            Assert.AreEqual(1, _stackableEffect.stackCount);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            Assert.AreEqual(2, _stackableEffect.stackCount);
        }

        [UnityTest]
        public IEnumerator GameplayEffectController_WhenDurationReached_RemoveStackAndRefresh()
        {
            yield return null;

            GameplayEffectController _effectController = m_Player.GetComponent<GameplayEffectController>();
            GameplayStackableEffectDefinition _effectDefinition
                = AssetDatabase.LoadAssetAtPath<GameplayStackableEffectDefinition>(
                    "Assets/@MyProject/AbilitySystem/Tests/ScriptableObjects/WhenDurationReached_RemoveStackAndRefresh/GameplayStackableEffectDefinition.asset");

            GameplayStackableEffect _stackableEffect = new GameplayStackableEffect(_effectDefinition, null, m_Player);
            _effectController.ApplyGameplayEffectToSelf(_stackableEffect);
            _effectController.ApplyGameplayEffectToSelf(new GameplayStackableEffect(_effectDefinition, null, m_Player));
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(3f, _stackableEffect.remainingDuration, 0.1f);
            Assert.AreEqual(2, _stackableEffect.stackCount);

            yield return new WaitForSeconds(3f);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(3f, _stackableEffect.remainingDuration, 0.1f);
            Assert.AreEqual(1, _stackableEffect.stackCount);
        }
    }
}