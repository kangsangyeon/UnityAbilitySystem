using System.Collections;
using NUnit.Framework;
using StatSystem;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class StatTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.LoadSceneInPlayMode(
            "Assets/@MyProject/StatSystem/Tests/Scenes/Test.unity",
            new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Stat_WhenModifierApplied_ChangesValue()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;

        StatController _statController = GameObject.FindObjectOfType<StatController>();
        Stat _physicalAttack = _statController.stats["PhysicalAttack"];
        int _prevValue = _physicalAttack.value;
        _physicalAttack.AddModifier(new StatModifier()
        {
            magnitude = 5,
            type = ModifierOperationType.Additive
        });
        Assert.AreEqual(_prevValue + 5, _physicalAttack.value);
    }
    
    [UnityTest]
    public IEnumerator Stat_WhenModifierApplied_DoesNotExceedCap()
    {
        yield return null;

        StatController _statController = GameObject.FindObjectOfType<StatController>();
        Stat _attackSpeed = _statController.stats["AttackSpeed"];
        Assert.AreEqual(0, _attackSpeed.value);
        _attackSpeed.AddModifier(new StatModifier()
        {
            magnitude = 5,
            type = ModifierOperationType.Additive
        });
        Assert.AreEqual(3, _attackSpeed.value);
    }

    [UnityTest]
    public IEnumerator Stat_WhenStrengthIncreased_UpdatePhysicalAttack()
    {
        yield return null;

        StatController _statController = GameObject.FindObjectOfType<StatController>();
        PrimaryStat _strength = _statController.stats["Strength"] as PrimaryStat;
        Stat _physicalAttack = _statController.stats["PhysicalAttack"];
        Assert.AreEqual(1, _strength.value);
        Assert.AreEqual(3, _physicalAttack.value);
        _strength.Add(3);
        Assert.AreEqual(12, _physicalAttack.value);
    }
}