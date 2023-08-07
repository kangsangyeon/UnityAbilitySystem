using LevelSystem;
using LevelSystem.Nodes;
using UnityEngine;

namespace StatSystem
{
    [RequireComponent(typeof(ILevelable))]
    public partial class PlayerStatController : StatController
    {
        protected ILevelable m_Levelable;

        protected int m_StatPoints = 5;

        public int statPoints => m_StatPoints;

        public event System.Action statPointsChanged;
        public event System.Action<int> onGainStatPoints;
        public event System.Action<PrimaryStat, int> onInvestStat_OnLocal;

        public bool CanInvest(PrimaryStat _primaryStat, int _points)
        {
            if (statPoints < _points)
            {
                // 남은 stat points가 투자하려는 points보다 적은 경우
                // 이 요청을 반려합니다.
                return false;
            }

            if (_primaryStat.definition.cap >= 0
                && _primaryStat.baseValue + _points > _primaryStat.definition.cap)
            {
                // primary stat의 한계치보다 높은 값을 가지게 되게끔 투자하려는 경우
                // 이 요청을 반려합니다.
                return false;
            }

            return true;
        }

        // note: 로컬에서 특정 stat에 투자하려 할 때 사용해야 합니다.
        // 만약 원격 클라이언트 또는 원격 서버에서 특정 stat에 투자하기 위해 이 함수를 RPC로 호출하려 할 때,
        // 이 함수를 호출하는 것 대신 StatController.ForceSetPrimaryStatBaseValue와 PlayerStatController.ForceSetStatPoints를 호출해야 하는 것이 의도에 적합할 수 있습니다.
        public bool TryInvest(PrimaryStat _primaryStat, int _points)
        {
            if (CanInvest(_primaryStat, _points) == false)
                return false;

            _primaryStat.Add(_points);
            m_StatPoints = statPoints - _points;

            statPointsChanged?.Invoke();
            onInvestStat_OnLocal?.Invoke(_primaryStat, _points);
            return true;
        }

        protected override void Awake()
        {
            m_Levelable = GetComponent<ILevelable>();
        }

        private void OnEnable()
        {
            m_Levelable.initialized += OnLevelableInitialized;
            m_Levelable.willUnitialize += UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                OnLevelableInitialized();
            }
        }

        private void OnDisable()
        {
            m_Levelable.initialized -= OnLevelableInitialized;
            m_Levelable.willUnitialize -= UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                UnregisterEvents();
            }
        }

        private void OnLevelableInitialized()
        {
            if (IsInitialized() == false)
            {
                Initialize();
            }

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
            int _points = 5;
            m_StatPoints += _points;
            statPointsChanged?.Invoke();
            onGainStatPoints?.Invoke(_points);
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