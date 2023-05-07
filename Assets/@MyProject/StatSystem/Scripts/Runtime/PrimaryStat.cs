using System.Runtime.CompilerServices;
using SaveSystem;

[assembly: InternalsVisibleTo("StatSystem.Tests")]

namespace StatSystem
{
    public class PrimaryStat : Stat, ISavable
    {
        private int m_BaseValue;
        public override int baseValue => m_BaseValue;

        public PrimaryStat(StatDefinition _definition) : base(_definition)
        {
        }

        public override void Initialize()
        {
            m_BaseValue = m_Definition.baseValue;
            base.Initialize();
        }

        internal void Add(int _amount)
        {
            m_BaseValue += _amount;
            CalculateValue();
        }

        internal void Subtract(int _amount)
        {
            m_BaseValue -= _amount;
            CalculateValue();
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