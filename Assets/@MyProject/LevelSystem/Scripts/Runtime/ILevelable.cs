using UnityEngine.Events;

namespace LevelSystem
{
    public interface ILevelable
    {
        int level { get; }
        int currentExperience { get; }
        int requiredExperience { get; }
        bool isInitilaized { get; }
        event System.Action levelChanged;
        event System.Action currentExperienceChanged;
        event System.Action initialized;
        event System.Action willUnitialize;
    }
}