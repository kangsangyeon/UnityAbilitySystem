﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem
{
    [RequireComponent(typeof(GameplayEffectController))]
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] private List<AbilityDefinition> m_AbilityDefinitions;

        protected Dictionary<string, Ability> m_Abilities = new Dictionary<string, Ability>();
        public Dictionary<string, Ability> abilities => m_Abilities;

        private GameplayEffectController m_EffectController;

        protected void Awake()
        {
            m_EffectController = GetComponent<GameplayEffectController>();
        }

        private void OnEnable()
        {
            m_EffectController.initialized.AddListener(OnEffectControllerInitialized);
            if (m_EffectController.isInitialized)
                OnEffectControllerInitialized();
        }

        private void OnDisable()
        {
            m_EffectController.initialized.RemoveListener(OnEffectControllerInitialized);
        }

        private void OnEffectControllerInitialized()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            foreach (AbilityDefinition _definition in m_AbilityDefinitions)
            {
                AbilityAttributeType _attribute = _definition.GetType().GetCustomAttributes(false)
                    .OfType<AbilityAttributeType>().FirstOrDefault();
                Ability _ability = Activator.CreateInstance(
                    _attribute.type,
                    _definition, // definition
                    this // controller
                ) as Ability;

                m_Abilities.Add(_definition.name, _ability);

                if (_ability is PassiveAbility _passiveAbility)
                {
                    _passiveAbility.ApplyEffects(gameObject);
                }
            }
        }
    }
}