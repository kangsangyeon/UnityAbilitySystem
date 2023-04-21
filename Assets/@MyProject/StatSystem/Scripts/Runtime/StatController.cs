using System;
using System.Collections.Generic;
using StatSystem.Nodes;
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

        protected virtual void Awake()
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

        protected void Initialize()
        {
            foreach (var _definition in m_StatDatabase.stats)
            {
                m_Stats.Add(_definition.name, new Stat(_definition));
            }

            foreach (var _definition in m_StatDatabase.attributes)
            {
                m_Stats.Add(_definition.name, new Attribute(_definition));
            }

            foreach (var _definition in m_StatDatabase.primaryStats)
            {
                m_Stats.Add(_definition.name, new PrimaryStat(_definition));
            }
            
            InitializeStatFormulas();

            foreach (Stat _stat in m_Stats.Values)
            {
                _stat.Initialize();
            }
            
            initialized.Invoke();

            m_IsInitialized = true;
        }

        protected virtual void InitializeStatFormulas()
        {
            foreach (Stat _currentStat in m_Stats.Values)
            {
                if (_currentStat.definition.formula != null && _currentStat.definition.formula.rootNode != null)
                {
                    List<StatNode> _statNodes = _currentStat.definition.formula.FindNodesOfType<StatNode>();
                    _statNodes.ForEach(n =>
                    {
                        if (m_Stats.TryGetValue(n.statName.Trim(), out Stat _stat))
                        {
                            n.stat = _stat;
                            _stat.valueChanged.AddListener(_currentStat.CalculateValue);
                        }
                        else
                        {
                            Debug.LogWarning($"Stat {n.statName.Trim()} does not exist!");
                        }
                    });
                }
            }
        }
    }
}