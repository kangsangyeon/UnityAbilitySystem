﻿using AbilitySystem;
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
        }
    }
}