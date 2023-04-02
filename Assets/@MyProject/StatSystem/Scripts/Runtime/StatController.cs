using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StatSystem
{
    public class StatController : MonoBehaviour
    {
        [SerializeField] private StatDatabase m_StatDatabase;
        protected Dictionary<string, Stat> m_Stats = new Dictionary<string, Stat>(StringComparer.OrdinalIgnoreCase);
        private bool m_IsInitialized;

        public Dictionary<string, Stat> stats => m_Stats;
        public bool IsInitialized() => m_IsInitialized;
        public UnityEvent initialized = new UnityEvent();
        public UnityEvent willUninitialize = new UnityEvent();

        private void Awake()
        {
            if (m_IsInitialized == false)
            {
                m_IsInitialized = true;
                Initialize();
                initialized.Invoke();
            }
        }

        private void OnDestroy()
        {
            willUninitialize.Invoke();
        }

        private void Initialize()
        {
            foreach (var _definition in m_StatDatabase.stats)
            {
                m_Stats.Add(_definition.name, new Stat(_definition));
            }
            
            foreach (var _definition in m_StatDatabase.attributes)
            {
                m_Stats.Add(_definition.name, new Attribute(_definition));
            }
        }
    }
}