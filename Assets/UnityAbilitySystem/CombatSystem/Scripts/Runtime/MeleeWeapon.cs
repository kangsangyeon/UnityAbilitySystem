using System;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem
{
    public class MeleeWeapon : MonoBehaviour
    {
        public event Action<CollisionData> hit;

        private void OnTriggerEnter(Collider other)
        {
            hit?.Invoke(new CollisionData
            {
                target = other.gameObject,
                source = this
            });
        }
    }
}