using AbilitySystem;
using AbilitySystem.UI;
using Game.Scripts.Runtime;
using LevelSystem;
using StatSystem;
using UnityEngine;
using UnityEngine.AI;

namespace MyGame.Scripts
{
    [RequireComponent(typeof(AbilityController))]
    [RequireComponent(typeof(PlayerStatController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : CombatableCharacter
    {
        private ILevelable m_Levelable;
        [SerializeField] private AbilitiesUI m_AbilitiesUI;
        [SerializeField] private GameObject m_Target;
        private NavMeshAgent m_NavMeshAgent;
        private AbilityController m_AbilityController;

        protected override void Awake()
        {
            base.Awake();
            m_Levelable = m_StatController.GetComponent<ILevelable>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_AbilityController = GetComponent<AbilityController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                (m_Target.GetComponent<StatController>().stats["Health"] as Health)
                    .ApplyModifier(new HealthModifier() { magnitude = 100 });
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                (GetComponent<StatController>().stats["Mana"] as Attribute)
                    .ApplyModifier(new StatModifier() { magnitude = 100 });
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_AbilityController.TryActivateAbility("Shock", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                m_AbilityController.TryActivateAbility("Heal", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                m_AbilityController.TryActivateAbility("Regeneration", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                m_AbilityController.TryActivateAbility("Poison", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                m_AbilityController.TryActivateAbility("Malediction", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                m_Levelable.currentExperience += 100;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                m_AbilitiesUI.Show();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                m_AbilitiesUI.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                m_AbilityController.TryActivateAbility("Fireball", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                m_AbilityController.TryActivateAbility("Cleanse", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                m_AbilityController.TryActivateAbility("ShieldAura", m_Target);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                m_AbilityController.TryActivateAbility("Combustion", m_Target);
            }
        }
    }
}