using UnityEngine.Events;

namespace LevelSystem
{
    public interface ILevelable
    {
        int level { get; }
        int currentExperience { get; set; }
        int requiredExperience { get; }
        bool isInitialized { get; }
        event System.Action levelChanged;
        event System.Action currentExperienceChanged;
        event System.Action initialized;
        event System.Action willUnitialize;
        event System.Action loaded;
    }
}