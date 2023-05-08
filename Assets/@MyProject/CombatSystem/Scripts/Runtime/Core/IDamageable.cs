using UnityEngine.Events;

namespace CombatSystem.Core
{
    public interface IDamageable
    {
        int health { get; }
        int maxHealth { get; }
        UnityEvent healthChanged { get; set; }
        UnityEvent maxHealthChanged { get; set; }
        bool isInitialized { get; }
        UnityEvent initialized { get; set; }
        UnityEvent willUninitialize { get; set; }
        UnityEvent defeated { get; set; }
        UnityEvent<int> healed { get; set; }
        UnityEvent<int, bool> damaged { get; set; }
        void TakeDamage(IDamage _rawDamage);
    }
}