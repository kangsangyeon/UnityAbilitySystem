using System;
using LevelSystem;
using LevelSystem.Nodes;
using UnityEngine;

namespace StatSystem
{
    [RequireComponent(typeof(ILevelable))]
    public class PlayerStatController : StatController
    {
        protected ILevelable m_Levelable;

        protected int m_StatPoints = 5;
        public event System.Action statPointsChanged;

        public int statPoints
        {
            get => m_StatPoints;
            internal set
            {
                m_StatPoints = value;
                statPointsChanged?.Invoke();
            }
        }

        protected override void Awake()
        {
            m_Levelable = GetComponent<ILevelable>();
        }

        private void OnEnable()
        {
            m_Levelable.initialized += OnLevelableInitialized;
            m_Levelable.willUnitialize += UnregisterEvents;
            if (m_Levelable.isInitilaized)
            {
                OnLevelableInitialized();
            }
        }

        private void OnDisable()
        {
            m_Levelable.initialized -= OnLevelableInitialized;
            m_Levelable.willUnitialize -= UnregisterEvents;
            if (m_Levelable.isInitilaized)
            {
                UnregisterEvents();
            }
        }

        private void OnLevelableInitialized()
        {
            Initialize();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            m_Levelable.levelChanged += OnLevelChanged;
        }

        private void UnregisterEvents()
        {
            m_Levelable.levelChanged -= OnLevelChanged;
        }

        private void OnLevelChanged()
        {
            m_StatPoints += 5;
        }

        protected override void InitializeStatFormulas()
        {
            base.InitializeStatFormulas();
            foreach (Stat _currentStat in m_Stats.Values)
            {
                if (_currentStat.definition.formula != null && _currentStat.definition.formula.rootNode != null)
                {
                    var _levelNodes = _currentStat.definition.formula.FindNodesOfType<LevelNode>();
                    foreach (LevelNode _node in _levelNodes)
                    {
                        _node.levelable = m_Levelable;
                        m_Levelable.levelChanged += _currentStat.CalculateValue;
                    }
                }
            }
        }

        #region Stat System

        [System.Serializable]
        protected class PlayerStatControllerData : StatControllerData
        {
            public int statPoints;

            public PlayerStatControllerData(StatControllerData _statControllerData)
            {
                stats = _statControllerData.stats;
            }
        }

        public override object data
        {
            get { return new PlayerStatControllerData(base.data as StatControllerData) { statPoints = statPoints }; }
        }

        public override void Load(object _data)
        {
            base.Load(_data);
            PlayerStatControllerData _playerStatControllerData = (PlayerStatControllerData)_data;
            m_StatPoints = _playerStatControllerData.statPoints;
            statPointsChanged?.Invoke();
        }

        #endregion
    }
}