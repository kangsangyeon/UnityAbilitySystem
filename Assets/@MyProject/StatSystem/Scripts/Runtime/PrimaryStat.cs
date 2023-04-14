using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StatSystem.Tests")]

namespace StatSystem
{
    public class PrimaryStat : Stat
    {
        private int m_BaseValue;
        public override int baseValue => m_BaseValue;

        public PrimaryStat(StatDefinition _definition) : base(_definition)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_BaseValue = m_Definition.baseValue;
            CalculateValue();
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
    }
}