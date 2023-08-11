using CombatSystem;
using CombatSystem.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace AbilitySystem
{
    public class ProjectileAbility : ActiveAbility
    {
        public new ProjectileAbilityDefinition definition => m_Definition as ProjectileAbilityDefinition;

        private ObjectPool<Projectile> m_Pool;
        protected CombatController m_CombatController;

        public ProjectileAbility(
            AbilityDefinition _definition,
            AbilityController _controller)
            : base(_definition, _controller)
        {
            m_Pool = new ObjectPool<Projectile>(OnCreate, OnGet, OnRelease);
            m_CombatController = _controller.GetComponent<CombatController>();
        }

        public void Shoot(GameObject _target)
        {
            if (m_CombatController.rangedWeapons.TryGetValue(definition.weaponId, out RangedWeapon _weapon))
            {
                Projectile _projectile = m_Pool.Get();
                _weapon.Shoot(_projectile, _target.transform,
                    definition.projectileSpeed, definition.shotType, definition.isSpinning);
            }
        }

        private Projectile OnCreate()
        {
            Projectile _projectile = GameObject.Instantiate(definition.projectilePrefab);
            _projectile.gameObject.SetActive(false);
            _projectile.hit += OnHit;
            return _projectile;
        }

        private void OnGet(Projectile _projectile)
        {
            _projectile.gameObject.SetActive(true);
        }

        private void OnRelease(Projectile _projectile)
        {
            _projectile.rigidbody.velocity = Vector3.zero;
            _projectile.gameObject.SetActive(false);
        }

        private void OnHit(CollisionData _data)
        {
            m_Pool.Release(_data.source as Projectile);
            ApplyEffects(_data.target);
        }
    }
}