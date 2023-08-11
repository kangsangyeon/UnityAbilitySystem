using CombatSystem.Core;
using Core;
using UnityEngine;

namespace CombatSystem
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private VisualEffect m_CollisionVisualEffectPrefab;

        protected Rigidbody m_Rigidbody;
        public Rigidbody rigidbody => m_Rigidbody;

        protected Collider m_Collider;

        public event System.Action<CollisionData> hit;

        protected void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision _other)
        {
            HandleCollision(_other.gameObject);
        }

        protected void HandleCollision(GameObject _other)
        {
            if (m_CollisionVisualEffectPrefab != null)
            {
                VisualEffect _effect =
                    Instantiate(m_CollisionVisualEffectPrefab, transform.position, transform.rotation);
                _effect.finished += _effect => Destroy(_effect.gameObject);
                _effect.Play();
            }

            hit?.Invoke(new CollisionData() { source = this, target = _other });
        }
    }
}