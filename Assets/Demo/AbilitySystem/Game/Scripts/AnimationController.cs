using AbilitySystem;
using StatSystem;
using UnityEngine;
using UnityEngine.AI;

namespace MyGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(StatController))]
    [RequireComponent(typeof(AbilityController))]
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private float m_BaseSpeed = 3.5f;
        private Animator m_Animator;
        private NavMeshAgent m_NavMeshAgent;
        private StatController m_StatController;
        private AbilityController m_AbilityController;
        private static readonly int MOVEMENT_SPEED = Animator.StringToHash("MovementSpeed");
        private static readonly int VELOCITY = Animator.StringToHash("Velocity");
        private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_StatController = GetComponent<StatController>();
            m_AbilityController = GetComponent<AbilityController>();
        }

        private void Update()
        {
            m_Animator.SetFloat(VELOCITY, m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed);
        }

        private void OnEnable()
        {
            m_StatController.initialized += OnStatControllerInitialized;
            if (m_StatController.IsInitialized())
            {
                OnStatControllerInitialized();
            }

            m_AbilityController.activatedAbility += OnActivateAbility;
        }

        private void OnDisable()
        {
            m_StatController.initialized -= OnStatControllerInitialized;
            if (m_StatController.IsInitialized())
            {
                m_StatController.stats["MovementSpeed"].valueChanged -= OnMovementSpeedChanged;
                m_StatController.stats["AttackSpeed"].valueChanged -= OnAttackSpeedChanged;
            }

            m_AbilityController.activatedAbility -= OnActivateAbility;
        }

        private void OnStatControllerInitialized()
        {
            OnMovementSpeedChanged();
            OnAttackSpeedChanged();
            m_StatController.stats["MovementSpeed"].valueChanged += OnMovementSpeedChanged;
            m_StatController.stats["AttackSpeed"].valueChanged += OnAttackSpeedChanged;
        }

        private void OnMovementSpeedChanged()
        {
            m_Animator.SetFloat(MOVEMENT_SPEED, m_StatController.stats["MovementSpeed"].value / 100f);
            m_NavMeshAgent.speed = m_BaseSpeed * m_StatController.stats["MovementSpeed"].value / 100f;
        }

        private void OnAttackSpeedChanged()
        {
            m_Animator.SetFloat(AttackSpeed, m_StatController.stats["AttackSpeed"].value / 100.0f);
        }

        private void OnActivateAbility(ActiveAbility _ability)
        {
            m_Animator.SetTrigger(_ability.definition.animationName);
        }

        #region Animation Events

        public void Cast()
        {
            if (m_AbilityController.currentAbility is SingleTargetAbility _singleTargetAbility)
            {
                _singleTargetAbility.Cast(m_AbilityController.target);
            }
        }

        public void Shoot()
        {
            if (m_AbilityController.currentAbility is ProjectileAbility _projectileAbility)
            {
                _projectileAbility.Shoot(m_AbilityController.target);
            }
        }

        #endregion
    }
}