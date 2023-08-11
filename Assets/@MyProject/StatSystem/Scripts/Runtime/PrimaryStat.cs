using System.Runtime.CompilerServices;
using SaveSystem;

[assembly: InternalsVisibleTo("StatSystem.Tests")]

namespace StatSystem
{
    public class PrimaryStat : Stat, ISavable
    {
        private int m_BaseValue;
        public override int baseValue => m_BaseValue;

        public event System.Action<int> onBaseValueAdded_OnServer; // network 지원 전용 이벤트입니다. 이 이벤트는 서버에서만 구독합니다. param: <add_amount>

        public PrimaryStat(StatDefinition _definition, StatController _controller) : base(_definition, _controller)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_BaseValue = m_Definition.baseValue;
            CalculateValue();
        }

        // note: 로컬의 base value값을 수정하려 할 때 사용해야 합니다.
        // 만약 원격 클라이언트의 base value를 수정하기 위해 이 함수를 RPC로 호출하려 할 때,
        // 이 함수를 호출하는 것 대신 FN_StatController.ForceSetPrimaryStatBaseValue를 호출해야 하는 것이 의도에 적합할 수 있습니다.
        public void Add(int _amount)
        {
            m_BaseValue += _amount;
            CalculateValue();
            onBaseValueAdded_OnServer?.Invoke(_amount);
        }

        #region Stat System

        [System.Serializable]
        protected class PrimaryStatData
        {
            public int baseValue;
        }

        public object data => new PrimaryStatData() { baseValue = baseValue };

        public void Load(object _data)
        {
            PrimaryStatData _primaryStatData = (PrimaryStatData)_data;
            m_BaseValue = _primaryStatData.baseValue;
            CalculateValue();
        }

        #endregion
    }
}