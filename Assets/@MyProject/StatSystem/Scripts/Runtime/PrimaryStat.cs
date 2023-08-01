using System.Runtime.CompilerServices;
using SaveSystem;
using UnityEngine.Events;

[assembly: InternalsVisibleTo("StatSystem.Tests")]

namespace StatSystem
{
    public class PrimaryStat : Stat, ISavable
    {
        private int m_BaseValue;
        public override int baseValue => m_BaseValue;
        public UnityEvent<int> valueAdded = new UnityEvent<int>();

        public PrimaryStat(StatDefinition _definition, StatController _controller) : base(_definition, _controller)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_BaseValue = m_Definition.baseValue;
            CalculateValue();
        }

        public void Add(int _amount)
        {
            m_BaseValue += _amount;
            CalculateValue();
            valueAdded?.Invoke(_amount);
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