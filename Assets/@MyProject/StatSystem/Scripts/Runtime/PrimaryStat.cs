namespace StatSystem
{
    public class PrimaryStat : Stat
    {
        private int m_BaseValue;
        public override int baseValue => m_BaseValue;

        public PrimaryStat(StatDefinition _definition) : base(_definition)
        {
            m_BaseValue = _definition.baseValue;
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