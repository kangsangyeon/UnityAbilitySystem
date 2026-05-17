using Core;

namespace CombatSystem.Core
{
    public interface IDamageable
    {
        int health { get; }
        int maxHealth { get; }
        event System.Action healthChanged;
        event System.Action maxHealthChanged;
        bool isInitialized { get; }
        event System.Action initialized;
        event System.Action willUninitialize;
        event System.Action defeated;
        event System.Action<int> healed;
        event System.Action<int, bool> damaged;
        void TakeDamage(IDamage _rawDamage);
    }
}