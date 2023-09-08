using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using SaveSystem;
using StatSystem.Attributes;
using StatSystem.Nodes;
using UnityEngine;

namespace StatSystem
{
    [RequireComponent(typeof(TagController))]
    public partial class StatController : MonoBehaviour, ISavable
    {
        [SerializeField] private StatDatabase m_StatDatabase;
        protected Dictionary<string, Stat> m_Stats = new Dictionary<string, Stat>(StringComparer.OrdinalIgnoreCase);
        private bool m_IsInitialized;

        public Dictionary<string, Stat> stats => m_Stats;
        public bool IsInitialized() => m_IsInitialized;
        public event System.Action initialized;
        public event System.Action willUninitialize;

        private TagController m_TagController;

        protected virtual void Awake()
        {
            m_TagController = GetComponent<TagController>();

            if (m_IsInitialized == false)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            willUninitialize?.Invoke();
        }

        protected void Initialize()
        {
            foreach (var _definition in m_StatDatabase.stats)
            {
                m_Stats.Add(_definition.name, new Stat(_definition, this));
            }

            Dictionary<string, Type> _attributeTypes
                = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                    .Where(t => typeof(Attribute).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .ToDictionary(t =>
                    {
                        string _attributeName = string.Empty;

                        if (t.GetCustomAttributes(typeof(CustomAttribute), false) is CustomAttribute[] _arr
                            && _arr.Length > 0)
                        {
                            _attributeName = _arr[0].attributeName;
                        }

                        return _attributeName;
                    });

            foreach (var _definition in m_StatDatabase.attributes)
            {
                if (_attributeTypes.TryGetValue(_definition.name, out Type _attributeType))
                {
                    Attribute _attribute =
                        Activator.CreateInstance(
                            _attributeType,
                            _definition, // definition
                            this, // stat controller
                            m_TagController // tag controller
                        ) as Attribute;
                    m_Stats.Add(_definition.name, _attribute);
                }
                else if (_definition.name.Equals("Health", StringComparison.OrdinalIgnoreCase))
                {
                    m_Stats.Add(_definition.name, new Health(_definition, this, m_TagController));
                }
                else
                {
                    m_Stats.Add(_definition.name, new Attribute(_definition, this));
                }
            }

            foreach (var _definition in m_StatDatabase.primaryStats)
            {
                m_Stats.Add(_definition.name, new PrimaryStat(_definition, this));
            }

            InitializeStatFormulas();

            foreach (Stat _stat in m_Stats.Values)
            {
                _stat.Initialize();
            }

            initialized?.Invoke();

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
                            _stat.valueChanged += _currentStat.CalculateValue;
                        }
                        else
                        {
                            Debug.LogWarning($"Stat {n.statName.Trim()} does not exist!");
                        }
                    });
                }
            }
        }

        #region Save System

        [System.Serializable]
        protected class StatControllerData
        {
            public Dictionary<string, object> stats;
        }

        public virtual object data
        {
            get
            {
                Dictionary<string, object> stats = new Dictionary<string, object>();
                foreach (Stat _stat in m_Stats.Values)
                {
                    if (_stat is ISavable _savable)
                        stats.Add(_stat.definition.name, _savable.data);
                }

                return new StatControllerData { stats = stats };
            }
        }

        public virtual void Load(object _data)
        {
            StatControllerData _statControllerData = (StatControllerData)_data;
            foreach (Stat _stat in m_Stats.Values)
            {
                if (_stat is ISavable _savable)
                    _savable.Load(_statControllerData.stats[_stat.definition.name]);
            }
        }

        #endregion
    }
}